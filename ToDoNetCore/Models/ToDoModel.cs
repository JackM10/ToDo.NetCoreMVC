using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ToDoNetCore.Models
{
    public class ToDoModel
    {
        [Key]
        public int TaskId { get; set; }
        [StringLength(60, MinimumLength = 5)]
        [Required]
        public string ShortName { get; set; }
        [StringLength(60, MinimumLength = 5)]
        [Required]
        public string Description { get; set; }
    }

    public class FileModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }

    public class ToDoContext : DbContext
    {
        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options) { }

        public DbSet<ToDoModel> ToDo { get; set; }
        public DbSet<FileModel> File { get; set; }
    }
}
