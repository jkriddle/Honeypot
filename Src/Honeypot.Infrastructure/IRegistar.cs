using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;

namespace Honeypot.Infrastructure
{
    public interface IRegistar
    {
        void Register(WindsorContainer windsorContainer);
    }

}
