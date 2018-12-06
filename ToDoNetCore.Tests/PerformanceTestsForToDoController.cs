using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ToDoNetCore.Controllers;
using ToDoNetCore.Models;
using Xunit;
using System.Threading.Tasks;

namespace ToDoNetCore.Tests
{
    public class PerformanceTestsForToDoController
    {
        [Fact]
        public void ListReturnAllValuesFromRepoLessThan100ms()
        {
            //Arrange & Act
            var benchSummary = BenchmarkRunner.Run<ToDoControllersBenchmarks>();
            
            //Assert
            Assert.True(benchSummary.Reports[0].ResultStatistics.Mean < 100);
        }
    }

    public class ToDoControllersBenchmarks
    {
        [Benchmark]
        public async Task ReturnAllValuesFromRepo()
        {
            //Arrange
            var data = (new List<ToDoModel> { new ToDoModel { ShortName = "testName", Description = "testDescription" } }).AsQueryable();
            Mock<IToDoRepository> mock = new Mock<IToDoRepository>();
            mock.SetupGet(s => s.ToDo).Returns(data);
            ToDoController controller = new ToDoController(mock.Object, null, null, null);

            //Act
            var result = await controller.List();

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(data, result.ViewData.Model);
        }
    }
}
