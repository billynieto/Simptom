using System;

using Simptom.Framework.Models;

namespace Simptom.Models
{
	public partial class Participation : IParticipation
	{
		public Participation()
			: base(new ParticipationKey(Guid.Empty))
		{
		}
	}

	public partial class ParticipationKey : IParticipationKey
	{
		public ParticipationKey()
		{
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public partial class ParticipationSearch : IParticipationSearch
	{
		public Guid ID { get; set; }
		public DateTime? PerformedOn { get; set; }
		public Guid UserID { get; set; }

		public ParticipationSearch()
		{
			ID = Guid.Empty;
			UserID = Guid.Empty;
		}

		public void Validate()
		{
		}
	}

	public partial class ParticipationsSearch : IParticipationsSearch
	{
		public DateTime? End { get; set; }
		public DateTime? Start { get; set; }
		public Guid UserID { get; set; }

		public void Validate()
		{
		}
	}
}
