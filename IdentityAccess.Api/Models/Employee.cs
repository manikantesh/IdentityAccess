using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAccess.Api.Models
{
	public class Employee
	{
		public int EmployeeId { get; set; }
		public string Fname { get; set; }
		public string Lname { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
	}
}
