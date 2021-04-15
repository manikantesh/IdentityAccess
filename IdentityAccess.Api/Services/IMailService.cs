using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAccess.Api.Services
{
	public interface IMailService
	{
		Task SendEmailAsync(string toEmail, string subject, string content);
	}

	public class SendGridMailSerice : IMailService
	{
		private IConfiguration _configuration;

		public SendGridMailSerice(IConfiguration configuration) {
			_configuration = configuration;
		}

		public async Task SendEmailAsync(string toEmail, string subject, string content)
		{
			var apiKey = _configuration["SendGridAPIKey"];
			var client = new SendGridClient(apiKey);
			var from = new EmailAddress("kmanikantesh@bsu.edu", "Mani Kilaru");
			var to = new EmailAddress(toEmail);
			var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
			var response = await client.SendEmailAsync(msg);
		}
	}
}
