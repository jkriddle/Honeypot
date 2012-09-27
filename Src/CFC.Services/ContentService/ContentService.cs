using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFC.Domain;
using CFC.Infrastructure.PageRepository;
using CFC.Services.Validation;

namespace CFC.Services.ContentService
{
    public class ContentService : IContentService
    {
        private IPageRepository _contentRepository;

        public ContentService(IPageRepository pageRepository)
        {
            _contentRepository = pageRepository;
        }

        public IList<Page> GetPages()
        {
            return _contentRepository.FindAll().ToList();
        }

        public Page GetPageById(int id)
        {
            return _contentRepository.FindOne(id);
        }

        public Page GetPageByName(string name)
        {
            return _contentRepository.FindAll().FirstOrDefault(p => p.Name == name);
        }

        /// <summary>
        /// Update an existing page.
        /// </summary>
        /// <param name="page">Page to update</param>
        public void UpdatePage(Page page)
        {
            _contentRepository.Save(page);
        }


        /// <summary>
        /// Validate a page
        /// </summary>
        /// <param name="page">Page to validate</param>
        /// <param name="validationDictionary">Validation information</param>
        public bool ValidatePage(Page page, IValidationDictionary validationDictionary)
        {
            if (String.IsNullOrEmpty(page.Name))
            {
                validationDictionary.AddError("Name", "Page name is required.");
            }

            return validationDictionary.IsValid;
        }

    }
}
