using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityAccess.Api.Models;
using IdentityAccess.Api.Services;
using IdentityAccessShared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IdentityAccess.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private IUserService _userService;
		private IMailService _mailService;
		private IConfiguration _configuration;

		public AuthController(IUserService userService,IMailService mailService, IConfiguration configuration) {
			_userService = userService;
			_mailService = mailService;
			_configuration = configuration;
		}


		//api/auth/register
		[HttpPost("Register")]
		public async Task<IActionResult> RegisterAsync([FromBody]RegisterViewModel model) {

			if (ModelState.IsValid) {
				var result = await _userService.RegisterUserAsync(model);

				if (result.IsSuccess) {
					return Ok(result);
				}

				return BadRequest(result);
			}

			return BadRequest("Some properties are not valid");
		}


		//api/auth/login
		[HttpPost("Login")]
		public async Task<IActionResult> LoginAsync([FromBody] LoginViewModel model) {
			if (ModelState.IsValid) {
				var result = await _userService.LoginUser(model);
				if (result.IsSuccess)
				{
					await _mailService.SendEmailAsync(model.Email, "New Login", "<h1>New Login to your account noticed</h1><p>New Login to your account at "+ DateTime.Now+ "</p>");
					return Ok(result);
				}
				else {
					return BadRequest(result);
				}
			}
			return BadRequest("Some properties are not valid");
		}

		//api/auth/confirmEmail?userid&token
		[HttpGet("confirmEmail")]
		public async Task<IActionResult> ConfirmEmail(string userId, string token) {
			if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token)) {
				return NotFound();
			}

			var result = await _userService.ConfirmEmailAsync(userId, token);

			if (result.IsSuccess) {
				return Redirect($"{_configuration["AppUrl"]}/ConfirmEmail.html");
			}
			return BadRequest(result);
		}

		//api/auth/forgetpassword?emailid&token
		[HttpPost("forgetPassword")]
		public async Task<IActionResult> ForgetPassword(string emailId)
		{
			if (string.IsNullOrWhiteSpace(emailId))
			{
				return NotFound();
			}

			var result = await _userService.ForgetPasswordAsync(emailId);

			if (result.IsSuccess)
			{
				return Ok(result); //200
			}
			return BadRequest(result); //400
		}

		//api/auth/resetpassword
		[HttpPost("resetPassword")]
		public async Task<IActionResult> ResetPassword([FromForm]ResetPassword model)
		{
			if (ModelState.IsValid) {
				var result = await _userService.ResetPasswordAsync(model);

				if (result.IsSuccess) {
					return Ok(result);//200
				}
				return BadRequest(result);//400
			}
			return BadRequest("Some properties are missing");
		}
	}
}
