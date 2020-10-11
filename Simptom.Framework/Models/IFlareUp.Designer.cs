using System;
using System.Collections.Generic;

using Repository.Framework;

using Simptom.Framework;

namespace Simptom.Framework.Models
{
	public partial interface IFlareUp : IModel<IFlareUpKey>
	{
		DateTime ExperiencedOn { get; set; }
		double Severity { get; set; }
		ISymptom Symptom { get; set; }
		IUser User { get; set; }
	}

	public interface IFlareUpKey : IKey
	{
		Guid ID { get; }
	}
}
