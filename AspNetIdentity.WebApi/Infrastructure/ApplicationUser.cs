using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetIdentity.WebApi.Infrastructure
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        public byte Level { get; set; }

        //public string From { get; set; }
        //public string Password { get; set; }

        [Required]
        public DateTimeOffset JoinDate { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Add custom user claims here

            return await manager.CreateIdentityAsync(this, authenticationType);
        }
    }
}