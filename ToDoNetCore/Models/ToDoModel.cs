using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoNetCore.Models
{
    public class ToDoModel
    {
        public int TaskId { get; set; }
        [StringLength(60, MinimumLength = 5)]
        [Required]
        public string ShortName { get; set; }
        [StringLength(60, MinimumLength = 5)]
        [Required]
        public string Description { get; set; }
    }
}
