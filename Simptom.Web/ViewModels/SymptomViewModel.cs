using System;
using System.Collections.Generic;

using Simptom.Framework.Models;

namespace Simptom.Web.ViewModels
{
    public class SymptomViewModel
    {
        public IEnumerable<ISymptomCategory> Categories { get; set; }
        public string ErrorMessage { get; set; }
        public ISymptom Symptom { get; set; }
    }
}