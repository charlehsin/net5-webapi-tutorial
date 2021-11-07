using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Moq;
using TodoApi.Authentication;
using TodoApi.Controllers;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Tests
{
    [TestClass]
    public class TodoItemsControllerTest
    {
        private Mock<IMyClaim> _mockIMyClaim;
        private Mock<ITodoItemsRepository> _mockITodoItemsRepository;
        private Mock<ILogger<TodoItemsController>> _mockILogger;
        private TodoItemsController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockIMyClaim = new Mock<IMyClaim>();
            _mockITodoItemsRepository = new Mock<ITodoItemsRepository>();
            _mockILogger = new Mock<ILogger<TodoItemsController>>();
            _controller = new TodoItemsController(_mockIMyClaim.Object, _mockITodoItemsRepository.Object,
                _mockILogger.Object);

            _mockIMyClaim
                .Setup(myClaim => myClaim.ParseAuthClaim(It.IsAny<HttpContext>()))
                .Returns(new ParsedClaim
                {
                    UserName = "tester",
                    Roles = new List<string>()
                });
        }

        #region GetTodoItemsAsync

        [TestMethod]
        public async Task GetTodoItemsAsync_HappyPath_ShouldReturnMatchingList()
        {
            _mockITodoItemsRepository
                .Setup(repo => repo.GetAllAsync().Result)
                .Returns(new List<TodoItem>
                {
                    new TodoItem
                    {
                        Id = 1,
                        Name = "N1",
                        IsComplete = true,
                        Secret = "S1"
                    }
                });

            var actionResult = await _controller.GetTodoItemsAsync();

            IEnumerable<TodoItemDTO> enumerable = actionResult.Value as IEnumerable<TodoItemDTO>;
            var result = enumerable.ToList();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("N1", result[0].Name);
            Assert.AreEqual(true, result[0].IsComplete);
        }

        #endregion

        #region GetTodoItemByIdAsync

        [TestMethod]
        public async Task GetTodoItemByIdAsync_DoesNotExist_ShouldReturnNotFound()
        {
            _mockITodoItemsRepository
                .Setup(repo => repo.FindAsync(It.IsAny<long>()))
                .Returns(Task.FromResult<TodoItem>(null));

            var actionResult = await _controller.GetTodoItemByIdAsync(1);

            Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetTodoItemByIdAsync_Exist_ShouldReturnMatchingItem()
        {
            _mockITodoItemsRepository
                .Setup(repo => repo.FindAsync(1).Result)
                .Returns(
                    new TodoItem
                    {
                        Id = 1,
                        Name = "N1",
                        IsComplete = true,
                        Secret = "S1"
                    });

            var actionResult = await _controller.GetTodoItemByIdAsync(1);
            var result = actionResult.Value as TodoItemDTO;
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("N1", result.Name);
            Assert.AreEqual(true, result.IsComplete);
        }

        #endregion

        #region UpdateTodoItemAsync

        [TestMethod]
        public async Task UpdateTodoItemAsync_MismatchingId_ShouldReturnBadRequest()
        {
            var actionResult = await _controller.UpdateTodoItemAsync(1,
                new TodoItemDTO
                {
                    Id = 2
                });

            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task UpdateTodoItemAsync_DoesNotExist_ShouldReturnNotFound()
        {
            _mockITodoItemsRepository
                .Setup(repo => repo.FindAsync(It.IsAny<long>()))
                .Returns(Task.FromResult<TodoItem>(null));

            var actionResult = await _controller.UpdateTodoItemAsync(1,
                new TodoItemDTO
                {
                    Id = 1
                });

            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task UpdateTodoItemAsync_HappyPath_ShouldReturnNoContent()
        {
            _mockITodoItemsRepository
                .Setup(repo => repo.FindAsync(1).Result)
                .Returns(
                    new TodoItem
                    {
                        Id = 1,
                        Name = "N1",
                        IsComplete = true,
                        Secret = "S1"
                    });
            _mockITodoItemsRepository
                .Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var actionResult = await _controller.UpdateTodoItemAsync(1,
                new TodoItemDTO
                {
                    Id = 1
                });

            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task UpdateTodoItemAsync_ThrowTargetExceptionAndDoesNotExist_ShouldReturnNotFound()
        {
            _mockITodoItemsRepository
                .Setup(repo => repo.FindAsync(1).Result)
                .Returns(
                    new TodoItem
                    {
                        Id = 1,
                        Name = "N1",
                        IsComplete = true,
                        Secret = "S1"
                    });
            _mockITodoItemsRepository
                .Setup(repo => repo.SaveChangesAsync())
                .ThrowsAsync(new DbUpdateConcurrencyException());
            _mockITodoItemsRepository
                .Setup(repo => repo.DoesItemExist(1))
                .Returns(false);

            var actionResult = await _controller.UpdateTodoItemAsync(1,
                new TodoItemDTO
                {
                    Id = 1
                });

            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        #endregion

        #region CreateTodoItemAsync

        [TestMethod]
        public async Task CreateTodoItemAsync_HappyPath_ShouldReturnCreatedAtAction()
        {
            _mockITodoItemsRepository
                .Setup(repo => repo.AddAsync(It.IsAny<TodoItem>()))
                .Returns(Task.CompletedTask);

            var actionResult = await _controller.CreateTodoItemAsync(new TodoItemDTO
            {
                Id = 1,
                Name = "N1",
                IsComplete = true
            });

            Assert.IsInstanceOfType(actionResult.Result, typeof(CreatedAtActionResult));

            var result = actionResult.Result as CreatedAtActionResult;
            var item = result.Value as TodoItemDTO;
            Assert.AreEqual(1, item.Id);
            Assert.AreEqual("N1", item.Name);
            Assert.AreEqual(true, item.IsComplete);
        }

        #endregion

        #region DeleteTodoItemAsync

        [TestMethod]
        public async Task DeleteTodoItemAsync_DoesNotExist_ShouldReturnNotFound()
        {
            _mockITodoItemsRepository
                .Setup(repo => repo.FindAsync(It.IsAny<long>()))
                .Returns(Task.FromResult<TodoItem>(null));

            var actionResult = await _controller.DeleteTodoItemAsync(1);

            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeleteTodoItemAsync_Exist_ShouldReturnNoContent()
        {
            _mockITodoItemsRepository
                .Setup(repo => repo.FindAsync(1).Result)
                .Returns(
                    new TodoItem
                    {
                        Id = 1,
                        Name = "N1",
                        IsComplete = true,
                        Secret = "S1"
                    });
            _mockITodoItemsRepository
                .Setup(repo => repo.RemoveAsync(It.IsAny<TodoItem>()))
                .Returns(Task.CompletedTask);

            var actionResult = await _controller.DeleteTodoItemAsync(1);

            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }

        #endregion
    }
}
