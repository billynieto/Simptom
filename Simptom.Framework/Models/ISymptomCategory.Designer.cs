using System;
using System.Collections.Generic;

using Repository.Framework;

using Simptom.Framework;

namespace Simptom.Framework.Models
{
	public partial interface ISymptomCategory : IModel<ISymptomCategoryKey>
	{
		string Name { get; set; }
	}

	public interface ISymptomCategoryKey : IKey
	{
		Guid ID { get; }
	}
}
