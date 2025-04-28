using Microsoft.AspNetCore.Mvc;
using FitnessAuthBackend.Data;
using FitnessAuthBackend.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FitnessAuthBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExercisesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExercisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Exercises
        [HttpPost]
        public async Task<IActionResult> AddExercise([FromBody] Exercise exercise)
        {
            if (exercise == null)
                return BadRequest("Exercise data is null.");

            await _context.Exercises.AddAsync(exercise);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Exercise added successfully!" });
        }

        // POST: api/Exercises/bulk
        [HttpPost("bulk")]
        public async Task<IActionResult> AddExercisesBulk([FromBody] List<Exercise> exercises)
        {
            if (exercises == null || exercises.Count == 0)
                return BadRequest("No exercise data provided.");

            await _context.Exercises.AddRangeAsync(exercises);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"{exercises.Count} exercises added successfully!" });
        }
    }
}
