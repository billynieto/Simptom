using System;
using System.Collections.Generic;

using Repository.Framework;

using Simptom.Framework.Models;
using Simptom.Framework.Repositories;

namespace Simptom.Framework.Services
{
	public partial interface IActivityCategoryService : IService<IActivityCategory, IActivityCategoryKey, IActivityCategorySearch, IActivityCategoriesSearch>
	{
	}
}
