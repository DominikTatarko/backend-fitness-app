using FitnessAuthBackend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessAuthBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgramsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProgramsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("API is working");
        }

        // Get all programs
        [HttpGet]
        public async Task<IActionResult> GetPrograms()
        {
            var programs = await _context.FitnessPrograms.ToListAsync();
            return Ok(programs); // Return all programs
        }

        // Get available days for a specific program
        [HttpGet("{programId:int}/days")]
        public async Task<IActionResult> GetDays(int programId)
        {
            var days = await _context.ProgramWorkouts
                .Where(pw => pw.ProgramId == programId)
                .Select(pw => pw.Day)
                .Distinct()
                .ToListAsync();

            if (!days.Any())
            {
                return NotFound($"No days found for ProgramId {programId}.");
            }

            return Ok(days); // Return available days
        }

        // Get workouts for a specific program and day
        [HttpGet("{programId:int}/days/{day:int}/workouts")]
        public async Task<IActionResult> GetWorkouts(int programId, int day)
        {
            var workouts = await _context.ProgramWorkouts
                .Where(pw => pw.ProgramId == programId && pw.Day == day)
                .Include(pw => pw.Program)
                .Include(pw => pw.Exercise)
                .Select(pw => new
                {
                    pw.ProgramWorkoutId,
                    pw.ProgramId,
                    ProgramName = pw.Program.Name,
                    ProgramDescription = pw.Program.Description,
                    pw.Day,
                    pw.ExerciseOrder,
                    pw.Sets,
                    pw.Reps,
                    pw.Rest,
                    ExerciseName = pw.Exercise.ExerciseName,
                     YouTubeLink = pw.Exercise.ShortYouTubeDemo
                })
                .ToListAsync();

            if (!workouts.Any())
            {
                return NotFound($"No workouts found for ProgramId {programId}, Day {day}.");
            }

            return Ok(workouts); // Return workouts for the given day
        }
    }
}
