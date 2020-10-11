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
	public abstract class IParticipationRepositoryTest
	{
		protected IList<Mock<IActivity>> activities;
		protected Mock<IActivityCategory> activityCategory;
		protected Mock<IActivityCategoryKey> activityCategoryKey;
		protected Mock<IActivity> activity;
		protected Mock<IActivityKey> activityKey;
		protected Mock<IParticipationKey> key;
		protected IList<Mock<IParticipationKey>> keys;
		protected Mock<IModelFactory> modelFactory;
		protected Mock<IParticipation> participation;
		protected IList<Mock<IParticipation>> participations;
		protected IParticipationRepository repository;
		protected Mock<IParticipationSearch> search;
		protected Mock<IParticipationsSearch> searchMultiple;
		protected TestContext testContext;
		protected Mock<IDbTransaction> transaction;
		protected Mock<IUser> user;
		protected Mock<IUserKey> userKey;
		protected IList<Mock<IUser>> users;
		
		public TestContext TestContext { get { return this.testContext; } set { this.testContext = value; } }
		
		[TestInitialize]
		public virtual void TestInitialize()
		{
			this.activityCategoryKey = new Mock<IActivityCategoryKey>();
			this.activityCategoryKey.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.activityCategory = new Mock<IActivityCategory>();
			this.activityCategory.SetupAllProperties();
			this.activityCategory.Setup(_activityCategory => _activityCategory.Key).Returns(() => { return this.activityCategoryKey.Object; });
			this.activityCategory.Setup(_activityCategory => _activityCategory.Name).Returns("sPHVFFPRQmspouboOSKIwplqCUCIFeHUGuWaswzwPzCGr");
			this.activityKey = new Mock<IActivityKey>();
			this.activityKey.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.activity = new Mock<IActivity>();
			this.activity.SetupAllProperties();
			this.activity.Setup(_model => _model.Key).Returns(() => { return this.activityKey.Object; });
			this.activity.Setup(_activity => _activity.Category).Returns(() => this.activityCategory.Object);
			this.activity.Setup(_activity => _activity.Name).Returns("tomPvHYEETpEUkJUEgnjEUklrzotCyLoiHoY");
			this.activities = new List<Mock<IActivity>> { this.activity };
			this.userKey = new Mock<IUserKey>();
			this.userKey.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.user = new Mock<IUser>();
			this.user.SetupAllProperties();
			this.user.Setup(_model => _model.Key).Returns(() => { return this.userKey.Object; });
			this.user.Setup(_user => _user.Name).Returns("lxIWioSXUIBSvukBwhzdaxVsFbfiav");
			this.user.Setup(_user => _user.Password).Returns("WKpsJEmvGsnwQ");
			this.users = new List<Mock<IUser>> { this.user };
			
			this.key = new Mock<IParticipationKey>();
			this.key.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.keys = new List<Mock<IParticipationKey>>() { this.key };
			
			this.participation = new Mock<IParticipation>();
			this.participation.SetupAllProperties();
			this.participation.Setup(_model => _model.Key).Returns(() => { return this.key.Object; });
			this.participation.Setup(_model => _model.Activity).Returns(() => { return this.activity.Object; });
			this.participation.Setup(_model => _model.PerformedOn).Returns(new DateTime(2020, 5, 22, 5, 48, 51));
			this.participation.Setup(_model => _model.Severity).Returns(0.743831012278717d);
			this.participation.Setup(_model => _model.User).Returns(() => { return this.user.Object; });
			this.participations = new List<Mock<IParticipation>>() { participation };
			
			this.modelFactory = new Mock<IModelFactory>();
			this.modelFactory.Setup(mf => mf.GenerateActivityCategoryKey(It.IsAny<Guid>())).Returns(this.activityCategoryKey.Object);
			this.modelFactory.Setup(mf => mf.GenerateActivityCategory(It.IsAny<IActivityCategoryKey>())).Returns(this.activityCategory.Object);
			this.modelFactory.Setup(mf => mf.GenerateActivityKey(It.IsAny<Guid>())).Returns(this.activityKey.Object);
			this.modelFactory.Setup(mf => mf.GenerateActivity(It.IsAny<IActivityKey>())).Returns(this.activity.Object);
			this.modelFactory.Setup(mf => mf.GenerateUserKey(It.IsAny<Guid>())).Returns(this.userKey.Object);
			this.modelFactory.Setup(mf => mf.GenerateUser(It.IsAny<IUserKey>())).Returns(this.user.Object);
			this.modelFactory.Setup(mf => mf.GenerateParticipationKey(It.IsAny<Guid>())).Returns(this.key.Object);
			this.modelFactory.Setup(mf => mf.GenerateParticipation(It.IsAny<IParticipationKey>())).Returns(this.participation.Object);
			
			this.search = new Mock<IParticipationSearch>();
			this.searchMultiple = new Mock<IParticipationsSearch>();
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
			this.transaction = null;
		}
		
		public virtual void AddRow(IParticipation participation)
		{
			throw new NotImplementedException();
		}
		
		public virtual void AddRows(IEnumerable<IParticipation> participations)
		{
			foreach(IParticipation participation in participations)
				AddRow(participation);
		}
		
		#region Delete
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_Multiple_Participations_Test_Throws_Error_When_Any_Key_Is_Null()
		{
			//Arrange
			IList<IParticipationKey> injected = new List<IParticipationKey>() { new Mock<IParticipationKey>().Object, null };
			
			//Act
			this.repository.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_Participation_Test_Throws_Error_When_Key_Is_Null()
		{
			//Arrange
			IParticipationKey injected = null;
			
			//Act
			this.repository.Delete(injected, this.transaction.Object);
			
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
			
			//Act
			actual = this.repository.Exists(injected);
			
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
			
			AddRow(this.participation.Object);
			
			//Act
			actual = this.repository.Exists(injected);
			
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
			
			//Act
			actual = this.repository.Exists(injected).Count();
			
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
			
			AddRow(this.participation.Object);
			
			//Act
			actual = this.repository.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Participations_Test_Returns_Non_Null_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			IEnumerable<IParticipationKey> actual;
			IEnumerable<IParticipationKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			//Act
			actual = this.repository.Exists(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		#endregion Exists
		
		#region Insert
		
		[TestMethod]
		public virtual void Insert_Multiple_Participations_Test_Gets_Keys_For_Participation_Activity()
		{
			//Arrange
			IList<Mock<IActivity>> expected = this.activities;
			IList<IParticipation> injected = this.participations.Select(e => e.Object).ToList();
			
			//Act
			this.repository.Insert(injected, this.transaction.Object );
			
			//Assert
			foreach(Mock<IActivity> activity in expected)
				activity.VerifyGet(_activity => _activity.Key);
		}
		
		[TestMethod]
		public virtual void Insert_Multiple_Participations_Test_Gets_Keys_For_Participation_User()
		{
			//Arrange
			IList<Mock<IUser>> expected = this.users;
			IList<IParticipation> injected = this.participations.Select(e => e.Object).ToList();
			
			//Act
			this.repository.Insert(injected, this.transaction.Object );
			
			//Assert
			foreach(Mock<IUser> user in expected)
				user.VerifyGet(_user => _user.Key);
		}
		
		[TestMethod]
		public virtual void Insert_Multiple_Participations_Test_Gets_PerformedOn_For_Every_Participation()
		{
			//Arrange
			IList<Mock<IParticipation>> expected = new List<Mock<IParticipation>>() { this.participation };
			IList<IParticipation> injected = this.participations.Select(e => e.Object).ToList();
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<IParticipation> participation in expected)
				participation.VerifyGet(_participation => _participation.PerformedOn, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Insert_Multiple_Participations_Test_Gets_Severity_For_Every_Participation()
		{
			//Arrange
			IList<Mock<IParticipation>> expected = new List<Mock<IParticipation>>() { this.participation };
			IList<IParticipation> injected = this.participations.Select(e => e.Object).ToList();
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<IParticipation> participation in expected)
				participation.VerifyGet(_participation => _participation.Severity, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Insert_Multiple_Participations_Test_Throws_Error_When_Any_Participation_Is_Null()
		{
			//Arrange
			IEnumerable<IParticipation> injected = new List<IParticipation>() { this.participation.Object, null };
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Insert_Multiple_Participations_Test_Throws_Error_When_Participation_Is_Null()
		{
			//Arrange
			IEnumerable<IParticipation> injected = null;
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Insert_Participation_Test_Gets_Participation_Activity_Key()
		{
			//Arrange
			Mock<IActivity> expected = this.activity;
			Mock<IParticipation> injected = this.participation;
			
			//Act
			this.repository.Insert(injected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Insert_Participation_Test_Gets_Participation_PerformedOn()
		{
			//Arrange
			Mock<IParticipation> expected = this.participation;
			
			//Act
			this.repository.Insert(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.PerformedOn, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Insert_Participation_Test_Gets_Participation_Severity()
		{
			//Arrange
			Mock<IParticipation> expected = this.participation;
			
			//Act
			this.repository.Insert(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Severity, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Insert_Participation_Test_Gets_Participation_User_Key()
		{
			//Arrange
			Mock<IUser> expected = this.user;
			Mock<IParticipation> injected = this.participation;
			
			//Act
			this.repository.Insert(injected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Insert_Participation_Test_Throws_Error_When_Participation_Is_Null()
		{
			//Arrange
			IParticipation injected = null;
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Insert
		
		#region Select
		
		[TestMethod]
		public virtual void Select_Participations_Test_Calls_Generate_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IParticipationsSearch> injected = this.searchMultiple;
			
			AddRow(this.participation.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateParticipation(It.IsAny<IParticipationKey>()));
		}
		
		[TestMethod]
		public virtual void Select_Participations_Test_Calls_GenerateParticipationKey_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IParticipationsSearch> injected = this.searchMultiple;
			
			AddRow(this.participation.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateParticipationKey(It.IsAny<Guid>()));
		}
		
		#endregion Select
		
		#region Select Single
		
		[TestMethod]
		public virtual void SelectSingle_Participation_Test_Calls_Generate_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IParticipationSearch> injected = this.search;
			
			AddRow(this.participation.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateParticipation(It.IsAny<IParticipationKey>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_Participation_Test_Calls_Generate_On_ModelFactory_For_Activity()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IParticipationSearch> injected = this.search;
			
			AddRow(this.participation.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateActivity(It.IsAny<IActivityKey>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_Participation_Test_Calls_Generate_On_ModelFactory_For_User()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IParticipationSearch> injected = this.search;
			
			AddRow(this.participation.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateUser(It.IsAny<IUserKey>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_Participation_Test_Calls_GenerateActivityKey_On_ModelFactory_For_Activity()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IParticipationSearch> injected = this.search;
			
			AddRow(this.participation.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateActivityKey(It.IsAny<Guid>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_Participation_Test_Calls_GenerateParticipationKey_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IParticipationSearch> injected = this.search;
			
			AddRow(this.participation.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateParticipationKey(It.IsAny<Guid>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_Participation_Test_Calls_GenerateUserKey_On_ModelFactory_For_User()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IParticipationSearch> injected = this.search;
			
			AddRow(this.participation.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateUserKey(It.IsAny<Guid>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_Participation_Test_Returns_Null_When_Participation_Is_Not_Found()
		{
			//Arrange
			IParticipation actual;
			Mock<IParticipationSearch> injected = this.search;
			
			//Act
			actual = this.repository.Select(injected.Object);
			
			//Assert
			Assert.IsNull(actual);
		}
		
		[TestMethod]
		public virtual void SelectSingle_Participation_Test_Returns_Participation_When_Is_Found()
		{
			//Arrange
			IParticipation actual;
			Mock<IParticipation> expected = this.participation;
			Mock<IParticipationSearch> injected = this.search;
			
			AddRow(expected.Object);
			
			//Act
			actual = this.repository.Select(injected.Object);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void SelectSingle_Participation_Test_Sets_Participation_Activity_Properties()
		{
			//Arrange
			Mock<IActivity> expected = this.activity;
			Mock<IParticipationSearch> injected = this.search;
			
			AddRow(this.participation.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.VerifySet(e => e.Category = It.IsAny<IActivityCategory>(), "Activity Category");
			expected.VerifySet(e => e.Name = It.IsAny<string>(), "Activity Name");
		}
		
		[TestMethod]
		public virtual void SelectSingle_Participation_Test_Sets_Participation_PerformedOn()
		{
			//Arrange
			Mock<IParticipation> expected = this.participation;
			Mock<IParticipationSearch> injected = this.search;
			
			AddRow(expected.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.VerifySet(e => e.PerformedOn = It.IsAny<DateTime>());
		}
		
		[TestMethod]
		public virtual void SelectSingle_Participation_Test_Sets_Participation_Severity()
		{
			//Arrange
			Mock<IParticipation> expected = this.participation;
			Mock<IParticipationSearch> injected = this.search;
			
			AddRow(expected.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.VerifySet(e => e.Severity = It.IsAny<double>());
		}
		
		[TestMethod]
		public virtual void SelectSingle_Participation_Test_Sets_Participation_User_Properties()
		{
			//Arrange
			Mock<IUser> expected = this.user;
			Mock<IParticipationSearch> injected = this.search;
			
			AddRow(this.participation.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.VerifySet(e => e.Name = It.IsAny<string>(), "User Name");
			expected.VerifySet(e => e.Password = It.IsAny<string>(), "User Password");
		}
		
		#endregion Select Single
		
		#region Update
		
		[TestMethod]
		public virtual void Update_Multiple_Participations_Test_Gets_Key_For_Participation()
		{
			//Arrange
			IList<Mock<IParticipation>> expected = new List<Mock<IParticipation>>() { this.participation };
			IList<IParticipation> injected = expected.Select(e => e.Object).ToList();
			
			AddRow(this.participation.Object);
			
			//Act
			foreach(Mock<IParticipation> participation in expected)
				participation.Invocations.Clear();
			
			this.repository.Update(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<IParticipation> participation in expected)
				participation.VerifyGet(m => m.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Multiple_Participations_Test_Gets_Key_For_Participation_Activity()
		{
			//Arrange
			IList<Mock<IActivity>> expected = this.activities;
			IList<IParticipation> injected = this.participations.Select(_mock => _mock.Object).ToList();
			
			AddRow(this.participation.Object);
			
			//Act
			foreach(Mock<IActivity> activity in expected)
				activity.Invocations.Clear();
			
			this.repository.Update(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<IActivity> activity in expected)
				activity.VerifyGet(m => m.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Multiple_Participations_Test_Gets_Key_For_Participation_User()
		{
			//Arrange
			IList<Mock<IUser>> expected = this.users;
			IList<IParticipation> injected = this.participations.Select(_mock => _mock.Object).ToList();
			
			AddRow(this.participation.Object);
			
			//Act
			foreach(Mock<IUser> user in expected)
				user.Invocations.Clear();
			
			this.repository.Update(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<IUser> user in expected)
				user.VerifyGet(m => m.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Multiple_Participations_Test_Gets_PerformedOn_For_Participation()
		{
			//Arrange
			IList<Mock<IParticipation>> expected = new List<Mock<IParticipation>>() { this.participation };
			
			AddRow(this.participation.Object);
			
			//Act
			foreach(Mock<IParticipation> participation in expected)
				participation.Invocations.Clear();
			
			this.repository.Update(expected.Select(e => e.Object).ToList(), this.transaction.Object);
			
			//Assert
			foreach(Mock<IParticipation> participation in expected)
				participation.VerifyGet(m => m.PerformedOn, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Multiple_Participations_Test_Gets_Severity_For_Participation()
		{
			//Arrange
			IList<Mock<IParticipation>> expected = new List<Mock<IParticipation>>() { this.participation };
			
			AddRow(this.participation.Object);
			
			//Act
			foreach(Mock<IParticipation> participation in expected)
				participation.Invocations.Clear();
			
			this.repository.Update(expected.Select(e => e.Object).ToList(), this.transaction.Object);
			
			//Assert
			foreach(Mock<IParticipation> participation in expected)
				participation.VerifyGet(m => m.Severity, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Update_Multiple_Participations_Test_Throws_Error_When_Any_Participation_Is_Null()
		{
			//Arrange
			IEnumerable<IParticipation> expected = new List<IParticipation>() { this.participation.Object, null };
			
			//Act
			this.repository.Update(expected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Update_Multiple_Participations_Test_Throws_Error_When_Participation_Is_Null()
		{
			//Arrange
			IEnumerable<IParticipation> expected = null;
			
			//Act
			this.repository.Update(expected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Update_Participation_Test_Gets_Participation_Activity_Key()
		{
			//Arrange
			Mock<IActivity> expected = this.activity;
			
			AddRow(this.participation.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(this.participation.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Participation_Test_Gets_Participation_Key()
		{
			//Arrange
			Mock<IParticipation> expected = this.participation;
			
			AddRow(expected.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Participation_Test_Gets_Participation_PerformedOn()
		{
			//Arrange
			Mock<IParticipation> expected = this.participation;
			
			AddRow(expected.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.PerformedOn, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Participation_Test_Gets_Participation_Severity()
		{
			//Arrange
			Mock<IParticipation> expected = this.participation;
			
			AddRow(expected.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Severity, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Participation_Test_Gets_Participation_User_Key()
		{
			//Arrange
			Mock<IUser> expected = this.user;
			
			AddRow(this.participation.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(this.participation.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Update_Participation_Test_Throws_Error_When_Participation_Is_Null()
		{
			//Arrange
			IParticipation injected = null;
			
			//Act
			this.repository.Update(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Update
	}
}
