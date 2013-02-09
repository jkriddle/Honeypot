using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Honeypot.Services
{
    public interface INotificationTemplate
    {
        string ContainingDirectory { get; }
        XElement Read();
    }
}
