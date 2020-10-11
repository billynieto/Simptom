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
	public abstract class IFlareUpServiceTest
	{
		protected Mock<IFlareUp> flareUp;
		protected IList<Mock<IFlareUp>> flareUps;
		protected Mock<IFlareUpKey> key;
		protected IList<Mock<IFlareUpKey>> keys;
		protected Mock<IModelFactory> modelFactory;
		protected Mock<IFlareUpRepository> repository;
		protected Mock<IFlareUpSearch> search;
		protected Mock<IFlareUpsSearch> searchMultiple;
		protected IFlareUpService service;
		protected ConnectionState state;
		protected Mock<ISymptom> symptom;
		protected Mock<ISymptomKey> symptomKey;
		protected IList<Mock<ISymptomKey>> symptomKeys;
		protected IList<Mock<ISymptom>> symptoms;
		protected Mock<ISymptomService> symptomService;
		protected Mock<ISymptomsSearch> symptomsSearch;
		protected TestContext testContext;
		protected Mock<IDbTransaction> transaction;
		protected Mock<IUser> user;
		protected Mock<IUserKey> userKey;
		protected IList<Mock<IUserKey>> userKeys;
		protected IList<Mock<IUser>> users;
		protected Mock<IUserService> userService;
		protected Mock<IUsersSearch> usersSearch;
		
		public TestContext TestContext { get { return this.testContext; } set { this.testContext = value; } }
		
		[TestInitialize]
		public virtual void TestInitialize()
		{
			this.symptomKey = new Mock<ISymptomKey>();
			this.symptomKey.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.symptomKeys = new List<Mock<ISymptomKey>>() { this.symptomKey };
			this.symptom = new Mock<ISymptom>();
			this.symptom.SetupAllProperties();
			this.symptom.Setup(_model => _model.Key).Returns(() => { return this.symptomKey.Object; });
			this.symptom.Setup(_symptom => _symptom.Category).Returns((ISymptomCategory)null);
			this.symptom.Setup(_model => _model.Name).Returns("TzXUiXuezTQILaWlQYA");
			this.symptoms = new List<Mock<ISymptom>> { this.symptom };
			this.symptomService = new Mock<ISymptomService>();
			this.symptomService.Setup(_service => _service.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_symptomKey => _symptomKey.Object); });
			this.symptomsSearch = new Mock<ISymptomsSearch>();
			this.userKey = new Mock<IUserKey>();
			this.userKey.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.userKeys = new List<Mock<IUserKey>>() { this.userKey };
			this.user = new Mock<IUser>();
			this.user.SetupAllProperties();
			this.user.Setup(_model => _model.Key).Returns(() => { return this.userKey.Object; });
			this.user.Setup(_model => _model.Name).Returns("ewdygSrGosgnQByUuxmowDylunN");
			this.user.Setup(_model => _model.Password).Returns("iqfFIEpxvzOpGOPhNCRjBcQsLmg");
			this.users = new List<Mock<IUser>> { this.user };
			this.userService = new Mock<IUserService>();
			this.userService.Setup(_service => _service.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_userKey => _userKey.Object); });
			this.usersSearch = new Mock<IUsersSearch>();
			
			this.key = new Mock<IFlareUpKey>();
			this.key.SetupAllProperties();
			this.key.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.keys = new List<Mock<IFlareUpKey>>() { this.key };
			this.search = new Mock<IFlareUpSearch>();
			this.searchMultiple = new Mock<IFlareUpsSearch>();
			
			this.flareUp = new Mock<IFlareUp>();
			this.flareUp.SetupAllProperties();
			this.flareUp.Setup(m => m.Key).Returns(this.key.Object);
			this.flareUp.Setup(m => m.Symptom).Returns(() => { return this.symptom.Object; });
			this.flareUp.Setup(m => m.User).Returns(() => { return this.user.Object; });
			this.flareUps = new List<Mock<IFlareUp>>() { this.flareUp };
			
			this.modelFactory = new Mock<IModelFactory>();
			this.modelFactory.Setup(_modelFactory => _modelFactory.GenerateSymptomsSearch()).Returns(() => { return this.symptomsSearch.Object; });
			this.modelFactory.Setup(_modelFactory => _modelFactory.GenerateUsersSearch()).Returns(() => { return this.usersSearch.Object; });
			
			this.state = ConnectionState.Closed;
			this.repository = new Mock<IFlareUpRepository>();
			this.repository.Setup(_repository => _repository.Open()).Callback(() => { this.state = ConnectionState.Open; });
			this.repository.Setup(_repository => _repository.Close()).Callback(() => { this.state = ConnectionState.Closed; });
			this.repository.Setup(_repository => _repository.IsOpen).Returns(() => { return this.state == ConnectionState.Open; });
			this.transaction = new Mock<IDbTransaction>();
		}
		
		[TestCleanup]
		public virtual void TestCleanup()
		{
			this.symptomKey = null;
			this.symptom = null;
			this.symptoms = null;
			this.userKey = null;
			this.user = null;
			this.users = null;
			
			this.repository = null;
		}
		
		#region Delete
		
		[TestMethod]
		public virtual void Delete_FlareUp_Test_Calls_Delete_On_Repository()
		{
			//Arrange
			IFlareUpKey injected = flareUp.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Delete(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())); }
			catch { this.repository.Verify(r => r.Delete(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())); }
		}
		
		[TestMethod]
		public virtual void Delete_FlareUp_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			IFlareUpKey injected = flareUp.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<IFlareUpKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Delete_FlareUp_Test_Does_Not_Call_Delete_On_SymptomService()
		{
			//Arrange
			IFlareUpKey injected = flareUp.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			this.symptomService.Verify(s => s.Delete(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.symptomService.Verify(s => s.Delete(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Delete_FlareUp_Test_Does_Not_Call_Delete_On_UserService()
		{
			//Arrange
			IFlareUpKey injected = flareUp.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			this.userService.Verify(s => s.Delete(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.userService.Verify(s => s.Delete(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_FlareUp_Test_Throws_Error_When_FlareUpKey_Is_Null()
		{
			//Arrange
			IFlareUpKey injected = null;
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public virtual void Delete_FlareUp_Test_Throws_Error_When_FlareUpKey_Not_Found_In_Repository()
		{
			//Arrange
			IFlareUpKey injected = flareUp.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(new List<IFlareUpKey>());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IFlareUpKey>());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Delete_Multiple_FlareUps_Test_Calls_Delete_On_Repository()
		{
			//Arrange
			IList<IFlareUpKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Delete(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>()), Times.Exactly(flareUps.Count)); }
			catch { this.repository.Verify(r => r.Delete(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>()), Times.Exactly(1)); }
		}
		
		[TestMethod]
		public virtual void Delete_Multiple_FlareUps_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			IList<IFlareUpKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<IFlareUpKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Delete_Multiple_FlareUps_Test_Does_Not_Call_Delete_On_SymptomService()
		{
			//Arrange
			IList<IFlareUpKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			this.symptomService.Verify(s => s.Delete(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.symptomService.Verify(s => s.Delete(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Delete_Multiple_FlareUps_Test_Does_Not_Call_Delete_On_UserService()
		{
			//Arrange
			IList<IFlareUpKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			this.userService.Verify(s => s.Delete(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.userService.Verify(s => s.Delete(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public virtual void Delete_Multiple_FlareUps_Test_Throws_Error_When_FlareUpKey_Is_Not_Found_In_Repository()
		{
			//Arrange
			IList<IFlareUpKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(new List<IFlareUpKey>());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IFlareUpKey>());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_Multiple_FlareUps_Test_Throws_Error_When_List_Of_FlareUpKey_Is_Null()
		{
			//Arrange
			IList<IFlareUpKey> injected = null;
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Delete
		
		#region Exists
		
		[TestMethod]
		public virtual void Exists_FlareUp_Test_Returns_False_When_Key_Was_Not_Found()
		{
			//Arrange
			bool actual;
			bool expected = false;
			IFlareUpKey injected = this.key.Object;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(new List<IFlareUpKey>());
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_FlareUp_Test_Returns_True_When_Key_Was_Found()
		{
			//Arrange
			bool actual;
			bool expected = true;
			IFlareUpKey injected = this.key.Object;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_FlareUps_Test_Returns_Empty_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			int actual;
			int expected = 0;
			IEnumerable<IFlareUpKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(new List<IFlareUpKey>());
			
			//Act
			actual = this.service.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_FlareUps_Test_Returns_List_Of_Keys_When_They_Were_Found()
		{
			//Arrange
			int actual;
			int expected = 1;
			IEnumerable<IFlareUpKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			actual = this.service.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_FlareUps_Test_Returns_Non_Null_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			IEnumerable<IFlareUpKey> actual;
			IEnumerable<IFlareUpKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IFlareUpKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(new List<IFlareUpKey>());
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		#endregion Exists
		
		#region Find
		
		[TestMethod]
		public virtual void Find_FlareUps_Test_Calls_Select_On_Repository()
		{
			//Arrange
			IFlareUpsSearch injected = this.searchMultiple.Object;
			
			this.repository.Setup(r => r.Select(It.IsAny<IFlareUpsSearch>())).Returns(new List<IFlareUp>());
			
			//Act
			this.service.Find(injected);
			
			//Assert
			this.repository.Verify(r => r.Select(It.IsAny<IFlareUpsSearch>()), Times.Exactly(1));
		}
		
		[TestMethod]
		public virtual void Find_FlareUps_Test_Calls_Validate_On_Search()
		{
			//Arrange
			Mock<IFlareUpsSearch> expected = this.searchMultiple;
			IFlareUpsSearch injected = this.searchMultiple.Object;
			
			this.repository.Setup(r => r.Select(It.IsAny<IFlareUpsSearch>())).Returns(new List<IFlareUp>());
			
			//Act
			this.service.Find(injected);
			
			//Assert
			expected.Verify(r => r.Validate());
		}
		
		[TestMethod]
		public virtual void Find_FlareUps_Test_Does_Not_Call_Find_On_ISymptom_Service()
		{
			//Arrange
			IFlareUpsSearch injected = this.searchMultiple.Object;
			
			//Act
			this.service.Find(injected);
			
			//Assert
			this.symptomService.Verify(r => r.Find(It.IsAny<ISymptomsSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Find_FlareUps_Test_Does_Not_Call_Find_On_IUser_Service()
		{
			//Arrange
			IFlareUpsSearch injected = this.searchMultiple.Object;
			
			//Act
			this.service.Find(injected);
			
			//Assert
			this.userService.Verify(r => r.Find(It.IsAny<IUsersSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Find_FlareUps_Test_Does_Not_Call_FindSingle_On_ISymptom_Service()
		{
			//Arrange
			IFlareUpsSearch injected = this.searchMultiple.Object;
			
			//Act
			this.service.Find(injected);
			
			//Assert
			this.symptomService.Verify(r => r.FindSingle(It.IsAny<ISymptomSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Find_FlareUps_Test_Does_Not_Call_FindSingle_On_IUser_Service()
		{
			//Arrange
			IFlareUpsSearch injected = this.searchMultiple.Object;
			
			//Act
			this.service.Find(injected);
			
			//Assert
			this.userService.Verify(r => r.FindSingle(It.IsAny<IUserSearch>()), Times.Never());
		}
		
		#endregion Find
		
		#region Find Single
		
		[TestMethod]
		public virtual void FindSingle_FlareUp_Test_Calls_Select_On_Repository()
		{
			//Arrange
			IFlareUpSearch injected = this.search.Object;
			
			
			this.repository.Setup(r => r.Select(It.IsAny<IFlareUpSearch>())).Returns(this.flareUp.Object);
			
			//Act
			this.service.FindSingle(injected);
			
			//Assert
			this.repository.Verify(r => r.Select(It.IsAny<IFlareUpSearch>()), Times.Exactly(1));
		}
		
		[TestMethod]
		public virtual void FindSingle_FlareUp_Test_Calls_Validate_On_Search()
		{
			//Arrange
			Mock<IFlareUpSearch> expected = this.search;
			IFlareUpSearch injected = this.search.Object;
			
			
			this.repository.Setup(r => r.Select(It.IsAny<IFlareUpSearch>())).Returns(this.flareUp.Object);
			
			//Act
			this.service.FindSingle(injected);
			
			//Assert
			expected.Verify(e => e.Validate());
		}
		
		[TestMethod]
		public virtual void FindSingle_FlareUp_Test_Returns_FlareUp_When_Is_Found_In_Repository()
		{
			//Arrange
			IFlareUp actual;
			IFlareUpSearch injected = this.search.Object;
			
			
			this.repository.Setup(r => r.Select(It.IsAny<IFlareUpSearch>())).Returns(this.flareUp.Object);
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void FindSingle_FlareUp_Test_Returns_Null_When_FlareUp_Is_Not_Found_In_Repository()
		{
			//Arrange
			IFlareUp actual;
			IFlareUpSearch injected = this.search.Object;
			
			this.repository.Setup(r => r.Select(It.IsAny<IFlareUpSearch>())).Returns((IFlareUp)null);
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNull(actual);
		}
		
		#endregion Find Single
		
		#region Save
		
		[TestMethod]
		public virtual void Save_FlareUp_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			Mock<IFlareUp> expected = this.flareUp;
			IFlareUp injected = this.flareUp.Object;
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(new List<IFlareUpKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IFlareUpKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<IFlareUpKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_FlareUp_Test_Calls_Exists_On_SymptomService()
		{
			//Arrange
			Mock<IFlareUp> expected = this.flareUp;
			IFlareUp injected = this.flareUp.Object;
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(new List<IFlareUpKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IFlareUpKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.symptomService.Verify(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())); }
			catch {
				try { this.symptomService.Verify(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.symptomService.Verify(s => s.Exists(It.IsAny<ISymptomKey>())); }
					catch { this.symptomService.Verify(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_FlareUp_Test_Calls_Exists_On_UserService()
		{
			//Arrange
			Mock<IFlareUp> expected = this.flareUp;
			IFlareUp injected = this.flareUp.Object;
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(new List<IFlareUpKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IFlareUpKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.userService.Verify(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())); }
			catch {
				try { this.userService.Verify(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.userService.Verify(s => s.Exists(It.IsAny<IUserKey>())); }
					catch { this.userService.Verify(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_FlareUp_Test_Calls_Insert_On_Repository_For_New_FlareUps()
		{
			//Arrange
			Mock<IFlareUp> expected = this.flareUp;
			IFlareUp injected = this.flareUp.Object;
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(new List<IFlareUpKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IFlareUpKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Insert(It.IsAny<IFlareUp>(), It.IsAny<IDbTransaction>())); }
			catch { this.repository.Verify(r => r.Insert(It.IsAny<IEnumerable<IFlareUp>>(), It.IsAny<IDbTransaction>())); }
		}
		
		[TestMethod]
		public virtual void Save_FlareUp_Test_Calls_Update_On_Repository_For_Existing_FlareUps()
		{
			//Arrange
			Mock<IFlareUp> expected = this.flareUp;
			IFlareUp injected = this.flareUp.Object;
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Update(It.IsAny<IFlareUp>(), It.IsAny<IDbTransaction>())); }
			catch { this.repository.Verify(r => r.Update(It.IsAny<IEnumerable<IFlareUp>>(), It.IsAny<IDbTransaction>())); }
		}
		
		[TestMethod]
		public virtual void Save_FlareUp_Test_Does_Not_Call_Delete_On_SymptomService()
		{
			//Arrange
			Mock<IFlareUp> expected = this.flareUp;
			IFlareUp injected = this.flareUp.Object;
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.symptomService.Verify(s => s.Delete(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.symptomService.Verify(s => s.Delete(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_FlareUp_Test_Does_Not_Call_Delete_On_UserService()
		{
			//Arrange
			Mock<IFlareUp> expected = this.flareUp;
			IFlareUp injected = this.flareUp.Object;
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.userService.Verify(s => s.Delete(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.userService.Verify(s => s.Delete(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_FlareUp_Test_Does_Not_Call_Find_On_SymptomService()
		{
			//Arrange
			Mock<IFlareUp> expected = this.flareUp;
			IFlareUp injected = expected.Object;
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.symptomService.Verify(s => s.Find(It.IsAny<ISymptomsSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_FlareUp_Test_Does_Not_Call_Find_On_UserService()
		{
			//Arrange
			Mock<IFlareUp> expected = this.flareUp;
			IFlareUp injected = expected.Object;
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.userService.Verify(s => s.Find(It.IsAny<IUsersSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_FlareUp_Test_Does_Not_Call_FindSingle_On_SymptomService()
		{
			//Arrange
			Mock<IFlareUp> expected = this.flareUp;
			IFlareUp injected = expected.Object;
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.symptomService.Verify(s => s.FindSingle(It.IsAny<ISymptomSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_FlareUp_Test_Does_Not_Call_FindSingle_On_UserService()
		{
			//Arrange
			Mock<IFlareUp> expected = this.flareUp;
			IFlareUp injected = expected.Object;
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.userService.Verify(s => s.FindSingle(It.IsAny<IUserSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_FlareUp_Test_Does_Not_Call_Save_On_SymptomService()
		{
			//Arrange
			Mock<IFlareUp> expected = this.flareUp;
			IFlareUp injected = this.flareUp.Object;
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.symptomService.Verify(s => s.Save(It.IsAny<IEnumerable<ISymptom>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_FlareUp_Test_Does_Not_Call_Save_On_UserService()
		{
			//Arrange
			Mock<IFlareUp> expected = this.flareUp;
			IFlareUp injected = this.flareUp.Object;
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.userService.Verify(s => s.Save(It.IsAny<IEnumerable<IUser>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Save_FlareUp_Test_Throws_Error_When_FlareUp_Is_Null()
		{
			//Arrange
			IFlareUp injected = null;
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Save_Multiple_FlareUps_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			List<IFlareUp> injected = this.flareUps.Select(e => e.Object).ToList();
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(new List<IFlareUpKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IFlareUpKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<IFlareUpKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_Multiple_FlareUps_Test_Calls_Exists_On_SymptomService()
		{
			//Arrange
			List<IFlareUp> injected = this.flareUps.Select(e => e.Object).ToList();
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.symptomService.Verify(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())); }
			catch {
				try { this.symptomService.Verify(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.symptomService.Verify(s => s.Exists(It.IsAny<ISymptomKey>())); }
					catch { this.symptomService.Verify(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_Multiple_FlareUps_Test_Calls_Exists_On_UserService()
		{
			//Arrange
			List<IFlareUp> injected = this.flareUps.Select(e => e.Object).ToList();
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.userService.Verify(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())); }
			catch {
				try { this.userService.Verify(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.userService.Verify(s => s.Exists(It.IsAny<IUserKey>())); }
					catch { this.userService.Verify(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_Multiple_FlareUps_Test_Calls_Insert_On_Repository_For_New_FlareUps()
		{
			//Arrange
			List<IFlareUp> injected = this.flareUps.Select(e => e.Object).ToList();
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(new List<IFlareUpKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IFlareUpKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.repository.Verify(r => r.Insert(It.IsAny<IEnumerable<IFlareUp>>(), It.IsAny<IDbTransaction>()));
		}
		
		[TestMethod]
		public virtual void Save_Multiple_FlareUps_Test_Calls_Update_On_Repository_For_Existing_FlareUps()
		{
			//Arrange
			List<IFlareUp> injected = this.flareUps.Select(e => e.Object).ToList();
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.repository.Verify(r => r.Update(It.IsAny<IEnumerable<IFlareUp>>(), It.IsAny<IDbTransaction>()));
		}
		
		[TestMethod]
		public virtual void Save_Multiple_FlareUps_Test_Does_Not_Call_Delete_On_SymptomService()
		{
			//Arrange
			IList<Mock<IFlareUp>> expected = new List<Mock<IFlareUp>>() { this.flareUp };
			List<IFlareUp> injected = this.flareUps.Select(e => e.Object).ToList();
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.symptomService.Verify(s => s.Delete(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.symptomService.Verify(s => s.Delete(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Multiple_FlareUps_Test_Does_Not_Call_Delete_On_UserService()
		{
			//Arrange
			IList<Mock<IFlareUp>> expected = new List<Mock<IFlareUp>>() { this.flareUp };
			List<IFlareUp> injected = this.flareUps.Select(e => e.Object).ToList();
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.userService.Verify(s => s.Delete(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.userService.Verify(s => s.Delete(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Multiple_FlareUps_Test_Does_Not_Call_Find_On_SymptomService()
		{
			//Arrange
			IList<Mock<IFlareUp>> expected = new List<Mock<IFlareUp>>() { this.flareUp };
			List<IFlareUp> injected = this.flareUps.Select(e => e.Object).ToList();
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.symptomService.Verify(s => s.Find(It.IsAny<ISymptomsSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Multiple_FlareUps_Test_Does_Not_Call_Find_On_UserService()
		{
			//Arrange
			IList<Mock<IFlareUp>> expected = new List<Mock<IFlareUp>>() { this.flareUp };
			List<IFlareUp> injected = this.flareUps.Select(e => e.Object).ToList();
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.userService.Verify(s => s.Find(It.IsAny<IUsersSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Multiple_FlareUps_Test_Does_Not_Call_FindSingle_On_SymptomService()
		{
			//Arrange
			IList<Mock<IFlareUp>> expected = new List<Mock<IFlareUp>>() { this.flareUp };
			List<IFlareUp> injected = this.flareUps.Select(e => e.Object).ToList();
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.symptomService.Verify(s => s.FindSingle(It.IsAny<ISymptomSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Multiple_FlareUps_Test_Does_Not_Call_FindSingle_On_UserService()
		{
			//Arrange
			IList<Mock<IFlareUp>> expected = new List<Mock<IFlareUp>>() { this.flareUp };
			List<IFlareUp> injected = this.flareUps.Select(e => e.Object).ToList();
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.userService.Verify(s => s.FindSingle(It.IsAny<IUserSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Multiple_FlareUps_Test_Does_Not_Call_Save_On_SymptomService()
		{
			//Arrange
			IList<Mock<IFlareUp>> expected = new List<Mock<IFlareUp>>() { this.flareUp };
			List<IFlareUp> injected = this.flareUps.Select(e => e.Object).ToList();
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.symptomService.Verify(s => s.Save(It.IsAny<ISymptom>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.symptomService.Verify(s => s.Save(It.IsAny<IEnumerable<ISymptom>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Multiple_FlareUps_Test_Does_Not_Call_Save_On_UserService()
		{
			//Arrange
			IList<Mock<IFlareUp>> expected = new List<Mock<IFlareUp>>() { this.flareUp };
			List<IFlareUp> injected = this.flareUps.Select(e => e.Object).ToList();
			
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.symptomService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IFlareUpKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IFlareUpKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.userService.Verify(s => s.Save(It.IsAny<IUser>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.userService.Verify(s => s.Save(It.IsAny<IEnumerable<IUser>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Save_Multiple_FlareUps_Test_Throws_Error_When_Any_FlareUp_Is_Null()
		{
			//Arrange
			List<IFlareUp> injected = new List<IFlareUp>() { new Mock<IFlareUp>().Object, null, new Mock<IFlareUp>().Object };
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Save
	}
}
