using auth16.Data;
using auth16.Services;
using auth16.Models;
using Microsoft.AspNetCore.Mvc;
using auth16.DTO;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;

namespace auth16.Controllers;

[ApiController]
[Route("auth")]

public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _client;
    private readonly JwtServices _jwtServices;
    public AuthController(AppDbContext context, IHttpClientFactory client,JwtServices jwtServices)
    {
        _context = context;
        _client = client;
        _jwtServices = jwtServices;
    }
    [HttpPost("reg")]
    public async Task<IActionResult> RegUser(RegDto dto)
    {
        if(string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Password))
        {
            return BadRequest("Please add userName and Password");
        }
        if(await _context.User.AnyAsync(u=>(u.Name ?? "").ToLower() == dto.Name.ToLower()))
        {
            return BadRequest("User name already in use");
        }
        var user = new User
        {
            Name = dto.Name,
            HashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.User.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtServices.GenerateToken(user);
        return Ok(new
        {
            Message = "user registered",
            Token = token,
            User = new
            {
                user.Id,
                user.Name
            }
        });


    }
    [HttpPost("log")]
    public async Task<IActionResult> LogUser(LogDto dto)
    {
        if(string.IsNullOrEmpty(dto.Name) || string.IsNullOrEmpty(dto.Password))
        {
            return BadRequest("Please add username and Password");

        }
        var user = await _context.User.FirstOrDefaultAsync(u=>(u.Name ?? "unknown").ToLower() == dto.Name.ToLower());
        if(user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.HashedPassword))
        {
            return BadRequest(" invalid userName or password");
        }
        var token = _jwtServices.GenerateToken(user);
        return Ok(new
        {
            Message = "User logger",
            Token = token, 
            User = new LogResp{Id= user.Id,Name=user.Name}
        });
    }

   
}
