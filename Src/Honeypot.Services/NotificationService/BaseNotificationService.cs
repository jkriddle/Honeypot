using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Honeypot.Domain;

namespace Honeypot.Services
{
    /// <summary>
    /// Base class for notification services.
    /// </summary>
    public abstract class BaseNotificationService : INotificationService
    {

        #region Fields

        protected readonly ILogService LogService;

        #endregion

        #region Constructor

        protected BaseNotificationService(ILogService logService)
        {
            LogService = logService;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Parse a template that does not contain any placeholders
        /// </summary>
        /// <param name="nodeName">Name of the node containing the template contents</param>
        /// <param name="template">Template to be parsed</param>
        /// <returns></returns>
        public IDictionary<string, string> ParseTemplate(string nodeName,
            INotificationTemplate template)
        {
            return ParseTemplate(nodeName, template, null);
        }

        /// <summary>
        /// Parse a template and replace placeholders with specified values.
        /// </summary>
        /// <param name="nodeName">Name of the node containing the template contents</param>
        /// <param name="template">Template to be parsed</param>
        /// <param name="templateParams">Object contaiing placeholder propreties to be replaced</param>
        /// <returns></returns>
        public IDictionary<string, string> ParseTemplate(string nodeName,
            INotificationTemplate template, object templateParams)
        {
            // Load template
            var templateNode = template.Read();
            if (templateNode == null)
            {
                throw new XmlException(
                    string.Format("Could not find template node {0} in template.", nodeName));
            }

            var parsedNodes = new Dictionary<string, string>();

            // Parse placeholders
            foreach (var node in templateNode.Descendants())
            {
                // Loop through template properties and replace placeholders with values
                if (templateParams != null)
                {
                    var properties = templateParams.GetType()
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    foreach (var prop in properties)
                    {
                        var value = (string)prop.GetValue(templateParams, null);
                        string placeholder = "{{" + prop.Name + "}}";
                        parsedNodes.Add(node.Name.ToString(), node.Value.Replace(placeholder, value));
                    }
                } else
                {
                    parsedNodes.Add(node.Name.ToString(), node.Value);
                }
            }

            return parsedNodes;
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Send a notification that does not contain any placeholders
        /// </summary>
        /// <param name="recipient">User that will receive the notification</param>
        /// <param name="template">Template to be parsed</param>
        /// <returns></returns>
        public Task<bool> Notify(User recipient, INotificationTemplate template)
        {
            return Notify(recipient, template, null);
        }

        /// <summary>
        /// Send a notification with placeholders replaced with specified parameters
        /// </summary>
        /// <param name="recipient">User that will receive the notification</param>
        /// <param name="template">Template to be parsed</param>
        /// <param name="messageParams">Object containing propreties to be replaced</param>
        /// <returns></returns>
        public abstract Task<bool> Notify(User recipient, INotificationTemplate template, object messageParams);

        #endregion
    }
}
