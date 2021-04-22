using Microsoft.EntityFrameworkCore;
using PublicElections.Domain.Entities;
using System;

namespace PublicElections.Infrastructure.EntityFramework
{
    public static class DataInitializer
    {
        public static void InitializeData(this ModelBuilder builder)
        {
            builder.Entity<Election>()
                .HasData(new Election
                {
                    Id = 1,
                    Name = "Przykładowy obiekt bazy danych",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddHours(12)
                });
        }
    }
}
