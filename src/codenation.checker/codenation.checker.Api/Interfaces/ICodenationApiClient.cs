using codenation.checker.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace codenation.checker.Api.Interfaces
{
    public interface ICodenationApiClient
    {
        Task Execute();

        Task<IList<Stage>> GetInformationAboutModule();
    }
}
