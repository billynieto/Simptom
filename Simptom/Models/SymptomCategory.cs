using System;

using Simptom.Framework.Models;

namespace Simptom.Models
{
	public partial class SymptomCategory : ISymptomCategory
	{
		public SymptomCategory()
			: base(new SymptomCategoryKey(Guid.Empty))
		{
		}
	}

	public partial class SymptomCategoryKey : ISymptomCategoryKey
	{
		public SymptomCategoryKey()
		{
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public partial class SymptomCategorySearch : ISymptomCategorySearch
	{
		public Guid ID { get; set; }
		public string Name { get; set; }

		public SymptomCategorySearch()
		{
			ID = Guid.Empty;
		}

		public void Validate()
		{
		}
	}

	public partial class SymptomCategoriesSearch : ISymptomCategoriesSearch
	{
		public string Name { get; set; }

		public void Validate()
		{
		}
	}
}
