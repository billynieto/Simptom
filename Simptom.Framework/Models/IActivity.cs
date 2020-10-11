using Repository.Framework;
using System;

namespace Simptom.Framework.Models
{
	public partial interface IActivity
	{
	}

	public interface IActivitySearch : ISingleSearch
	{
		Guid ID { get; set; }
        string Name { get; set; }
    }

    public interface IActivitiesSearch : IMultipleSearch
    {
        string Name { get; set; }
    }
}
