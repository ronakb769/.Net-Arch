using LearnArchitecture.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnArchitecture.Data.Context
{
    public class LearnArchitectureDbContext:DbContext
    {
        public LearnArchitectureDbContext(DbContextOptions<LearnArchitectureDbContext> options): base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<UserRoleMapping> UserRoleMapping { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<RolePermission> RolePermission { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
    }
}
