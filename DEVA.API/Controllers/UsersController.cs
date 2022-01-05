using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DEVA.API.Models;
using Microsoft.AspNetCore.Identity;
using DEVA.API.ViewModels;
using Microsoft.Extensions.Options;
using DEVA.API.DTOs;
using DEVA.API.Interfaces;
using System.Web;

namespace DEVA.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IOptions<EmailOptionsDTO> _emailOptions;
        private readonly IEmail _email;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
            IOptions<EmailOptionsDTO> emailOptions, IEmail email)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _emailOptions = emailOptions;
            _email = email;
        }

        // POST: api/users/create
        [HttpPost("create")]
        public async Task<ActionResult> Create(CreateUserViewModel userViewModel)
        {
            if (!(await _roleManager.RoleExistsAsync("User")))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            var user = new User
            {
                UserName = userViewModel.Username,
                Email = userViewModel.Email,
            };

            var result = await _userManager.CreateAsync(user, userViewModel.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            //Send Email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmEmailUrl = Request.Headers["confirmEmailUrl"];//http://localhost:4200/email-confirm

            var uriBuilder = new UriBuilder(confirmEmailUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["token"] = token;
            query["userid"] = user.Id;
            uriBuilder.Query = query.ToString();
            var urlString = uriBuilder.ToString();

            var emailBody = $"Please confirm your email by clicking on the link below </br>{urlString}";
            await _email.Send(userViewModel.Email, emailBody, _emailOptions.Value);
            //---------------------------------------------------------------------------------------------//

            var userFromDb = await _userManager.FindByNameAsync(user.UserName);
            await _userManager.AddToRoleAsync(userFromDb, "User");

            return Ok(result);
        }

    }
        
}