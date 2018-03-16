using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ToDoNetCore.Models;

namespace ToDoNetCore.Controllers
{
    public class ToDoController : Controller
    {
        #region Fields

        private IToDoRepository _context;
        private IHostingEnvironment _appEnvironment;
        private readonly ILogger<ToDoController> _log;

        #endregion

        #region Constructors

        public ToDoController(IToDoRepository context, IHostingEnvironment appEnvironment, ILogger<ToDoController> log)
        {
            _context = context;
            _appEnvironment = appEnvironment;
            _log = log;
        }

        #endregion

        #region Action Methods

        public async Task<IActionResult> List() => View(await _context.ToDo.ToListAsync());
        
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var todo = await _context.ToDo.SingleOrDefaultAsync(m => m.TaskId == id);
            if (todo == null)
            {
                return NotFound();
            }
            return View(todo);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int TaskId, ToDoModel editedToDo)
        {
            if (TaskId != editedToDo.TaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Update(editedToDo);
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

            var todo = await _context.FindToDoInRepository(entityNameToRemove);
            await _context.Remove(todo);

            return RedirectToAction(nameof(List));
        }
        
        public ViewResult New() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New([Bind("ShortName,Description")] ToDoModel tdModel, IFormFile uploadedFile)
        {
            if (!ModelState.IsValid)
            {
                return View(tdModel);
            }

            await _context.Add(tdModel);

            if (uploadedFile != null)
            {
                //ToDo: After using of IToDoRepo interface it's required to rebuild file upload functionality
                //string path = "/Files/" + uploadedFile.FileName;
                //using (var filestraem = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                //{
                //    await uploadedFile.CopyToAsync(filestraem);
                //}
                //FileModel file = new FileModel { Name = uploadedFile.FileName, Path = path };
                //_context.File.Add(file);
                //_context.SaveChanges();
            }
            TempData["ToDoIsCreated"] = "ToDo Sucesfully created!";
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
            ViewBag.CachedTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            return View();
        }

        #endregion

        #region Handling Http Errors

        public IActionResult Errors(string errorCode)
        {
            if (errorCode == "404" | errorCode == "500")
            {
                return View($"~/Views/ToDo/dnserror[1].html");
            }

            return View($"~/Views/ToDo/dnserror[1].html");
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
