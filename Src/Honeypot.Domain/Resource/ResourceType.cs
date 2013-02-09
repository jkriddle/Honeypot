using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Honeypot.Domain
{
    public enum ResourceType
    {
        /// <summary>
        /// Non-HTML string (such as names, URLs, etc)
        /// </summary>
        [Description("String")]
        String = 1,

        /// <summary>
        /// HTML block
        /// </summary>
        [Description("Html")]
        Html
    }
}
