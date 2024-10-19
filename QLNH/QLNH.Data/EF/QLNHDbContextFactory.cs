using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNH.Data.EF
{
    public class QLNHDbContextFactory:IDesignTimeDbContextFactory<QLNHDbContext>
    {
        public QLNHDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration= new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration.GetConnectionString("QLNHDB");
            var optionsBuilder = new DbContextOptionsBuilder<QLNHDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            return new QLNHDbContext(optionsBuilder.Options);
        }
    }
}
