using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Simptom.Framework.Models;
using Simptom.Framework.Repositories;
using Simptom.Framework.Services;
using Simptom.Test.Factories;

namespace Simptom.Test.Integration.Core.Services
{
	public abstract class IUserServiceTest
	{
		protected IUser initial;
		protected IModelFactory modelFactory;
		protected IUsersSearch searchMultipleThatFindsNothing;
		protected IUsersSearch searchMultipleThatFindsSomething;
		protected IUserSearch searchThatReturnsNull;
		protected IUserSearch searchThatReturnsSomething;
		protected IUserService service;
		protected TestContext testContext;
		protected IUser user;
		protected IUser userNotFound;
		
		public TestContext TestContext { get { return this.testContext; } set { this.testContext = value; } }
		
		[TestInitialize]
		public virtual void TestInitialize()
		{
			this.modelFactory = new ValidModelFactory();
		}
		
		[TestCleanup]
		public virtual void TestCleanup()
		{
			if(this.service.Exists(this.userNotFound.Key))
				this.service.Delete(this.userNotFound.Key, null);
			
			if(this.initial != null)
				this.service.Save(this.initial, null);
		}
		
		public virtual IUserSearch PrepareSearch(IUser user)
		{
			throw new NotImplementedException();
		}
		
		#region Delete
		
		#endregion Delete
		
		#region Exists
		
