using System;
using System.Collections.Generic;

using Repository.Framework;

using Simptom.Framework;

namespace Simptom.Framework.Models
{
	public partial interface IUser : IModel<IUserKey>
	{
		string Name { get; set; }
		string Password { get; set; }
	}

	public interface IUserKey : IKey
	{
		Guid ID { get; }
	}
}
