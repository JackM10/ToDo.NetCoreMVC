using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoNetCore.Models;

namespace ToDoNetCore.API.Controllers
{
    [Route("api/[controller]")]
    public class ToDoApiController : Controller
    {
        #region Fields

        private IToDoRepository _context;

        #endregion

        #region Constructors

        public ToDoApiController(IToDoRepository context)
        {
            _context = context;
        }

        #endregion

        #region API Methods

        [HttpGet]
        public async Task<List<ToDoModel>> List() => await _context.ToDo.AsNoTracking().ToListAsync();

        #endregion
    }
}
