using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Repository;
using Repository.Framework;

using Simptom.Framework;
using Simptom.Framework.Models;
using Simptom.Framework.Repositories;

namespace Simptom.Test.Unit.Core.Repositories
{
	public abstract class IUserRepositoryTest
	{
		protected Mock<IUserKey> key;
		protected IList<Mock<IUserKey>> keys;
		protected Mock<IModelFactory> modelFactory;
		protected IUserRepository repository;
		protected Mock<IUserSearch> search;
		protected Mock<IUsersSearch> searchMultiple;
		protected TestContext testContext;
		protected Mock<IDbTransaction> transaction;
		protected Mock<IUser> user;
		protected IList<Mock<IUser>> users;
		
		public TestContext TestContext { get { return this.testContext; } set { this.testContext = value; } }
		
		[TestInitialize]
		public virtual void TestInitialize()
		{
			this.key = new Mock<IUserKey>();
			this.key.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.keys = new List<Mock<IUserKey>>() { this.key };
			
			this.user = new Mock<IUser>();
			this.user.SetupAllProperties();
			this.user.Setup(_model => _model.Key).Returns(() => { return this.key.Object; });
			this.user.Setup(_model => _model.Name).Returns("MgPLEhpWoxIqYvgsacZQCOWjRNwJlmhcJO");
			this.user.Setup(_model => _model.Password).Returns("zsuxtHnwvdwcJzMTYIwgYkFTRKFLcqFGj");
			this.users = new List<Mock<IUser>>() { user };
			
			this.modelFactory = new Mock<IModelFactory>();
			this.modelFactory.Setup(mf => mf.GenerateUserKey(It.IsAny<Guid>())).Returns(this.key.Object);
			this.modelFactory.Setup(mf => mf.GenerateUser(It.IsAny<IUserKey>())).Returns(this.user.Object);
			
			this.search = new Mock<IUserSearch>();
			this.searchMultiple = new Mock<IUsersSearch>();
			this.transaction = new Mock<IDbTransaction>();
		}
		
		[TestCleanup]
		public virtual void TestCleanup()
		{
			this.transaction = null;
		}
		
		public virtual void AddRow(IUser user)
		{
			throw new NotImplementedException();
		}
		
		public virtual void AddRows(IEnumerable<IUser> users)
		{
			foreach(IUser user in users)
				AddRow(user);
		}
		
