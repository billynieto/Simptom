using System;
using System.Collections.Generic;

using Repository.Framework;

using Simptom.Framework.Models;

namespace Simptom.Framework.Repositories
{
	public partial interface IActivityCategoryRepository : IRepository<IActivityCategory, IActivityCategoryKey, IActivityCategorySearch, IActivityCategoriesSearch>
	{
	}
}
