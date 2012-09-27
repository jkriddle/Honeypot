using System;
using System.Collections.Generic;
using System.Linq;
using CFC.Domain;

using CFC.Domain.Filters;
using CFC.Infrastructure.UserRepository;
using CFC.Services.EmailService;
using CFC.Services.PagedList;
using CFC.Services.Validation;
using MEF.FacebookPlugin;


namespace CFC.Services.UserService
{
    public class UserService : IUserService
    {
        #region Fields

        private const int ResetPasswordTokenLength = 16;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;


        #endregion

        #region Constructor

        public UserService(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        #endregion

        #region CRUD Methods

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
                numPerPage, ref totalRecords).ToList();
            return new PagedList<User>(users, currentPage, numPerPage, totalRecords);
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
        /// Retrieve a single user by their Facebook ID
        /// </summary>
        /// <param name="facebookId">Facebook ID of associated user</param>
        /// <returns>Associated user</returns>
        public User GetUserByFacebookId(long facebookId)
        {
            return _userRepository.FindAll().FirstOrDefault(u => u.FacebookId == facebookId);
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
        /// Create a new user
        /// </summary>
        /// <param name="user">User to create</param>
        public void CreateUser(User user)
        {
            user.CellPhone = PhoneHelper.GetNumbers(user.CellPhone);
            var sms = SMS.Carriers();
            foreach (var pair in sms.Where(pair => user.Carrier == pair.Key))
            {
                user.SmS = pair.Value.Replace("@number", user.CellPhone);
            }
            _userRepository.Save(user);
        }

        /// <summary>
        /// Update an existing user.
        /// </summary>
        /// <param name="user">User to update</param>
        /// <param name="newPassword">New password for user</param>
        public void UpdateUser(User user, string newPassword = null)
        {
            if (!String.IsNullOrEmpty(newPassword))
            {
                GenerateUserPassword(user, newPassword);
            }
            user.CellPhone = PhoneHelper.GetNumbers(user.CellPhone);
            var sms = SMS.Carriers();
            foreach (var pair in sms.Where(pair => user.Carrier == pair.Key))
            {
                user.SmS = pair.Value.Replace("@number", user.CellPhone);
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
            if (String.IsNullOrEmpty(user.FirstName))
                validationDictionary.AddError("FirstName", "First name is required.");

            if (String.IsNullOrEmpty(user.LastName))
                validationDictionary.AddError("LastName", "Last name is required.");

            if (String.IsNullOrEmpty(user.State))
                validationDictionary.AddError("State", "State is required.");

            if (String.IsNullOrEmpty(user.Email))
            {
                validationDictionary.AddError("Email", "Email is required.");
            }
            else
            {
                // Validate email format
                if (!EmailHelper.IsValid(user.Email.Trim()))
                {
                    validationDictionary.AddError("Email", "Invalid email address.");
                }
                else
                {
                    // Validate uniqueness of email.
                    User existing = GetUserByEmail(user.Email);
                    if (existing != null && existing.Id != user.Id)
                    {
                        validationDictionary.AddError("Email", "Email is already in use.");
                    }
                }
            }

            if (user.HashedPassword == null || user.HashedPassword.Length == 0)
                validationDictionary.AddError("HashedPassword", "Password is required.");


            if (!String.IsNullOrEmpty(user.CellPhone))
            {
                // Check for exactly 10 numbers left over
                if (!PhoneHelper.IsValid(user.CellPhone))
                {
                    validationDictionary.AddError("Cellphone", "Cell phone must be 10 digit phone number.");
                }
            }
            return validationDictionary.IsValid;
        }
        #endregion


        #region Authentication and Passwords

        /// <summary>
        /// Check user authentication token for valid sessions
        /// </summary>
        /// <param name="authToken">Authentication token</param>
        /// <returns>Associated user if exists</returns>
        public User Authenticate(string authToken)
        {
            return _userRepository.FindAll().FirstOrDefault(u => u.AuthToken == authToken);
        }

        /// <summary>
        /// Check credentials and log a user into the system.
        /// </summary>
        /// <param name="email">Email to authenticate</param>
        /// <param name="password">Password to authenticate</param>
        /// <param name="hardwareId">Mobile device Id</param>
        /// <returns>True if user was successfully logged in.</returns>
        public User Authenticate(string email, string password, string hardwareId = null)
        {
            var user = GetUserByEmail(email);
            if (user == null) return null;
            var hashedPassword = HashSalt.HashPassword(password, user.Salt);
            if (!ByteArraysEqual(user.HashedPassword, hashedPassword)) return null;
            // Create a random authentication token. Must use REQUEST-FRIENDLY characters
            // to avoid a "potentially hazardous request" error in MVC frameworks.
            GenerateRandomPassword(64, "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789");
            _userRepository.Save(user);
            return user;
        }

        /// <summary>
        /// Authenticate a Facebook user. If user is not found an account will
        /// be created for them.
        /// </summary>
        /// <param name="profile">Facebook profile information</param>
        /// <returns></returns>
        public bool AuthenticateFacebookUser(UserProfile profile)
        {
            var user = GetUserByFacebookId(profile.FacebookId) ?? GetUserByEmail(profile.Email);

            if (user != null)
            {
                // User already exists, log them in. Update Facebook ID in
                // case this is their first time signing in through FB.
                user.FacebookId = profile.FacebookId;
                user.AccessToken = profile.AccessToken;
                UpdateUser(user);
                return true;
            }

            // User is not registered through Facebook
            return false;
        }

        /// <summary>
        /// Create an account in the system for an unregistered Facebook user.
        /// </summary>
        /// <param name="profile">Facebook profile information</param>
        /// <returns>Newly registered user information</returns>
        public User RegisterFacebookUser(UserProfile profile)
        {
            // Create a new user
            var user = new User
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Email = profile.Email,
                FacebookId = profile.FacebookId
            };

            GenerateUserPassword(user, GenerateRandomPassword());
            CreateUser(user);
            return user;
        }

        /// <summary>
        /// Create a forgotten password link and send out an email to the user.
        /// </summary>
        /// <param name="resetRootUrl">Base URL for email links</param>
        /// <param name="email">Email to generate link for</param>
        /// <returns>True if user was found.</returns>
        public bool GenerateForgotPasswordLink(string resetRootUrl, string email)
        {
            User user = GetUserByEmail(email);
            if (user == null) return false;

            user.ResetPasswordToken = KeyGenerator.GetUniqueKey(ResetPasswordTokenLength);
            string resetLink = resetRootUrl + "?token=" + user.ResetPasswordToken;
            _userRepository.Save(user);

            var emailParams = new EmailParams {{"ResetLink", resetLink}};
            _emailService.Send(email, "Password Reset Link", "ForgotPassword.html", emailParams);

            return true;
        }

       

        /// <summary>
        /// Update a user's password and send notification email.
        /// </summary>
        /// <param name="token">Reset password token</param>
        /// <param name="newPassword">New password</param>
        /// <returns>True if reset token was found</returns>
        public bool ResetPassword(string token, string newPassword)
        {
            User user = _userRepository.FindAll().FirstOrDefault(u => u.ResetPasswordToken == token);
            if (user == null) return false;

            // Update password
            user.Salt = HashSalt.GenerateSalt();
            user.HashedPassword = HashSalt.HashPassword(newPassword, user.Salt);
            user.ResetPasswordToken = null;
            _userRepository.SaveAndEvict(user);

            // Send notification email
            var emailParams = new EmailParams();
            _emailService.Send(user.Email, "Your Password Has Been Reset", "PasswordReset.html", emailParams);

            return true;
        }

        /// <summary>
        /// Create a random password (for users created using OAuth)
        /// </summary>
        /// <returns>Random password</returns>
        public string GenerateRandomPassword(int passwordLength = 16, string whitelistChars = "")
        {
            if (String.IsNullOrEmpty(whitelistChars))
            {
                whitelistChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@#$?_-<>$%^&*()`'<>,./";
            }

            var chars = new char[passwordLength];
            var rd = new Random();

            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = whitelistChars[rd.Next(0, whitelistChars.Length)];
            }

            return new string(chars);
        }

        /// <summary>
        /// Create a forgotten password link and send out an email to the user.
        /// </summary>
        /// <param name="user">User to generate password for</param>
        /// <param name="plainTextPassword">Password to encrypt.</param>
        public void GenerateUserPassword(User user, string plainTextPassword)
        {
            user.Salt = HashSalt.GenerateSalt();
            user.HashedPassword = HashSalt.HashPassword(plainTextPassword, user.Salt);
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
