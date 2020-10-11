using System;
using System.Collections.Generic;

using Repository.Framework;

using Simptom.Framework;

namespace Simptom.Framework.Models
{
	public partial interface IActivity : IModel<IActivityKey>
	{
		IActivityCategory Category { get; set; }
		string Name { get; set; }
	}

	public interface IActivityKey : IKey
	{
		Guid ID { get; }
	}
}
