using PocketCartApp.Domain.Identity_Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Domain.Domain_Models
{
    public class Receipt : BaseEntity
    {
        public Guid ShoppingCartId { get; set; }
        public ShoppingCart? ShoppingCart { get; set; }
        public required string userId { get; set; }
        public PocketCartApplicationUser? PocketCartApplicationUser { get; set; }
        public double total { get; set; }
        public string? currency {  get; set; } //WILL BE A DROP DOWN MENU N SEARCH FOR THEM TO CHOOSE A CURRENCY, ALLOWS FLEXIBILITY
    }
}
