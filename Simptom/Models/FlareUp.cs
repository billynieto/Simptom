using System;

using Simptom.Framework.Models;

namespace Simptom.Models
{
	public partial class FlareUp : IFlareUp
	{
		public FlareUp()
			: base(new FlareUpKey(Guid.Empty))
		{
		}
	}

	public partial class FlareUpKey : IFlareUpKey
	{
		public FlareUpKey()
		{
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public partial class FlareUpSearch : IFlareUpSearch
	{
		public Guid ID { get; set; }
		public DateTime? ExperiencedOn { get; set; }
		public Guid UserID { get; set; }

		public FlareUpSearch()
		{
			ID = Guid.Empty;
		}

		public void Validate()
		{
		}
	}

	public partial class FlareUpsSearch : IFlareUpsSearch
	{
		public DateTime? End { get; set; }
		public DateTime? Start { get; set; }
		public Guid UserID { get; set; }

		public void Validate()
		{
		}
	}
}
