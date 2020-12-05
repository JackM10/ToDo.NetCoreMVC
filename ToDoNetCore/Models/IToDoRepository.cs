using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoNetCore.DAL.Models;

namespace ToDoNetCore.Models
{
    public interface IToDoRepository
    {
        IQueryable<ToDoModel> ToDo { get; }
        Task<ToDoModel> FindToDoInRepository(string toDoName);
        Task Update(ToDoModel editedToDo);
        Task Remove(ToDoModel toDoToDelete);
        Task Add(ToDoModel tdModel);
    }
}
