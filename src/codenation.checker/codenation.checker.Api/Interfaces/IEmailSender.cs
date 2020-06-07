using System.Collections.Generic;
using System.Threading.Tasks;

namespace codenation.checker.Api.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailTo(List<string> sendTo);
    }
}
