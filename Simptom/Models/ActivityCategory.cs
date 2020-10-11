using System;

using Simptom.Framework.Models;

namespace Simptom.Models
{
	public partial class ActivityCategory : IActivityCategory
	{
		public ActivityCategory()
			: base(new ActivityCategoryKey(Guid.Empty))
		{
		}
	}

	public partial class ActivityCategoryKey : IActivityCategoryKey
	{
		public ActivityCategoryKey()
		{
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public partial class ActivityCategorySearch : IActivityCategorySearch
	{
		public Guid ID { get; set; }
		public string Name { get; set; }

		public ActivityCategorySearch()
		{
			ID = Guid.Empty;
		}

		public void Validate()
		{
		}
	}

	public partial class ActivityCategoriesSearch : IActivityCategoriesSearch
	{
		public string Name { get; set; }

		public void Validate()
		{
		}
	}
}
