using Honeypot.Domain;
using Honeypot.Domain.Filters;

namespace Honeypot.Services
{
    
    public interface ILogService
    {
        Log GetLogById(int id);
        IPagedList<Log> GetLogs(LogFilter filter, int currentPage, int numPerPage);
        void CreateLog(Log log);
    }
}
