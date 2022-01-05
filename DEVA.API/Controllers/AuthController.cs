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
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Web;
using Microsoft.Extensions.Options;
using DEVA.API.DTOs;
using DEVA.API.Interfaces;

namespace DEVA.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly IOptions<EmailOptionsDTO> _emailOptions;
        private readonly IEmail _email;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config,
            IOptions<EmailOptionsDTO> emailOptions, IEmail email)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _emailOptions = emailOptions;
            _email = email;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginViewModel userViewModel)
        {
            var user = await _userManager.FindByEmailAsync(userViewModel.Email);
            if(user == null)
            {
                return BadRequest();
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, userViewModel.Password, false);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(new {
                result = result,
                token = JwtTokenGeneratorMachine(user).Result
            });
        }

        // POST: api/auth/resetpassword
        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel userViewModel)
        {
            var user = await _userManager.FindByEmailAsync(userViewModel.Email);
            if (user != null || user.EmailConfirmed)
            {
                //Send Email
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var changePasswordUrl = Request.Headers["changePasswordUrl"];//http://localhost:4200/change-password

                var uriBuilder = new UriBuilder(changePasswordUrl);
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["token"] = token;
                query["userid"] = user.Id;
                uriBuilder.Query = query.ToString();
                var urlString = uriBuilder.ToString();

                var emailBody = $"Click on link to change password </br>{urlString}";
                await _email.Send(userViewModel.Email, emailBody, _emailOptions.Value);

                return Ok();
            }

            return Unauthorized();
        }

        // POST: api/auth/changepassword
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel userViewModel)
        {
            var user = await _userManager.FindByIdAsync(userViewModel.UserId);
            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, Uri.UnescapeDataString(userViewModel.Token), userViewModel.Password);

            if (resetPasswordResult.Succeeded)
            {
                return Ok();
            }

            return Unauthorized();
        }

        // POST: api/auth/confirmemail
        [HttpPost("confirmemail")]
        public async Task<ActionResult> ConfirmEmail(ConfirmEmailViewModel userViewModel)
        {
            var user = await _userManager.FindByIdAsync(userViewModel.UserId);
            var confirm = await _userManager.ConfirmEmailAsync(user, Uri.UnescapeDataString(userViewModel.Token));

            if (confirm.Succeeded)
            {
                return Ok();
            }

            return Unauthorized();
        }

        private async Task<string> JwtTokenGeneratorMachine(User userInfo)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userInfo.Id),
                new Claim(ClaimTypes.Name, userInfo.UserName)
            };

            var roles = await _userManager.GetRolesAsync(userInfo);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }


            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
             .GetBytes(_config.GetSection("AppSettings:Key").Value));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
        
}