using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ToDoNetCore.Controllers;
using ToDoNetCore.Models;
using Xunit;

namespace ToDoNetCore.Tests
{
    public class ToDoModelTests
    {
        [Fact]
        public void CanEditToDo()
        {
            //Arrange:
            var mock = MockRepository();

            ToDoModel newToDo = new ToDoModel {TaskId = 6, ShortName = "shortName6", Description = "description6"};

            ToDoController controller = new ToDoController(mock.Object, null, null);

            //Act:
            var result = (controller.Edit(newToDo.TaskId, newToDo).Result as ToDoModel);

            //Assert
            Assert.Equal(newToDo.TaskId, mock.Object.ToDo.FirstOrDefault().TaskId);
            Assert.Equal(newToDo.ShortName, mock.Object.ToDo.FirstOrDefault().ShortName);
            Assert.Equal(newToDo.Description, mock.Object.ToDo.FirstOrDefault().Description);
        }


        #region Mock Data

        private static Mock<IToDoRepository> MockRepository()
        {
            Mock<IToDoRepository> mock = new Mock<IToDoRepository>();
            mock.Setup(td => td.ToDo).Returns((new[]
            {
                new ToDoModel {TaskId = 1, ShortName = "shortName1", Description = "description1"},
                new ToDoModel {TaskId = 2, ShortName = "shortName2", Description = "description2"},
                new ToDoModel {TaskId = 3, ShortName = "shortName3", Description = "description3"},
                new ToDoModel {TaskId = 4, ShortName = "shortName4", Description = "description4"},
                new ToDoModel {TaskId = 5, ShortName = "shortName5", Description = "description5"}
            }).AsQueryable());
            return mock;
        }

        #endregion

    }
}
