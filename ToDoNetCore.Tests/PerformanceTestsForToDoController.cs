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
using ToDoNetCore.Infrastructure.Cache;
using Microsoft.EntityFrameworkCore;
using ToDoNetCore.DAL;
using ToDoNetCore.DAL.Models;

namespace ToDoNetCore.Tests
{
    public class PerformanceTestsForToDoController
    {
        private readonly ToDoContext _context;

        public PerformanceTestsForToDoController()
        {
            var ctxFactory = new ToDoContextFactory(""); //connection string via DI?
            _context = ctxFactory.CreateDbContext(null);
        }

        [Fact]
        public async Task ListReturnAllValuesFromRepoLessThan100ms()
        {
            //Arrange & Act
            var benchSummary = BenchmarkRunner.Run<ToDoControllersBenchmarks>();

            //get previous measurments:
            var previousMeasurments = await GetPreviousMeasurments();

            await SaveCurrentMeasurments();

            //If amount of measurments is small, warn about it
            var isMoreMeasurmentsRequired = DoWeHaveEnoughOfData(previousMeasurments);

            while (isMoreMeasurmentsRequired)
            {
                benchSummary = BenchmarkRunner.Run<ToDoControllersBenchmarks>();
                previousMeasurments = await GetPreviousMeasurments();
                isMoreMeasurmentsRequired = DoWeHaveEnoughOfData(previousMeasurments);
            }

            
            //Assert
            Assert.True(benchSummary.Reports[0].ResultStatistics.Mean < 100);

            bool DoWeHaveEnoughOfData(List<PerformanceMeasurment> data)
            {
                if (data.Count <= 50)
                {
                    return true;
                }
                return false;
            }

            async Task SaveCurrentMeasurments()
            {
                await _context.Measurments.AddAsync(new PerformanceMeasurment { Median = benchSummary.Reports[0].ResultStatistics.Mean, TestName = nameof(ListReturnAllValuesFromRepoLessThan100ms) });
                await _context.SaveChangesAsync();
            }

            async Task<List<PerformanceMeasurment>> GetPreviousMeasurments() =>
                await _context.Measurments.Where(m => m.TestName.Equals(nameof(ListReturnAllValuesFromRepoLessThan100ms))).ToListAsync();
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
            ToDoController controller = new ToDoController(mock.Object, null, null, new ToDoMemCache(), null);

            //Act
            var result = await controller.List();

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(data, result.ViewData.Model);
        }
    }
}
