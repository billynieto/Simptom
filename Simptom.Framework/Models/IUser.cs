using System;

using Repository.Framework;

namespace Simptom.Framework.Models
{
	public partial interface IUser
	{
	}

	public interface IUserSearch : ISingleSearch
	{
		Guid ID { get; set; }
        string Name { get; set; }
    }

    public interface IUsersSearch : IMultipleSearch
    {
        string Name { get; set; }
    }
}
