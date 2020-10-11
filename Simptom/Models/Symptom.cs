using System;

using Simptom.Framework.Models;

namespace Simptom.Models
{
	public partial class Symptom : ISymptom
	{
		public Symptom()
			: base(new SymptomKey(Guid.Empty))
		{
		}
	}

	public partial class SymptomKey : ISymptomKey
	{
		public SymptomKey()
		{
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public partial class SymptomSearch : ISymptomSearch
	{
		public Guid ID { get; set; }
		public string Name { get; set; }

		public SymptomSearch()
		{
			ID = Guid.Empty;
		}

		public void Validate()
		{
		}
	}

	public partial class SymptomsSearch : ISymptomsSearch
	{
		public string Name { get; set; }

		public void Validate()
		{
		}
	}
}
