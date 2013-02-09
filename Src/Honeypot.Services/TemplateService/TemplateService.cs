using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Honeypot.Services
{
    /// <summary>
    /// Base class for notification services.
    /// </summary>
    public class TemplateService : ITemplateService
    {
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
                }
                else
                {
                    parsedNodes.Add(node.Name.ToString(), node.Value);
                }
            }

            return parsedNodes;
        }

        #endregion
    }
}
