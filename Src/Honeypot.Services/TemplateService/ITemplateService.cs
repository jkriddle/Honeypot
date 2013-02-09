using System.Collections.Generic;

namespace Honeypot.Services
{
    public interface ITemplateService
    {
        IDictionary<string, string> ParseTemplate(string nodeName, 
            INotificationTemplate template);

        IDictionary<string, string> ParseTemplate(string nodeName, 
            INotificationTemplate template, object templateParams);

    }
}
