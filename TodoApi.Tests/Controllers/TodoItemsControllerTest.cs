using Microsoft.VisualStudio.TestTools.UnitTesting;
using TodoApi.Controllers;
using TodoApi.Models;

namespace TodoApi.Tests
{
    [TestClass]
    public class TodoItemsControllerTest
    {
        #region ItemToDTO

        [TestMethod]
        public void ItemToDTO_HappyPath_ShouldReturnMatchObject()
        {
            var item = new TodoItem
            {
                Id = 2,
                Name = "n2",
                IsComplete = true,
                Secret = "s2"
            };
            var dto = TodoItemsController.ItemToDTO(item);
            Assert.AreEqual(2, dto.Id);
        }

        #endregion
    }
}
