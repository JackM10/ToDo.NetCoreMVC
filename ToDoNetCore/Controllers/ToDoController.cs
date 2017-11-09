using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
//using System.Web.Caching;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ToDoNetCore.Models;

namespace ToDoNetCore.Controllers
{
    public class ToDoController : Controller
    {
        #region Fields

        private readonly ToDoContext _context;
        private IHostingEnvironment _appEnvironment;
        private readonly ILogger<ToDoController> _log;

        #endregion

        #region Constructors

        public ToDoController(ToDoContext context, IHostingEnvironment appEnvironment, ILogger<ToDoController> log)
        {
            _context = context;
            _appEnvironment = appEnvironment;
            _log = log;
        }

        #endregion

        #region Action Methods

        public async Task<IActionResult> List()
        {
            return View(await _context.ToDo.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int entityId, [Bind("entityId")] ToDoModel editedToDo)
        {
            if (entityId != editedToDo.TaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(editedToDo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (ToDoExist(editedToDo.TaskId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(List));
            }

            return View(editedToDo);

            //if (editedToDo.ShortName != null && editedToDo.Description != null)
            //{
            //    var toDoEntityToReplace = await _context.ToDo.SingleOrDefaultAsync(p => p.TaskId == entityId);
            //    toDoEntityToReplace.ShortName = editedToDo.ShortName;
            //    toDoEntityToReplace.Description = editedToDo.Description;
            //    return RedirectToAction("List");
            //}
            //ModelState.ClearValidationState("ShortName");
            //ModelState.ClearValidationState("Description");
            //return View(ToDoList[entityId]);
        }

        private bool ToDoExist(int taskId)
        {
            return _context.ToDo.Any(e => e.TaskId == taskId);
        }

        public async Task<IActionResult> Delete(string entityNameToRemove)
        {
            if (entityNameToRemove == null)
            {
                return NotFound();
            }

            var todo = await _context.ToDo.SingleOrDefaultAsync(m => m.ShortName == entityNameToRemove);
            _context.ToDo.Remove(todo);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }

        public async Task<IActionResult> New([Bind("ShortName,Description")] ToDoModel tdModel, IFormFile uploadedFile)
        {
            if (!ModelState.IsValid)
            {
                return View(tdModel);
            }

            _context.Add(tdModel);

            if (uploadedFile != null)
            {
                string path = "/Files/" + uploadedFile.FileName;
                using (var filestraem = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(filestraem);
                }
                FileModel file = new FileModel { Name = uploadedFile.FileName, Path = path };
                _context.File.Add(file);
                _context.SaveChanges();
            }
            await _context.SaveChangesAsync();
            TempData["ToDoIsCreated"] = true;
            return RedirectToAction(nameof(List));
        }

        public IActionResult ViewOneItem(int id)
        {
            foreach (var td in _context.ToDo)
            {
                if (td.TaskId == id)
                {
                    ViewBag.EntityDescrToView = td.Description;
                    ViewBag.EntityNameToView = td.ShortName;
                    break;
                }
            }
            ViewBag.EntityIdToView = id;

            return View();
        }

        [ResponseCache(Duration = 30)]
        //  try to click on ClientCached link and you will see that it is cached:
        // explanation: During a browser session, browsing multiple pages within the website or using back and forward button to visit the pages, 
        // content will be served from the local browser cache (if not expired). 
        // But when page is refreshed via F5 , the request will be go to the server and page content will get refreshed. 
        // You can verify it via refreshing contact page using F5. So when you hit F5, response caching expiration value has no role to play to serve the content. 
        // You should see 200 response for contact request.
        // http://www.talkingdotnet.com/response-caching-in-asp-net-core-1-1/
        public IActionResult ClientCached()
        {
            _log.LogInformation("hit!");
            ViewBag.CachedTime = DateTime.Now.ToString();
            return View();
        }

        #endregion

        #region Helpers

        [AcceptVerbs("Get", "Post")]
        public IActionResult IsToDoExists(string shortName)
        {
            if (_context.ToDo.Any(td => td.ShortName == shortName))
            {
                return Json(data: $"ToDo with the same name already in DB.");
            }

            return Json(data: true);
        }

        #endregion
    }
}
