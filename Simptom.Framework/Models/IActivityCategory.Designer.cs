using System;
using System.Collections.Generic;

using Repository.Framework;

using Simptom.Framework;

namespace Simptom.Framework.Models
{
	public partial interface IActivityCategory : IModel<IActivityCategoryKey>
	{
		string Name { get; set; }
	}

	public interface IActivityCategoryKey : IKey
	{
		Guid ID { get; }
	}
}
