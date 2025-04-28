using FitnessAuthBackend.Data;
using FitnessAuthBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FitnessAuthBackend.Controllers
{
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

        [HttpGet("{date}")]
        [Authorize]
        public async Task<IActionResult> GetNotes(DateTime date)

        {
            Console.WriteLine($"Received date: {date.ToString("yyyy-MM-dd")}");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notes = await _context.Notes
                .Where(n => n.UserId == userId && n.Date.Date == date.Date)
                .ToListAsync();

            return Ok(notes);
            
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddNote([FromBody] Note model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            model.UserId = userId;

            _context.Notes.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null)
            {
                return NotFound("Note not found.");
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return Ok("Note deleted.");
        }
    }

}
