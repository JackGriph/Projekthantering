using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projekthantering.Repositories;
using Projekthantering.Shared.DTOs;

namespace Projekthantering.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var users = await _userRepository.GetAllAsync();
        var dtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username
        }).ToList();

        return Ok(dtos);
    }
}
