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
using Simptom.Framework.Services;
using Simptom.Services;

namespace Simptom.Test.Unit.Core.Services
{
	public abstract class IUserServiceTest
	{
		protected Mock<IUserKey> key;
		protected IList<Mock<IUserKey>> keys;
		protected Mock<IModelFactory> modelFactory;
		protected Mock<IUserRepository> repository;
		protected Mock<IUserSearch> search;
		protected Mock<IUsersSearch> searchMultiple;
		protected IUserService service;
		protected ConnectionState state;
		protected TestContext testContext;
		protected Mock<IDbTransaction> transaction;
		protected Mock<IUser> user;
		protected IList<Mock<IUser>> users;
		
		public TestContext TestContext { get { return this.testContext; } set { this.testContext = value; } }
		
		[TestInitialize]
		public virtual void TestInitialize()
		{
			this.key = new Mock<IUserKey>();
			this.key.SetupAllProperties();
			this.key.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.keys = new List<Mock<IUserKey>>() { this.key };
			this.search = new Mock<IUserSearch>();
			this.searchMultiple = new Mock<IUsersSearch>();
			
			this.user = new Mock<IUser>();
			this.user.SetupAllProperties();
			this.user.Setup(m => m.Key).Returns(this.key.Object);
			this.users = new List<Mock<IUser>>() { this.user };
			
			this.modelFactory = new Mock<IModelFactory>();
			
			this.state = ConnectionState.Closed;
			this.repository = new Mock<IUserRepository>();
			this.repository.Setup(_repository => _repository.Open()).Callback(() => { this.state = ConnectionState.Open; });
			this.repository.Setup(_repository => _repository.Close()).Callback(() => { this.state = ConnectionState.Closed; });
			this.repository.Setup(_repository => _repository.IsOpen).Returns(() => { return this.state == ConnectionState.Open; });
			this.transaction = new Mock<IDbTransaction>();
		}
		
		[TestCleanup]
		public virtual void TestCleanup()
		{
			this.repository = null;
		}
		
		#region Delete
		
