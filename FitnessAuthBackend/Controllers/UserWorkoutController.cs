using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitnessAuthBackend.Data;
using FitnessAuthBackend.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FitnessAuthBackend.Models.Dto;

namespace FitnessAuthBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // This applies to all endpoints in this controller
    public class UserWorkoutController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserWorkoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Only authenticated users can access this
        [HttpPost("create")]
        public async Task<IActionResult> CreateWorkout([FromBody] CreateWorkoutDto createWorkoutDto)
        {
            if (createWorkoutDto == null || string.IsNullOrEmpty(createWorkoutDto.WorkoutName))
            {
                return BadRequest("Invalid workout data");
            }

            // Retrieve the user ID from the authentication context
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Adjust the claim if necessary

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            // Create a new UserWorkout instance
            var userWorkout = new UserWorkout
            {
                UserId = userId,
                WorkoutName = createWorkoutDto.WorkoutName,
                CreatedAt = DateTime.UtcNow
            };

            // Add the new workout to the database
            _context.UserWorkouts.Add(userWorkout);
            await _context.SaveChangesAsync();

            // Return the created workout with a success response
            return Ok(userWorkout);
        }



        // Only authenticated users can access this
        [HttpPost("addExercise")]
        public async Task<IActionResult> AddExerciseToWorkout([FromBody] UserExerciseDto userExerciseDto)
        {
            if (userExerciseDto == null || userExerciseDto.UserWorkoutId <= 0 || userExerciseDto.ExerciseId <= 0)
            {
                return BadRequest("Invalid exercise data. Please provide valid UserWorkoutId and ExerciseId.");
            }

            // Get the currently authenticated user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            // Validate the workout exists and belongs to the current user
            var workout = await _context.UserWorkouts
                .FirstOrDefaultAsync(w => w.UserWorkoutId == userExerciseDto.UserWorkoutId && w.UserId == userId);

            if (workout == null)
            {
                return NotFound("Workout not found or you are not authorized to add exercises.");
            }

            // Validate the exercise exists
            var exercise = await _context.Exercises
                .FirstOrDefaultAsync(e => e.ExerciseId == userExerciseDto.ExerciseId);

            if (exercise == null)
            {
                return NotFound($"Exercise with ID {userExerciseDto.ExerciseId} not found.");
            }

            // Create a new UserExercise record
            var userExercise = new UserExercise
            {
                UserWorkoutId = userExerciseDto.UserWorkoutId,
                ExerciseId = userExerciseDto.ExerciseId,  // Use ExerciseId directly
                Sets = userExerciseDto.Sets,
                Reps = userExerciseDto.Reps,
                Rest = userExerciseDto.Rest
            };

            // Save the new UserExercise to the database
            _context.UserExercises.Add(userExercise);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Exercise added successfully.", userExercise });
        }





        // Only authenticated users can access this
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkoutDetails(int id)
        {
            // Fetch the workout details along with its exercises, filtering by the current user
            var workout = await _context.UserWorkouts
                .Where(w => w.UserWorkoutId == id && w.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .Include(w => w.UserExercises)  // Include exercises associated with the workout
                .ThenInclude(ue => ue.Exercise)  // Include the exercise details
                .FirstOrDefaultAsync();

            if (workout == null)
            {
                return NotFound("Workout not found or user is not authorized to access it");
            }

            // Transform the data into a DTO
            var workoutDetailsDto = new WorkoutDetailsDto
            {
                UserWorkoutId = workout.UserWorkoutId,
                WorkoutName = workout.WorkoutName,
                CreatedAt = workout.CreatedAt,
                Exercises = workout.UserExercises.Select(ue => new ExerciseDto
                {
                    UserExerciseId = ue.UserExerciseId,
                    ExerciseId = ue.ExerciseId,
                    ExerciseName = ue.Exercise.ExerciseName,
                    Sets = ue.Sets,
                    Reps = ue.Reps,
                    Rest = ue.Rest
                }).ToList()
            };

            return Ok(workoutDetailsDto);
        }


        [HttpGet("filter")]
        [AllowAnonymous]
        public async Task<IActionResult> GetExercisesByFilter(int? muscleGroupId, int? difficultyId, string searchTerm = "")
        {
            // Start with the base query for exercises
            var query = _context.Exercises.AsQueryable();

            // Apply the muscle group filter if provided
            if (muscleGroupId.HasValue)
            {
                query = query.Where(e => e.MuscleGroupId == muscleGroupId.Value);
            }

            // Apply the difficulty filter if provided
            if (difficultyId.HasValue)
            {
                query = query.Where(e => e.DifficultyId == difficultyId.Value);
            }

            // Apply the search term filter if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var lowerCaseSearchTerm = searchTerm.ToLower();
                query = query.Where(e => e.ExerciseName.ToLower().Contains(lowerCaseSearchTerm));
            }

            // Include related entities to avoid null references
            query = query.Include(e => e.MuscleGroup).Include(e => e.Difficulty);

            // Execute the query and retrieve the results
            var exercises = await query.ToListAsync();

            // Transform the results into DTOs with null checks
            var exerciseDtos = exercises.Select(e => new ExerciseDto
            {
                ExerciseId = e.ExerciseId,
                ExerciseName = e.ExerciseName,
                MuscleGroupName = e.MuscleGroup?.MuscleGroupName ?? "Unknown",
                DifficultyName = e.Difficulty?.DifficultyName ?? "Unknown"
            }).ToList();

            return Ok(exerciseDtos);
        }

        [HttpGet("filter-data")]
        [AllowAnonymous] // Make this endpoint accessible without authentication
        public async Task<IActionResult> GetFilterData()
        {
            // Fetch all muscle groups
            var muscleGroups = await _context.MuscleGroups
                .Select(mg => new
                {
                    MuscleGroupId = mg.Id,
                    MuscleGroupName = mg.MuscleGroupName
                })
                .ToListAsync();

            // Fetch all difficulties
            var difficulties = await _context.Difficulties
                .Select(d => new
                {
                    DifficultyId = d.DifficultyId,
                    DifficultyName = d.DifficultyName
                })
                .ToListAsync();

            // Return both muscle groups and difficulties
            return Ok(new
            {
                MuscleGroups = muscleGroups,
                Difficulties = difficulties
            });
        }




        // Endpoint to delete a workout
        [HttpDelete("deleteWorkout/{id}")]
        public async Task<IActionResult> DeleteWorkout(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            // Find the workout and ensure it belongs to the authenticated user
            var workout = await _context.UserWorkouts
                .Include(w => w.UserExercises) // Include associated exercises
                .FirstOrDefaultAsync(w => w.UserWorkoutId == id && w.UserId == userId);

            if (workout == null)
            {
                return NotFound("Workout not found or user is not authorized to delete it.");
            }

            // Remove the workout and its associated exercises
            _context.UserExercises.RemoveRange(workout.UserExercises); // Delete associated exercises
            _context.UserWorkouts.Remove(workout); // Delete the workout itself
            await _context.SaveChangesAsync();

            return Ok(new { message = "Workout and its exercises deleted successfully." });
        }

        // Endpoint to delete an exercise from a workout
        [HttpDelete("deleteExercise/{exerciseId}")]
        public async Task<IActionResult> DeleteExercise(int exerciseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            // Find the UserExercise and ensure the parent workout belongs to the authenticated user
            var userExercise = await _context.UserExercises
                .Include(ue => ue.UserWorkout) // Include the parent workout
                .FirstOrDefaultAsync(ue => ue.UserExerciseId == exerciseId && ue.UserWorkout.UserId == userId);

            if (userExercise == null)
            {
                return NotFound("Exercise not found or user is not authorized to delete it.");
            }

            // Remove the exercise
            _context.UserExercises.Remove(userExercise);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Exercise deleted successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWorkouts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var workouts = await _context.UserWorkouts
                .Where(w => w.UserId == userId)
                .Include(w => w.UserExercises)
                .ToListAsync();

            return Ok(workouts);
        }




    }
}
