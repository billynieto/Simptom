using System;

using Simptom.Framework.Models;

namespace Simptom.Models
{
	public partial class User : IUser
	{
		public User()
			: base(new UserKey(Guid.Empty))
		{
		}
	}

	public partial class UserKey : IUserKey
	{
		public UserKey()
		{
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public partial class UserSearch : IUserSearch
	{
		public Guid ID { get; set; }
		public string Name { get; set; }

		public UserSearch()
		{
			ID = Guid.Empty;
		}

		public void Validate()
		{
		}
	}

	public partial class UsersSearch : IUsersSearch
	{
		public string Name { get; set; }

		public void Validate()
		{
		}
	}
}
