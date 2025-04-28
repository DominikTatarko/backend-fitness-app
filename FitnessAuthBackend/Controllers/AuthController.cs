using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FitnessAuthBackend.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
    ILogger<AuthController> logger) 
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _logger = logger;// Assign to a field
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if a user with the same email already exists
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            _logger.LogWarning($"Registration attempt failed. Email {model.Email} is already in use.");
            return BadRequest("A user with this email already exists.");
        }

        // Create the new user
        var user = new ApplicationUser
        {
            UserName = model.Username,
            Email = model.Email,
            Weight = model.Weight,
            Level = model.Level
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation($"New user registered with ID: {user.Id}");
            return Ok("User registered successfully.");
        }

        // Return validation errors if creation failed
        return BadRequest(result.Errors);
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Unauthorized("Invalid login attempt. User not found.");
        }

        var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        return Unauthorized("Invalid login attempt. Check your email or password.");
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"];
        if (string.IsNullOrEmpty(secretKey) || Encoding.UTF8.GetBytes(secretKey).Length < 32)
        {
            throw new Exception("The JWT SecretKey is missing or too short. It must be at least 32 characters.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? string.Empty),          // Subject (email)
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),          // Token ID
        new Claim(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),              // User ID
    };

        foreach (var claim in claims)
        {
            Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }



    [HttpPost("rehash-password")]
    public async Task<IActionResult> RehashPassword(string email, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, newPassword);
        await _userManager.UpdateAsync(user);

        return Ok("Password rehashed.");
    }

    [HttpGet("protected-endpoint")]
    [Authorize]
    public IActionResult GetProtectedData()
    {
        return Ok("This is protected data.");
    }

    
    [HttpGet("user-info")]
    [Authorize]
    public async Task<IActionResult> GetUserInfo()
    {
        var claims = User.Claims.ToList();
        foreach (var claim in claims)
        {
            _logger.LogInformation($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
        }
        // Extract the user ID from the JWT
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("User ID not found in the token claims.");
            return NotFound("User not found.");
        }

        // Log the extracted user ID
        _logger.LogInformation("Extracted userId: {UserId}");

        // Retrieve the user from the database by Email
        var user = await _userManager.FindByEmailAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found in the database.", userId);
            return NotFound("User not found.");
        }

        _logger.LogInformation("Successfully retrieved user info for user {Email}.", user.Email);

        return Ok(new { user.UserName, user.Email });
    }
}
