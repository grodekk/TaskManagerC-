using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TaskManagerAPI.Services;

public class AuthService
{
    private readonly AppDbContext _db;

    public AuthService(AppDbContext db)
    {
        _db = db;
    }

    public bool UserExists(string username)
        => _db.Users.Any(u => u.Username == username);

    public void Register(RegisterDto dto)
    {
        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _db.Users.Add(user);
        _db.SaveChanges();
    }


    public bool Login(LoginDto dto)
    {
        var user = _db.Users.FirstOrDefault(u => u.Username == dto.Username);
        if (user == null)
            return false;

        var isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        return isValid;
    }    


    public string GenerateToken(string username, IConfiguration config)
    {
        var claims = new[]
        {
        new Claim(ClaimTypes.Name, username)
    };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["Jwt:Key"]!));

        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}