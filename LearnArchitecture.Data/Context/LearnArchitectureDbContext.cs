using LearnArchitecture.Core.Entities;
using LearnArchitecture.Core.Models.ResponseModel;
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
        public DbSet<LoginHistory> LoginHistory { get; set; }   
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<ResultModel> resultModels { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResultModel>().HasNoKey(); // ✅ This tells EF Core it's keyless
        }
    }
}
