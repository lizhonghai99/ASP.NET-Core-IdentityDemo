using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Management.Service.Models;

namespace User.Management.Service.Services
{
    public interface IEmailService
    {
        void SendEmail(Message message);

        Task SendEmailAsync(Message message);
    }
}
