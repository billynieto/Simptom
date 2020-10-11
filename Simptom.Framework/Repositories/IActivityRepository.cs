using Repository.Framework;

using Simptom.Framework.Models;

namespace Simptom.Framework.Repositories
{
	public partial interface IActivityRepository : IRepository<IActivity, IActivityKey, IActivitySearch, IActivitiesSearch>
	{
	}
}
