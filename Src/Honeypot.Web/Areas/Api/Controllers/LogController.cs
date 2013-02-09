using System.Web;
using System.Web.Http;
using Honeypot.Domain.Filters;
using Honeypot.Services;
using Honeypot.Web.Areas.Api.Models;
using Honeypot.Web.Attributes;

namespace Honeypot.Web.Areas.Api.Controllers
{
    [NoCache]
    public class LogController : BaseController
    {
        #region Fields

        private readonly IMapperService _mapper;

        #endregion

        #region Constructor

        public LogController(IUserService userService,
            ILogService logService,
            IMapperService mapper)
            : base(userService, logService)
        {
            _mapper = mapper;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieve a single log by ID
        /// </summary>
        /// <param name="id">Log's ID</param>
        /// <returns>Log data</returns>
        [HttpGet, ApiAuth]
        public LogViewModel GetOne(int id)
        {
            var log = LogService.GetLogById(id);
            if (log == null)
            {
                throw new HttpException(404, "Log not found.");
            }

            return new LogViewModel(log);
        }

        /// <summary>
        /// Retrieve a list of logs based on filter criteria
        /// </summary>
        /// <param name="inputModel">Filter criteria</param>
        /// <returns>Paged list of logs</returns>
        [HttpGet, ApiAuth]
        public LogListViewModel Get([FromUri]LogListInputModel inputModel)
        {
            if (inputModel == null) inputModel = new LogListInputModel();

            var filter = new LogFilter();
            _mapper.Map(inputModel, filter);

            var users = LogService.GetLogs(filter, inputModel.CurrentPage, inputModel.NumPerPage);
            return new LogListViewModel(users);
        }

        #endregion

        #region Private Helper Methods


        #endregion
    }

}
