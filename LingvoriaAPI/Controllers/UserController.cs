using System.Security.Claims;
using Core.Interfaces;
using Core.Models.UserModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LingvoriaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IAccountService accountService, IEmailService emailService) : ControllerBase
{
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromForm] LoginForm form)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var response = await accountService.Login(form);
        return response.Code switch
        {
            200 => Ok(response),
            400 => BadRequest(response),
            404 => NotFound(response),
            500 => Problem(response.Message, statusCode: response.Code),
            _ => BadRequest("Something went wrong")
        };
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var response = await accountService.GetUsers();
        return response.Code switch
        {
            200 => Ok(response),
            400 => BadRequest(response),
            404 => NotFound(response),
            _ => BadRequest("Something went wrong")
        };
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromForm] RegisterForm form)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var response = await accountService.Register(form);
        return response.Code switch
        {
            200 => Ok(response),
            400 => BadRequest(response),
            500 => Problem(response.Message, statusCode: response.Code),
            _ => BadRequest("Something went wrong")
        };
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    [Route("user/short")]
    public async Task<IActionResult> GetShortUserInfo()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        var response = await accountService.GetShortUserInfo(userId);
        return response.Code switch
        {
            200 => Ok(response.Data),
            404 => NotFound(response.Data),
            _ => BadRequest("Something went wrong")
        };
    }
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    [Route("user")]
    public async Task<IActionResult> UpdateUser(string? avatarUrl, string username, string? fullName, string email)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        var response = await accountService.UpdateUserInfo(avatarUrl, username,  fullName,  email, userId);
        return response.Code switch
        {
            200 => Ok(response.Data),
            404 => NotFound(response.Data),
            _ => BadRequest("Something went wrong")
        };
    }

    [HttpGet("/imlogined")]
    public async Task<IActionResult> ImLogined()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        return Ok();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("/get-verification-email-code")]
    public async Task<IActionResult> GetVerificationEmailCode()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        var response = await emailService.SendVerificationEmail(userId);
        return response.Code switch
        {
            200 => Ok(response.Data),
            400 => BadRequest(response.Message),
            404 => NotFound(response.Data),
            _ => BadRequest("Something went wrong")
        };
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("/verify-email-code")]
    public async Task<IActionResult> VeryficateEmail(string code)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        var response = await emailService.VerificateEmail(code, userId);
        // return response.Code switch
        // {
        //     200 => Ok(response.Data),
        //     400 => BadRequest(response.Message),
        //     404 => NotFound(response.Data),
        //     _ => BadRequest("Something went wrong")
        // };
        return Ok();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("get-my-id")]
    public async Task<IActionResult> GetMyId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        var response = await accountService.GetMyId(userId);
        return response.Code switch
        {
            200 => Ok(response.Data.ToString()),
            400 => BadRequest(response.Message),
            404 => NotFound(response.Data),
            _ => BadRequest("Something went wrong")
        };
    }
}