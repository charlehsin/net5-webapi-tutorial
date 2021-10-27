using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        }

        #region GetTodoItems

        [TestMethod]
        public async Task GetTodoItems_HappyPath_ShouldReturnMatchingList()
        {
            _mockITodoItemsRepository
                .Setup(repo => repo.GetAllAsync().Result)
                .Returns(new List<TodoItem> {
                    new TodoItem
                    {
                        Id = 1,
                        Name = "N1",
                        IsComplete = true,
                        Secret = "S1"
                    }
                });

            var actionResult = await _controller.GetTodoItems();

            IEnumerable<TodoItemDTO> enumerable = actionResult.Value as IEnumerable<TodoItemDTO>;
            var result = enumerable.ToList();
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("N1", result[0].Name);
            Assert.AreEqual(true, result[0].IsComplete);
        }

        #endregion

        #region GetTodoItemById

        [TestMethod]
        public async Task GetTodoItemById_DoesNotExist_ShouldReturnNotFound()
        {
            _mockITodoItemsRepository
                .Setup(repo => repo.FindAsync(It.IsAny<long>()))
                .Returns(Task.FromResult<TodoItem>(null));

            var actionResult = await _controller.GetTodoItemById(1);
            
            Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetTodoItemById_Exist_ShouldReturnMatchingItem()
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

            var actionResult = await _controller.GetTodoItemById(1);
            var result = actionResult.Value as TodoItemDTO;
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("N1", result.Name);
            Assert.AreEqual(true, result.IsComplete);
        }

        #endregion

        #region UpdateTodoItem

        [TestMethod]
        public async Task UpdateTodoItem_MismatchingId_ShouldReturnBadRequest()
        {
            var actionResult = await _controller.UpdateTodoItem(1,
                new TodoItemDTO
                {
                    Id = 2
                });
            
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task UpdateTodoItem_DoesNotExist_ShouldReturnNotFound()
        {
            _mockITodoItemsRepository
                .Setup(repo => repo.FindAsync(It.IsAny<long>()))
                .Returns(Task.FromResult<TodoItem>(null));

            var actionResult = await _controller.UpdateTodoItem(1,
                new TodoItemDTO
                {
                    Id = 1
                });
            
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task UpdateTodoItem_HappyPath_ShouldReturnNoContent()
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

            var actionResult = await _controller.UpdateTodoItem(1,
                new TodoItemDTO
                {
                    Id = 1
                });
            
            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task UpdateTodoItem_ThrowTargetExceptionAndDoesNotExist_ShouldReturnNotFound()
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

            var actionResult = await _controller.UpdateTodoItem(1,
                new TodoItemDTO
                {
                    Id = 1
                });
            
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        #endregion

        #region CreateTodoItem

        [TestMethod]
        public async Task CreateTodoItem_HappyPath_ShouldReturnCreatedAtAction()
        {
            _mockITodoItemsRepository
                .Setup(repo => repo.AddAsync(It.IsAny<TodoItem>()))
                .Returns(Task.CompletedTask);

            var actionResult = await _controller.CreateTodoItem(new TodoItemDTO
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

        #region DeleteTodoItem

        [TestMethod]
        public async Task DeleteTodoItem_DoesNotExist_ShouldReturnNotFound()
        {
            _mockITodoItemsRepository
                .Setup(repo => repo.FindAsync(It.IsAny<long>()))
                .Returns(Task.FromResult<TodoItem>(null));

            var actionResult = await _controller.DeleteTodoItem(1);

            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeleteTodoItem_Exist_ShouldReturnNoContent()
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

            var actionResult = await _controller.DeleteTodoItem(1);

            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }

        #endregion
    }
}
