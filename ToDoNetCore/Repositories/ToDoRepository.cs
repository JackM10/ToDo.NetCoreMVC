using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoNetCore.Models;

namespace ToDoNetCore.Repositories
{
    public class ToDoRepository
    {
        private IToDoRepository _context { get; }

        public ToDoRepository(IToDoRepository context)
        {
            _context = context;
        }
        public async Task<List<ToDoModel>> GetAllToDos()
        {
            return await _context.ToDo.AsNoTracking().ToListAsync();
        }

        //ToDo: to move all queries from ToDo Controller to this repo
    }
}
