using System;

using Simptom.Framework.Models;

namespace Simptom.Models
{
	public partial class Activity : IActivity
	{
		public Activity()
			: base(new ActivityKey(Guid.Empty))
		{
		}
	}

	public partial class ActivityKey : IActivityKey
	{
		public ActivityKey()
		{
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public partial class ActivitySearch : IActivitySearch
	{
		public Guid ID { get; set; }
		public string Name { get; set; }

		public ActivitySearch()
		{
			ID = Guid.Empty;
		}

		public void Validate()
		{
		}
	}

	public partial class ActivitiesSearch : IActivitiesSearch
	{
		public string Name { get; set; }
		
		public void Validate()
		{
		}
	}
}
