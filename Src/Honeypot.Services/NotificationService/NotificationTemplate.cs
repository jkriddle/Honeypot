using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Honeypot.Services
{
    public class NotificationTemplate : INotificationTemplate
    {
        private readonly string _templatePath;

        public string ContainingDirectory
        {
            get { return Path.GetDirectoryName(_templatePath); }
        }

        /// <summary>
        /// Create a new notificatino template using the full path to the XML template
        /// </summary>
        /// <param name="templatePath">Full path to template</param>
        public NotificationTemplate(string templatePath)
        {
            _templatePath = templatePath;
        }

        /// <summary>
        /// Creates a new notification template based on root path, and the template
        /// file name.
        /// </summary>
        /// <param name="templateDirectory">Directory that contains templates</param>
        /// <param name="templateFileName">Name of this template (no extension)</param>
        public NotificationTemplate(string templateDirectory, string templateFileName)
        {
            _templatePath = Path.Combine(templateDirectory, templateFileName + ".xml");
        }

        public XElement Read()
        {
            return XElement.Load(_templatePath);
        }
    }
}
