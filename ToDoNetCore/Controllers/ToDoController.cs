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
using ToDoNetCore.Models;

namespace ToDoNetCore.Controllers
{
    public class ToDoController : Controller
    {
        #region Fields

        //public static List<ToDoModel> ToDoList = new List<ToDoModel>
        //{
        //    new ToDoModel { TaskId = 0, ShortName = "ToLearnMVC", Description = "I must gain Junior level in .NET MVC"},
        //    new ToDoModel { TaskId = 1, ShortName = "ToLearnJS", Description = "I must gain Trainee level in JavaScript"},
        //    new ToDoModel { TaskId = 2, ShortName = "ToLearnTFS", Description = "I must look how MS Team Foundation Server works"},
        //    new ToDoModel { TaskId = 3, ShortName = "ToExploreNode.JS", Description = "I must explore framework Node.JS"},
        //    new ToDoModel { TaskId = 4, ShortName = "ToExploreAngular.JS", Description = "I must explore framework Angular.JS"}
        //};

        private readonly ToDoContext _context;
        private IHostingEnvironment _appEnvironment;

        #endregion

        #region Constructors

        public ToDoController(ToDoContext context, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
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

        /// <summary>
        /// This method will recreate list of tasks, so it'll have correct ID's 
        /// (if we remove 2nd elemend, we'll have 0, 1, 3 list, this method will rebuild it to 0, 1, 2)
        /// </summary>
        //private void RebuildList()
        //{
        //    for (int i = 0; i < ToDoList.Count; i++)
        //    {
        //        ToDoList[i].TaskId = i;
        //    }
        //}

        public async Task<IActionResult> New([Bind("TaskId,ShortName,Description")] ToDoModel tdModel, IFormFile uploadedFile)
        {
            if (!ModelState.IsValid)
            {
                return View(tdModel);
            }

            tdModel.TaskId = 15;

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
            return RedirectToAction(nameof(List));
        }

        //[ActionName("New")]
        //public IActionResult CreateNewItem(string ShortName, string Description)
        //{
        //    if (ShortName != null && Description != null)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            int maxId = new int();
        //            foreach (var td in ToDoList)
        //            {
        //                maxId = td.TaskId;
        //            }
        //            int idOfNewItem = ++maxId;
        //            ToDoList.Add(new ToDoModel { TaskId = idOfNewItem, ShortName = ShortName, Description = Description });

        //            return RedirectToAction("List");
        //        }

        //        return View(List());
        //    }

        //    return View();
        //}

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

        [ResponseCache(Duration = Int32.MaxValue)]
        public IActionResult About()
        {
            ViewBag.CachedTime = DateTime.Now.Second.ToString();
            return View();
        }

        #endregion
    }
}
