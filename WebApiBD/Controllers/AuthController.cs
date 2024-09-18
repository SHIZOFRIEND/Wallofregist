using HashLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApiBD.Models;

namespace WebApiBD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Invalid login or password.");
            }

            string hashedPassword = HashPassword.HashPasswort(request.Password);

          
            var user = await _context.Polzovateli
                .Include(u => u.Roli)
                .FirstOrDefaultAsync(u => u.Logini == request.Login && u.Paroli == hashedPassword);

            if (user == null)
            {
                return Unauthorized("Invalid login or password.");
            }

            var userEmail = await _context.Sotrudniki
                .Where(s => s.IDPolzovateliaDlyaAvtorizacii == user.IDPolzovateliaDlyaAvtorizacii)
                .Select(s => s.Pochta)
                .FirstOrDefaultAsync();

            var response = new LoginResponse
            {
                Login = user.Logini,
                Email = userEmail,
                TwoFactorEnabled = user.TwoFactorAvtor == 1,
                Role = user.Roli?.NazvanieRoli
              


            };

            return Ok(response);
        }
    }

    public class LoginRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string Role { get; set; }
    }
}
