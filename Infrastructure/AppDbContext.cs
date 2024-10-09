using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;

namespace Infrastructure
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<BoardGameNight> BoardGameNights { get; set; }
        public DbSet<BoardGame> BoardGames { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Owned properties for Address in Person and BoardGameNight
            modelBuilder.Entity<Person>()
                .OwnsOne(p => p.Address);

            modelBuilder.Entity<BoardGameNight>()
                .OwnsOne(bgn => bgn.Address);

            // Relationship between BoardGameNight and Organizer (Person)
            modelBuilder.Entity<BoardGameNight>()
                .HasOne(bgn => bgn.Organizer)
                .WithMany()  // A Person can organize multiple game nights
                .HasForeignKey(bgn => bgn.OrganizerId)
                .OnDelete(DeleteBehavior.Cascade);  // Delete organized game nights when the organizer is deleted

            // Many-to-many relationship between Person and BoardGameNight for Participants
            modelBuilder.Entity<Person>()
                .HasMany(p => p.Participations)
                .WithMany(bgn => bgn.Participants)
                .UsingEntity<Dictionary<string, object>>(
                    "BoardGameNightParticipants",
                    j => j
                        .HasOne<BoardGameNight>()
                        .WithMany()
                        .HasForeignKey("BoardGameNightId")
                        .OnDelete(DeleteBehavior.Restrict),  // No action for BoardGameNight deletion
                    j => j
                        .HasOne<Person>()
                        .WithMany()
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Restrict) // No Action for Person deletion
                );
            // Relationship between Review and BoardGameNight
            modelBuilder.Entity<Review>()
                .HasOne(r => r.BoardGameNight)
                .WithMany(bgn => bgn.Reviews)
                .HasForeignKey(r => r.BoardGameNightId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent multiple cascade paths



            // Relationship between Review and Person (Reviewer)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Cascade);  // Optionally set to null instead of cascading delete

            // Define relationships here as needed
            modelBuilder.Entity<BoardGameNight>()
                .HasMany(bgn => bgn.BoardGames)
                .WithMany(bg => bg.BoardGameNights); // This will automatically create a join table

            // Ensure ReviewId is the primary key and is auto-incrementing
            modelBuilder.Entity<Review>()
                .Property(r => r.ReviewId)
                .ValueGeneratedOnAdd();  // Auto-increment identity column

            // Define ReviewId as the primary key (this is automatically done by EF if the name is 'ReviewId')
            modelBuilder.Entity<Review>()
                .HasKey(r => r.ReviewId);

        }

    }
}
