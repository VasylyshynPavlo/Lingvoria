using Core;
using Core.Interfaces;
using Core.Models.UserModels;
using Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LingvoriaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IAccountService accountService) : ControllerBase
    {
        #region POST

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromForm] LoginForm form)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var response = await accountService.Login(form);
            return response.Code switch
            {
                200 => Ok(response),
                400 => BadRequest(response),
                404 => NotFound(response),
                _ => BadRequest("Something went wrong")
            };
        }

        #endregion
        
        #region GET

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

        #endregion
    }
}
