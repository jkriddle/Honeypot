namespace CFC.Domain
{
    using SharpArch.Domain.DomainModel;

    public class User : Entity
    {
        private const int DefaultServiceareaDistance = 10;
        private int? _serviceAreaDistance;

        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string FullName { get { return FirstName + " " + LastName; } }
        public virtual string Email { get; set; }
        public virtual string State { get; set; }
        public virtual string CellPhone { get; set; }
        public virtual string Carrier { get; set; }
        public virtual byte[] HashedPassword { get; set; }
        public virtual byte[] Salt { get; set; }
        public virtual string ResetPasswordToken { get; set; }
        public virtual Role Role { get; set; }
        public virtual string SmS { get; set; }
        public virtual byte[] Image { get; set; }

 

        /// <summary>
        /// User's Facebook ID
        /// </summary>
        public virtual long FacebookId { get; set; }

        /// <summary>
        /// User's access token for Facebook
        /// </summary>
        public virtual string AccessToken { get; set; }

        /// <summary>
        /// API auth token
        /// </summary>
        public virtual string AuthToken { get; set; }


        public User()
        {
            Role = Role.Role1;
        }
    }
}