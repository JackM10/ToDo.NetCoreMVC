using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoNetCore.Infrastructure.Cache
{
    public class ToDoMemCache
    {
        public MemoryCache Cache { get; set; }
        public ToDoMemCache()
        {
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 8192
            });
        }
    }
}
