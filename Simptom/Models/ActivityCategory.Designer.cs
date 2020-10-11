using System;
using System.Collections.Generic;

using Repository;
using Repository.Framework;

using Simptom.Framework;
using Simptom.Framework.Models;

namespace Simptom.Models
{
	public partial class ActivityCategory : Model<IActivityCategoryKey>, IActivityCategory
	{
		public string Name { get; set; }
		
		public ActivityCategory(IActivityCategoryKey activityCategoryKey)
			: base(activityCategoryKey)
		{
		}
		
		#region IModel
		
		public override void Validate()
		{
			base.Validate();
			
			RepositoryHelper.ValidateNotNull("Name", this.Name);
			RepositoryHelper.ValidateBounds("Name", this.Name, 1, 50);
		}
		
		#endregion IModel
	}

	public partial class ActivityCategoryKey : IActivityCategoryKey
	{
		public Guid ID { get; set; }
		
		public ActivityCategoryKey(Guid id)
		{
			this.ID = id;
		}
		
		public int CompareTo(object obj)
		{
			throw new NotImplementedException();
		}
		
		public override bool Equals(object obj)
		{
			if(obj == null)
				throw new ArgumentNullException("Object was null when comparing ActivityCategory keys");
			
			if(!(obj is IActivityCategoryKey))
				return false;
			
			IActivityCategoryKey activityCategoryKey = (IActivityCategoryKey)obj;
			
			return this.ID == activityCategoryKey.ID;
		}
		
		public void Validate()
		{
		}
	}
}
