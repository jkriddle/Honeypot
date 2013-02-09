using System;
using System.Collections.Generic;
using System.Linq;
using Honeypot.Domain;
using Honeypot.Domain.Filters;
using Honeypot.Infrastructure;

namespace Honeypot.Services
{
    public class UserService : IUserService
    {
        #region Fields

        private readonly IConfigService _configService;
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IAuthTokenRepository _authTokenRepository;
        private readonly INotificationService _notificationService;
        private const int ResetPasswordTokenLength = 20;

        #endregion

        #region Constructor

        public UserService(IConfigService configService,
            IUserRepository userRepository, 
            IPermissionRepository permissionRepository,
            IAuthTokenRepository authTokenRepository,
            INotificationService notificationService)
        {
            _configService = configService;
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _authTokenRepository = authTokenRepository;
            _notificationService = notificationService;
        }

        #endregion

        #region CRUD Methods

        public void DeleteUser(User user)
        {
            user.Status = UserStatus.Deleted;
            _userRepository.Save(user);
        }

        /// <summary>
        /// Retrieve a single user by their unique ID.
        /// </summary>
        /// <param name="id">ID of associated user</param>
        /// <returns>Associated user</returns>
        public User GetUserById(int id)
        {
            return _userRepository.FindOne(id);
        }

        /// <summary>
        /// Retrieve a single user by their email.
        /// </summary>
        /// <param name="email">Email of associated user</param>
        /// <returns>Associated user</returns>
        public User GetUserByEmail(string email)
        {
            return _userRepository.FindAll().FirstOrDefault(u => u.Email == email);
        }

        /// <summary>
        /// Retrieve a single user by their auth token.
        /// </summary>
        /// <param name="token">Token of associated user</param>
        /// <returns>Associated user</returns>
        public User GetUserByToken(string token)
        {
            var user = _userRepository.FindAll().FirstOrDefault(u => u.AuthToken.Token == token);
            if (user != null && user.AuthToken.Expires <= DateTime.UtcNow) return null;
            return user;
        }


        /// <summary>
        /// Retrieve a single user by their reset token.
        /// </summary>
        /// <param name="token">Token for associated user</param>
        /// <returns>Associated user</returns>
        public User GetUserByResetToken(string token)
        {
            return _userRepository.FindAll().FirstOrDefault(u => u.ResetPasswordToken == token);
        }

