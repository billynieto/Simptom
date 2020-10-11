using System;

using Repository.Framework;

namespace Simptom.Framework.Models
{
	public partial interface IParticipation
	{
	}

	public interface IParticipationSearch : ISingleSearch
	{
		Guid ID { get; set; }
		DateTime? PerformedOn { get; set; }
        Guid UserID { get; set; }
    }

	public interface IParticipationsSearch : IMultipleSearch
	{
		DateTime? End { get; set; }
		DateTime? Start { get; set; }
		Guid UserID { get; set; }
	}
}
