using Microsoft.AspNetCore.Identity;
using PocketCartApp.Domain.Domain_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Domain.Identity_Models
{
    public class PocketCartApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }  //IF CONTRACT IS FIXED
        public Contract_Type? contract_Type { get; set; }
    }
}
