using FitnessAuthBackend.Data;
using FitnessAuthBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class MealsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MealsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Meals
    [HttpGet]
    public async Task<ActionResult> GetAllMeals()
    {
        var meals = await _context.Jedla
            .Include(j => j.Kroky)
            .Include(j => j.TypJedla)
            .Select(j => new
            {
                j.JedloId,
                j.NazovJedla,
                j.Kalorie,
                j.Sacharidy,
                j.Tuky,
                j.NazovObrJedlo,
                TypJedla = j.TypJedla.TypJedlaNazov,
                Kroky = j.Kroky.Select(k => new { k.CisloKroku, k.PopisKroku })
            })
            .ToListAsync();

        return Ok(meals);
    }

    // GET: api/Meals/5
    [HttpGet("{id}")]
    public async Task<ActionResult> GetMealById(int id, string TypJedla)
    {
        var meal = await _context.Jedla
            .Include(j => j.Kroky)
            .Include(j => j.TypJedla)
            .Where(j => j.JedloId == id)
            .Select(j => new
            {
                j.JedloId,
                j.NazovJedla,
                j.Kalorie,
                j.Sacharidy,
                j.Tuky,
                TypJedla = j.TypJedla.TypJedlaNazov,
                Kroky = j.Kroky.Select(k => new { k.CisloKroku, k.PopisKroku })
            })
            .FirstOrDefaultAsync();

        if (meal == null) return NotFound();

        return Ok(meal);
    }

    // POST: api/Meals
    [HttpPost]
    public async Task<ActionResult> CreateMeal(Jedlo meal)
    {
        _context.Jedla.Add(meal);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMealById), new { id = meal.JedloId }, meal);
    }

    // PUT: api/Meals/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMeal(int id, Jedlo meal)
    {
        if (id != meal.JedloId) return BadRequest();

        _context.Entry(meal).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MealExists(id)) return NotFound();
            else throw;
        }

        return NoContent();
    }

    // DELETE: api/Meals/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMeal(int id)
    {
        var meal = await _context.Jedla.FindAsync(id);
        if (meal == null) return NotFound();

        _context.Jedla.Remove(meal);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool MealExists(int id)
    {
        return _context.Jedla.Any(e => e.JedloId == id);
    }
}
