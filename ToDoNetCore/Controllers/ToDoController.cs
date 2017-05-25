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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ToDoNetCore.Models;

namespace ToDoNetCore.Controllers
{
    public class ToDoController : Controller
    {
        #region Fields
        public static List<ToDoModel> ToDoList = new List<ToDoModel>
        {
            new ToDoModel { TaskId = 0, ShortName = "ToLearnMVC", Description = "I must gain Junior level in .NET MVC"},
            new ToDoModel { TaskId = 1, ShortName = "ToLearnJS", Description = "I must gain Trainee level in JavaScript"},
            new ToDoModel { TaskId = 2, ShortName = "ToLearnTFS", Description = "I must look how MS Team Foundation Server works"},
            new ToDoModel { TaskId = 3, ShortName = "ToExploreNode.JS", Description = "I must explore framework Node.JS"},
            new ToDoModel { TaskId = 4, ShortName = "ToExploreAngular.JS", Description = "I must explore framework Angular.JS"}
        };

        private IHostingEnvironment _enviroment;
        private ApplicationContext _context;
        private IHostingEnvironment _appEnvironment;

        #endregion

        #region Constructors

        public ToDoController(IHostingEnvironment environment)
        {
            _enviroment = environment;
        }

        #endregion

        #region Action Methods
        
        public IActionResult List()
        {
            return View(ToDoList);
        }

        public IActionResult Edit(int entityId, ToDoModel editedToDo = null)
        {
            //ViewBag.EntityID = entityId;
            //ViewBag.EntityName = ToDoList[entityId].ShortName;
            //ViewBag.EntityDescr = ToDoList[entityId].Description;
            if (editedToDo.ShortName != null && editedToDo.Description != null)
            {
                var toDoEntityToReplace = ToDoList.Find(p => p.TaskId == entityId);
                toDoEntityToReplace.ShortName = editedToDo.ShortName;
                toDoEntityToReplace.Description = editedToDo.Description;
                return RedirectToAction("List");
            }
            ModelState.ClearValidationState("ShortName");
            ModelState.ClearValidationState("Description");
            return View(ToDoList[entityId]);
        }

        public IActionResult Delete(string entityNameToRemove)
        {
            foreach (var td in ToDoList)
            {
                if (td.ShortName == entityNameToRemove)
                {
                    int idOfRemovingItem = td.TaskId;
                    ToDoList.RemoveAt(idOfRemovingItem);
                    RebuildList();
                    break;
                }
            }

            return RedirectToAction("List");
        }

        /// <summary>
        /// This method will recreate list of tasks, so it'll have correct ID's 
        /// (if we remove 2nd elemend, we'll have 0, 1, 3 list, this method will rebuild it to 0, 1, 2)
        /// </summary>
        private void RebuildList()
        {
            for (int i = 0; i < ToDoList.Count; i++)
            {
                ToDoList[i].TaskId = i;
            }
        }

        [ActionName("New")]
        public IActionResult CreateNewItem(string ShortName, string Description)
        {
            if (ShortName != null && Description != null)
            {
                if (ModelState.IsValid)
                {
                    int maxId = new int();
                    foreach (var td in ToDoList)
                    {
                        maxId = td.TaskId;
                    }
                    int idOfNewItem = ++maxId;
                    ToDoList.Add(new ToDoModel { TaskId = idOfNewItem, ShortName = ShortName, Description = Description });

                    return RedirectToAction("List");
                }

                return View(List());
            }

            return View();
        }

        public IActionResult ViewOneItem(int id)
        {
            foreach (var td in ToDoList)
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

        [HttpPost]
        public async Task<IActionResult> FileUpload(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                string pathToTheFileInApp = "/UploadedFiles/" + uploadedFile.FileName;
                //saving file in UploadedFiles folder in App:
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + pathToTheFileInApp, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                var file = new FileModel {Name = uploadedFile.FileName, Path = pathToTheFileInApp};
                _context.File.Add(file);
                _context.SaveChanges();
            }

            return RedirectToAction("List");
        }

        public IActionResult About()
        {
            return View();
        }

        #endregion
    }
}
