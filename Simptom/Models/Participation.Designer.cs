using System;
using System.Collections.Generic;

using Repository;
using Repository.Framework;

using Simptom.Framework;
using Simptom.Framework.Models;

namespace Simptom.Models
{
	public partial class Participation : Model<IParticipationKey>, IParticipation
	{
		private DateTime performedOn;

		public IActivity Activity { get; set; }
		public DateTime PerformedOn { get { return this.performedOn; } set { this.performedOn = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0); } }
		public double Severity { get; set; }
		public IUser User { get; set; }
		
		public Participation(IParticipationKey participationKey)
			: base(participationKey)
		{
		}
		
		#region IModel
		
		public override void Validate()
		{
			base.Validate();
			
			RepositoryHelper.ValidateBounds("PerformedOn", this.PerformedOn, DateTime.MinValue, DateTime.MaxValue);
			RepositoryHelper.ValidateBounds("Severity", this.Severity, 0, 1);
		}
		
		#endregion IModel
	}

	public partial class ParticipationKey : IParticipationKey
	{
		public Guid ID { get; set; }
		
		public ParticipationKey(Guid id)
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
				throw new ArgumentNullException("Object was null when comparing Participation keys");
			
			if(!(obj is IParticipationKey))
				return false;
			
			IParticipationKey participationKey = (IParticipationKey)obj;
			
			return this.ID == participationKey.ID;
		}
		
		public void Validate()
		{
		}
	}
}
