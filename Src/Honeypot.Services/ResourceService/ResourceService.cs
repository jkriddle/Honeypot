using System;
using System.Collections.Generic;
using System.Linq;
using Honeypot.Domain;
using Honeypot.Domain.Filters;
using Honeypot.Infrastructure;

namespace Honeypot.Services
{
    public class ResourceService : IResourceService
    {
        #region Fields

        private readonly IResourceRepository _resourceRepository;

        #endregion

        #region Constructor

        public ResourceService(IResourceRepository resourceRepository)
        {
            _resourceRepository = resourceRepository;
        }

        #endregion

        #region CRUD Methods

        /// <summary>
        /// Retrieve a paged list of resources
        /// </summary>
        /// <param name="filter">Search criteria</param>
        /// <param name="currentPage">Current page</param>
        /// <param name="numPerPage">Number of items per page</param>
        /// <returns>Paged list of matching resources</returns>
        public PagedList<Resource> GetResources(ResourceFilter filter, int currentPage, int numPerPage)
        {
            int totalRecords = 0;
            List<Resource> resources = _resourceRepository.Search(filter, currentPage,
                numPerPage, out totalRecords).ToList();
            return new PagedList<Resource>(resources, currentPage, numPerPage, totalRecords);
        }

        /// <summary>
        /// Retrieve a resource by the specified ID
        /// </summary>
        /// <param name="name">ID of resource</param>
        /// <returns>Matching resource</returns>
        public Resource GetResourceById(int id)
        {
            return _resourceRepository.FindOne(id);
        }

        /// <summary>
        /// Retrieve a resource by the specified name
        /// </summary>
        /// <param name="name">Name of resource</param>
        /// <returns>Matching resource</returns>
        public Resource GetResourceByName(string name)
        {
            return _resourceRepository.FindAll().FirstOrDefault(n => n.Name == name);
        }

        /// <summary>
        /// Add a resource to the system
        /// </summary>
        /// <param name="resource">Resource to add</param>
        public void CreateResource(Resource resource)
        {
            _resourceRepository.Save(resource);
        }

        /// <summary>
        /// Update a resource's information
        /// </summary>
        /// <param name="resource">Resource to update</param>
        public void UpdateResource(Resource resource)
        {
            if (resource.IsReadOnly)
            {
                throw new InvalidOperationException("Cannot update a read-only resource.");
            }
            _resourceRepository.Save(resource);
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validate resource information is valid to save
        /// </summary>
        /// <param name="resource">Resource to validate</param>
        /// <param name="validationDictionary">Validation error dictionary</param>
        /// <returns>If resource is valid</returns>
        public bool ValidateResource(Resource resource, IValidationDictionary validationDictionary)
        {
            if (String.IsNullOrEmpty(resource.Name))
                validationDictionary.AddError("Name", "Resource name is required.");

            return validationDictionary.IsValid;
        }

        #endregion

    }
}
