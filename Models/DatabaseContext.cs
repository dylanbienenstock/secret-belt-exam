using Microsoft.EntityFrameworkCore;

namespace BeltExam.Models
{
	public class DatabaseContext : DbContext
	{
		public DbSet<User> Users { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityUserJoin> ActivityUserJoins { get; set; }

		public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
	}
}