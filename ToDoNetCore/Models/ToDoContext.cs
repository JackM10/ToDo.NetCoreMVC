using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ToDoNetCore.Models
{
    public class ToDoContext : DbContext
    {
        public ToDoContext() : base() { }
        public DbSet<ToDoModel> Entities { get; set; }
    }
}
