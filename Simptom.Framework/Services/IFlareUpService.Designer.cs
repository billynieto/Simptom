using System;
using System.Collections.Generic;

using Repository.Framework;

using Simptom.Framework.Models;
using Simptom.Framework.Repositories;

namespace Simptom.Framework.Services
{
	public partial interface IFlareUpService : IService<IFlareUp, IFlareUpKey, IFlareUpSearch, IFlareUpsSearch>
	{
	}
}
