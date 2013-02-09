using System.Collections.Generic;
using System.Linq;
using SharpArch.Domain.DomainModel;

namespace Honeypot.Domain
{
    public class User : Entity
    {

        #region Constructor

        public User()
        {
            Status = UserStatus.Active;
            Permissions = new List<UserPermission>();
            Logs = new List<Log>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// User's email address
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// User's one-way encrypted password hash
        /// </summary>
        public virtual byte[] HashedPassword { get; set; }

        /// <summary>
        /// Salt used for encryption
        /// </summary>
        public virtual byte[] Salt { get; set; }

        /// <summary>
        /// Authorization token for API login
        /// </summary>
        public virtual AuthToken AuthToken { get; set; }

        /// <summary>
        /// User's role
        /// </summary>
        public virtual Role Role { get; set; }

        /// <summary>
        /// User's status
        /// </summary>
        public virtual UserStatus Status { get; set; }

        /// <summary>
        /// Granular permissions assigned to user
        /// </summary>
        public virtual IList<UserPermission> Permissions { get; set; }

        /// <summary>
        /// Logs attributed to this user
        /// </summary>
        public virtual IList<Log> Logs { get; set; }

        /// <summary>
        /// Unique string used to reset user's password
        /// </summary>
        public virtual string ResetPasswordToken { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Indictates if this user has permission to the specified
        /// action.
        /// </summary>
        /// <param name="permission">Permission to check</param>
        /// <returns>If user has permission to specified action</returns>
        public virtual bool HasPermission(string permission)
        {
            return Permissions.Any(p => p.Permission.Name == permission);
        }

        #endregion

    }
}