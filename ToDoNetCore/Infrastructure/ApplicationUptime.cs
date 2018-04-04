using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoNetCore.Infrastructure
{
    public class ApplicationUptime
    {
        private Stopwatch uptime;

        public ApplicationUptime() => uptime = Stopwatch.StartNew();

        public long Uptime() => uptime.ElapsedMilliseconds;
    }
}
