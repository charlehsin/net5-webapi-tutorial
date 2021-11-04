using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using TodoApi.Authentication;
using TodoApi.Controllers;
using TodoApi.Identity;
using TodoApi.Models;
using System.Net;

namespace TodoApi.Tests
{
    [TestClass]
    public class UsersControllerTest
    {
        private Mock<IJwtAuth> _mockIJwtAuth;
        private Mock<ICookieAuth> _mockICookieAuth;
        private Mock<IMyClaim> _mockIMyClaim;
        private Mock<ILogger<UsersController>> _mockILogger;
        private Mock<IUserManagerWrapper> _mockIUserManagerWrapper;
        private Mock<IRoleManagerWrapper> _mockIRoleManagerWrapper;
        private UsersController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockIJwtAuth = new Mock<IJwtAuth>();
            _mockICookieAuth = new Mock<ICookieAuth>();
            _mockIMyClaim = new Mock<IMyClaim>();
            _mockILogger = new Mock<ILogger<UsersController>>();
            _mockIUserManagerWrapper = new Mock<IUserManagerWrapper>();
            _mockIRoleManagerWrapper = new Mock<IRoleManagerWrapper>();
            _controller = new UsersController(_mockIJwtAuth.Object, _mockICookieAuth.Object,
                _mockIMyClaim.Object, _mockILogger.Object,
                _mockIUserManagerWrapper.Object, _mockIRoleManagerWrapper.Object);

            _mockIMyClaim
                .Setup(myClaim => myClaim.ParseAuthClaim(It.IsAny<HttpContext>()))
                .Returns(new ParsedClaim
                {
                    UserName = "tester",
                    Roles = new List<string>()
                });
        }

        #region CreateAsync

        [TestMethod]
        public async Task CreateAsync_AlreadyExists_ShouldReturnConflict()
        {
            _mockIUserManagerWrapper
                .Setup(wrapper => wrapper.FindByNameAsync(It.IsAny<string>()).Result)
                .Returns(new AppUser());

            var actionResult = await _controller.CreateAsync(new UserRegister());

            Assert.IsInstanceOfType(actionResult.Result, typeof(ConflictResult));
        }

        [TestMethod]
        public async Task CreateAsync_FailsToCreate_ShouldReturnStatus500()
        {
            _mockIUserManagerWrapper
                .Setup(wrapper => wrapper.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult<AppUser>(null));

            _mockIUserManagerWrapper
                .Setup(wrapper => wrapper.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()).Result)
                .Returns(IdentityResult.Failed());

            var actionResult = await _controller.CreateAsync(new UserRegister());

            Assert.IsInstanceOfType(actionResult.Result, typeof(ObjectResult));
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, (actionResult.Result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task CreateAsync_DoesNotExistAfterCreation_ShouldReturnStatus500()
        {
            _mockIUserManagerWrapper
                .SetupSequence(wrapper => wrapper.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult<AppUser>(null))
                .Returns(Task.FromResult<AppUser>(null));

            _mockIUserManagerWrapper
                .Setup(wrapper => wrapper.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()).Result)
                .Returns(IdentityResult.Success);

            var actionResult = await _controller.CreateAsync(new UserRegister());

            Assert.IsInstanceOfType(actionResult.Result, typeof(ObjectResult));
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, (actionResult.Result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task CreateAsync_HapptyPath_ShouldReturnCreatedAtAction()
        {
            _mockIUserManagerWrapper
                .SetupSequence(wrapper => wrapper.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult<AppUser>(null))
                .Returns(Task.FromResult(new AppUser
                {
                    Id = "Id1",
                    UserName = "UserName1",
                    Email = "Email1"
                }));

            _mockIUserManagerWrapper
                .Setup(wrapper => wrapper.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()).Result)
                .Returns(IdentityResult.Success);

            _mockIRoleManagerWrapper
                .Setup(wrapper => wrapper.RoleExistsAsync(It.IsAny<string>()).Result)
                .Returns(true);

            _mockIUserManagerWrapper
                .Setup(wrapper => wrapper.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()).Result)
                .Returns(IdentityResult.Success);

            var actionResult = await _controller.CreateAsync(new UserRegister());

            Assert.IsInstanceOfType(actionResult.Result, typeof(CreatedAtActionResult));

            var result = actionResult.Result as CreatedAtActionResult;
            var user = result.Value as User;
            Assert.AreEqual("Id1", user.Id);
            Assert.AreEqual("UserName1", user.UserName);
            Assert.AreEqual("Email1", user.Email);
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public async Task DeleteAsync_DoesNotExist_ShouldReturnNotFound()
        {
            _mockIUserManagerWrapper
                .Setup(wrapper => wrapper.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult<AppUser>(null));

            var actionResult = await _controller.DeleteAsync("Id1");

            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeleteAsync_FailsToDelete_ShouldReturnStatus500()
        {
            _mockIUserManagerWrapper
                .Setup(wrapper => wrapper.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new AppUser()));

            _mockIUserManagerWrapper
                .Setup(wrapper => wrapper.GetRolesAsync(It.IsAny<AppUser>()).Result)
                .Returns(new List<string>());

            _mockIUserManagerWrapper
                .Setup(wrapper => wrapper.DeleteAsync(It.IsAny<AppUser>()).Result)
                .Returns(IdentityResult.Failed());

            var actionResult = await _controller.DeleteAsync("Id1");

            Assert.IsInstanceOfType(actionResult, typeof(ObjectResult));
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, (actionResult as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task DeleteAsync_HappyPath_ShouldReturnNoContent()
        {
            _mockIUserManagerWrapper
                .Setup(wrapper => wrapper.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new AppUser()));

            _mockIUserManagerWrapper
                .Setup(wrapper => wrapper.GetRolesAsync(It.IsAny<AppUser>()).Result)
                .Returns(new List<string>());

            _mockIUserManagerWrapper
                .Setup(wrapper => wrapper.DeleteAsync(It.IsAny<AppUser>()).Result)
                .Returns(IdentityResult.Success);

            var actionResult = await _controller.DeleteAsync("Id1");

            Assert.IsInstanceOfType(actionResult, typeof(NoContentResult));
        }

        #endregion

        // TODO: Add more unit testing for this controller.

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