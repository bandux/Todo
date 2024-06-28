using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Controllers;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Tests
{
    public class TodoItemsControllerTests
    {
        private readonly TodoContext _context;
        private readonly TodoItemsController _controller;

        public TodoItemsControllerTests()
        {
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: "TodoTestDb")
                .Options;
            _context = new TodoContext(options);
            _controller = new TodoItemsController(_context);
        }

        [Fact]
        public async Task GetTodoItems_ReturnsEmptyList()
        {
            // Arrange
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();

            // Act
            var result = await _controller.GetTodoItems();

            // Assert
            var items = Assert.IsType<ActionResult<IEnumerable<TodoItem>>>(result);
            Assert.Empty(items.Value);
        }

        [Fact]
        public async Task PostTodoItem_ReturnsCreatedAtAction()
        {
            // Arrange
            var todoItem = new TodoItem { Title = "Test Item", Description = "Test description", IsCompleted = false };

            // Act
            var result = await _controller.PostTodoItem(todoItem);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var model = Assert.IsAssignableFrom<TodoItem>(createdAtActionResult.Value);
            Assert.Equal(todoItem.Title, model.Title);
        }

        [Fact]
        public async Task GetTodoItem_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = 999;

            // Act
            var result = await _controller.GetTodoItem(nonExistentId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetTodoItem_ReturnsTodoItem()
        {
            // Arrange
            var todoItem = new TodoItem { Title = "Test Item", Description = "Test description", IsCompleted = false };
            await _controller.PostTodoItem(todoItem);

            // Act
            var result = await _controller.GetTodoItem(todoItem.Id);

            // Assert
            var okObjectResult = Assert.IsType<ActionResult<TodoItem>>(result);
            Assert.Equal(todoItem.Title, okObjectResult.Value.Title);
        }

        [Fact]
        public async Task PutTodoItem_ReturnsBadRequest_WhenIdsMismatch()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Description = "Test description", Title = "Test Item", IsCompleted = false };

            // Act
            var result = await _controller.PutTodoItem(2, todoItem);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteTodoItem_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = 999;

            // Act
            var result = await _controller.DeleteTodoItem(nonExistentId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteTodoItem_ReturnsNoContent()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Test Item", Description= "Test description", IsCompleted = false };
            await _controller.PostTodoItem(todoItem);

            // Act
            var result = await _controller.DeleteTodoItem(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
