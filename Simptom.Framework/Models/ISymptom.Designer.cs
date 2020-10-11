using System;
using System.Collections.Generic;

using Repository.Framework;

using Simptom.Framework;

namespace Simptom.Framework.Models
{
	public partial interface ISymptom : IModel<ISymptomKey>
	{
		ISymptomCategory Category { get; set; }
		string Name { get; set; }
	}

	public interface ISymptomKey : IKey
	{
		Guid ID { get; }
	}
}
