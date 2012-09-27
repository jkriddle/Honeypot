using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace CFC.Services.Validation
{
    public abstract class IValidationDictionary
    {
        public abstract void AddError(string key, string errorMessage);
        public abstract bool IsValid { get; }
        public abstract IList<ValidationError> Errors { get; }
        public abstract void Remove(string key);
    }
}
