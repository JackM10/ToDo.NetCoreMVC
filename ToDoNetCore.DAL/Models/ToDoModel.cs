using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoNetCore.DAL.Models
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
}
