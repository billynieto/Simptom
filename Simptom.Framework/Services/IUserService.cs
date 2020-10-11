using Repository.Framework;

using Simptom.Framework.Models;

namespace Simptom.Framework.Services
{
	public partial interface IUserService : IService<IUser, IUserKey, IUserSearch, IUsersSearch>
	{
	}
}
