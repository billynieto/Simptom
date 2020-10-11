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
	public abstract class IActivityRepositoryTest
	{
		protected IList<Mock<IActivity>> activities;
		protected Mock<IActivity> activity;
		protected IList<Mock<IActivityCategory>> activityCategories;
		protected Mock<IActivityCategory> activityCategory;
		protected Mock<IActivityCategoryKey> activityCategoryKey;
		protected Mock<IActivityKey> key;
		protected IList<Mock<IActivityKey>> keys;
		protected Mock<IModelFactory> modelFactory;
		protected IActivityRepository repository;
		protected Mock<IActivitySearch> search;
		protected Mock<IActivitiesSearch> searchMultiple;
		protected TestContext testContext;
		protected Mock<IDbTransaction> transaction;
		
		public TestContext TestContext { get { return this.testContext; } set { this.testContext = value; } }
		
		[TestInitialize]
		public virtual void TestInitialize()
		{
			this.activityCategoryKey = new Mock<IActivityCategoryKey>();
			this.activityCategoryKey.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.activityCategory = new Mock<IActivityCategory>();
			this.activityCategory.SetupAllProperties();
			this.activityCategory.Setup(_model => _model.Key).Returns(() => { return this.activityCategoryKey.Object; });
			this.activityCategory.Setup(_activityCategory => _activityCategory.Name).Returns("InMjBHqDIaTHMDMuspEPkEIQ");
			this.activityCategories = new List<Mock<IActivityCategory>> { this.activityCategory };
			
			this.key = new Mock<IActivityKey>();
			this.key.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.keys = new List<Mock<IActivityKey>>() { this.key };
			
			this.activity = new Mock<IActivity>();
			this.activity.SetupAllProperties();
			this.activity.Setup(_model => _model.Key).Returns(() => { return this.key.Object; });
			this.activity.Setup(_model => _model.Category).Returns(() => { return this.activityCategory.Object; });
			this.activity.Setup(_model => _model.Name).Returns("gXNoMIeaQYgEB");
			this.activities = new List<Mock<IActivity>>() { activity };
			
			this.modelFactory = new Mock<IModelFactory>();
			this.modelFactory.Setup(mf => mf.GenerateActivityCategoryKey(It.IsAny<Guid>())).Returns(this.activityCategoryKey.Object);
			this.modelFactory.Setup(mf => mf.GenerateActivityCategory(It.IsAny<IActivityCategoryKey>())).Returns(this.activityCategory.Object);
			this.modelFactory.Setup(mf => mf.GenerateActivityKey(It.IsAny<Guid>())).Returns(this.key.Object);
			this.modelFactory.Setup(mf => mf.GenerateActivity(It.IsAny<IActivityKey>())).Returns(this.activity.Object);
			
			this.search = new Mock<IActivitySearch>();
			this.searchMultiple = new Mock<IActivitiesSearch>();
			this.transaction = new Mock<IDbTransaction>();
		}
		
		[TestCleanup]
		public virtual void TestCleanup()
		{
			this.activityCategoryKey = null;
			this.activityCategory = null;
			this.activityCategories = null;
			this.transaction = null;
		}
		
		public virtual void AddRow(IActivity activity)
		{
			throw new NotImplementedException();
		}
		
		public virtual void AddRows(IEnumerable<IActivity> activities)
		{
			foreach(IActivity activity in activities)
				AddRow(activity);
		}
		
		#region Delete
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_Activity_Test_Throws_Error_When_Key_Is_Null()
		{
			//Arrange
			IActivityKey injected = null;
			
			//Act
			this.repository.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_Multiple_Activities_Test_Throws_Error_When_Any_Key_Is_Null()
		{
			//Arrange
			IList<IActivityKey> injected = new List<IActivityKey>() { new Mock<IActivityKey>().Object, null };
			
			//Act
			this.repository.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Delete
		
		#region Exists
		
		[TestMethod]
		public virtual void Exists_Activities_Test_Returns_Empty_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			int actual;
			int expected = 0;
			IEnumerable<IActivityKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			//Act
			actual = this.repository.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Activities_Test_Returns_List_Of_Keys_When_They_Were_Found()
		{
			//Arrange
			int actual;
			int expected = 1;
			IEnumerable<IActivityKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			AddRow(this.activity.Object);
			
			//Act
			actual = this.repository.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Activities_Test_Returns_Non_Null_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			IEnumerable<IActivityKey> actual;
			IEnumerable<IActivityKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			//Act
			actual = this.repository.Exists(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void Exists_Activity_Test_Returns_False_When_Key_Was_Not_Found()
		{
			//Arrange
			bool actual;
			bool expected = false;
			IActivityKey injected = this.key.Object;
			
			//Act
			actual = this.repository.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Activity_Test_Returns_True_When_Key_Was_Found()
		{
			//Arrange
			bool actual;
			bool expected = true;
			IActivityKey injected = this.key.Object;
			
			AddRow(this.activity.Object);
			
			//Act
			actual = this.repository.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		#endregion Exists
		
		#region Insert
		
		[TestMethod]
		public virtual void Insert_Activity_Test_Gets_Activity_Name()
		{
			//Arrange
			Mock<IActivity> expected = this.activity;
			
			//Act
			this.repository.Insert(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Name, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Insert_Activity_Test_Throws_Error_When_Activity_Is_Null()
		{
			//Arrange
			IActivity injected = null;
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Insert_Multiple_Activities_Test_Gets_Name_For_Every_Activity()
		{
			//Arrange
			IList<Mock<IActivity>> expected = new List<Mock<IActivity>>() { this.activity };
			IList<IActivity> injected = this.activities.Select(e => e.Object).ToList();
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<IActivity> activity in expected)
				activity.VerifyGet(_activity => _activity.Name, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Insert_Multiple_Activities_Test_Throws_Error_When_Activity_Is_Null()
		{
			//Arrange
			IEnumerable<IActivity> injected = null;
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Insert_Multiple_Activities_Test_Throws_Error_When_Any_Activity_Is_Null()
		{
			//Arrange
			IEnumerable<IActivity> injected = new List<IActivity>() { this.activity.Object, null };
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Insert
		
		#region Select
		
		[TestMethod]
		public virtual void Select_Activities_Test_Calls_Generate_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IActivitiesSearch> injected = this.searchMultiple;
			
			AddRow(this.activity.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateActivity(It.IsAny<IActivityKey>()));
		}
		
		[TestMethod]
		public virtual void Select_Activities_Test_Calls_GenerateActivityKey_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IActivitiesSearch> injected = this.searchMultiple;
			
			AddRow(this.activity.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateActivityKey(It.IsAny<Guid>()));
		}
		
		#endregion Select
		
		#region Select Single
		
		[TestMethod]
		public virtual void SelectSingle_Activity_Test_Calls_Generate_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IActivitySearch> injected = this.search;
			
			AddRow(this.activity.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateActivity(It.IsAny<IActivityKey>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_Activity_Test_Calls_Generate_On_ModelFactory_For_Category()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IActivitySearch> injected = this.search;
			
			AddRow(this.activity.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateActivityCategory(It.IsAny<IActivityCategoryKey>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_Activity_Test_Calls_GenerateActivityCategoryKey_On_ModelFactory_For_Category()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IActivitySearch> injected = this.search;
			
			AddRow(this.activity.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateActivityCategoryKey(It.IsAny<Guid>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_Activity_Test_Calls_GenerateActivityKey_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IActivitySearch> injected = this.search;
			
			AddRow(this.activity.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateActivityKey(It.IsAny<Guid>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_Activity_Test_Returns_Activity_When_Is_Found()
		{
			//Arrange
			IActivity actual;
			Mock<IActivity> expected = this.activity;
			Mock<IActivitySearch> injected = this.search;
			
			AddRow(expected.Object);
			
			//Act
			actual = this.repository.Select(injected.Object);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void SelectSingle_Activity_Test_Returns_Null_When_Activity_Is_Not_Found()
		{
			//Arrange
			IActivity actual;
			Mock<IActivitySearch> injected = this.search;
			
			//Act
			actual = this.repository.Select(injected.Object);
			
			//Assert
			Assert.IsNull(actual);
		}
		
		[TestMethod]
		public virtual void SelectSingle_Activity_Test_Sets_Activity_Category_Properties()
		{
			//Arrange
			Mock<IActivityCategory> expected = this.activityCategory;
			Mock<IActivitySearch> injected = this.search;
			
			AddRow(this.activity.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.VerifySet(e => e.Name = It.IsAny<string>(), "ActivityCategory Name");
		}
		
		[TestMethod]
		public virtual void SelectSingle_Activity_Test_Sets_Activity_Name()
		{
			//Arrange
			Mock<IActivity> expected = this.activity;
			Mock<IActivitySearch> injected = this.search;
			
			AddRow(expected.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.VerifySet(e => e.Name = It.IsAny<string>());
		}
		
		#endregion Select Single
		
		#region Update
		
		[TestMethod]
		public virtual void Update_Activity_Test_Gets_Activity_Key()
		{
			//Arrange
			Mock<IActivity> expected = this.activity;
			
			AddRow(expected.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Activity_Test_Gets_Activity_Name()
		{
			//Arrange
			Mock<IActivity> expected = this.activity;
			
			AddRow(expected.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Name, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Update_Activity_Test_Throws_Error_When_Activity_Is_Null()
		{
			//Arrange
			IActivity injected = null;
			
			//Act
			this.repository.Update(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Update_Multiple_Activities_Test_Gets_Key_For_Activity()
		{
			//Arrange
			IList<Mock<IActivity>> expected = new List<Mock<IActivity>>() { this.activity };
			IList<IActivity> injected = expected.Select(e => e.Object).ToList();
			
			AddRow(this.activity.Object);
			
			//Act
			foreach(Mock<IActivity> activity in expected)
				activity.Invocations.Clear();
			
			this.repository.Update(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<IActivity> activity in expected)
				activity.VerifyGet(m => m.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Multiple_Activities_Test_Gets_Name_For_Activity()
		{
			//Arrange
			IList<Mock<IActivity>> expected = new List<Mock<IActivity>>() { this.activity };
			
			AddRow(this.activity.Object);
			
			//Act
			foreach(Mock<IActivity> activity in expected)
				activity.Invocations.Clear();
			
			this.repository.Update(expected.Select(e => e.Object).ToList(), this.transaction.Object);
			
			//Assert
			foreach(Mock<IActivity> activity in expected)
				activity.VerifyGet(m => m.Name, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Update_Multiple_Activities_Test_Throws_Error_When_Activity_Is_Null()
		{
			//Arrange
			IEnumerable<IActivity> expected = null;
			
			//Act
			this.repository.Update(expected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Update_Multiple_Activities_Test_Throws_Error_When_Any_Activity_Is_Null()
		{
			//Arrange
			IEnumerable<IActivity> expected = new List<IActivity>() { this.activity.Object, null };
			
			//Act
			this.repository.Update(expected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Update
	}
}
