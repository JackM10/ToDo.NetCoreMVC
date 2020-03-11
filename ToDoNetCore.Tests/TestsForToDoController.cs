using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ToDoNetCore.Controllers;
using ToDoNetCore.Infrastructure.Cache;
using ToDoNetCore.Models;
using Xunit;

namespace ToDoNetCore.Tests
{
    [NonController]
    public class TestsForToDoController
    {
        [Fact]
        public async Task ListReturnAllValuesFromRepo()
        {
            //Arrange
            var data = (new List<ToDoModel> { new ToDoModel { ShortName = "testName", Description = "testDescription" } }).AsQueryable();
            Mock<IToDoRepository> mock = new Mock<IToDoRepository>();
            mock.SetupGet(s => s.ToDo).Returns(data);
            ToDoController controller = new ToDoController(mock.Object, null, null, new ToDoMemCache(), null);

            //Act
            var result = await controller.List();

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(data, result.ViewData.Model);
        }


        [Fact]
        public void IsToDoExistsValidationHelperMethod()
        {
            //Arrange
            var data = (new List<ToDoModel> { new ToDoModel { ShortName = "testName", Description = "testDescription" } }).AsQueryable();
            Mock<IToDoRepository> mock = new Mock<IToDoRepository>();
            mock.SetupGet(s => s.ToDo).Returns(data);

            ToDoController controller = new ToDoController(mock.Object, null, null, new ToDoMemCache(), null);

            //Act
            var result = controller.IsToDoExists(data.FirstOrDefault().ShortName);

            //Assert
            Assert.IsType<JsonResult>(result);
        }
    }
}
