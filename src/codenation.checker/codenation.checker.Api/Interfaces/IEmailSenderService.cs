using System.Collections.Generic;
using System.Threading.Tasks;

namespace codenation.checker.Api.Interfaces
{
    public interface IEmailSenderService
    {
        Task SendEmailToAll();
    }
}
