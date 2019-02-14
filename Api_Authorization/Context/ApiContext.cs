using Api_Authorization.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Authorization
{
    public class ApiContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }

    }
}
