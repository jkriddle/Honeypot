using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Honeypot.Services
{
    public interface IMapperService
    {
        TDest Map<TSrc, TDest>(TSrc source, TDest dest) where TDest : class;
    }
}
