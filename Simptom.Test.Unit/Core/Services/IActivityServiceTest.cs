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
	public abstract class IActivityServiceTest
	{
		protected IList<Mock<IActivity>> activities;
		protected Mock<IActivity> activity;
		protected IList<Mock<IActivityCategory>> activityCategories;
		protected Mock<IActivityCategoriesSearch> activityCategoriesSearch;
		protected Mock<IActivityCategory> activityCategory;
		protected Mock<IActivityCategoryKey> activityCategoryKey;
		protected IList<Mock<IActivityCategoryKey>> activityCategoryKeys;
		protected Mock<IActivityCategoryService> activityCategoryService;
		protected Mock<IActivityKey> key;
		protected IList<Mock<IActivityKey>> keys;
		protected Mock<IModelFactory> modelFactory;
		protected Mock<IActivityRepository> repository;
		protected Mock<IActivitySearch> search;
		protected Mock<IActivitiesSearch> searchMultiple;
		protected IActivityService service;
		protected ConnectionState state;
		protected TestContext testContext;
		protected Mock<IDbTransaction> transaction;
		
		public TestContext TestContext { get { return this.testContext; } set { this.testContext = value; } }
		
		[TestInitialize]
		public virtual void TestInitialize()
		{
			this.activityCategoryKey = new Mock<IActivityCategoryKey>();
			this.activityCategoryKey.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.activityCategoryKeys = new List<Mock<IActivityCategoryKey>>() { this.activityCategoryKey };
			this.activityCategory = new Mock<IActivityCategory>();
			this.activityCategory.SetupAllProperties();
			this.activityCategory.Setup(_model => _model.Key).Returns(() => { return this.activityCategoryKey.Object; });
			this.activityCategory.Setup(_model => _model.Name).Returns("CWDjg");
			this.activityCategories = new List<Mock<IActivityCategory>> { this.activityCategory };
			this.activityCategoryService = new Mock<IActivityCategoryService>();
			this.activityCategoryService.Setup(_service => _service.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(() => { return this.activityCategoryKeys.Select(_activityCategoryKey => _activityCategoryKey.Object); });
			this.activityCategoriesSearch = new Mock<IActivityCategoriesSearch>();
			
			this.key = new Mock<IActivityKey>();
			this.key.SetupAllProperties();
			this.key.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.keys = new List<Mock<IActivityKey>>() { this.key };
			this.search = new Mock<IActivitySearch>();
			this.searchMultiple = new Mock<IActivitiesSearch>();
			
			this.activity = new Mock<IActivity>();
			this.activity.SetupAllProperties();
			this.activity.Setup(m => m.Key).Returns(this.key.Object);
			this.activity.Setup(m => m.Category).Returns(() => { return this.activityCategory.Object; });
			this.activities = new List<Mock<IActivity>>() { this.activity };
			
			this.modelFactory = new Mock<IModelFactory>();
			this.modelFactory.Setup(_modelFactory => _modelFactory.GenerateActivityCategoriesSearch()).Returns(() => { return this.activityCategoriesSearch.Object; });
			
			this.state = ConnectionState.Closed;
			this.repository = new Mock<IActivityRepository>();
			this.repository.Setup(_repository => _repository.Open()).Callback(() => { this.state = ConnectionState.Open; });
			this.repository.Setup(_repository => _repository.Close()).Callback(() => { this.state = ConnectionState.Closed; });
			this.repository.Setup(_repository => _repository.IsOpen).Returns(() => { return this.state == ConnectionState.Open; });
			this.transaction = new Mock<IDbTransaction>();
		}
		
		[TestCleanup]
		public virtual void TestCleanup()
		{
			this.activityCategoryKey = null;
			this.activityCategory = null;
			this.activityCategories = null;
			
			this.repository = null;
		}
		
		#region Delete
		
		[TestMethod]
		public virtual void Delete_Activity_Test_Calls_Delete_On_Repository()
		{
			//Arrange
			IActivityKey injected = activity.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Delete(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())); }
			catch { this.repository.Verify(r => r.Delete(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())); }
		}
		
		[TestMethod]
		public virtual void Delete_Activity_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			IActivityKey injected = activity.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<IActivityKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_Activity_Test_Throws_Error_When_ActivityKey_Is_Null()
		{
			//Arrange
			IActivityKey injected = null;
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public virtual void Delete_Activity_Test_Throws_Error_When_ActivityKey_Not_Found_In_Repository()
		{
			//Arrange
			IActivityKey injected = activity.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(new List<IActivityKey>());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IActivityKey>());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Delete_Multiple_Activities_Test_Calls_Delete_On_Repository()
		{
			//Arrange
			IList<IActivityKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Delete(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>()), Times.Exactly(activities.Count)); }
			catch { this.repository.Verify(r => r.Delete(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>()), Times.Exactly(1)); }
		}
		
		[TestMethod]
		public virtual void Delete_Multiple_Activities_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			IList<IActivityKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<IActivityKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public virtual void Delete_Multiple_Activities_Test_Throws_Error_When_ActivityKey_Is_Not_Found_In_Repository()
		{
			//Arrange
			IList<IActivityKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(new List<IActivityKey>());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IActivityKey>());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_Multiple_Activities_Test_Throws_Error_When_List_Of_ActivityKey_Is_Null()
		{
			//Arrange
			IList<IActivityKey> injected = null;
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
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
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(new List<IActivityKey>());
			
			//Act
			actual = this.service.Exists(injected).Count();
			
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
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			actual = this.service.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Activities_Test_Returns_Non_Null_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			IEnumerable<IActivityKey> actual;
			IEnumerable<IActivityKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(new List<IActivityKey>());
			
			//Act
			actual = this.service.Exists(injected);
			
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
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(new List<IActivityKey>());
			
			//Act
			actual = this.service.Exists(injected);
			
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
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		#endregion Exists
		
		#region Find
		
		[TestMethod]
		public virtual void Find_Activities_Test_Calls_Select_On_Repository()
		{
			//Arrange
			IActivitiesSearch injected = this.searchMultiple.Object;
			
			this.repository.Setup(r => r.Select(It.IsAny<IActivitiesSearch>())).Returns(new List<IActivity>());
			
			//Act
			this.service.Find(injected);
			
			//Assert
			this.repository.Verify(r => r.Select(It.IsAny<IActivitiesSearch>()), Times.Exactly(1));
		}
		
		[TestMethod]
		public virtual void Find_Activities_Test_Calls_Validate_On_Search()
		{
			//Arrange
			Mock<IActivitiesSearch> expected = this.searchMultiple;
			IActivitiesSearch injected = this.searchMultiple.Object;
			
			this.repository.Setup(r => r.Select(It.IsAny<IActivitiesSearch>())).Returns(new List<IActivity>());
			
			//Act
			this.service.Find(injected);
			
			//Assert
			expected.Verify(r => r.Validate());
		}
		
		#endregion Find
		
		#region Find Single
		
		[TestMethod]
		public virtual void FindSingle_Activity_Test_Calls_Select_On_Repository()
		{
			//Arrange
			IActivitySearch injected = this.search.Object;
			
			
			this.repository.Setup(r => r.Select(It.IsAny<IActivitySearch>())).Returns(this.activity.Object);
			
			//Act
			this.service.FindSingle(injected);
			
			//Assert
			this.repository.Verify(r => r.Select(It.IsAny<IActivitySearch>()), Times.Exactly(1));
		}
		
		[TestMethod]
		public virtual void FindSingle_Activity_Test_Calls_Validate_On_Search()
		{
			//Arrange
			Mock<IActivitySearch> expected = this.search;
			IActivitySearch injected = this.search.Object;
			
			
			this.repository.Setup(r => r.Select(It.IsAny<IActivitySearch>())).Returns(this.activity.Object);
			
			//Act
			this.service.FindSingle(injected);
			
			//Assert
			expected.Verify(e => e.Validate());
		}
		
		[TestMethod]
		public virtual void FindSingle_Activity_Test_Returns_Activity_When_Is_Found_In_Repository()
		{
			//Arrange
			IActivity actual;
			IActivitySearch injected = this.search.Object;
			
			
			this.repository.Setup(r => r.Select(It.IsAny<IActivitySearch>())).Returns(this.activity.Object);
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void FindSingle_Activity_Test_Returns_Null_When_Activity_Is_Not_Found_In_Repository()
		{
			//Arrange
			IActivity actual;
			IActivitySearch injected = this.search.Object;
			
			this.repository.Setup(r => r.Select(It.IsAny<IActivitySearch>())).Returns((IActivity)null);
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNull(actual);
		}
		
		#endregion Find Single
		
		#region Save
		
		[TestMethod]
		public virtual void Save_Activity_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			Mock<IActivity> expected = this.activity;
			IActivity injected = this.activity.Object;
			
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>())).Returns(() => { return true; });
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(() => { return this.activityCategoryKeys.Select(_key => _key.Object); });
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(new List<IActivityKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IActivityKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<IActivityKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_Activity_Test_Calls_Insert_On_Repository_For_New_Activities()
		{
			//Arrange
			Mock<IActivity> expected = this.activity;
			IActivity injected = this.activity.Object;
			
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>())).Returns(() => { return true; });
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(() => { return this.activityCategoryKeys.Select(_key => _key.Object); });
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(new List<IActivityKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IActivityKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Insert(It.IsAny<IActivity>(), It.IsAny<IDbTransaction>())); }
			catch { this.repository.Verify(r => r.Insert(It.IsAny<IEnumerable<IActivity>>(), It.IsAny<IDbTransaction>())); }
		}
		
		[TestMethod]
		public virtual void Save_Activity_Test_Calls_Update_On_Repository_For_Existing_Activities()
		{
			//Arrange
			Mock<IActivity> expected = this.activity;
			IActivity injected = this.activity.Object;
			
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>())).Returns(() => { return true; });
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(() => { return this.activityCategoryKeys.Select(_key => _key.Object); });
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Update(It.IsAny<IActivity>(), It.IsAny<IDbTransaction>())); }
			catch { this.repository.Verify(r => r.Update(It.IsAny<IEnumerable<IActivity>>(), It.IsAny<IDbTransaction>())); }
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Save_Activity_Test_Throws_Error_When_Activity_Is_Null()
		{
			//Arrange
			IActivity injected = null;
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Activities_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			List<IActivity> injected = this.activities.Select(e => e.Object).ToList();
			
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>())).Returns(() => { return true; });
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(() => { return this.activityCategoryKeys.Select(_key => _key.Object); });
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(new List<IActivityKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IActivityKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<IActivityKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Activities_Test_Calls_Insert_On_Repository_For_New_Activities()
		{
			//Arrange
			List<IActivity> injected = this.activities.Select(e => e.Object).ToList();
			
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>())).Returns(() => { return true; });
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(() => { return this.activityCategoryKeys.Select(_key => _key.Object); });
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(new List<IActivityKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IActivityKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.repository.Verify(r => r.Insert(It.IsAny<IEnumerable<IActivity>>(), It.IsAny<IDbTransaction>()));
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Activities_Test_Calls_Update_On_Repository_For_Existing_Activities()
		{
			//Arrange
			List<IActivity> injected = this.activities.Select(e => e.Object).ToList();
			
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>())).Returns(() => { return true; });
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(() => { return this.activityCategoryKeys.Select(_key => _key.Object); });
			this.activityCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.activityCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.repository.Verify(r => r.Update(It.IsAny<IEnumerable<IActivity>>(), It.IsAny<IDbTransaction>()));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Save_Multiple_Activities_Test_Throws_Error_When_Any_Activity_Is_Null()
		{
			//Arrange
			List<IActivity> injected = new List<IActivity>() { new Mock<IActivity>().Object, null, new Mock<IActivity>().Object };
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Save
	}
}
