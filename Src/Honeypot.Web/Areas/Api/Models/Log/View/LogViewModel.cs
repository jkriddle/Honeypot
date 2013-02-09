using Honeypot.Domain;
using Honeypot.Web.Models;

namespace Honeypot.Web.Areas.Api.Models
{
    public class LogViewModel : BaseViewModel
    {
        #region Fields

        private readonly Log _innerLog;

        #endregion

        #region Constructor

        public LogViewModel(Log log)
        {
            _innerLog = log;
        }

        #endregion

        #region Properties

        public int LogId { get { return _innerLog.Id; } }
        public string Category { get { return _innerLog.Category.GetDescription(); } }
        public string Details { get { return _innerLog.Details; } }
        public string Message { get { return _innerLog.Message; } }
        public string IpAddress { get { return _innerLog.IpAddress; } }
        public string Level { get { return _innerLog.Level.GetDescription(); } }
        public string LogDate 
        {
            get { return _innerLog.LogDate.ToString("MM/dd/yyyy hh:mm:ss tt"); } 
        }
        public int? UserId
        {
            get
            {
                return _innerLog.User == null ? (int?)null : _innerLog.User.Id;
            }
        }
        public string UserEmail
        {
            get
            {
                return _innerLog.User == null ? string.Empty : _innerLog.User.Email;
            }
        }

        #endregion


    }
}