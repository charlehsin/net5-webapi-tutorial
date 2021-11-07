using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoApi.Authentication;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Controllers
{
    [Authorize(AuthenticationSchemes = GeneralAuth.AuthSchemes)]
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly IMyClaim _myClaim;
        private readonly ITodoItemsRepository _todoItemsRepository;
        private readonly ILogger _logger;

        public TodoItemsController(IMyClaim myClaim, ITodoItemsRepository todoItemsRepository,
            ILogger<TodoItemsController> logger)
        {
            _myClaim = myClaim;
            _todoItemsRepository = todoItemsRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get all the items.
        /// </summary>
        /// <returns>List of items</returns>
        /// <response code="200">Returns the list of items</response>
        /// <response code="401">If this is not authorized</response>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItemsAsync()
        {
            var parsedClaim = _myClaim.ParseAuthClaim(HttpContext);

            _logger.Log(LogLevel.Debug, $"User {parsedClaim.UserName} is getting all items.");

            var items = await _todoItemsRepository.GetAllAsync();
            return items.Select(x => ItemToDTO(x)).ToList();
        }

        /// <summary>
        /// Get the target item.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The target item</returns>
        /// <response code="200">Returns the target item</response>
        /// <response code="401">If this is not authorized</response>
        /// <response code="404">If the target item does not exist</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItemByIdAsync(long id)
        {
            var parsedClaim = _myClaim.ParseAuthClaim(HttpContext);

            var todoItem = await _todoItemsRepository.FindAsync(id);
            if (todoItem == null)
            {
                _logger.Log(LogLevel.Debug, $"User {parsedClaim.UserName} is trying to get an item with id {id} that does not exist.");
                return NotFound();
            }

            _logger.Log(LogLevel.Debug, $"User {parsedClaim.UserName} is getting an item with id {id}.");
            return ItemToDTO(todoItem);
        }

        /// <summary>
        /// Update the target item.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="todoItemDTO"></param>
        /// <returns></returns>
        /// <response code="204">If the item is updated</response>
        /// <response code="400">If the id does not match the item id</response>
        /// <response code="401">If this is not authorized</response>
        /// <response code="404">If the target item does not exist</response>
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTodoItemAsync(long id, TodoItemDTO todoItemDTO)
        {
            var parsedClaim = _myClaim.ParseAuthClaim(HttpContext);

            if (id != todoItemDTO.Id)
            {
                _logger.Log(LogLevel.Debug, $"User {parsedClaim.UserName} is sending bad request with mismatching id.");
                return BadRequest();
            }

            var todoItem = await _todoItemsRepository.FindAsync(id);
            if (todoItem == null)
            {
                _logger.Log(LogLevel.Debug, $"User {parsedClaim.UserName} is trying to update an item with id {id} that does not exist.");
                return NotFound();
            }

            todoItem.Name = todoItemDTO.Name;
            todoItem.IsComplete = todoItemDTO.IsComplete;

            try
            {
                await _todoItemsRepository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!_todoItemsRepository.DoesItemExist(id))
            {
                _logger.Log(LogLevel.Error, $"User {parsedClaim.UserName} fails to update an item with id {id}.");
                return NotFound();
            }

            _logger.Log(LogLevel.Debug, $"User {parsedClaim.UserName} updated an item with id {id}.");
            return NoContent();
        }

        /// <summary>
        /// Create a new item.
        /// </summary>
        /// <param name="todoItemDTO"></param>
        /// <returns></returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="401">If this is not authorized</response>
        [HttpPost]
        [ActionName("CreateTodoItemAsync")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TodoItemDTO>> CreateTodoItemAsync(TodoItemDTO todoItemDTO)
        {
            var parsedClaim = _myClaim.ParseAuthClaim(HttpContext);

            var todoItem = new TodoItem
            {
                Id = todoItemDTO.Id,
                IsComplete = todoItemDTO.IsComplete,
                Name = todoItemDTO.Name
            };
            await _todoItemsRepository.AddAsync(todoItem);

            _logger.Log(LogLevel.Debug, $"User {parsedClaim.UserName} created an item with id {todoItemDTO.Id}.");
            return CreatedAtAction(nameof(CreateTodoItemAsync), ItemToDTO(todoItem));
        }

        /// <summary>
        /// Delete the target item.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="204">If the item is deleted</response>
        /// <response code="401">If this is not authorized</response>
        /// <response code="404">If the target item does not exist</response>
        [HttpDelete("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTodoItemAsync(long id)
        {
            var parsedClaim = _myClaim.ParseAuthClaim(HttpContext);

            var todoItem = await _todoItemsRepository.FindAsync(id);
            if (todoItem == null)
            {
                _logger.Log(LogLevel.Debug, $"User {parsedClaim.UserName} tries to delete an item with id {id} that does not exist.");
                return NotFound();
            }

            await _todoItemsRepository.RemoveAsync(todoItem);

            _logger.Log(LogLevel.Debug, $"User {parsedClaim.UserName} deleted an item with id {id}.");
            return NoContent();
        }

        /// <summary>
        /// Map TodoItem object to TodoItemDTO object
        /// </summary>
        /// <param name="todoItem"></param>
        /// <returns>TodoItemDTO object</returns>
        private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
            new()
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };
    }
}
