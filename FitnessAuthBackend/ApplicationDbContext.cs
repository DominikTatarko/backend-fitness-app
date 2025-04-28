using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FitnessAuthBackend.Models;

namespace FitnessAuthBackend.Data
{


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<FitnessProgram> FitnessPrograms { get; set; }
        public DbSet<ProgramWorkout> ProgramWorkouts { get; set; }
        public DbSet<UserExercise> UserExercises { get; set; }
        public DbSet<UserWorkout> UserWorkouts { get; set; }
        public DbSet<Difficulty> Difficulties { get; set; }
        public DbSet<MuscleGroup> MuscleGroups { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<UserWorkout> UserWorkout { get; set; }
        public DbSet<Jedlo> Jedla { get; set; }
        public DbSet<TypJedla> TypyJedla { get; set; }
        public DbSet<Krok> Kroky{ get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Performance> Performances { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            

            


            // Map FitnessLevel enum to a string column
            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Level)
                .HasConversion<string>();

            modelBuilder.Entity<Exercise>()
               .HasOne(e => e.Difficulty)
               .WithMany() // No navigation property in Difficulty
               .HasForeignKey(e => e.DifficultyId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MuscleGroup>()
                .HasKey(mg => mg.Id);

            modelBuilder.Entity<Exercise>()
                .HasOne(e => e.MuscleGroup)
                .WithMany() // No navigation property in MuscleGroup
                .HasForeignKey(e => e.MuscleGroupId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Exercise>()
                .Property(e => e.ExerciseId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Jedlo>()
           .HasOne(j => j.TypJedla)
           .WithMany(tj => tj.Jedla)
           .HasForeignKey(j => j.TypJedlaId);

            modelBuilder.Entity<Krok>()
                .HasOne(k => k.Jedlo)
                .WithMany(j => j.Kroky)
                .HasForeignKey(k => k.JedloId);

            

            modelBuilder.Entity<MuscleGroup>().HasData(
                new MuscleGroup { Id = 1, MuscleGroupName = "Abdominals" },
                new MuscleGroup { Id = 2, MuscleGroupName = "Shoulders" },
                new MuscleGroup { Id = 3, MuscleGroupName = "Chest" },
                new MuscleGroup { Id = 4, MuscleGroupName = "Back" },
                new MuscleGroup { Id = 5, MuscleGroupName = "Biceps" },
                new MuscleGroup { Id = 6, MuscleGroupName = "Glutes" },
                new MuscleGroup { Id = 7, MuscleGroupName = "Quadriceps" },
                new MuscleGroup { Id = 8, MuscleGroupName = "Calves" },
                new MuscleGroup { Id = 9, MuscleGroupName = "Triceps" },
                new MuscleGroup { Id = 10, MuscleGroupName = "Forearms" },
                new MuscleGroup { Id = 11, MuscleGroupName = "Hamstrings" },
                new MuscleGroup { Id = 12, MuscleGroupName = "Trapezius" },
                new MuscleGroup { Id = 13, MuscleGroupName = "Adductors" },
                new MuscleGroup { Id = 14, MuscleGroupName = "Abductors" },
                new MuscleGroup { Id = 15, MuscleGroupName = "Shins" },
                new MuscleGroup { Id = 16, MuscleGroupName = "Hip Flexors" }
            );
            modelBuilder.Entity<Difficulty>().HasData(
                new Difficulty { DifficultyId = 1, DifficultyName = "Beginner" },
                new Difficulty { DifficultyId = 2, DifficultyName = "Intermediate" },
                new Difficulty { DifficultyId = 3, DifficultyName = "Advanced" }
    
            );
            modelBuilder.Entity<TypJedla>().HasData(
                new TypJedla { Id = 1, TypJedlaNazov = "Raňajky" },
                new TypJedla { Id = 2, TypJedlaNazov = "Obed" },
                new TypJedla { Id = 3, TypJedlaNazov = "Večera" },
                new TypJedla { Id = 4, TypJedlaNazov = "Olovarant" }


            );
        }

    }
}

