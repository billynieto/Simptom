using System;
using System.Collections.Generic;

using Repository.Framework;

using Simptom.Framework;

namespace Simptom.Framework.Models
{
	public partial interface IParticipation : IModel<IParticipationKey>
	{
		IActivity Activity { get; set; }
		DateTime PerformedOn { get; set; }
		double Severity { get; set; }
		IUser User { get; set; }
	}

	public interface IParticipationKey : IKey
	{
		Guid ID { get; }
	}
}
