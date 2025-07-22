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
            string? connectionString = _config["ConnectionStrings:DefaultConnection"];
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Verhindert, dass beim Löschen eines Users alle seine Items automatisch gelöscht werden (besser für Datenintegrität)
            // Item-Owner Beziehung (kann optional Restrict sein, je nachdem, ob du alle Items löschen willst)
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Owner)
                .WithMany(u => u.OwnedItems)
                .HasForeignKey(i => i.OwnerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // Oder Cascade, wenn du möchtest

            // Verhindert den Fehler "multiple cascade paths" – Mieten bleiben bestehen, auch wenn ein User gelöscht wird
            // Rental-Renter Beziehung (niemals mit Cascade, da es zu den Konflikten führt)
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Renter)
                .WithMany(u => u.Rentals)
                .HasForeignKey(r => r.RenterId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Logisch sinnvoll: Wenn ein Item gelöscht wird, sollen auch alle zugehörigen Mietvorgänge verschwinden
            // Rental-Item Beziehung (Cascade ist hier unproblematisch, wenn gewünscht)
            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Item)
                .WithMany(i => i.Rentals)
                .HasForeignKey(r => r.ItemId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }


        //dotnet tool install --global dotnet-ef --version 9.0.7
        //dotnet tool update --global dotnet-ef --version 9.0.7
        //dotnet ef migrations add InitialCreate
        //dotnet ef database update
        //dotnet ef database drop --force

    }
}
