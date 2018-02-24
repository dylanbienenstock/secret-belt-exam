using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BeltExam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BeltExam
{
	public class BeltController : Controller
	{
        private DatabaseContext _context;

        public BeltController(DatabaseContext context)
        {
            _context = context;

            UserManager.SetDatabaseContext(_context);
        }

        private string GetCoordinator(Activity activity)
        {
            User coordinator = _context.Users.SingleOrDefault(u => u.UserId == activity.CreatorId);

            if (coordinator != null)
            {
                return coordinator.FirstName;
            }

            return "?";
        }

        private int GetParticipantCount(Activity activity)
        {
            return _context.ActivityUserJoins.Where(auj => auj.ActivityId == activity.ActivityId).Count();
        }

        private List<string> GetParticipants(Activity activity)
        {
            var userIds = _context.ActivityUserJoins
                        .Where(auj => auj.ActivityId == activity.ActivityId)
                        .Select(auj => auj.UserId);

            List<string> userFirstNames = new List<string>();

            foreach (int UserId in userIds)
            {
                User user = _context.Users.SingleOrDefault(u => u.UserId == UserId);

                if (user != null)
                {
                    userFirstNames.Add(user.FirstName);
                }
            }

            return userFirstNames;
        }

        private bool HasJoinedActivity(Activity activity, Identity identity)
        {
            return _context.ActivityUserJoins.Where(auj => auj.ActivityId == activity.ActivityId)
                        .FirstOrDefault(auj => auj.UserId == identity.UserId) != null;
        }

        [HttpGet]
        [Route("Home")]
        public IActionResult Home()
		{
            Identity identity = UserManager.Validate(HttpContext.Session);

            if (!identity.Valid)
            {
                return RedirectToAction("Index", "Account");
            }

			UserManager.BagUp(identity, ViewBag);
			ViewBag.Activities = _context.Activities.Where(a => a.DateTime > DateTime.Now).OrderByDescending(a => a.ActivityId); // Don't display past activities
			ViewBag.ActivityCoordinators = new Dictionary<int, string>();
            ViewBag.ActivityParticipantCounts = new Dictionary<int, int>();
            ViewBag.ActivityActions = new Dictionary<int, Tuple<string, string>>(); 
                                                           // Action, Action Text

			foreach (Activity activity in ViewBag.Activities)
			{
                // Determine coordinator name
                ViewBag.ActivityCoordinators[activity.ActivityId] = GetCoordinator(activity);

                // Determine participants
                ViewBag.ActivityParticipantCounts[activity.ActivityId] = GetParticipantCount(activity);

                // Determine proper action
                string action = "DeleteActivity";
                string actionText = "Delete";

                if (activity.CreatorId != identity.UserId) // We didn't make the activity
                {
                    bool joined = HasJoinedActivity(activity, identity);
                    action = joined ? "LeaveActivity" : "JoinActivity";
                    actionText = joined ? "Leave" : "Join";
                }

                ViewBag.ActivityActions[activity.ActivityId] = 
                    new Tuple<string, string>(action, actionText);
            }

			return View();
		}

		[HttpGet]
		[Route("New")]
		public IActionResult NewActivity()
		{
			Identity identity = UserManager.Validate(HttpContext.Session);

			if (!identity.Valid)
			{
				return RedirectToAction("Index", "Account");
			}

			return View();
		}

        [HttpPost]
        [Route("New/Submit")]
        public IActionResult NewActivitySubmit(ActivityViewModel model)
        {
            Identity identity = UserManager.Validate(HttpContext.Session);

            if (!identity.Valid)
            {
                return RedirectToAction("Index", "Account");
            }

			if (ModelState.IsValid)
			{
				DateTime activityDateTime = new DateTime(
					((DateTime)model.Date).Year,
                    ((DateTime)model.Date).Month,
                    ((DateTime)model.Date).Day,
					((DateTime)model.Time).Hour,
                    ((DateTime)model.Time).Minute,
                    ((DateTime)model.Time).Second
				);

                if (activityDateTime >DateTime.Now)
                {
                    Activity activity = new Activity
                    {
                        Title = model.Title,
                        Description = model.Description,
                        DateTime = activityDateTime,
                        Duration = (float)model.Duration,
                        DurationUnits = model.DurationUnits,
                        CreatorId = identity.UserId
                    };

                    _context.Activities.Add(activity);
                    _context.SaveChanges();

                    return RedirectToAction("Home");
                }
                else
                {
                    ModelState.AddModelError("Date", "Date/Time must be in the future.");
                }
			}

            List<string> errors = new List<string>();

            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
            }

            ViewBag.NewActivityErrors = errors;

            return View("NewActivity");
        }

        [HttpGet]
        [Route("Activity/{id}")]
        public IActionResult ViewActivity(int id)
        {
            Identity identity = UserManager.Validate(HttpContext.Session);

            if (!identity.Valid)
            {
                return RedirectToAction("Index", "Account");
            }

			Activity activity = _context.Activities.SingleOrDefault(a => a.ActivityId == id);
			User coordinator = _context.Users.SingleOrDefault(u => u.UserId == activity.CreatorId);

			ViewBag.Activity = activity;
            ViewBag.ActivityCoordinator = "?";
            ViewBag.Participants = GetParticipants(activity);

            string action = "/Activity/Delete/" + activity.ActivityId.ToString();
            string actionText = "Delete";

            if (activity.CreatorId != identity.UserId) // We didn't make the activity
            {
                bool joined = HasJoinedActivity(activity, identity);
                action = joined ? ("/Activity/Leave/" + activity.ActivityId.ToString()) : ("/Activity/Join/" + activity.ActivityId.ToString());
                actionText = joined ? "Leave" : "Join";
            }

            ViewBag.ButtonAction = action;
            ViewBag.ButtonText = actionText;

            if (coordinator != null)
			{
                ViewBag.ActivityCoordinator = coordinator.FirstName;
            }

            return View();
        }

        [Route("Activity/Delete/{id}")]
        public IActionResult DeleteActivity(int id)
        {
            Identity identity = UserManager.Validate(HttpContext.Session);

            if (!identity.Valid)
            {
                return RedirectToAction("Index", "Account");
            }

            Activity activity = _context.Activities.SingleOrDefault(a => a.ActivityId == id);

            if (activity != null && activity.CreatorId == identity.UserId)
            {
                _context.ActivityUserJoins.RemoveRange(
                    _context.ActivityUserJoins.Where(auj => auj.ActivityId == id)
                );

                _context.Activities.Remove(activity);
                _context.SaveChanges();
            }

            return RedirectToAction("Home");
        }

        public bool TimeConflict(Activity activity, Identity identity)
        {
            List<int> joinedIds = _context.ActivityUserJoins
                                    .Where(auj => auj.UserId == identity.UserId)
                                    .Select(auj => auj.ActivityId).ToList();

            foreach (int joinedId in joinedIds)
            {
                Activity joinedActivity = _context.Activities.SingleOrDefault(a => a.ActivityId == joinedId);
                
                if (joinedActivity != null)
                {
                    DateTime joinedEnd = GetActivityEnd(joinedActivity);
                    DateTime activityEnd = GetActivityEnd(activity);

                    return activity.DateTime < joinedEnd && joinedActivity.DateTime < activityEnd;
                }
            }

            return false;
        }

        public DateTime GetActivityEnd(Activity activity)
        {
            switch (activity.DurationUnits)
            {
                case "Days":
                    return activity.DateTime.Add(new TimeSpan((int)activity.Duration, 0, 0, 0));
                case "Hours":
                    return activity.DateTime.Add(new TimeSpan((int)activity.Duration, 0, 0));
            }

            // Minutes
            return activity.DateTime.Add(new TimeSpan(0, (int)activity.Duration, 0));
        }

        [HttpGet]
        [Route("Activity/Join/{id}")]
        public IActionResult JoinActivity(int id)
        {
            Identity identity = UserManager.Validate(HttpContext.Session);

            if (!identity.Valid)
            {
                return RedirectToAction("Index", "Account");
            }

            Activity activity = _context.Activities.SingleOrDefault(a => a.ActivityId == id);
            User user = _context.Users.SingleOrDefault(u => u.UserId == identity.UserId);

            if (activity != null && !HasJoinedActivity(activity, identity)) 
            {
                if (!TimeConflict(activity, identity))
                {
                    ActivityUserJoin activityUserJoin = new ActivityUserJoin
                    {
                        ActivityId = activity.ActivityId,
                        Activity = activity,
                        UserId = user.UserId,
                        User = user
                    };

                    _context.ActivityUserJoins.Add(activityUserJoin);
                    _context.SaveChanges();
                }
                else
                {
                    return RedirectToAction("TimeConflict");
                }
            }

            return RedirectToAction("Home");
        }

        [Route("Activity/TimeConflict")]
        public IActionResult TimeConflict()
        {
            return View();
        }

        [Route("Activity/Leave/{id}")]
        public IActionResult LeaveActivity(int id)
        {
            Identity identity = UserManager.Validate(HttpContext.Session);

            if (!identity.Valid)
            {
                return RedirectToAction("Index", "Account");
            }

            ActivityUserJoin activityUserJoin = _context.ActivityUserJoins
                .SingleOrDefault(auj => auj.ActivityId == id && auj.UserId == identity.UserId);

            _context.ActivityUserJoins.Remove(activityUserJoin);
            _context.SaveChanges();

            return RedirectToAction("Home");
        }
    }
}