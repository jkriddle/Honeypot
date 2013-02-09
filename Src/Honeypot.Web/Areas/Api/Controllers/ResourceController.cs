using System;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using Honeypot.Domain;
using Honeypot.Domain.Filters;
using Honeypot.Services;
using Honeypot.Web.Areas.Api.Models;
using Honeypot.Web.Attributes;
using Honeypot.Web.Providers;

namespace Honeypot.Web.Areas.Api.Controllers
{
    [NoCache]
    public class ResourceController : BaseController
    {
        #region Fields

        private readonly IResourceService _resourceService;
        private readonly IMapperService _mapper;

        #endregion

        #region Constructor

        public ResourceController(IUserService userService, 
            IResourceService resourceService, 
            ILogService logService,
            IMapperService mapper)
            : base(userService, logService)
        {
            _mapper = mapper;
            _resourceService = resourceService;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieve a single resource by ID
        /// </summary>
        /// <param name="id">Resource's ID</param>
        /// <returns>Resource data</returns>
        [HttpGet, ApiAuth]
        public ResourceViewModel GetOne(int? id)
        {
            var resource = _resourceService.GetResourceById(id.Value);
            if (resource == null)
            {
                throw new HttpException(404, "Resource not found.");
            }

            return new ResourceViewModel(resource);
        }

        /// <summary>
        /// Retrieve a list of resources based on filter criteria
        /// </summary>
        /// <param name="inputModel">Filter criteria</param>
        /// <returns>Paged list of resources</returns>
        [HttpGet, ApiAuth]
        public ResourceListViewModel Get([FromUri]ResourceListInputModel inputModel)
        {
            if (inputModel == null) inputModel = new ResourceListInputModel();

            var filter = new ResourceFilter();
            _mapper.Map(inputModel, filter);

            var resources = _resourceService.GetResources(filter, inputModel.CurrentPage, inputModel.NumPerPage);
            return new ResourceListViewModel(resources);
        }

        /// <summary>
        /// Update a resource's information.
        /// </summary>
        /// <param name="inputModel">Info to update</param>
        /// <returns></returns>
        [HttpPost, ApiAuth]
        public BaseResponseModel Update(UpdateResourceInputModel inputModel)
        {
            var vm = new BaseResponseModel();

            // Validate request
            var validationState = new ValidationDictionary();

            // Get existing resource
            var resource = _resourceService.GetResourceById(inputModel.ResourceId);
            if (resource == null)
            {
                throw new HttpException(404, "Resource not found.");
            }

            // Do not allow editing of resources other than yourself if you
            // don't have permissions
            if (!CurrentUser.HasPermission(Permission.EditResources))
            {
                throw new HttpException(401, "You do not have permissions to complete this action.");
            }

            // Copy properties
            resource.Value = inputModel.Value;

            if (_resourceService.ValidateResource(resource, validationState))
            {
                _resourceService.UpdateResource(resource);

                LogService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    IpAddress = GetClientIp(ControllerContext.Request),
                    Level = LogLevel.Info,
                    Message = "Resource " + resource.Name + " (ID #" + resource.Id + ") was updated.",
                    User = CurrentUser
                });

                vm.Success = true;
            }

            vm.Errors = validationState.Errors;
            return vm;
        }

        #endregion

        #region Private Helper Methods

        #endregion
    }

}
