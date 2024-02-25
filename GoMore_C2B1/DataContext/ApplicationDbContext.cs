using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using GoMore_C2B1.Models;

namespace GoMore_C2B1.DataContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base(nameOrConnectionString: "BIES_DB")
        {
        }
        public virtual DbSet<USERCLASS> USER { get; set; }
    }
}