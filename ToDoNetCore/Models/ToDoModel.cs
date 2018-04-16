using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ToDoNetCore.Models
{
    public class ToDoModel
    {
        [Key]
        public int TaskId { get; set; }
        [Required]
        [FromForm]
        [StringLength(60, MinimumLength = 5)]
        [Remote(action: "IsToDoExists", controller: "ToDo")]
        public string ShortName { get; set; }
        [Required]
        [FromForm]
        [StringLength(60, MinimumLength = 5)]
        public string Description { get; set; }
    }

    public class FileModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