		#region Delete
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_Multiple_Users_Test_Throws_Error_When_Any_Key_Is_Null()
		{
			//Arrange
			IList<IUserKey> injected = new List<IUserKey>() { new Mock<IUserKey>().Object, null };
			
			//Act
			this.repository.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_User_Test_Throws_Error_When_Key_Is_Null()
		{
			//Arrange
			IUserKey injected = null;
			
			//Act
			this.repository.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Delete
		
		#region Exists
		
		[TestMethod]
		public virtual void Exists_User_Test_Returns_False_When_Key_Was_Not_Found()
		{
			//Arrange
			bool actual;
			bool expected = false;
			IUserKey injected = this.key.Object;
			
			//Act
			actual = this.repository.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_User_Test_Returns_True_When_Key_Was_Found()
		{
			//Arrange
			bool actual;
			bool expected = true;
			IUserKey injected = this.key.Object;
			
			AddRow(this.user.Object);
			
			//Act
			actual = this.repository.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Users_Test_Returns_Empty_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			int actual;
			int expected = 0;
			IEnumerable<IUserKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			//Act
			actual = this.repository.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Users_Test_Returns_List_Of_Keys_When_They_Were_Found()
		{
			//Arrange
			int actual;
			int expected = 1;
			IEnumerable<IUserKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			AddRow(this.user.Object);
			
			//Act
			actual = this.repository.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Users_Test_Returns_Non_Null_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			IEnumerable<IUserKey> actual;
			IEnumerable<IUserKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			//Act
			actual = this.repository.Exists(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		#endregion Exists
		
		#region Insert
		
		[TestMethod]
		public virtual void Insert_Multiple_Users_Test_Gets_Name_For_Every_User()
		{
			//Arrange
			IList<Mock<IUser>> expected = new List<Mock<IUser>>() { this.user };
			IList<IUser> injected = this.users.Select(e => e.Object).ToList();
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<IUser> user in expected)
				user.VerifyGet(_user => _user.Name, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Insert_Multiple_Users_Test_Gets_Password_For_Every_User()
		{
			//Arrange
			IList<Mock<IUser>> expected = new List<Mock<IUser>>() { this.user };
			IList<IUser> injected = this.users.Select(e => e.Object).ToList();
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<IUser> user in expected)
				user.VerifyGet(_user => _user.Password, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Insert_Multiple_Users_Test_Throws_Error_When_Any_User_Is_Null()
		{
			//Arrange
			IEnumerable<IUser> injected = new List<IUser>() { this.user.Object, null };
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Insert_Multiple_Users_Test_Throws_Error_When_User_Is_Null()
		{
			//Arrange
			IEnumerable<IUser> injected = null;
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Insert_User_Test_Gets_User_Name()
		{
			//Arrange
			Mock<IUser> expected = this.user;
			
			//Act
			this.repository.Insert(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Name, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Insert_User_Test_Gets_User_Password()
		{
			//Arrange
			Mock<IUser> expected = this.user;
			
			//Act
			this.repository.Insert(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Password, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Insert_User_Test_Throws_Error_When_User_Is_Null()
		{
			//Arrange
			IUser injected = null;
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Insert
		
		#region Select
		
		[TestMethod]
		public virtual void Select_Users_Test_Calls_Generate_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IUsersSearch> injected = this.searchMultiple;
			
			AddRow(this.user.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateUser(It.IsAny<IUserKey>()));
		}
		
		[TestMethod]
		public virtual void Select_Users_Test_Calls_GenerateUserKey_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IUsersSearch> injected = this.searchMultiple;
			
			AddRow(this.user.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateUserKey(It.IsAny<Guid>()));
		}
		
		#endregion Select
		
		#region Select Single
		
		[TestMethod]
		public virtual void SelectSingle_User_Test_Calls_Generate_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IUserSearch> injected = this.search;
			
			AddRow(this.user.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateUser(It.IsAny<IUserKey>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_User_Test_Calls_GenerateUserKey_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IUserSearch> injected = this.search;
			
			AddRow(this.user.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateUserKey(It.IsAny<Guid>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_User_Test_Returns_Null_When_User_Is_Not_Found()
		{
			//Arrange
			IUser actual;
			Mock<IUserSearch> injected = this.search;
			
			//Act
			actual = this.repository.Select(injected.Object);
			
			//Assert
			Assert.IsNull(actual);
		}
		
		[TestMethod]
		public virtual void SelectSingle_User_Test_Returns_User_When_Is_Found()
		{
			//Arrange
			IUser actual;
			Mock<IUser> expected = this.user;
			Mock<IUserSearch> injected = this.search;
			
			AddRow(expected.Object);
			
			//Act
			actual = this.repository.Select(injected.Object);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void SelectSingle_User_Test_Sets_User_Name()
		{
			//Arrange
			Mock<IUser> expected = this.user;
			Mock<IUserSearch> injected = this.search;
			
			AddRow(expected.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.VerifySet(e => e.Name = It.IsAny<string>());
		}
		
		[TestMethod]
		public virtual void SelectSingle_User_Test_Sets_User_Password()
		{
			//Arrange
			Mock<IUser> expected = this.user;
			Mock<IUserSearch> injected = this.search;
			
			AddRow(expected.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.VerifySet(e => e.Password = It.IsAny<string>());
		}
		
		#endregion Select Single
		
		#region Update
		
		[TestMethod]
		public virtual void Update_Multiple_Users_Test_Gets_Key_For_User()
		{
			//Arrange
			IList<Mock<IUser>> expected = new List<Mock<IUser>>() { this.user };
			IList<IUser> injected = expected.Select(e => e.Object).ToList();
			
			AddRow(this.user.Object);
			
			//Act
			foreach(Mock<IUser> user in expected)
				user.Invocations.Clear();
			
			this.repository.Update(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<IUser> user in expected)
				user.VerifyGet(m => m.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Multiple_Users_Test_Gets_Name_For_User()
		{
			//Arrange
			IList<Mock<IUser>> expected = new List<Mock<IUser>>() { this.user };
			
			AddRow(this.user.Object);
			
			//Act
			foreach(Mock<IUser> user in expected)
				user.Invocations.Clear();
			
			this.repository.Update(expected.Select(e => e.Object).ToList(), this.transaction.Object);
			
			//Assert
			foreach(Mock<IUser> user in expected)
				user.VerifyGet(m => m.Name, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Multiple_Users_Test_Gets_Password_For_User()
		{
			//Arrange
			IList<Mock<IUser>> expected = new List<Mock<IUser>>() { this.user };
			
			AddRow(this.user.Object);
			
			//Act
			foreach(Mock<IUser> user in expected)
				user.Invocations.Clear();
			
			this.repository.Update(expected.Select(e => e.Object).ToList(), this.transaction.Object);
			
			//Assert
			foreach(Mock<IUser> user in expected)
				user.VerifyGet(m => m.Password, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Update_Multiple_Users_Test_Throws_Error_When_Any_User_Is_Null()
		{
			//Arrange
			IEnumerable<IUser> expected = new List<IUser>() { this.user.Object, null };
			
			//Act
			this.repository.Update(expected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Update_Multiple_Users_Test_Throws_Error_When_User_Is_Null()
		{
			//Arrange
			IEnumerable<IUser> expected = null;
			
			//Act
			this.repository.Update(expected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Update_User_Test_Gets_User_Key()
		{
			//Arrange
			Mock<IUser> expected = this.user;
			
			AddRow(expected.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_User_Test_Gets_User_Name()
		{
			//Arrange
			Mock<IUser> expected = this.user;
			
			AddRow(expected.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Name, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_User_Test_Gets_User_Password()
		{
			//Arrange
			Mock<IUser> expected = this.user;
			
			AddRow(expected.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Password, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Update_User_Test_Throws_Error_When_User_Is_Null()
		{
			//Arrange
			IUser injected = null;
			
			//Act
			this.repository.Update(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Update
	}
}
