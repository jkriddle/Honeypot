using System.Security.AccessControl;
using SharpArch.Domain.DomainModel;

namespace Honeypot.Domain
{
    public class Resource : Entity
    {
        /// <summary>
        /// Category of resource this content contains
        /// </summary>
        public virtual ResourceCategory Category { get; set; }

        /// <summary>
        /// Type of resource this content contains
        /// </summary>
        public virtual ResourceType Type { get; set; }

        /// <summary>
        /// Unique name of resource
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Resource content value
        /// </summary>
        public virtual string Value { get; set; }

        /// <summary>
        /// Indicates if this resource may be modified by authorized users
        /// </summary>
        public virtual bool IsReadOnly { get; set; }
    }
}
