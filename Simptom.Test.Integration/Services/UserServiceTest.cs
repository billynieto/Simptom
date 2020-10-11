using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Simptom.Framework.Models;
using Simptom.Framework.Repositories;
using Simptom.Framework.Services;
using Simptom.Server.Repositories;
using Simptom.Services;
using Simptom.Test.Integration.Core.Services;

namespace Simptom.Test.Integration.Services
{
	[TestClass]
	public class UserServiceTest : IUserServiceTest
	{
		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();

			IDbConnection connection = new SqlConnection("Data Source='BADASS-TABLET-P\\SQLEXPRESS'; Initial Catalog = 'Simptom'; User ID = 'SystemUser'; Password = 'insecure'");

			IUserRepository repository = new UserRepository(connection, this.modelFactory);
			this.service = new UserService(repository, this.modelFactory);

			IUsersSearch search = this.modelFactory.GenerateUsersSearch();
			this.user = this.service.Find(search).FirstOrDefault();
			
			Guid id = Guid.NewGuid();
			IUserKey userKeyNotFound = this.modelFactory.GenerateUserKey(id);
			this.userNotFound = this.modelFactory.GenerateUser(userKeyNotFound);
			this.userNotFound.Name = "Doesn't Exist";
			this.userNotFound.Password = "Horrible Password";

			this.searchMultipleThatFindsNothing = this.modelFactory.GenerateUsersSearch();
			this.searchMultipleThatFindsNothing.Name = this.userNotFound.Name;

			this.searchMultipleThatFindsSomething = this.modelFactory.GenerateUsersSearch();
			this.searchMultipleThatFindsSomething.Name = this.user.Name;

			this.searchThatReturnsNull = this.modelFactory.GenerateUserSearch();
			this.searchThatReturnsNull.ID = this.userNotFound.Key.ID;

			this.searchThatReturnsSomething = this.modelFactory.GenerateUserSearch();
			this.searchThatReturnsSomething.ID = this.user.Key.ID;
		}
		
		[TestCleanup]
		public override void TestCleanup()
		{
			base.TestCleanup();
			
			this.service = null;
			
			this.searchMultipleThatFindsNothing = null;
			this.searchMultipleThatFindsSomething = null;
			this.searchThatReturnsNull = null;
			this.searchThatReturnsSomething = null;
		}
		
		public override IUserSearch PrepareSearch(IUser user)
		{
			IUserSearch search = this.modelFactory.GenerateUserSearch();
			search.Name = user.Name;

			return search;
		}
	}
}
