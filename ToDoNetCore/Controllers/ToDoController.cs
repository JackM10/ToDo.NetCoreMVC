﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
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

        #endregion

        public IActionResult List()
        {
            return View(ToDoList);
        }

        public IActionResult Edit(int entityId, string editedName, string editedDescr)
        {
            ViewBag.EntityID = entityId;
            ViewBag.EntityName = ToDoList[entityId].ShortName;
            ViewBag.EntityDescr = ToDoList[entityId].Description;
            if (editedName != null && editedDescr != null)
            {
                ToDoList.Find(p => p.TaskId == entityId).ShortName = editedName;
                ToDoList.Find(p => p.TaskId == entityId).Description = editedDescr;
                return RedirectToAction("List");
            }
            return View();
        }

        public IActionResult Delete(int entityId)
        {
            ToDoList.RemoveAt(entityId);
            RebuildList();
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
        public IActionResult CreateNewItem(string newEntityName, string newEntityDescr)
        {
            int maxId = new int();
            foreach (var td in ToDoList)
            {
                maxId = td.TaskId;
            }
            int idOfNewItem = ++maxId;

            if (newEntityName != null && newEntityDescr != null)
            {
                ToDoList.Add(new ToDoModel{TaskId = idOfNewItem, ShortName = newEntityName, Description = newEntityDescr});

                return RedirectToAction("List");
            }
            return View();
        }
    }
}