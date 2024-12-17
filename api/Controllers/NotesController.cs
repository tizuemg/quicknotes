using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    private readonly UserManager<ApplicationUser> _userManager;

    public NotesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetNotes()
    {
        var userId = _userManager.GetUserId(User);
        var notes = await _context.Notes
            .Where(n => n.UserId == userId && !n.IsDeleted)
            .OrderByDescending(n => n.UpdatedDate)
            .ToListAsync();
        return Ok(notes);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetNoteById(int id)
    {
        var userId = _userManager.GetUserId(User);
        var note = await _context.Notes
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId && !n.IsDeleted);
        if (note == null)
        {
            return NotFound();
        }
        return Ok(note);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateNote([FromBody] Note note)
    {
        var userId = _userManager.GetUserId(User);
        note.UserId = userId;
        _context.Notes.Add(note);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetNoteById), new { id = note.Id }, note);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateNote(int id, [FromBody] Note note)
    {
        var userId = _userManager.GetUserId(User);
        var existingNote = await _context.Notes
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId && !n.IsDeleted);
        if (existingNote == null)
        {
            return NotFound();
        }
        existingNote.Title = note.Title;
        existingNote.Description = note.Description;
        _context.Notes.Update(note);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteNote(int id)
    {
        var userId = _userManager.GetUserId(User);
        var note = await _context.Notes
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId && !n.IsDeleted);
        if (note == null)
        {
            return NotFound();
        }
        note.IsDeleted = true;
        _context.Notes.Update(note);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}