using codenation.checker.Api.Interfaces;
using codenation.checker.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace codenation.checker.Api.Services
{
    public class CodenationApiClientService : ICodenationApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<CodenationApiClientService> _logger;
        private readonly AppSettings _appSettings;

        public CodenationApiClientService(
            ILogger<CodenationApiClientService> logger,
            IConfiguration configuration,
            IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
            _configuration = configuration;

            _appSettings = _configuration.GetSection("AppSettings").Get<AppSettings>();
        }

        public async Task Execute()
        {
            var data = await GetInformationAboutModule();

            if (IsModuleAvailable(_appSettings.WeekModule, data))
            {
                //_emailSender.SendEmailTo();
            }
        }

        public async Task<IList<Stage>> GetInformationAboutModule()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", _appSettings.Authorization);

                var response = await httpClient.GetAsync(_appSettings.URL);

                var json = await response.Content.ReadAsStringAsync();

                var StageList = JObject.Parse(json)["stage"];

                return StageList.ToObject<List<Stage>>();
            }
        }

        private bool IsModuleAvailable(string moduleName, IList<Stage> listOfModules)
        {
            Stage selectedModule = listOfModules.FirstOrDefault(x => x.name == moduleName);

            if (selectedModule != null)
            {
                if (!selectedModule.locked && selectedModule.challenge_count > 1)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
