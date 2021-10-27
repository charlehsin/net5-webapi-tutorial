using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoApi.Authentication;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Controllers
{
    [Authorize]
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

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            var username = _myClaim.ParseAuthClaim(HttpContext);

            _logger.Log(LogLevel.Debug, $"User {username} is getting all items.");

            var items = await _todoItemsRepository.GetAllAsync();
            return items.Select(x => ItemToDTO(x)).ToList();
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItemById(long id)
        {
            var username = _myClaim.ParseAuthClaim(HttpContext);

            var todoItem = await _todoItemsRepository.FindAsync(id);
            if (todoItem == null)
            {
                _logger.Log(LogLevel.Debug, $"User {username} is trying to get an item with id {id} that does not exist.");
                return NotFound();
            }

            _logger.Log(LogLevel.Debug, $"User {username} is getting an item with id {id}.");
            return ItemToDTO(todoItem);
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(long id, TodoItemDTO todoItemDTO)
        {
            var username = _myClaim.ParseAuthClaim(HttpContext);

            if (id != todoItemDTO.Id)
            {
                _logger.Log(LogLevel.Debug, $"User {username} is sending bad request with mismatching id.");
                return BadRequest();
            }

            var todoItem = await _todoItemsRepository.FindAsync(id);
            if (todoItem == null)
            {
                _logger.Log(LogLevel.Debug, $"User {username} is trying to update an item with id {id} that does not exist.");
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
                _logger.Log(LogLevel.Error, $"User {username} fails to update an item with id {id}.");
                return NotFound();
            }

            _logger.Log(LogLevel.Debug, $"User {username} updated an item with id {id}.");
            return NoContent();
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> CreateTodoItem(TodoItemDTO todoItemDTO)
        {
            var username = _myClaim.ParseAuthClaim(HttpContext);

            var todoItem = new TodoItem
            {
                Id = todoItemDTO.Id,
                IsComplete = todoItemDTO.IsComplete,
                Name = todoItemDTO.Name
            };
            await _todoItemsRepository.AddAsync(todoItem);

            _logger.Log(LogLevel.Debug, $"User {username} created an item with id {todoItemDTO.Id}.");
            return CreatedAtAction(nameof(CreateTodoItem), new { id = todoItem.Id }, ItemToDTO(todoItem));
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var username = _myClaim.ParseAuthClaim(HttpContext);
            
            var todoItem = await _todoItemsRepository.FindAsync(id);
            if (todoItem == null)
            {
                _logger.Log(LogLevel.Debug, $"User {username} tries to delete an item with id {id} that does not exist.");
                return NotFound();
            }

            await _todoItemsRepository.RemoveAsync(todoItem);

            _logger.Log(LogLevel.Debug, $"User {username} deleted an item with id {id}.");
            return NoContent();
        }

        /// <summary>
        /// Map TodoItem object to TodoItemDTO object
        /// </summary>
        /// <param name="todoItem"></param>
        /// <returns>TodoItemDTO object</returns>
        private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
            new TodoItemDTO
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };
    }
}
