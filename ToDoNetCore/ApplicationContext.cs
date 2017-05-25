using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoNetCore.Models;

namespace ToDoNetCore
{
    public class ApplicationContext : DbContext
    {
        #region Application Context

        public DbSet<FileModel> File { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        #endregion
    }
}
