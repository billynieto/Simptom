using System;
using System.Collections.Generic;

using Repository;
using Repository.Framework;

using Simptom.Framework;
using Simptom.Framework.Models;

namespace Simptom.Models
{
	public partial class User : Model<IUserKey>, IUser
	{
		public string Name { get; set; }
		public string Password { get; set; }

		public User(IUserKey userKey)
			: base(userKey)
		{
		}
		
		#region IModel
		
		public override void Validate()
		{
			base.Validate();
			
			RepositoryHelper.ValidateNotNull("Name", this.Name);
			RepositoryHelper.ValidateBounds("Name", this.Name, 1, 50);
			RepositoryHelper.ValidateNotNull("Password", this.Password);
			RepositoryHelper.ValidateBounds("Password", this.Password, 1, 50);
		}
		
		#endregion IModel
	}

	public partial class UserKey : IUserKey
	{
		public Guid ID { get; set; }

		public UserKey(Guid id)
		{
			this.ID = id;
		}
		
		public int CompareTo(object obj)
		{
			throw new NotImplementedException();
		}
		
		public override bool Equals(object obj)
		{
			if(obj == null)
				throw new ArgumentNullException("Object was null when comparing User keys");
			
			if(!(obj is IUserKey))
				return false;
			
			IUserKey userKey = (IUserKey)obj;
			
			return this.ID == userKey.ID;
		}
		
		public void Validate()
		{
		}
	}
}
