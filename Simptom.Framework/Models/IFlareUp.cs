using System;

using Repository.Framework;

namespace Simptom.Framework.Models
{
	public partial interface IFlareUp
	{
	}

	public interface IFlareUpSearch : ISingleSearch
	{
		Guid ID { get; set; }
		DateTime? ExperiencedOn { get; set; }
        Guid UserID { get; set; }
    }

	public interface IFlareUpsSearch : IMultipleSearch
	{
		DateTime? End { get; set; }
		DateTime? Start { get; set; }
		Guid UserID { get; set; }
    }
}
