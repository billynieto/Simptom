using System;
using System.Collections.Generic;

using Simptom.Framework.Models;

namespace Simptom.Web.ViewModels
{
    public class SymptomsViewModel
    {
        public IEnumerable<ISymptomCategory> Categories { get; set; }
        public IEnumerable<ISymptom> Symptoms { get; set; }
    }
}