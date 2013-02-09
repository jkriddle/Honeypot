using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Honeypot.Services
{
    public abstract class IValidationDictionary
    {
        #region Fields

        public readonly IList<ValidationError> ErrorDictionary;

        #endregion

        #region Constructor

        protected IValidationDictionary()
        {
            ErrorDictionary = new List<ValidationError>();
        }

        #endregion

        public abstract void AddError(string key, string errorMessage);
        public abstract bool IsValid { get; }
        public abstract IList<string> Errors { get; }
        public abstract void Remove(string key);
        public abstract void Merge(IValidationDictionary validationDictionary);
    }
}
