using codenation.checker.Api.Interfaces;
using codenation.checker.Api.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace codenation.checker.Api.Services
{
    public class CodenationApiClientService : ICodenationApiClientService
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailSenderService _emailSender;

        private readonly AppSettings _appSettings;
        private readonly UserLogin _userLogin;

        public CodenationApiClientService(
            IConfiguration configuration,
            IEmailSenderService emailSender)
        {
            _emailSender = emailSender;
            _configuration = configuration;

            _appSettings = _configuration.GetSection("AppSettings").Get<AppSettings>();
            _userLogin = _configuration.GetSection("UserLogin").Get<UserLogin>();
        }

        public async Task Execute()
        {
            UserInfo userInfo = await GetAuthorization();

            if (userInfo != null)
            {
                IList<ModuleInfo> listOfModules = await GetModuleInformation(userInfo);

                if (IsModuleAvailable(_appSettings.WeekModule, listOfModules))
                {
                    await _emailSender.SendEmailToAll();
                }
            }
        }

        /// <summary>
        /// Retrieves a List of modules
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private async Task<IList<ModuleInfo>> GetModuleInformation(UserInfo userInfo)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", userInfo.token);

                var response = await httpClient.GetAsync(_appSettings.URL);

                var json = await response.Content.ReadAsStringAsync();

                var StageList = JObject.Parse(json)["stage"];

                return StageList.ToObject<List<ModuleInfo>>();
            }
        }


        /// <summary>
        /// Retrieves UserId and JWT Token from Codenation's API
        /// </summary>
        /// <returns> <see cref="UserInfo"/> </returns>
        private async Task<UserInfo> GetAuthorization()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var payload = JObject.FromObject(_userLogin);

                    var response = await httpClient.PostAsync(_appSettings.AuthorizationURL, new StringContent(payload.ToString()));

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();

                        var jObject = JObject.Parse(json);

                        return jObject.ToObject<UserInfo>();
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        ///  Check if module is available 
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="listOfModules"></param>
        /// <returns></returns>
        private bool IsModuleAvailable(string moduleName, IList<ModuleInfo> listOfModules)
        {
            ModuleInfo selectedModule = listOfModules.FirstOrDefault(x => x.name == moduleName);

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
