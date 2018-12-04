using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheRightDecision.Models;

namespace TheRightDecision.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Alternative> Alternatives { get; set; }
        public DbSet<Vector> Vectors { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<Criterion> Criteria { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{

        //}
    }
}
