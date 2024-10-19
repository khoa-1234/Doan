using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLNH.Data.EF
{
    public class QLNHDbContext : DbContext
    {
        public QLNHDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
