using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ToDoNetCore.DAL;
using ToDoNetCore.DAL.Models;

namespace ToDoNetCore.Models
{
    public class EFToDoRepository : IToDoRepository
    {
        private ToDoContext context;

        public EFToDoRepository(ToDoContext ctx)
        {
            context = ctx;
        }

        public IQueryable<ToDoModel> ToDo => context.ToDo;
        public async Task<ToDoModel> FindToDoInRepository(string toDoName)
        {
            ToDoModel toDo = await context.ToDo.SingleOrDefaultAsync(m => m.ShortName == toDoName);
            //ToDo: if no such todo in Repo - send error message somehow
            return toDo;
        }

        public async Task Update(ToDoModel editedToDo)
        {
            context.ToDo.Update(editedToDo);
            await SaveChangesAsync();
        }

        public async Task Remove(ToDoModel toDoToDelete)
        {
            var parammTaskId = new SqlParameter("@ToDoId", toDoToDelete.TaskId);
        }

        public async Task Add(ToDoModel tdModel)
        {
            context.Add(tdModel);
            await SaveChangesAsync();
        }

        private async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
