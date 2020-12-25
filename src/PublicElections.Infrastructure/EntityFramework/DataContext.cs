using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PublicElections.Domain.Entities;

namespace PublicElections.Infrastructure.EntityFramework
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }

        public DbSet<Election> Elections { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Participation> Participations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Participation>()
                .HasIndex(p => new { p.ElectionId, p.UserId });

            builder.Entity<Vote>()
                .HasOne(v => v.Election)
                .WithMany()
                .HasForeignKey(v => v.ElectionId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Vote>()
                .HasOne(v => v.Candidate)
                .WithMany()
                .HasForeignKey(v => v.CandidateId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.InitializeData();

            base.OnModelCreating(builder);
        }
    }
}
