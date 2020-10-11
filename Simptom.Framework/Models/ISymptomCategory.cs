using System;

using Repository.Framework;

namespace Simptom.Framework.Models
{
	public partial interface ISymptomCategory
	{
	}

	public interface ISymptomCategorySearch : ISingleSearch
	{
		Guid ID { get; set; }
        string Name { get; set; }
    }

    public interface ISymptomCategoriesSearch : IMultipleSearch
    {
        string Name { get; set; }
    }
}