		[TestMethod]
		public virtual void Delete_Multiple_Users_Test_Calls_Delete_On_Repository()
		{
			//Arrange
			IList<IUserKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IUserKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Delete(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>()), Times.Exactly(users.Count)); }
			catch { this.repository.Verify(r => r.Delete(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>()), Times.Exactly(1)); }
		}
		
		[TestMethod]
		public virtual void Delete_Multiple_Users_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			IList<IUserKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IUserKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<IUserKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_Multiple_Users_Test_Throws_Error_When_List_Of_UserKey_Is_Null()
		{
			//Arrange
			IList<IUserKey> injected = null;
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public virtual void Delete_Multiple_Users_Test_Throws_Error_When_UserKey_Is_Not_Found_In_Repository()
		{
			//Arrange
			IList<IUserKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IUserKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(new List<IUserKey>());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IUserKey>());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Delete_User_Test_Calls_Delete_On_Repository()
		{
			//Arrange
			IUserKey injected = user.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IUserKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Delete(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())); }
			catch { this.repository.Verify(r => r.Delete(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())); }
		}
		
		[TestMethod]
		public virtual void Delete_User_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			IUserKey injected = user.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IUserKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<IUserKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_User_Test_Throws_Error_When_UserKey_Is_Null()
		{
			//Arrange
			IUserKey injected = null;
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public virtual void Delete_User_Test_Throws_Error_When_UserKey_Not_Found_In_Repository()
		{
			//Arrange
			IUserKey injected = user.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IUserKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(new List<IUserKey>());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IUserKey>());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
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
			
			this.repository.Setup(r => r.Exists(It.IsAny<IUserKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(new List<IUserKey>());
			
			//Act
			actual = this.service.Exists(injected);
			
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
			
			this.repository.Setup(r => r.Exists(It.IsAny<IUserKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			actual = this.service.Exists(injected);
			
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
			
			this.repository.Setup(r => r.Exists(It.IsAny<IUserKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(new List<IUserKey>());
			
			//Act
			actual = this.service.Exists(injected).Count();
			
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
			
			this.repository.Setup(r => r.Exists(It.IsAny<IUserKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			actual = this.service.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Users_Test_Returns_Non_Null_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			IEnumerable<IUserKey> actual;
			IEnumerable<IUserKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IUserKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(new List<IUserKey>());
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		#endregion Exists
		
		#region Find
		
		[TestMethod]
		public virtual void Find_Users_Test_Calls_Select_On_Repository()
		{
			//Arrange
			IUsersSearch injected = this.searchMultiple.Object;
			
			this.repository.Setup(r => r.Select(It.IsAny<IUsersSearch>())).Returns(new List<IUser>());
			
			//Act
			this.service.Find(injected);
			
			//Assert
			this.repository.Verify(r => r.Select(It.IsAny<IUsersSearch>()), Times.Exactly(1));
		}
		
		[TestMethod]
		public virtual void Find_Users_Test_Calls_Validate_On_Search()
		{
			//Arrange
			Mock<IUsersSearch> expected = this.searchMultiple;
			IUsersSearch injected = this.searchMultiple.Object;
			
			this.repository.Setup(r => r.Select(It.IsAny<IUsersSearch>())).Returns(new List<IUser>());
			
			//Act
			this.service.Find(injected);
			
			//Assert
			expected.Verify(r => r.Validate());
		}
		
		#endregion Find
		
		#region Find Single
		
		[TestMethod]
		public virtual void FindSingle_User_Test_Calls_Select_On_Repository()
		{
			//Arrange
			IUserSearch injected = this.search.Object;
			
			
			this.repository.Setup(r => r.Select(It.IsAny<IUserSearch>())).Returns(this.user.Object);
			
			//Act
			this.service.FindSingle(injected);
			
			//Assert
			this.repository.Verify(r => r.Select(It.IsAny<IUserSearch>()), Times.Exactly(1));
		}
		
		[TestMethod]
		public virtual void FindSingle_User_Test_Calls_Validate_On_Search()
		{
			//Arrange
			Mock<IUserSearch> expected = this.search;
			IUserSearch injected = this.search.Object;
			
			
			this.repository.Setup(r => r.Select(It.IsAny<IUserSearch>())).Returns(this.user.Object);
			
			//Act
			this.service.FindSingle(injected);
			
			//Assert
			expected.Verify(e => e.Validate());
		}
		
		[TestMethod]
		public virtual void FindSingle_User_Test_Returns_Null_When_User_Is_Not_Found_In_Repository()
		{
			//Arrange
			IUser actual;
			IUserSearch injected = this.search.Object;
			
			this.repository.Setup(r => r.Select(It.IsAny<IUserSearch>())).Returns((IUser)null);
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNull(actual);
		}
		
		[TestMethod]
		public virtual void FindSingle_User_Test_Returns_User_When_Is_Found_In_Repository()
		{
			//Arrange
			IUser actual;
			IUserSearch injected = this.search.Object;
			
			
			this.repository.Setup(r => r.Select(It.IsAny<IUserSearch>())).Returns(this.user.Object);
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		#endregion Find Single
		
		#region Save
		
		[TestMethod]
		public virtual void Save_Multiple_Users_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			List<IUser> injected = this.users.Select(e => e.Object).ToList();
			
			this.repository.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(new List<IUserKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IUserKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<IUserKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Users_Test_Calls_Insert_On_Repository_For_New_Users()
		{
			//Arrange
			List<IUser> injected = this.users.Select(e => e.Object).ToList();
			
			this.repository.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(new List<IUserKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IUserKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.repository.Verify(r => r.Insert(It.IsAny<IEnumerable<IUser>>(), It.IsAny<IDbTransaction>()));
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Users_Test_Calls_Update_On_Repository_For_Existing_Users()
		{
			//Arrange
			List<IUser> injected = this.users.Select(e => e.Object).ToList();
			
			this.repository.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.repository.Verify(r => r.Update(It.IsAny<IEnumerable<IUser>>(), It.IsAny<IDbTransaction>()));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Save_Multiple_Users_Test_Throws_Error_When_Any_User_Is_Null()
		{
			//Arrange
			List<IUser> injected = new List<IUser>() { new Mock<IUser>().Object, null, new Mock<IUser>().Object };
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Save_User_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			Mock<IUser> expected = this.user;
			IUser injected = this.user.Object;
			
			this.repository.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(new List<IUserKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IUserKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<IUserKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_User_Test_Calls_Insert_On_Repository_For_New_Users()
		{
			//Arrange
			Mock<IUser> expected = this.user;
			IUser injected = this.user.Object;
			
			this.repository.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(new List<IUserKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IUserKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Insert(It.IsAny<IUser>(), It.IsAny<IDbTransaction>())); }
			catch { this.repository.Verify(r => r.Insert(It.IsAny<IEnumerable<IUser>>(), It.IsAny<IDbTransaction>())); }
		}
		
		[TestMethod]
		public virtual void Save_User_Test_Calls_Update_On_Repository_For_Existing_Users()
		{
			//Arrange
			Mock<IUser> expected = this.user;
			IUser injected = this.user.Object;
			
			this.repository.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Update(It.IsAny<IUser>(), It.IsAny<IDbTransaction>())); }
			catch { this.repository.Verify(r => r.Update(It.IsAny<IEnumerable<IUser>>(), It.IsAny<IDbTransaction>())); }
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Save_User_Test_Throws_Error_When_User_Is_Null()
		{
			//Arrange
			IUser injected = null;
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Save
	}
}
