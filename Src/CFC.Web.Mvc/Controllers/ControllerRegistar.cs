using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using CFC.Infrastructure;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace CFC.Web.Mvc.Controllers
{
    [Export(typeof(IRegistar))]
    public class ControllerRegistrar : IRegistar
    {
        public void Register(WindsorContainer windsorContainer)
        {
            throw new NotImplementedException();
        }
    }
}