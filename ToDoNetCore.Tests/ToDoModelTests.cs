using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ToDoNetCore.Models;
using Xunit;

namespace ToDoNetCore.Tests
{
    public class ToDoModelTests
    {
        [Fact]
        public void CanCreateToDo()
        {
            //Arrange:
            var ToDoModelToCreate = new ToDoModel
            {
                TaskId = 100500,
                ShortName = "TaskForCanCreateToDoTest",
                Description = "description"
            };

            //Act:
            //Assert.Throws( todo with same Id exists )

            //Assert
            //Assert.Equal();
        }
    }
}
