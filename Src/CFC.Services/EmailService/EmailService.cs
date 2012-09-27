using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Threading;

namespace CFC.Services.EmailService
{
    public class EmailService : IEmailService
    {

        #region Fields

        private readonly string _templateFileRoot;

        #endregion

        public EmailService(string templateFileRoot)
        {
            _templateFileRoot = templateFileRoot;
        }

        /// <summary>
        /// Sends an email based on specified template. Params are used to
        /// replace templated content.
        /// </summary>
        /// <param name="email">Recipient email</param>
        /// <param name="subject">Email subject</param>
        /// <param name="templatePath">Template file to use</param>
        /// <param name="emailParams">Parameters to replace template tokens</param>
        public void Send(string email, string subject, string templatePath, EmailParams emailParams)
        {
            // Setup Template Content
           
            var templateContent = File.ReadAllText(Path.Combine(_templateFileRoot, templatePath), Encoding.Unicode);

            foreach(KeyValuePair<string, string> param in emailParams)
            {
                templateContent = templateContent.Replace("{" + param.Key + "}", param.Value);
            }

            // Send Email
            
            var message = new MailMessage();
            message.From =  new MailAddress("support@carfarecompare.com", "Car Fare Compare Support");
            message.To.Add(email);
            message.Subject = subject;
            message.Body = templateContent;
            message.IsBodyHtml = true;
             
            //DevTesting
            var client = new SmtpClient
                             {
                                 Host = "smtp.gmail.com",
                                 Port = 587,
                                 EnableSsl = true,
                                 DeliveryMethod = SmtpDeliveryMethod.Network,
                                 UseDefaultCredentials = false,
                                 Credentials = new NetworkCredential("riddlebrothersmailtest@gmail.com", "R1ddl3br0s")
                             };
            string mailToken = email + "_" + DateTime.Now.Ticks.ToString();
            client.SendAsync(message, mailToken);
             
           
        }
    }
}
