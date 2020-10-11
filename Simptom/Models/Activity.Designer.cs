using System;
using System.Collections.Generic;

using Repository;
using Repository.Framework;

using Simptom.Framework;
using Simptom.Framework.Models;

namespace Simptom.Models
{
	public partial class Activity : Model<IActivityKey>, IActivity
	{
		public IActivityCategory Category { get; set; }
		public string Name { get; set; }
		
		public Activity(IActivityKey activityKey)
			: base(activityKey)
		{
		}
		
		#region IModel
		
		public override void Validate()
		{
			base.Validate();
			
			RepositoryHelper.ValidateNotNull("Category", this.Category);
			RepositoryHelper.ValidateNotNull("Name", this.Name);
			RepositoryHelper.ValidateBounds("Name", this.Name, 1, 50);
		}
		
		#endregion IModel
	}

	public partial class ActivityKey : IActivityKey
	{
		public Guid ID { get; set; }
		
		public ActivityKey(Guid id)
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
				throw new ArgumentNullException("Object was null when comparing Activity keys");
			
			if(!(obj is IActivityKey))
				return false;
			
			IActivityKey activityKey = (IActivityKey)obj;
			
			return this.ID == activityKey.ID;
		}
		
		public void Validate()
		{
		}
	}
}
