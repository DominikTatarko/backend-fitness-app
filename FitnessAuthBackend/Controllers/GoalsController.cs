// Ensure proper usage of async/await and update logic in your FitnessController
using FitnessAuthBackend.Data;
using FitnessAuthBackend.Models;
using FitnessAuthBackend.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FitnessAuthBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FitnessController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FitnessController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Set or update user's goal
        [HttpPost("set-goal")]
        public async Task<IActionResult> SetGoal([FromBody] GoalDto goalDto)
        {
            // Retrieve the authenticated user ID from the JWT token.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            // Find if there's an existing goal for this user.
            var existingGoal = await _context.Goals
                .FirstOrDefaultAsync(g => g.UserId == userId);

            if (existingGoal != null)
            {
                // If a goal already exists, update it.
                existingGoal.PushUpGoal = goalDto.PushUpGoal;
                existingGoal.PullUpGoal = goalDto.PullUpGoal;
                existingGoal.TargetDate = goalDto.TargetDate;
            }
            else
            {
                // If no goal exists, create a new goal for the user.
                var newGoal = new Goal
                {
                    UserId = userId, // UserId is automatically set from the JWT token
                    PushUpGoal = goalDto.PushUpGoal,
                    PullUpGoal = goalDto.PullUpGoal,
                    TargetDate = goalDto.TargetDate
                };

                await _context.Goals.AddAsync(newGoal);
            }

            try
            {
                // Attempt to save the changes to the database.
                await _context.SaveChangesAsync();
                return Ok("Goal set successfully.");
            }
            catch (DbUpdateException ex)
            {
                // Handle any DB related errors, like foreign key violations.
                return StatusCode(500, $"An error occurred while saving the goal: {ex.Message}");
            }
        }



        // Add or update performance for the current day
        [HttpPost("set-performance")]
        public async Task<IActionResult> SetPerformance([FromBody] PerformanceDto performanceDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            // Check if a performance entry already exists for this user and the provided date.
            var existingPerformance = await _context.Performances
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Date.Date == performanceDto.Date.Date);

            if (existingPerformance != null)
            {
                // If a performance entry exists for the provided date, return a message indicating submission is disabled.
                return BadRequest("You have already submitted your performance for this date. Please wait for the next day.");
            }

            // Create a new performance entry for the user for the provided date.
            var newPerformance = new Performance
            {
                UserId = userId,
                Date = performanceDto.Date,
                PushUpCount = performanceDto.PushUpCount,
                PullUpCount = performanceDto.PullUpCount
            };

            await _context.Performances.AddAsync(newPerformance);

            try
            {
                // Save the new performance entry to the database.
                await _context.SaveChangesAsync();
                return Ok("Performance entry successfully recorded.");
            }
            catch (DbUpdateException ex)
            {
                // Handle database errors.
                return StatusCode(500, $"An error occurred while saving the performance entry: {ex.Message}");
            }
        }



        [HttpGet("get-performance/{date}")]
        public async Task<IActionResult> GetPerformance(DateTime date)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            // Ensure comparison is based on date only.
            var performance = await _context.Performances
                .Where(p => p.UserId == userId && p.Date.Date == date.Date)
                .FirstOrDefaultAsync();

            if (performance == null)
            {
                return NotFound("No performance found for this date.");
            }

            var performanceDto = new PerformanceDto
            {
                UserId = performance.UserId,
                PushUpCount = performance.PushUpCount,
                PullUpCount = performance.PullUpCount,
                Date = performance.Date
            };

            return Ok(performanceDto);
        }



        // Get user's current goal
        [HttpGet("get-goal")]
        public async Task<IActionResult> GetGoal()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var goal = await _context.Goals
                .FirstOrDefaultAsync(g => g.UserId == userId);

            if (goal == null)
            {
                return NotFound("No goal found for this user.");
            }

            var goalDto = new Goal
            {
                UserId = goal.UserId,
                PushUpGoal = goal.PushUpGoal,
                PullUpGoal = goal.PullUpGoal,
                TargetDate = goal.TargetDate
            };

            return Ok(goalDto);
        }

        // Get all necessary statistics for push-ups and pull-ups
        [HttpGet("get-stats")]
        public async Task<IActionResult> GetStats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            // Get all performance entries for the user
            var allPerformances = await _context.Performances
                .Where(p => p.UserId == userId)
                .ToListAsync();

            if (allPerformances == null || !allPerformances.Any())
            {
                return NotFound("No performance data found.");
            }

            // Find the highest overall
            var highestPushUp = allPerformances.Max(p => p.PushUpCount);
            var highestPullUp = allPerformances.Max(p => p.PullUpCount);

           
            // Get the last recorded performance
            var lastPerformance = allPerformances.OrderByDescending(p => p.Date).FirstOrDefault();

            var stats = new
            {
                PushUpGoal = (await _context.Goals.FirstOrDefaultAsync(g => g.UserId == userId))?.PushUpGoal ?? 0,
                PullUpGoal = (await _context.Goals.FirstOrDefaultAsync(g => g.UserId == userId))?.PullUpGoal ?? 0,
                HighestPushUp = highestPushUp,
                HighestPullUp = highestPullUp,
                LastPushUpValue = lastPerformance?.PushUpCount ?? 0,
                LastPullUpValue = lastPerformance?.PullUpCount ?? 0
            };

            return Ok(stats);
        }


    }
}
