using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _service;

    public AuthController(AuthService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterDto dto)
    {
        if (_service.UserExists(dto.Username))
            return BadRequest("User already exists");

        _service.Register(dto);

        return Ok("User created");
    }

    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        if (!_service.Login(dto))
            return Unauthorized("Invalid credentials");

        var token = "GENERATE_JWT_HERE";

        return Ok(new { token });
    }
}