using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoNetCore.Models
{
    public class ToDoModel
    {
        public int TaskId { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
    }
}
