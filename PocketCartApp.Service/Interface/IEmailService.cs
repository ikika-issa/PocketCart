using PocketCartApp.Domain.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketCartApp.Service.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage message);
    }
}
