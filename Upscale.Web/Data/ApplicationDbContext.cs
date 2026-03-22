using Microsoft.EntityFrameworkCore;
using Upscale.Web.Models.Entities;

namespace Upscale.Web.Data
{
    /// <summary>
    /// Main EF Core database context for the Upscale application.
    /// Manages Users and UserProfiles with their relationships and constraints.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Users ────────────────────────────────────────────────────
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);

                entity.HasIndex(u => u.DocumentNumber).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(u => u.DocumentNumber)
                      .IsRequired()
                      .HasMaxLength(15)
                      .IsUnicode(false);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(150)
                      .IsUnicode(false);

                entity.Property(u => u.FailedAttempts).HasDefaultValue(0);
                entity.Property(u => u.IsLocked).HasDefaultValue(false);
                entity.Property(u => u.IsActive).HasDefaultValue(true);
            });

            // ── UserProfiles ─────────────────────────────────────────────
            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(p => p.ProfileId);

                // One-to-one: cada User tiene exactamente un UserProfile
                entity.HasOne(p => p.User)
                      .WithOne(u => u.Profile)
                      .HasForeignKey<UserProfile>(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(p => p.UserId).IsUnique();

                entity.Property(p => p.Nationality)
                      .HasDefaultValue("Peruvian");
            });
        }
    }
}