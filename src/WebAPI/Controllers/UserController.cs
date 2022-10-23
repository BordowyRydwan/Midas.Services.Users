using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Dto;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger, IUserService UserService)
    {
        _logger = logger;
        _userService = UserService;
    }
    
    [SwaggerOperation(Summary = "Register new user")]
    [HttpPost("Register", Name = nameof(RegisterNewUser))]
    [ProducesResponseType(typeof(UserRegisterReturnDto), 200)]
    public async Task<IActionResult> RegisterNewUser(UserRegisterDto user)
    {
        var registerReturnDto = await _userService.RegisterNewUser(user).ConfigureAwait(false);

        if (registerReturnDto is not null)
        {
            return Ok(registerReturnDto);
        }

        _logger.LogError("Could not register user with email: {Email}", user.Email);
        return BadRequest();
    }
    
    [SwaggerOperation(Summary = "Update data of existing user")]
    [HttpPatch("Update/Data", Name = nameof(UpdateUserData))]
    public async Task<IActionResult> UpdateUserData(UserUpdateDto user)
    {
        var updateSuccess = await _userService.UpdateUserData(user).ConfigureAwait(false);

        if (updateSuccess)
        {
            return Ok();
        }

        _logger.LogError("Could not update data for user with email: {Email}", user.Email);
        return BadRequest();
    }
    
    [SwaggerOperation(Summary = "Update email address of existing user")]
    [HttpPatch("Update/Email", Name = nameof(UpdateUserEmail))]
    public async Task<IActionResult> UpdateUserEmail(UserUpdateEmailDto user)
    {
        try
        {
            await _userService.UpdateUserEmail(user).ConfigureAwait(false);
        }
        catch (UserException ex)
        {
            _logger.LogError(ex.Message + "\n" + ex.StackTrace);
            return BadRequest(ex.Message);
        }
        
        return Ok();
    }
    
    [SwaggerOperation(Summary = "Get a user by email address")]
    [HttpGet("Email/{email}", Name = nameof(GetUserByEmail))]
    [ProducesResponseType(typeof(UserDto), 200)]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var user = await _userService.GetUserByEmail(email).ConfigureAwait(false);

        if (user is not null)
        {
            return Ok(user);
        }

        _logger.LogError("Could not find user with email: " + email);
        return NotFound();
    }
    
    [SwaggerOperation(Summary = "Get a user by id")]
    [HttpGet("Id/{id}", Name = nameof(GetUserById))]
    [ProducesResponseType(typeof(UserDto), 200)]
    public async Task<IActionResult> GetUserById(ulong id)
    {
        var user = await _userService.GetUserById(id).ConfigureAwait(false);

        if (user is not null)
        {
            return Ok(user);
        }

        _logger.LogError("Could not find user with id: " + id);
        return NotFound();
    }
    
    [SwaggerOperation(Summary = "Get an active user")]
    [HttpGet("", Name = nameof(GetActiveUser))]
    [ProducesResponseType(typeof(UserDto), 200)]
    public async Task<IActionResult> GetActiveUser()
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString();
        var handler = new JwtSecurityTokenHandler();
        var userId = Convert.ToUInt64(handler.ReadJwtToken(token).Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
        var user = await _userService.GetUserById(userId).ConfigureAwait(false);

        if (user is not null)
        {
            return Ok(user);
        }

        _logger.LogError("Could not find user with id: " + userId);
        return NotFound();
    }
}