using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Upscale.Web.Models.Entities;

namespace Upscale.Web.Data
{
    /// <summary>
    /// Seeds the database with initial users and their profiles.
    ///
    /// DESIGN DECISIONS:
    /// ─────────────────
    /// • Idempotent: checks existence before inserting, safe to run on every startup.
    /// • Passwords hashed with HMACSHA512 + random 128-byte salt, exactly as the
    ///   AccountController.VerifyPasswordHash expects.
    /// • Plain-text passwords are ONLY here for development seed purposes.
    ///   In production, remove or replace with environment-variable-driven seeding.
    /// • All seed data matches the original SQL scripts provided.
    /// </summary>
    public static class DbSeeder
    {
        // ── Seed users: DocumentNumber → (plainTextPassword, email, profile data) ──
        private static readonly SeedUser[] SeedUsers =
        {
            new("71234567",  "Admin@2026",  "july.vargas@ceplan.gob.pe",
                "July", "Vargas", "Mendoza", "DNI",
                new DateTime(1995, 3, 12), "Peruana", "Female",
                "Analista de Planeamiento", "CEPLAN", "Plazo Indeterminado"),

            new("45678901",  "User.9876",   "ricardo.luna@ceplan.gob.pe",
                "Ricardo", "Luna", "Perez", "DNI",
                new DateTime(1988, 7, 22), "Peruana", "Male",
                "Especialista Presupuestal", "CEPLAN", "Plazo Indeterminado"),

            new("12345678",  "Clave#2024",  "ana.torres@ceplan.gob.pe",
                "Fátima", "Encarnación", "Castro", "DNI",
                new DateTime(1992, 11, 5), "Peruana", "Female",
                "Coordinadora RRHH", "CEPLAN", "Plazo Indeterminado"),

            new("000123456", "P@ssw0rd1",   "manuel.gomez@externo.pe",
                "Manuel", "Gomez", "Ferrer", "CE",
                new DateTime(1985, 1, 30), "Venezolana", "Male",
                "Consultor IT", "Ministerio de Economía", "Plazo Indeterminado"),

            new("000987654", "Guest*55",    "elena.schmidt@externo.pe",
                "Elena", "Schmidt", "Braun", "CE",
                new DateTime(1990, 9, 15), "Alemana", "Female",
                "Asesora Técnica", "GIZ Cooperación", "Plazo Indeterminado"),
        };

        /// <summary>
        /// Applies pending migrations and seeds initial data.
        /// Called from Program.cs at application startup.
        /// </summary>
        public static async Task InitialiseAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

            try
            {
                // Apply any pending EF Core migrations (creates DB if it doesn't exist)
                await db.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully.");

                await SeedUsersAsync(db, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initialising the database.");
                throw;   // Let the app crash loudly on startup if DB init fails
            }
        }

        private static async Task SeedUsersAsync(ApplicationDbContext db, ILogger logger)
        {
            foreach (var seed in SeedUsers)
            {
                if (await db.Users.AnyAsync(u => u.DocumentNumber == seed.DocumentNumber))
                    continue;

                CreatePasswordHash(seed.Password, out var hash, out var salt);

                var user = new User
                {
                    DocumentNumber = seed.DocumentNumber,
                    Email = seed.Email,
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    IsActive = true,
                    IsLocked = false,
                    FailedAttempts = 0,
                    Profile = new UserProfile
                    {
                        FirstName = seed.FirstName,
                        FirstLastName = seed.FirstLastName,
                        SecondLastName = seed.SecondLastName,
                        DocumentType = seed.DocumentType,
                        BirthDate = seed.BirthDate,
                        Nationality = seed.Nationality,
                        Gender = seed.Gender,
                        JobTitle = seed.JobTitle,
                        Organization = seed.Organization,

                        MobilePhone = "9" + seed.DocumentNumber.Substring(0, Math.Min(8, seed.DocumentNumber.Length)),
                        ContractType = "Indefinido",
                        HireDate = DateTime.Now,

                        SecondaryEmail = null,
                        SecondaryPhone = null
                    }
                };

                db.Users.Add(user);
                logger.LogInformation("Seeding user: {DocumentNumber} ({Email}).", seed.DocumentNumber, seed.Email);
            }

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Creates a salted HMACSHA512 hash identical to what AccountController.VerifyPasswordHash expects:
        ///   hash  → 64 bytes  (HMACSHA512 output)
        ///   salt  → 128 bytes (HMACSHA512 key)
        /// </summary>
        private static void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;                                          // random 128-byte key
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)); // 64-byte hash
        }

        // ── Internal record to keep seed data compact ──
        private sealed record SeedUser(
            string DocumentNumber,
            string Password,
            string Email,
            string FirstName,
            string FirstLastName,
            string SecondLastName,
            string DocumentType,
            DateTime BirthDate,
            string Nationality,
            string Gender,
            string JobTitle,
            string Organization,
            string ContractType);
    }
}