using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Honeypot.Domain
{
    public enum ResourceCategory
    {
        /// <summary>
        /// Page or other public content
        /// </summary>
        [Description("Content")]
        Content = 1, 

        /// <summary>
        /// System setting
        /// </summary>
        [Description("Settings")]
        Settings
    }
}
