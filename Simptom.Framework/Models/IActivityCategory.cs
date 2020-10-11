using System;

using Repository.Framework;

namespace Simptom.Framework.Models
{
	public partial interface IActivityCategory
	{
	}

	public interface IActivityCategorySearch : ISingleSearch
	{
		Guid ID { get; set; }
		string Name { get; set; }
	}

	public interface IActivityCategoriesSearch : IMultipleSearch
	{
		string Name { get; set; }
	}
}
