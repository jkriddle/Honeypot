using CFC.Infrastructure.UserRepository;
using CFC.Services;
using CFC.Services.EmailService;
using CFC.Services.UserService;
using Moq;

namespace CFC.Tests
{
    public class ServiceMiniMart
    {
        #region UnitTesting Setup MockObjects
        /// <summary>
        /// Creates the user service.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <returns></returns>
        public static UserService CreateUserService(Mock<IUserRepository> userRepository)
        {
            var emailService = new Mock<IEmailService>();
            return new UserService(userRepository.Object, emailService.Object);
        }
        #endregion

        #region Integration Testing Setup

        /// <summary>
        /// Creates the user service.
        /// </summary>
        /// <returns></returns>
        public static UserService CreateUserService()
        {
            return new UserService(new UserRepository(), new EmailService("../"));
        }

        #endregion
    }
}
