using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoNetCore.DAL
{
    public class ToDoContextFactory : IDesignTimeDbContextFactory<ToDoContext>
    {
        private readonly string _connectionString;

        public ToDoContextFactory() { }

        public ToDoContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ToDoContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ToDoContext>();
            builder.UseSqlServer(_connectionString);

            return new ToDoContext(builder.Options);
        }
    }
}
