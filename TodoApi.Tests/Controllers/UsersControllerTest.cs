using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApi.Authentication;
using TodoApi.Controllers;
using Microsoft.AspNetCore.Http;

namespace TodoApi.Tests
{
    [TestClass]
    public class UsersControllerTest
    {
        private Mock<IJwtAuth> _mockIJwtAuth;
        private Mock<ICookieAuth> _mockICookieAuth;
        private Mock<IMyClaim> _mockIMyClaim;
        private Mock<ILogger<UsersController>> _mockILogger;
        private UsersController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockIJwtAuth = new Mock<IJwtAuth>();
            _mockICookieAuth = new Mock<ICookieAuth>();
            _mockIMyClaim = new Mock<IMyClaim>();
            _mockILogger = new Mock<ILogger<UsersController>>();
            _controller = new UsersController(_mockIJwtAuth.Object, _mockICookieAuth.Object,
                _mockIMyClaim.Object, _mockILogger.Object);
        }

        #region SignOutAsCookieAsync

        [TestMethod]
        public async Task SignOutAsCookieAsync_HappyPath_ShouldReturnNoContent()
        {
            _mockICookieAuth
                .Setup(auth => auth.SignOutAsync(It.IsAny<HttpContext>()))
                .Returns(Task.CompletedTask);
            
            var actionResult = await _controller.SignOutAsCookieAsync();

            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }

        #endregion
    }
}