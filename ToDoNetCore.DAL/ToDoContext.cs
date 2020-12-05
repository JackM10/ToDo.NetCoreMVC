using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoNetCore.DAL.Models;

namespace ToDoNetCore.DAL
{
    public class ToDoContext : DbContext
    {
        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options) { }

        public DbSet<ToDoModel> ToDo { get; set; }

        public DbSet<PerformanceMeasurment> Measurments { get; set; }
    }
}
