using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Identity;

public class NotesControllerTests
{
    private readonly NotesController _controller;
    private readonly ApplicationDbContext _context;
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;

    public NotesControllerTests()
    {
       var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "NotesDb")
            .Options;
        _context = new ApplicationDbContext(options);

        var userStore = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(userStore.Object, null, null, null, null, null, null, null, null);

        _controller = new NotesController(_context, _mockUserManager.Object);
    }

    [Fact]
    public async Task GetNotes_ReturnsOkResult_WithListOfNotes()
    {
        // Arrange
        var userId = "test-user-id";
        var notes = new List<Note>
        {
            new Note { Id = 1, Title = "Note 1", Description = "Description 1", UserId = userId },
            new Note { Id = 2, Title = "Note 2", Description = "Description 2", UserId = userId }
        };
        _context.Notes.AddRange(notes);
        await _context.SaveChangesAsync();
        _mockUserManager.Setup(um => um.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns(userId);

        // Act
        var result = await _controller.GetNotes();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnNotes = Assert.IsType<List<Note>>(okResult.Value);
        Assert.Equal(2, returnNotes.Count);
    }

   
}