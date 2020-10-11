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
	public abstract class IParticipationServiceTest
	{
		protected IList<Mock<IActivity>> activities;
		protected Mock<IActivitiesSearch> activitiesSearch;
		protected Mock<IActivity> activity;
		protected Mock<IActivityKey> activityKey;
		protected IList<Mock<IActivityKey>> activityKeys;
		protected Mock<IActivityService> activityService;
		protected Mock<IParticipationKey> key;
		protected IList<Mock<IParticipationKey>> keys;
		protected Mock<IModelFactory> modelFactory;
		protected Mock<IParticipation> participation;
		protected IList<Mock<IParticipation>> participations;
		protected Mock<IParticipationRepository> repository;
		protected Mock<IParticipationSearch> search;
		protected Mock<IParticipationsSearch> searchMultiple;
		protected IParticipationService service;
		protected ConnectionState state;
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
			this.activityKey = new Mock<IActivityKey>();
			this.activityKey.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.activityKeys = new List<Mock<IActivityKey>>() { this.activityKey };
			this.activity = new Mock<IActivity>();
			this.activity.SetupAllProperties();
			this.activity.Setup(_model => _model.Key).Returns(() => { return this.activityKey.Object; });
			this.activity.Setup(_activity => _activity.Category).Returns((IActivityCategory)null);
			this.activity.Setup(_model => _model.Name).Returns("YReSePyXmAEmmbyVIWHIOqyRvE");
			this.activities = new List<Mock<IActivity>> { this.activity };
			this.activityService = new Mock<IActivityService>();
			this.activityService.Setup(_service => _service.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_activityKey => _activityKey.Object); });
			this.activitiesSearch = new Mock<IActivitiesSearch>();
			this.userKey = new Mock<IUserKey>();
			this.userKey.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.userKeys = new List<Mock<IUserKey>>() { this.userKey };
			this.user = new Mock<IUser>();
			this.user.SetupAllProperties();
			this.user.Setup(_model => _model.Key).Returns(() => { return this.userKey.Object; });
			this.user.Setup(_model => _model.Name).Returns("f");
			this.user.Setup(_model => _model.Password).Returns("NUwxOzEuWkRqRUnXFXAEUJOEy");
			this.users = new List<Mock<IUser>> { this.user };
			this.userService = new Mock<IUserService>();
			this.userService.Setup(_service => _service.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_userKey => _userKey.Object); });
			this.usersSearch = new Mock<IUsersSearch>();
			
			this.key = new Mock<IParticipationKey>();
			this.key.SetupAllProperties();
			this.key.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.keys = new List<Mock<IParticipationKey>>() { this.key };
			this.search = new Mock<IParticipationSearch>();
			this.searchMultiple = new Mock<IParticipationsSearch>();
			
			this.participation = new Mock<IParticipation>();
			this.participation.SetupAllProperties();
			this.participation.Setup(m => m.Key).Returns(this.key.Object);
			this.participation.Setup(m => m.Activity).Returns(() => { return this.activity.Object; });
			this.participation.Setup(m => m.User).Returns(() => { return this.user.Object; });
			this.participations = new List<Mock<IParticipation>>() { this.participation };
			
			this.modelFactory = new Mock<IModelFactory>();
			this.modelFactory.Setup(_modelFactory => _modelFactory.GenerateActivitiesSearch()).Returns(() => { return this.activitiesSearch.Object; });
			this.modelFactory.Setup(_modelFactory => _modelFactory.GenerateUsersSearch()).Returns(() => { return this.usersSearch.Object; });
			
			this.state = ConnectionState.Closed;
			this.repository = new Mock<IParticipationRepository>();
			this.repository.Setup(_repository => _repository.Open()).Callback(() => { this.state = ConnectionState.Open; });
			this.repository.Setup(_repository => _repository.Close()).Callback(() => { this.state = ConnectionState.Closed; });
			this.repository.Setup(_repository => _repository.IsOpen).Returns(() => { return this.state == ConnectionState.Open; });
			this.transaction = new Mock<IDbTransaction>();
		}
		
		[TestCleanup]
		public virtual void TestCleanup()
		{
			this.activityKey = null;
			this.activity = null;
			this.activities = null;
			this.userKey = null;
			this.user = null;
			this.users = null;
			
			this.repository = null;
		}
		
		#region Delete
		
		[TestMethod]
		public virtual void Delete_Multiple_Participations_Test_Calls_Delete_On_Repository()
		{
			//Arrange
			IList<IParticipationKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Delete(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>()), Times.Exactly(participations.Count)); }
			catch { this.repository.Verify(r => r.Delete(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>()), Times.Exactly(1)); }
		}
		
		[TestMethod]
		public virtual void Delete_Multiple_Participations_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			IList<IParticipationKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<IParticipationKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Delete_Multiple_Participations_Test_Does_Not_Call_Delete_On_ActivityService()
		{
			//Arrange
			IList<IParticipationKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			this.activityService.Verify(s => s.Delete(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.activityService.Verify(s => s.Delete(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Delete_Multiple_Participations_Test_Does_Not_Call_Delete_On_UserService()
		{
			//Arrange
			IList<IParticipationKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			this.userService.Verify(s => s.Delete(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.userService.Verify(s => s.Delete(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_Multiple_Participations_Test_Throws_Error_When_List_Of_ParticipationKey_Is_Null()
		{
			//Arrange
			IList<IParticipationKey> injected = null;
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public virtual void Delete_Multiple_Participations_Test_Throws_Error_When_ParticipationKey_Is_Not_Found_In_Repository()
		{
			//Arrange
			IList<IParticipationKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(new List<IParticipationKey>());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IParticipationKey>());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Delete_Participation_Test_Calls_Delete_On_Repository()
		{
			//Arrange
			IParticipationKey injected = participation.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Delete(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())); }
			catch { this.repository.Verify(r => r.Delete(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())); }
		}
		
		[TestMethod]
		public virtual void Delete_Participation_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			IParticipationKey injected = participation.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<IParticipationKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Delete_Participation_Test_Does_Not_Call_Delete_On_ActivityService()
		{
			//Arrange
			IParticipationKey injected = participation.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			this.activityService.Verify(s => s.Delete(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.activityService.Verify(s => s.Delete(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Delete_Participation_Test_Does_Not_Call_Delete_On_UserService()
		{
			//Arrange
			IParticipationKey injected = participation.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			this.userService.Verify(s => s.Delete(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.userService.Verify(s => s.Delete(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_Participation_Test_Throws_Error_When_ParticipationKey_Is_Null()
		{
			//Arrange
			IParticipationKey injected = null;
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public virtual void Delete_Participation_Test_Throws_Error_When_ParticipationKey_Not_Found_In_Repository()
		{
			//Arrange
			IParticipationKey injected = participation.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(new List<IParticipationKey>());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IParticipationKey>());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Delete
		
		#region Exists
		
		[TestMethod]
		public virtual void Exists_Participation_Test_Returns_False_When_Key_Was_Not_Found()
		{
			//Arrange
			bool actual;
			bool expected = false;
			IParticipationKey injected = this.key.Object;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(new List<IParticipationKey>());
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Participation_Test_Returns_True_When_Key_Was_Found()
		{
			//Arrange
			bool actual;
			bool expected = true;
			IParticipationKey injected = this.key.Object;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Participations_Test_Returns_Empty_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			int actual;
			int expected = 0;
			IEnumerable<IParticipationKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(new List<IParticipationKey>());
			
			//Act
			actual = this.service.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Participations_Test_Returns_List_Of_Keys_When_They_Were_Found()
		{
			//Arrange
			int actual;
			int expected = 1;
			IEnumerable<IParticipationKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			actual = this.service.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Participations_Test_Returns_Non_Null_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			IEnumerable<IParticipationKey> actual;
			IEnumerable<IParticipationKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IParticipationKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(new List<IParticipationKey>());
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		#endregion Exists
		
		#region Find
		
		[TestMethod]
		public virtual void Find_Participations_Test_Calls_Select_On_Repository()
		{
			//Arrange
			IParticipationsSearch injected = this.searchMultiple.Object;
			
			this.repository.Setup(r => r.Select(It.IsAny<IParticipationsSearch>())).Returns(new List<IParticipation>());
			
			//Act
			this.service.Find(injected);
			
			//Assert
			this.repository.Verify(r => r.Select(It.IsAny<IParticipationsSearch>()), Times.Exactly(1));
		}
		
		[TestMethod]
		public virtual void Find_Participations_Test_Calls_Validate_On_Search()
		{
			//Arrange
			Mock<IParticipationsSearch> expected = this.searchMultiple;
			IParticipationsSearch injected = this.searchMultiple.Object;
			
			this.repository.Setup(r => r.Select(It.IsAny<IParticipationsSearch>())).Returns(new List<IParticipation>());
			
			//Act
			this.service.Find(injected);
			
			//Assert
			expected.Verify(r => r.Validate());
		}
		
		[TestMethod]
		public virtual void Find_Participations_Test_Does_Not_Call_Find_On_IActivity_Service()
		{
			//Arrange
			IParticipationsSearch injected = this.searchMultiple.Object;
			
			//Act
			this.service.Find(injected);
			
			//Assert
			this.activityService.Verify(r => r.Find(It.IsAny<IActivitiesSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Find_Participations_Test_Does_Not_Call_Find_On_IUser_Service()
		{
			//Arrange
			IParticipationsSearch injected = this.searchMultiple.Object;
			
			//Act
			this.service.Find(injected);
			
			//Assert
			this.userService.Verify(r => r.Find(It.IsAny<IUsersSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Find_Participations_Test_Does_Not_Call_FindSingle_On_IActivity_Service()
		{
			//Arrange
			IParticipationsSearch injected = this.searchMultiple.Object;
			
			//Act
			this.service.Find(injected);
			
			//Assert
			this.activityService.Verify(r => r.FindSingle(It.IsAny<IActivitySearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Find_Participations_Test_Does_Not_Call_FindSingle_On_IUser_Service()
		{
			//Arrange
			IParticipationsSearch injected = this.searchMultiple.Object;
			
			//Act
			this.service.Find(injected);
			
			//Assert
			this.userService.Verify(r => r.FindSingle(It.IsAny<IUserSearch>()), Times.Never());
		}
		
		#endregion Find
		
		#region Find Single
		
		[TestMethod]
		public virtual void FindSingle_Participation_Test_Calls_Select_On_Repository()
		{
			//Arrange
			IParticipationSearch injected = this.search.Object;
			
			
			this.repository.Setup(r => r.Select(It.IsAny<IParticipationSearch>())).Returns(this.participation.Object);
			
			//Act
			this.service.FindSingle(injected);
			
			//Assert
			this.repository.Verify(r => r.Select(It.IsAny<IParticipationSearch>()), Times.Exactly(1));
		}
		
		[TestMethod]
		public virtual void FindSingle_Participation_Test_Calls_Validate_On_Search()
		{
			//Arrange
			Mock<IParticipationSearch> expected = this.search;
			IParticipationSearch injected = this.search.Object;
			
			
			this.repository.Setup(r => r.Select(It.IsAny<IParticipationSearch>())).Returns(this.participation.Object);
			
			//Act
			this.service.FindSingle(injected);
			
			//Assert
			expected.Verify(e => e.Validate());
		}
		
		[TestMethod]
		public virtual void FindSingle_Participation_Test_Returns_Null_When_Participation_Is_Not_Found_In_Repository()
		{
			//Arrange
			IParticipation actual;
			IParticipationSearch injected = this.search.Object;
			
			this.repository.Setup(r => r.Select(It.IsAny<IParticipationSearch>())).Returns((IParticipation)null);
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNull(actual);
		}
		
		[TestMethod]
		public virtual void FindSingle_Participation_Test_Returns_Participation_When_Is_Found_In_Repository()
		{
			//Arrange
			IParticipation actual;
			IParticipationSearch injected = this.search.Object;
			
			
			this.repository.Setup(r => r.Select(It.IsAny<IParticipationSearch>())).Returns(this.participation.Object);
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		#endregion Find Single
		
		#region Save
		
		[TestMethod]
		public virtual void Save_Multiple_Participations_Test_Calls_Exists_On_ActivityService()
		{
			//Arrange
			List<IParticipation> injected = this.participations.Select(e => e.Object).ToList();
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.activityService.Verify(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())); }
			catch {
				try { this.activityService.Verify(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.activityService.Verify(s => s.Exists(It.IsAny<IActivityKey>())); }
					catch { this.activityService.Verify(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Participations_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			List<IParticipation> injected = this.participations.Select(e => e.Object).ToList();
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(new List<IParticipationKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IParticipationKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<IParticipationKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Participations_Test_Calls_Exists_On_UserService()
		{
			//Arrange
			List<IParticipation> injected = this.participations.Select(e => e.Object).ToList();
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
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
		public virtual void Save_Multiple_Participations_Test_Calls_Insert_On_Repository_For_New_Participations()
		{
			//Arrange
			List<IParticipation> injected = this.participations.Select(e => e.Object).ToList();
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(new List<IParticipationKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IParticipationKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.repository.Verify(r => r.Insert(It.IsAny<IEnumerable<IParticipation>>(), It.IsAny<IDbTransaction>()));
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Participations_Test_Calls_Update_On_Repository_For_Existing_Participations()
		{
			//Arrange
			List<IParticipation> injected = this.participations.Select(e => e.Object).ToList();
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.repository.Verify(r => r.Update(It.IsAny<IEnumerable<IParticipation>>(), It.IsAny<IDbTransaction>()));
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Participations_Test_Does_Not_Call_Delete_On_ActivityService()
		{
			//Arrange
			IList<Mock<IParticipation>> expected = new List<Mock<IParticipation>>() { this.participation };
			List<IParticipation> injected = this.participations.Select(e => e.Object).ToList();
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.activityService.Verify(s => s.Delete(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.activityService.Verify(s => s.Delete(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Participations_Test_Does_Not_Call_Delete_On_UserService()
		{
			//Arrange
			IList<Mock<IParticipation>> expected = new List<Mock<IParticipation>>() { this.participation };
			List<IParticipation> injected = this.participations.Select(e => e.Object).ToList();
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.userService.Verify(s => s.Delete(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.userService.Verify(s => s.Delete(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Participations_Test_Does_Not_Call_Find_On_ActivityService()
		{
			//Arrange
			IList<Mock<IParticipation>> expected = new List<Mock<IParticipation>>() { this.participation };
			List<IParticipation> injected = this.participations.Select(e => e.Object).ToList();
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.activityService.Verify(s => s.Find(It.IsAny<IActivitiesSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Participations_Test_Does_Not_Call_Find_On_UserService()
		{
			//Arrange
			IList<Mock<IParticipation>> expected = new List<Mock<IParticipation>>() { this.participation };
			List<IParticipation> injected = this.participations.Select(e => e.Object).ToList();
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.userService.Verify(s => s.Find(It.IsAny<IUsersSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Participations_Test_Does_Not_Call_FindSingle_On_ActivityService()
		{
			//Arrange
			IList<Mock<IParticipation>> expected = new List<Mock<IParticipation>>() { this.participation };
			List<IParticipation> injected = this.participations.Select(e => e.Object).ToList();
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.activityService.Verify(s => s.FindSingle(It.IsAny<IActivitySearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Participations_Test_Does_Not_Call_FindSingle_On_UserService()
		{
			//Arrange
			IList<Mock<IParticipation>> expected = new List<Mock<IParticipation>>() { this.participation };
			List<IParticipation> injected = this.participations.Select(e => e.Object).ToList();
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.userService.Verify(s => s.FindSingle(It.IsAny<IUserSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Participations_Test_Does_Not_Call_Save_On_ActivityService()
		{
			//Arrange
			IList<Mock<IParticipation>> expected = new List<Mock<IParticipation>>() { this.participation };
			List<IParticipation> injected = this.participations.Select(e => e.Object).ToList();
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.activityService.Verify(s => s.Save(It.IsAny<IActivity>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.activityService.Verify(s => s.Save(It.IsAny<IEnumerable<IActivity>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Participations_Test_Does_Not_Call_Save_On_UserService()
		{
			//Arrange
			IList<Mock<IParticipation>> expected = new List<Mock<IParticipation>>() { this.participation };
			List<IParticipation> injected = this.participations.Select(e => e.Object).ToList();
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.userService.Verify(s => s.Save(It.IsAny<IUser>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.userService.Verify(s => s.Save(It.IsAny<IEnumerable<IUser>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Save_Multiple_Participations_Test_Throws_Error_When_Any_Participation_Is_Null()
		{
			//Arrange
			List<IParticipation> injected = new List<IParticipation>() { new Mock<IParticipation>().Object, null, new Mock<IParticipation>().Object };
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Save_Participation_Test_Calls_Exists_On_ActivityService()
		{
			//Arrange
			Mock<IParticipation> expected = this.participation;
			IParticipation injected = this.participation.Object;
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(new List<IParticipationKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IParticipationKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.activityService.Verify(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())); }
			catch {
				try { this.activityService.Verify(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.activityService.Verify(s => s.Exists(It.IsAny<IActivityKey>())); }
					catch { this.activityService.Verify(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_Participation_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			Mock<IParticipation> expected = this.participation;
			IParticipation injected = this.participation.Object;
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(new List<IParticipationKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IParticipationKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<IParticipationKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_Participation_Test_Calls_Exists_On_UserService()
		{
			//Arrange
			Mock<IParticipation> expected = this.participation;
			IParticipation injected = this.participation.Object;
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(new List<IParticipationKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IParticipationKey>());
			
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
		public virtual void Save_Participation_Test_Calls_Insert_On_Repository_For_New_Participations()
		{
			//Arrange
			Mock<IParticipation> expected = this.participation;
			IParticipation injected = this.participation.Object;
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(new List<IParticipationKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IParticipationKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Insert(It.IsAny<IParticipation>(), It.IsAny<IDbTransaction>())); }
			catch { this.repository.Verify(r => r.Insert(It.IsAny<IEnumerable<IParticipation>>(), It.IsAny<IDbTransaction>())); }
		}
		
		[TestMethod]
		public virtual void Save_Participation_Test_Calls_Update_On_Repository_For_Existing_Participations()
		{
			//Arrange
			Mock<IParticipation> expected = this.participation;
			IParticipation injected = this.participation.Object;
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Update(It.IsAny<IParticipation>(), It.IsAny<IDbTransaction>())); }
			catch { this.repository.Verify(r => r.Update(It.IsAny<IEnumerable<IParticipation>>(), It.IsAny<IDbTransaction>())); }
		}
		
		[TestMethod]
		public virtual void Save_Participation_Test_Does_Not_Call_Delete_On_ActivityService()
		{
			//Arrange
			Mock<IParticipation> expected = this.participation;
			IParticipation injected = this.participation.Object;
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.activityService.Verify(s => s.Delete(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.activityService.Verify(s => s.Delete(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Participation_Test_Does_Not_Call_Delete_On_UserService()
		{
			//Arrange
			Mock<IParticipation> expected = this.participation;
			IParticipation injected = this.participation.Object;
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.userService.Verify(s => s.Delete(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.userService.Verify(s => s.Delete(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Participation_Test_Does_Not_Call_Find_On_ActivityService()
		{
			//Arrange
			Mock<IParticipation> expected = this.participation;
			IParticipation injected = expected.Object;
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.activityService.Verify(s => s.Find(It.IsAny<IActivitiesSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Participation_Test_Does_Not_Call_Find_On_UserService()
		{
			//Arrange
			Mock<IParticipation> expected = this.participation;
			IParticipation injected = expected.Object;
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.userService.Verify(s => s.Find(It.IsAny<IUsersSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Participation_Test_Does_Not_Call_FindSingle_On_ActivityService()
		{
			//Arrange
			Mock<IParticipation> expected = this.participation;
			IParticipation injected = expected.Object;
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.activityService.Verify(s => s.FindSingle(It.IsAny<IActivitySearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Participation_Test_Does_Not_Call_FindSingle_On_UserService()
		{
			//Arrange
			Mock<IParticipation> expected = this.participation;
			IParticipation injected = expected.Object;
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.userService.Verify(s => s.FindSingle(It.IsAny<IUserSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Participation_Test_Does_Not_Call_Save_On_ActivityService()
		{
			//Arrange
			Mock<IParticipation> expected = this.participation;
			IParticipation injected = this.participation.Object;
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.activityService.Verify(s => s.Save(It.IsAny<IEnumerable<IActivity>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Participation_Test_Does_Not_Call_Save_On_UserService()
		{
			//Arrange
			Mock<IParticipation> expected = this.participation;
			IParticipation injected = this.participation.Object;
			
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.activityService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IUserKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			this.userService.Setup(s => s.Exists(It.IsAny<IEnumerable<IUserKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.userKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IParticipationKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IParticipationKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.userService.Verify(s => s.Save(It.IsAny<IEnumerable<IUser>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Save_Participation_Test_Throws_Error_When_Participation_Is_Null()
		{
			//Arrange
			IParticipation injected = null;
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Save
	}
}
