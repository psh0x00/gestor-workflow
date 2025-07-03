using GestorWorkflow.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GestorWorkflow.Data.Context;
using Microsoft.Extensions.Configuration;

namespace GestorWorkflow.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly GestorWorkflowDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(GestorWorkflowDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Utilizadores.AnyAsync(u => u.Email == request.Email))
                return BadRequest("Email já registado.");

            var passwordHash = HashPassword(request.Password);
            var utilizador = new Utilizador
            {
                Nome = request.Nome,
                Funcao = request.Funcao,
                Email = request.Email,
                PasswordHash = passwordHash
            };
            _context.Utilizadores.Add(utilizador);
            await _context.SaveChangesAsync();
            return Ok("Utilizador registado com sucesso.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (utilizador == null || !VerifyPassword(request.Password, utilizador.PasswordHash))
                return Unauthorized("Credenciais inválidas.");

            // Gerar token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, utilizador.UtilizadorId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, utilizador.Email),
                new Claim("nome", utilizador.Nome),
                new Claim("funcao", utilizador.Funcao)
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return Ok(new { token = tokenString });
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }

    public class RegisterRequest
    {
        public string Nome { get; set; }
        public string Funcao { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
