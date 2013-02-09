using SharpArch.Domain.DomainModel;

namespace Honeypot.Domain
{
    public class Permission : Entity
    {
        public static string EditResources = "edit-resources";
        public static string EditUsers = "edit-users";

        /// <summary>
        /// Unique permission name that matches static permissions
        /// defined in this class.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// User-friendly description of this permission.
        /// </summary>
        public virtual string Description { get; set; }
    }
}