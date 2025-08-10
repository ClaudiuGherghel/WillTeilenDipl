using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class ApplicationDbContext : DbContext
    {

        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<User> Users { get; set; }


        private readonly IConfiguration _config;
        private readonly DbContextOptions<ApplicationDbContext>? _options;


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            _options = options;
        }

        /// <summary>
        /// Parameterloser Konstruktor liest Connection String aus appsettings.json (zur Designzeit)
        /// </summary>
        public ApplicationDbContext()
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Environment.CurrentDirectory).AddJsonFile
                        ("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                        optional: true, reloadOnChange: true);

            _config = builder.Build();
        }

        public ApplicationDbContext(IConfiguration configuration) : base()
        {
            _config = configuration;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_config is null || optionsBuilder.IsConfigured)
                return;

            var connectionString = _config["ConnectionStrings:DefaultConnection"];
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Ruft die Basiskonfiguration auf (wichtig für EF Core-internes Setup, z. B. Identity)

            // Beziehung: Rental → Renter (User)
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Renter)                // Ein Rental hat einen Renter (User)
                .WithMany(u => u.Rentals)             // Ein User kann viele Rentals haben
                .HasForeignKey(r => r.RenterId)       // Fremdschlüssel in Rental ist RenterId
                .OnDelete(DeleteBehavior.Restrict);   // Kein Cascade Delete → Benutzer muss erst manuell gelöscht werden (z. B. via Soft Delete)

            // Beziehung: Item → Owner (User)
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Owner)                 // Ein Item gehört einem Besitzer
                .WithMany(u => u.OwnedItems)          // Ein User kann viele Items besitzen
                .HasForeignKey(i => i.OwnerId)        // Fremdschlüssel ist OwnerId
                .OnDelete(DeleteBehavior.Restrict);   // Kein Cascade Delete

            // Beziehung: SubCategory → Category
            modelBuilder.Entity<SubCategory>()
                .HasOne(sc => sc.Category)            // Jede SubCategory gehört zu einer Category
                .WithMany(c => c.SubCategories)       // Eine Category kann viele SubCategories haben
                .HasForeignKey(sc => sc.CategoryId)   // Fremdschlüssel ist CategoryId
                .OnDelete(DeleteBehavior.Restrict);   // Kein Cascade Delete

            // Beziehung: Item → SubCategory
            modelBuilder.Entity<Item>()
                .HasOne(i => i.SubCategory)           // Jedes Item gehört zu einer SubCategory
                .WithMany(sc => sc.Items)             // Eine SubCategory kann viele Items enthalten
                .HasForeignKey(i => i.SubCategoryId)  // Fremdschlüssel ist SubCategoryId
                .OnDelete(DeleteBehavior.Restrict);   // Kein Cascade Delete

            // Beziehung: Image → Item
            modelBuilder.Entity<Image>()
                .HasOne(img => img.Item)              // Ein Bild gehört zu genau einem Item
                .WithMany(i => i.Images)              // Ein Item kann viele Bilder haben
                .HasForeignKey(img => img.ItemId)     // Fremdschlüssel ist ItemId
                .OnDelete(DeleteBehavior.Restrict);   // Kein Cascade Delete

            // Beziehung: Rental → Item
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Item)                  // Ein Rental bezieht sich auf ein Item
                .WithMany(i => i.Rentals)             // Ein Item kann mehrfach vermietet worden sein
                .HasForeignKey(r => r.ItemId)         // Fremdschlüssel ist ItemId
                .OnDelete(DeleteBehavior.Restrict);   // Kein Cascade Delete
        }



        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{

        //    // Verhindert, dass beim Löschen eines Users alle seine Items automatisch gelöscht werden (besser für Datenintegrität)
        //    // Item-Owner Beziehung (kann optional Restrict sein, je nachdem, ob du alle Items löschen willst)
        //    modelBuilder.Entity<Item>()
        //        .HasOne(i => i.Owner)
        //        .WithMany(u => u.OwnedItems)
        //        .HasForeignKey(i => i.OwnerId)
        //        .IsRequired()
        //        .OnDelete(DeleteBehavior.Restrict); // Oder Cascade, wenn du möchtest

        //    // Verhindert den Fehler "multiple cascade paths" – Mieten bleiben bestehen, auch wenn ein User gelöscht wird
        //    // Rental-Renter Beziehung (niemals mit Cascade, da es zu den Konflikten führt)
        //    modelBuilder.Entity<Rental>()
        //        .HasOne(r => r.Renter)
        //        .WithMany(u => u.Rentals)
        //        .HasForeignKey(r => r.RenterId)
        //        .IsRequired()
        //        .OnDelete(DeleteBehavior.Restrict);

        //    // Logisch sinnvoll: Wenn ein Item gelöscht wird, sollen auch alle zugehörigen Mietvorgänge verschwinden
        //    // Rental-Item Beziehung (Cascade ist hier unproblematisch, wenn gewünscht)
        //    modelBuilder.Entity<Rental>()
        //        .HasOne(r => r.Item)
        //        .WithMany(i => i.Rentals)
        //        .HasForeignKey(r => r.ItemId)
        //        .IsRequired()
        //        .OnDelete(DeleteBehavior.Cascade);
        //}


        //dotnet tool install --global dotnet-ef --version 9.0.7
        //dotnet tool update --global dotnet-ef --version 9.0.7
        //dotnet ef migrations add InitialCreate
        //dotnet ef database update
        //dotnet ef database drop --force


        /* VALIDIERUNG!
        1️. Ein Admin kann Kategorien per API erstellen →
        → DTO-Validation verhindert leere Namen beim API-Aufruf. ModelState prüft DTOs

        2️. Ein internes System importiert Daten direkt ins DbContext →
        → Hier greift nur die Entity-Validation oder ein ValidationService.
        */

    }
}
