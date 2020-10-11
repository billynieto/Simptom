using Repository.Framework;

using Simptom.Framework.Models;

namespace Simptom.Framework.Repositories
{
	public partial interface IUserRepository : IRepository<IUser, IUserKey, IUserSearch, IUsersSearch>
	{
	}
}
