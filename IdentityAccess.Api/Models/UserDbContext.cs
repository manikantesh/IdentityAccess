using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAccess.Api.Models
{
	public class UserDbContext:IdentityDbContext
	{
		public UserDbContext(DbContextOptions options):base(options) {
		
		}

		public DbSet<Employee> Employees { get; set; }
	}
}
