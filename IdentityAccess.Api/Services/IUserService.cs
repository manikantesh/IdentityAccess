using IdentityAccess.Api.Models;
using IdentityAccessShared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityAccess.Api.Services
{
	public interface IUserService
	{
		Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model);
		Task<UserManagerResponse> LoginUser(LoginViewModel model);
		Task<UserManagerResponse> ConfirmEmailAsync(string uerId,string token);
		Task<UserManagerResponse> ForgetPasswordAsync(string emailId);
		Task<UserManagerResponse> ResetPasswordAsync(ResetPassword model);
	}

	public class UserService : IUserService {

		private UserManager<IdentityUser> _userManager;
		private IConfiguration _configuration;
		private IMailService _mailService;
		public UserService(UserManager<IdentityUser> userManager,IConfiguration configuration,IMailService mailService) {

			_userManager = userManager;
			_configuration = configuration;
			_mailService = mailService;
		}

		public async Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model) {
			if (model == null) {
				throw new NullReferenceException("Register Model is Null");
			}

			if (model.Password != model.ConfirmPassword) {
				return new UserManagerResponse
				{
					Message = "Confirm password doesn't match the password",
					IsSuccess = false,
				};
			}
			var identityUser = new IdentityUser
			{
				Email = model.Email,
				UserName = model.Email,
			};

			var result = await _userManager.CreateAsync(identityUser, model.Password);


			if (result.Succeeded) {
				//TODO: Send confirmation email

				var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
				var encodedMailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
				var validEmailToken = WebEncoders.Base64UrlEncode(encodedMailToken);

				string url = $"{_configuration["AppUrl"]}/api/auth/confirmemail?userid={identityUser.Id}&token={validEmailToken}";

				await _mailService.SendEmailAsync(identityUser.Email, "Confirm your email", "<h1>Welcome to our Website</h1>" +
					$"<p>Please confirm your email by <a href='{url}'>Click here</a></p> ");

				return new UserManagerResponse {
					Message = "User created Successfully",
					IsSuccess = true,
				};
			}

			return new UserManagerResponse
			{
				Message = "User did not created!",
				IsSuccess = false,
				Errors = result.Errors.Select(e => e.Description)
			};
		}

		public async Task<UserManagerResponse> LoginUser(LoginViewModel model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);

			if (user == null) {
				return new UserManagerResponse
				{
					Message = "There is no user with that Email address",
					IsSuccess = false,
				};
			}
			var result = await _userManager.CheckPasswordAsync(user, model.Password);
			if (!result) {
				return new UserManagerResponse
				{
					Message = "Invalild Password",
					IsSuccess = false,
				};
			}

			var claims = new[] {
				new Claim("Email",model.Email),
				new Claim(ClaimTypes.NameIdentifier, user.Id),
			};
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));

			var token = new JwtSecurityToken(
				issuer: _configuration["AuthSettings:Issuer"],
				audience: _configuration["AuthSettings:Audience"],
				claims: claims,
				expires: DateTime.Now.AddDays(30),
				signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
			);

			string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

			return new UserManagerResponse
			{
				Message = tokenAsString,
				IsSuccess = true,
				Expire = token.ValidTo,
			};
		}

		public async Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null) {
				return new UserManagerResponse {
					IsSuccess = false,
					Message = "User Not Found"
				};
			}

			var decodedToken = WebEncoders.Base64UrlDecode(token);
			string normalToken = Encoding.UTF8.GetString(decodedToken);

			var result = await _userManager.ConfirmEmailAsync(user, normalToken);
			if (result.Succeeded)
			{
				return new UserManagerResponse
				{
					Message = "Email Confirmed Succesfully",
					IsSuccess = true
				};
			}
			else {
				return new UserManagerResponse
				{
					Message = "Email not matched",
					IsSuccess = false,
					Errors = result.Errors.Select(e => e.Description)
				};
			}
		}

		public async Task<UserManagerResponse> ForgetPasswordAsync(string emailId)
		{
			var user = await _userManager.FindByEmailAsync(emailId);
			if (user == null) {
				return new UserManagerResponse {
					IsSuccess = false,
					Message = "User not found!"
				};
			}

			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			var encodedToken = Encoding.UTF8.GetBytes(token);
			var validToken = WebEncoders.Base64UrlEncode(encodedToken);

			string url = $"{_configuration["AppUrl"]}/api/auth/resetpassword?emailid={emailId}&token={validToken}";

			await _mailService.SendEmailAsync(emailId, "Reset Your Password!", "<h1>Follow instructions to reset the password!</h1>" +
				$"<p>To reset password <a href='{url}'>Click here</a></p> ");

			return new UserManagerResponse
			{
				IsSuccess = true,
				Message="Reset password url has been sent to the email succesfully!"
			};
		}

		public async Task<UserManagerResponse> ResetPasswordAsync(ResetPassword model)
		{
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
			{
				return new UserManagerResponse
				{
					IsSuccess = false,
					Message = "User not found!"
				};
			}

			if (model.NewPassword != model.confirmPassword) {
				return new UserManagerResponse {
					IsSuccess = false,
					Message = "Passowrd not match"
				};
			}

			var validToken = WebEncoders.Base64UrlDecode(model.Token);
			string normalToken = Encoding.UTF8.GetString(validToken);

			var result = await _userManager.ResetPasswordAsync(user, normalToken, model.NewPassword);

			if (result.Succeeded) {
				return new UserManagerResponse {
					Message = "Password reset",
					IsSuccess = true
				};
			}else{
				return new UserManagerResponse {
					Message = "Not Success",
					IsSuccess = false,
					Errors = result.Errors.Select(e=>e.Description)
				};
			}
		}
	}
}
