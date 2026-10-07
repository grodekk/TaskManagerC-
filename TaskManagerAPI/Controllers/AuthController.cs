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
    private readonly IConfiguration _config;

    public AuthController(AuthService service, IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (await _service.UserExistsAsync(dto.Username))
            return BadRequest("User already exists");

        var result = await _service.RegisterAsync(dto);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("User created");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _service.LoginAsync(dto);

        if (user == null)
            return Unauthorized("Invalid credentials");

        var token = await _service.GenerateTokenAsync(user, _config);

        return Ok(new { token });
    }

}