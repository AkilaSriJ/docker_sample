using Genx.TrainTatkalBooking.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genx.TrainTatkalBooking.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Use your local MySQL port (the one mapped in docker-compose)
            optionsBuilder.UseMySql(
                "Server=localhost;Port=3307;Database=TrainTatkalDB;User=root;Password=;",
                new MySqlServerVersion(new Version(11, 5, 0))
            );

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
