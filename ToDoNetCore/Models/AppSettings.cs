using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoNetCore.Models
{
    public class AppSettings
    {
        public ApplicationConfigurations ApplicationConfigurations { get; set; }
    }

    public class ApplicationConfigurations
    {
        public string FooterMessage { get; set; }
    }
}