        /// <summary>
        /// Retrieve a paged list of all users.
        /// </summary>
        /// <param name="filter">Filter to search users</param>
        /// <param name="currentPage">Current page number </param>
        /// <param name="numPerPage"># of records per page</param>
        /// <returns>Paged list of users</returns>
        public IPagedList<User> GetUsers(UserFilter filter, int currentPage, int numPerPage)
        {
            int totalRecords = 0;
            List<User> users = _userRepository.Search(filter, currentPage,
                numPerPage, out totalRecords).ToList();
            return new PagedList<User>(users, currentPage, numPerPage, totalRecords);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="user">User to create</param>
        public void CreateUser(User user)
        {
            // Save permissions
            if (user.Role == Role.Administrator)
            {
                var p = _permissionRepository.FindAll().FirstOrDefault(pe => pe.Name == Permission.EditUsers);
                var up = new UserPermission()
                {
                    User = user,
                    Permission = p
                };
                user.Permissions.Add(up);

                p = _permissionRepository.FindAll().FirstOrDefault(pe => pe.Name == Permission.EditResources);
                up = new UserPermission()
                {
                    User = user,
                    Permission = p
                };
                user.Permissions.Add(up);
            }

            // Save user
            _userRepository.Save(user);
        }

        /// <summary>
        /// Update an existing user.
        /// </summary>
        /// <param name="user">User to update</param>
        /// <param name="newPassword">New password for user</param>
        public void UpdateUser(User user, string newPassword = null)
        {
            if (!string.IsNullOrEmpty(newPassword))
            {
                GenerateUserPassword(user, newPassword);
            }
            _userRepository.Save(user);
        }

        #endregion

        #region Validation

        /// <summary>
        /// Check that user data is valid.
        /// </summary>
        /// <param name="user">User to validate</param>
        /// <param name="validationDictionary">Dictionary of errors</param>
        /// <returns></returns>
        public bool ValidateUser(User user, IValidationDictionary validationDictionary)
        {
            // Email cannot be null
            if (string.IsNullOrEmpty(user.Email))
            {
                validationDictionary.AddError("Email", "Email is required.");
            } else if (!EmailValidator.IsValid(user.Email))
            {
                validationDictionary.AddError("Email", "Invalid email address.");
            } else {
                // Check for existing email
                var existing = GetUserByEmail(user.Email);
                if (existing != null && existing.Id != user.Id)
                {
                    validationDictionary.AddError("Email", "Email is already in use. Please try another");
                }
            }

            if (user.HashedPassword == null || user.HashedPassword.Length == 0)
            {
                validationDictionary.AddError("Password", "Password is required.");
            }
            return validationDictionary.IsValid;
        }

        #endregion

        #region Authentication and Passwords

        /// <summary>
        /// Check credentials for a user's token
        /// </summary>
        /// <param name="token">Token to authenticate</param>
        /// <returns>True if user was successfully logged in.</returns>
        public User Authenticate(string token)
        {
            var user = GetUserByToken(token);
            if (user == null) return null;

            // Do not allow deleted users to authenticate
            if (user.Status == UserStatus.Deleted) return null;

            return user;
        }

        /// <summary>
        /// Check credentials and log a user into the system.
        /// </summary>
        /// <param name="email">Email to authenticate</param>
        /// <param name="password">Password to authenticate</param>
        /// <returns>True if user was successfully logged in.</returns>
        public User Authenticate(string email, string password)
        {
            var user = GetUserByEmail(email);
            if (user == null) return null;
            var hashedPassword = HashSalt.HashPassword(password, user.Salt);
            if (!ByteArraysEqual(user.HashedPassword, hashedPassword)) return null;

            // Do not allow deleted users to authenticate
            if (user.Status == UserStatus.Deleted) return null;

            // Create new auth token
            if (user.AuthToken != null)
            {
                _authTokenRepository.Delete(user.AuthToken);
            }
            user.AuthToken = new AuthToken
                {
                    Token = Convert.ToBase64String(HashSalt.GenerateSalt()),
                    Expires = DateTime.UtcNow.AddHours(24)
                };
            _authTokenRepository.Save(user.AuthToken);

            _userRepository.Save(user);
            return user;
        }

        /// <summary>
        /// Generate an encrypted password for the specified user.
        /// </summary>
        /// <param name="user">User to generate password for</param>
        /// <param name="plainTextPassword">Password to encrypt.</param>
        public void GenerateUserPassword(User user, string plainTextPassword)
        {
            user.Salt = HashSalt.GenerateSalt();
            user.HashedPassword = HashSalt.HashPassword(plainTextPassword, user.Salt);
        }

        /// <summary>
        /// Generate a password reset token and email the link to the user. 
        /// </summary>
        /// <param name="user"></param>
        public void GenerateResetRequest(User user)
        {
            user.ResetPasswordToken = KeyGenerator.GetUniqueKey(ResetPasswordTokenLength);
            _userRepository.Save(user);

            _notificationService.Notify(user, new NotificationTemplate(
                _configService.AppSettings("NotificationTemplatePath"), "ForgotPassword"), new
                {
                    ResetToken = user.ResetPasswordToken
                });
        }

        /// <summary>
        /// Update a user's password and send notification email.
        /// </summary>
        /// <param name="user">User to reset password</param>
        /// <param name="newPassword">New password</param>
        /// <returns>True if reset token was found</returns>
        public void ResetPassword(User user, string newPassword)
        {
            if (String.IsNullOrEmpty(newPassword))
            {
                throw new Exception("New password cannot be empty.");
            }

            // Update password
            user.Salt = HashSalt.GenerateSalt();
            user.HashedPassword = HashSalt.HashPassword(newPassword, user.Salt);
            user.ResetPasswordToken = null;
            _userRepository.SaveAndEvict(user);

            // Send notification email
            _notificationService.Notify(user, new NotificationTemplate(
                _configService.AppSettings("NotificationTemplatePath"), "PasswordReset"), 
                null);
        }

        /// <summary>
        /// Check that two bytes are equal (for encrypted password comparison).
        /// </summary>
        /// <param name="b1">First byte</param>
        /// <param name="b2">Byte to compare</param>
        /// <returns>True if bytes are equal</returns>
        private bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            return !b1.Where((t, i) => t != b2[i]).Any();
        }

        #endregion

    }
}
