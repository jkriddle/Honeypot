namespace CFC.Services.EmailService 
{
    using System.Collections.Generic;
    public interface IEmailService
    {
        void Send(string email, string subject, string templatePath, EmailParams emailParams);
    }
}