		[TestMethod]
		public virtual void Exists_User_Test_Returns_False_When_User_Is_Not_Found()
		{
			//Arrange
			bool actual;
			bool expected = false;
			IUserKey injected = this.userNotFound.Key;
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_User_Test_Returns_True_When_User_Is_Found()
		{
			if(this.user == null)
				throw new AssertInconclusiveException("You must provide a User that is already in the repository for this test to be effective.");
			
			//Arrange
			bool actual;
			bool expected = true;
			IUserKey injected = this.user.Key;
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Users_Test_Does_Not_Return_Null_List_When_Nothing_Matches()
		{
			//Arrange
			IEnumerable<IUserKey> actual = null;
			IEnumerable<IUserKey> injected = new List<IUserKey>() { this.userNotFound.Key };
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void Exists_Users_Test_Returns_Empty_List_When_Nothing_Matches()
		{
			//Arrange
			int actual;
			int expected = 0;
			IEnumerable<IUserKey> injected = new List<IUserKey>() { this.userNotFound.Key };
			
			//Act
			actual = this.service.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Users_Test_Returns_List_Of_UserKeys_That_Match()
		{
			if(this.user == null)
				throw new AssertInconclusiveException("You must provide a User that is already in the repository for this test to be effective.");
			
			//Arrange
			IEnumerable<IUserKey> actual = null;
			IEnumerable<IUserKey> injected = new List<IUserKey>() { this.user.Key };
			IEnumerable<IUserKey> expected = injected;
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected.Count(), actual.Count());
			Assert.IsTrue(expected.All(_e => actual.Any(_a => _e.Equals(_a))));
			Assert.IsTrue(actual.All(_a => expected.Any(_e => _e.Equals(_a))));
		}
		
		#endregion Exists
		
		#region Find
		
		[TestMethod]
		public virtual void Find_Users_Test_Does_Not_Return_Null_List_When_Nothing_Matches()
		{
			//Arrange
			IEnumerable<IUser> actual;
			IUsersSearch injected = this.searchMultipleThatFindsNothing;
			
			//Act
			actual = this.service.Find(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void Find_Users_Test_Returns_Empty_List_When_Nothing_Matches()
		{
			//Arrange
			int actual;
			int expected = 0;
			IUsersSearch injected = this.searchMultipleThatFindsNothing;
			
			//Act
			actual = this.service.Find(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Find_Users_Test_Throws_Error_When_Search_Is_Null()
		{
			//Arrange
			IUsersSearch injected = null;
			
			//Act
			this.service.Find(injected);
			
			//Assert
		}
		
		#endregion Find
		
		#region Find Single
		
		[TestMethod]
		public virtual void FindSingle_User_Test_Returns_Model_When_Found()
		{
			if(this.user == null)
				throw new AssertInconclusiveException("You must provide a User that is already in the repository for this test to be effective.");
			
			//Arrange
			IUser actual;
			IUserSearch injected = this.searchThatReturnsSomething;
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void FindSingle_User_Test_Returns_Null_When_Not_Found()
		{
			//Arrange
			IUser actual;
			IUserSearch injected = this.searchThatReturnsNull;
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNull(actual);
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void FindSingle_User_Test_Throws_Error_When_Search_Is_Null()
		{
			//Arrange
			IUserSearch injected = null;
			
			//Act
			this.service.FindSingle(injected);
			
			//Assert
		}
		
		#endregion Find Single
		
		#region Save
		
		[TestMethod]
		public virtual void Save_Multiple_Users_Test_Preserves_Name_When_Is_Maxed_And_Users_Already_Exists()
		{
			if(this.user == null)
				throw new AssertInconclusiveException("You must provide a User that is already in the repository for this test to be effective.");
			
			//Arrange
			IEnumerable<IUser> injected = new List<IUser>() { this.user };
			//                .........10........20........30........40........50........60
			this.user.Name = "uLOUaFNJZIclXEABQfzABuRDFWcoXYVTZOUHUTprnguoUNfrgB";
			
			IUser actual = null;
			IUser expected = this.user;
			
			IUserSearch search = PrepareSearch(expected);
			if(search == null)
				throw new AssertInconclusiveException("You must override the function that prepares a User Search model for this test to be effective.");
			
			this.initial = this.service.FindSingle(this.searchThatReturnsSomething);
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			actual = this.service.FindSingle(search);
			
			Assert.AreEqual(expected.Name, actual.Name);
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Users_Test_Preserves_Password_When_Is_Maxed_And_Users_Already_Exists()
		{
			if(this.user == null)
				throw new AssertInconclusiveException("You must provide a User that is already in the repository for this test to be effective.");
			
			//Arrange
			IEnumerable<IUser> injected = new List<IUser>() { this.user };
			//                    .........10........20........30........40........50........60
			this.user.Password = "BWGmThkGeHZrCynRpogTfEwPgYtGtxaGbYpUeWDMJwCfVvgeWW";
			
			IUser actual = null;
			IUser expected = this.user;
			
			IUserSearch search = PrepareSearch(expected);
			if(search == null)
				throw new AssertInconclusiveException("You must override the function that prepares a User Search model for this test to be effective.");
			
			this.initial = this.service.FindSingle(this.searchThatReturnsSomething);
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			actual = this.service.FindSingle(search);
			
			Assert.AreEqual(expected.Password, actual.Password);
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Users_Test_Preserves_Users_When_They_Already_Exist()
		{
			if(this.user == null)
				throw new AssertInconclusiveException("You must provide a User that is already in the repository for this test to be effective.");
			
			//Arrange
			List<IUser> injected = new List<IUser>() { this.user };
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Users_Test_Preserves_Users_When_They_Are_New()
		{
			if(this.user == null)
				throw new AssertInconclusiveException("You must provide a User that is already in the repository for this test to be effective.");
			
			//Arrange
			List<IUser> injected = new List<IUser>() { this.userNotFound };
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			IUser expected = injected.First();
			IUserSearch search = PrepareSearch(expected);
			if(search != null)
			{
				IUser actual = this.userNotFound = this.service.FindSingle(search);
			
				Assert.IsNotNull(actual);
				Assert.AreEqual(expected.Name, actual.Name, "Name");
				Assert.AreEqual(expected.Password, actual.Password, "Password");
			}
		}
		
		[TestMethod]
		public virtual void Save_User_Test_Preserves_Name_When_Is_Maxed_And_Users_Already_Exists()
		{
			if(this.user == null)
				throw new AssertInconclusiveException("You must provide a User that is already in the repository for this test to be effective.");
			
			//Arrange
			IUser injected = this.user;
			//               .........10........20........30........40........50........60
			injected.Name = "ebPrcZUlpeCyLaNjmiKTHWQinvaCJRQLiwUUdHMUMIDkOPuuBK";
			
			IUser actual = null;
			IUser expected = injected;
			
			IUserSearch search = PrepareSearch(expected);
			if(search == null)
				throw new AssertInconclusiveException("You must override the function that prepares a User Search model for this test to be effective.");
			
			this.initial = this.service.FindSingle(this.searchThatReturnsSomething);
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			actual = this.service.FindSingle(search);
			
			Assert.AreEqual(expected.Name, actual.Name);
		}
		
		[TestMethod]
		public virtual void Save_User_Test_Preserves_Password_When_Is_Maxed_And_Users_Already_Exists()
		{
			if(this.user == null)
				throw new AssertInconclusiveException("You must provide a User that is already in the repository for this test to be effective.");
			
			//Arrange
			IUser injected = this.user;
			//                   .........10........20........30........40........50........60
			injected.Password = "zfnIFWzRfVODmGGtqEisNtLujIcrTLRoJwXVstssumQHXfkVyC";
			
			IUser actual = null;
			IUser expected = injected;
			
			IUserSearch search = PrepareSearch(expected);
			if(search == null)
				throw new AssertInconclusiveException("You must override the function that prepares a User Search model for this test to be effective.");
			
			this.initial = this.service.FindSingle(this.searchThatReturnsSomething);
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			actual = this.service.FindSingle(search);
			
			Assert.AreEqual(expected.Password, actual.Password);
		}
		
		[TestMethod]
		public virtual void Save_User_Test_Preserves_User_When_It_Already_Exists()
		{
			if(this.user == null)
				throw new AssertInconclusiveException("You must provide a User that is already in the repository for this test to be effective.");
			
			//Arrange
			IUser injected = this.user;
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Save_User_Test_Preserves_User_When_It_Is_New()
		{
			if(this.user == null)
				throw new AssertInconclusiveException("You must provide a User that is already in the repository for this test to be effective.");
			
			//Arrange
			IUser injected = this.userNotFound;
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			IUser expected = injected;
			IUserSearch search = PrepareSearch(expected);
			if(search != null)
			{
				IUser actual = this.userNotFound = this.service.FindSingle(search);
			
				Assert.IsNotNull(actual);
				Assert.AreEqual(expected.Name, actual.Name, "Name");
				Assert.AreEqual(expected.Password, actual.Password, "Password");
			}
		}
		
		#endregion Save
	}
}
