using Repository.Framework;

using Simptom.Framework.Models;

namespace Simptom.Framework.Services
{
	public partial interface IActivityService : IService<IActivity, IActivityKey, IActivitySearch, IActivitiesSearch>
	{
	}
}
