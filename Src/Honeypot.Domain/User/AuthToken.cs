using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpArch.Domain.DomainModel;

namespace Honeypot.Domain
{
    public class AuthToken : Entity
    {
        public virtual string Token { get; set; }
        public virtual DateTime Expires { get; set; }
    }
}
