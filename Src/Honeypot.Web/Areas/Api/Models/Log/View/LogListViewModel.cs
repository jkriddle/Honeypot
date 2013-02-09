using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Honeypot.Domain;
using Honeypot.Services;

namespace Honeypot.Web.Areas.Api.Models
{
    public class LogListViewModel : PagedViewModel<Log>
    {
        #region Constructor

        public LogListViewModel(IPagedList<Log> logs)
            : base(logs)
        {
            Logs = new List<LogViewModel>();
            foreach (var log in logs.Items)
            {
                Logs.Add(new LogViewModel(log));
            }
        }

        #endregion

        #region Public Properties

        public IList<LogViewModel> Logs { get; set; }

        #endregion

    }
}