using Microsoft.EntityFrameworkCore;
using Upscale.Web.Models.Entities;

namespace Upscale.Web.Data
{
    /// <summary>
    /// Database context responsible for managing user-related data and entity mapping.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the context using the specified options.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

        /// <summary>
        /// Configures the schema and entity relationships for the database.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Explicitly map entities to database tables
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<UserProfile>().ToTable("UserProfiles");
        }
    }
}