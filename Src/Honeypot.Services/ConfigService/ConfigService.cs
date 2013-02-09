using System.Collections.Specialized;
using System.Configuration;

namespace Honeypot.Services
{
    /// <summary>
    /// Wrapper for built in .NET configuration manager
    /// </summary>
    public class ConfigService : IConfigService
    {
        #region Fields

        private readonly NameValueCollection _appSettings;

        #endregion

        public ConfigService(NameValueCollection appSettings)
        {
            _appSettings = appSettings;
        }

        public string AppSettings(string name)
        {
            return _appSettings[name];
        }
    }
}
