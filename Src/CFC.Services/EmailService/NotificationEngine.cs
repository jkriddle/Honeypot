using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CFC.Domain;
using CFC.Services.EmailService;


namespace CFC.Services
{
    public class NotificationEngine : INotificationEngine
    {
        private static IEmailService _emailService;
        public NotificationEngine(IEmailService emailService)
        {
            _emailService = emailService;
        }
       
        private void ProcessEmail(NotificationType notificationType)
        {
            #region Fields

           
            //DEV testing
            
            EmailParams emailParams;
            #endregion
            var role = Role.Role1;
            
            #region Validate & Set Addresses
            

            #endregion


            switch (notificationType)
            {
                case NotificationType.Notification:
                    {
                        emailParams = new EmailParams
                                          {

                                          };
                        _emailService.Send("emailAddress", notificationType.GetDescription(), "GenericMessage.txt",
                                           emailParams);
                    }
                    break;
                   
            }
        }

        


    }
}
