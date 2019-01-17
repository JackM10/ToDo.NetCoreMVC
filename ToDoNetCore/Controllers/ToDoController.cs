using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using ToDoNetCore.Infrastructure;
using ToDoNetCore.Infrastructure.Cache;
using ToDoNetCore.Models;

namespace ToDoNetCore.Controllers
{
    public class ToDoController : Controller
    {
        #region Fields

        private IToDoRepository _context;
        private ApplicationUptime appUptime;
        private IHostingEnvironment _appEnvironment;
        private readonly ILogger<ToDoController> _log;
        //private IMemoryCache _cache;
        private MemoryCache _cache;

        #endregion

        #region Constructors

        public ToDoController(IToDoRepository context,
                              IHostingEnvironment appEnvironment,
                              ILogger<ToDoController> log,
                              //IMemoryCache cache,
                              ToDoMemCache cache,
                              ApplicationUptime up)
        {
            _context = context;
            _appEnvironment = appEnvironment;
            _log = log;
            _cache = cache.Cache;
            appUptime = up;
        }

        #endregion

        #region Action Methods

        public async Task<ViewResult> List() => View(await _context.ToDo.AsNoTracking().ToListAsync());

        [Authorize]
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
        [Authorize]
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
                    if (await ToDoExist(editedToDo.TaskId))
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

        private async Task<bool> ToDoExist(int taskId) =>
            await _context.ToDo.AsNoTracking().AnyAsync(e => e.TaskId == taskId);

        [Authorize]
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
        public async Task<IActionResult> New(ToDoModel tdModel, IFormFile uploadedFile)
        {
            if (!ModelState.IsValid)
            {
                return View(tdModel);
            }

            var saveToDoTask = _context.Add(tdModel);

            if (uploadedFile != null)
            {
                // full path to file in temp location
                var filePath = Path.Combine(_appEnvironment.WebRootPath, uploadedFile.FileName);

                if (uploadedFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(stream);
                    }
                }
            }

            await saveToDoTask;
            TempData["ToDoIsCreated"] = "ToDo Sucesfully created!";

            return RedirectToAction(nameof(List));
        }

        public async Task<IActionResult> ViewOneItem(int id)
        {
            ToDoModel selectedToDo;

            if (!_cache.TryGetValue(CacheKeys.Entry, out selectedToDo))
            {
                selectedToDo = await _context.ToDo.AsNoTracking().Where(t => t.TaskId == id).FirstOrDefaultAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSize(128)
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));
                _cache.Set(CacheKeys.Entry, selectedToDo, cacheEntryOptions);
            }
            if (selectedToDo != null)
            {
                return PartialView("ViewOneItem", selectedToDo);
            }

            return NotFound();
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
            ViewBag.AppUptime = $"{appUptime.Uptime()}ms";
            _log.LogInformation("hit!");
            ViewBag.CachedTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            return View();
        }

        [Authorize]
        public IActionResult UserInfo() => View(GetUserData(nameof(UserInfo)));

        #endregion

        #region Exception Handling

        public IActionResult Errors(string errorCode)
        {
            if (errorCode == "404" | errorCode == "500")
            {
                return View($"~/Views/ToDo/dnserror[1].html");
            }

            return View($"~/StaticPages/IE_PageNotFound/dnserror.html");
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Check if the user exists in DB or not
        /// </summary>
        /// <param name="shortName"></param>
        /// <returns></returns>
        [AcceptVerbs("Get", "Post")]
        public IActionResult IsToDoExists(string shortName)
        {
            if (_context.ToDo.AsNoTracking().Any(td => td.ShortName == shortName))
            {
                return Json(data: $"ToDo with the same name already in DB.");
            }

            return Json(data: true);
        }


        private Dictionary<string, object> GetUserData(string actionName) => new Dictionary<string, object>(4)
        {
            ["User"] = HttpContext.User.Identity.Name,
            ["Authenticated"] = HttpContext.User.Identity.IsAuthenticated,
            ["Auth Type"] = HttpContext.User.Identity.AuthenticationType,
            ["In Jack Role"] = HttpContext.User.IsInRole("JackRole")
        };

        #endregion
    }
}
