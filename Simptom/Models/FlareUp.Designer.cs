using System;
using System.Collections.Generic;

using Repository;
using Repository.Framework;

using Simptom.Framework;
using Simptom.Framework.Models;

namespace Simptom.Models
{
	public partial class FlareUp : Model<IFlareUpKey>, IFlareUp
	{
		private DateTime experiencedOn;

		public DateTime ExperiencedOn { get { return this.experiencedOn; } set { this.experiencedOn = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0); } }
		public double Severity { get; set; }
		public ISymptom Symptom { get; set; }
		public IUser User { get; set; }
		
		public FlareUp(IFlareUpKey flareUpKey)
			: base(flareUpKey)
		{
		}
		
		#region IModel
		
		public override void Validate()
		{
			base.Validate();
			
			RepositoryHelper.ValidateBounds("ExperiencedOn", this.ExperiencedOn, DateTime.MinValue, DateTime.MaxValue);
			RepositoryHelper.ValidateBounds("Severity", this.Severity, 0, 1);
		}
		
		#endregion IModel
	}

	public partial class FlareUpKey : IFlareUpKey
	{
		public Guid ID { get; set; }
		
		public FlareUpKey(Guid id)
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
				throw new ArgumentNullException("Object was null when comparing FlareUp keys");
			
			if(!(obj is IFlareUpKey))
				return false;
			
			IFlareUpKey flareUpKey = (IFlareUpKey)obj;
			
			return this.ID == flareUpKey.ID;
		}
		
		public void Validate()
		{
		}
	}
}
