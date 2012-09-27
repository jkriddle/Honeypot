using System.Collections.Generic;
using CFC.Domain;
using CFC.Services.Validation;

namespace CFC.Services.ContentService
{
    public interface IContentService
    {
        IList<Page> GetPages();
        Page GetPageById(int id);
        Page GetPageByName(string name);
        void UpdatePage(Page page);
        bool ValidatePage(Page page, IValidationDictionary validationDictionary);
    }
}
