using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAccess.Api.Models
{
	public class ResetPassword
	{
		[Required]
		public string Token { get; set; }
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		[Required]
		[StringLength(50, MinimumLength = 5)]
		public string NewPassword { get; set; }
		[Required]
		[StringLength(50, MinimumLength = 5)]
		public string confirmPassword { get; set; }
	}
}
