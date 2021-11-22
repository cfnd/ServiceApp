using Microsoft.EntityFrameworkCore;
using ServiceApp.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceApp.DataLayer
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }
    }
}
