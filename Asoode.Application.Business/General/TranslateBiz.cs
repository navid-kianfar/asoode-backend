using Asoode.Core.Contracts.General;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Asoode.Business.General
{
    internal class TranslateBiz : ITranslateBiz
    {
        public TranslateBiz(
            IConfiguration configuration,
            IJsonBiz jsonBiz,
            IServerInfo serverInfo)
        {
            DefaultCulture = configuration["Setting:I18n:Default"];
            AllowedCultures = jsonBiz.Deserialize<string[]>(configuration["Setting:I18n:Allowed"]);
            Vocabulary = new Dictionary<string, Dictionary<string, string>>();
            var files = Directory.GetFiles(serverInfo.I18nRootPath, "*.json");
            foreach (var file in files)
            {
                var content = File.ReadAllText(file);
                var key = Path.GetFileNameWithoutExtension(file);
                Vocabulary[key] = jsonBiz.Deserialize<Dictionary<string, string>>(content);
            }
        }

        public string[] AllowedCultures { get; set; }
        public string DefaultCulture { get; set; }
        public string Root { get; set; }
        public Dictionary<string, Dictionary<string, string>> Vocabulary { get; set; }

        public string Get(string key, string culture = null)
        {
            try
            {
                if (string.IsNullOrEmpty(culture))
                    culture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
                    // culture = DefaultCulture;
                return Vocabulary[culture][key];
            }
            catch
            {
                return key;
            }
        }
    }
}