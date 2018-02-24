using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BeltExam.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BeltExam
{
	public class AccountController : Controller
	{
		private DatabaseContext _context;

		public AccountController(DatabaseContext context)
		{
			_context = context;

			UserManager.SetDatabaseContext(_context);
		}

		public bool UniqueEmailAddress(RegisterViewModel model)
		{
			return _context.Users.SingleOrDefault(u => u.EmailAddress == model.EmailAddress) == null;
		}

		[HttpGet]
		[Route("")]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[Route("Account/Register/Submit")]
		public IActionResult RegisterSubmit(RegisterViewModel model)
		{
			if (!UniqueEmailAddress(model))
			{
				ModelState.AddModelError("EmailAddress", "Email address already in use.");
			}

			if (ModelState.IsValid)
			{
				// Successful registration
				UserManager.CreateAccount(model);
				UserManager.Login(model.EmailAddress, model.Password, HttpContext.Session);

				return RedirectToAction("Home", "Belt");
			}

			List<string> errors = new List<string>();

			foreach (var modelState in ViewData.ModelState.Values)
			{
				foreach (ModelError error in modelState.Errors)
				{
					errors.Add(error.ErrorMessage);
				}
			}

			ViewBag.RegisterErrors = errors;

			// Error
			return View("Index");
		}

		[HttpGet]
		[Route("Account/Login")]
		public IActionResult Login()
		{
			Identity identity = UserManager.Validate(HttpContext.Session);
			
			if (identity.Valid) // Already logged in
			{
				return RedirectToAction("AccessDenied");
			}

			return View();
		}

		[HttpPost]
		[Route("Account/Login/Submit")]
		public IActionResult LoginSubmit(LoginViewModel model)
		{
			if (string.IsNullOrEmpty(model.LoginPassword))
			{
				model.LoginPassword = "?"; // so no exceptions occur in hashing
			}
			
			if (UserManager.Login(model.LoginEmailAddress, model.LoginPassword, HttpContext.Session))
			{
				// Successful login
				return RedirectToAction("Home", "Belt");
			}

			ViewBag.LoginError = "* Incorrect login details.";

			// Error
			return View("Index");
		}

		[HttpGet]
		[Route("Account/LogOut")]
		public IActionResult LogOut()
		{
			UserManager.LogOut(HttpContext.Session);

			return RedirectToAction("Index");
		}

		[HttpGet]
		[Route("Account/AccessDenied")]
		public IActionResult AccessDenied()
		{
			return RedirectToAction("Index");
		}
	}
}