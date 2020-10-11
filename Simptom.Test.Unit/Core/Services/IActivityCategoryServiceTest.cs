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
	public abstract class IActivityCategoryServiceTest
	{
		protected IList<Mock<IActivityCategory>> activityCategories;
		protected Mock<IActivityCategory> activityCategory;
		protected Mock<IActivityCategoryKey> key;
		protected IList<Mock<IActivityCategoryKey>> keys;
		protected Mock<IModelFactory> modelFactory;
		protected Mock<IActivityCategoryRepository> repository;
		protected Mock<IActivityCategorySearch> search;
		protected Mock<IActivityCategoriesSearch> searchMultiple;
		protected IActivityCategoryService service;
		protected ConnectionState state;
		protected TestContext testContext;
		protected Mock<IDbTransaction> transaction;
		
		public TestContext TestContext { get { return this.testContext; } set { this.testContext = value; } }
		
		[TestInitialize]
		public virtual void TestInitialize()
		{
			this.key = new Mock<IActivityCategoryKey>();
			this.key.SetupAllProperties();
			this.key.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.keys = new List<Mock<IActivityCategoryKey>>() { this.key };
			this.search = new Mock<IActivityCategorySearch>();
			this.searchMultiple = new Mock<IActivityCategoriesSearch>();
			
			this.activityCategory = new Mock<IActivityCategory>();
			this.activityCategory.SetupAllProperties();
			this.activityCategory.Setup(m => m.Key).Returns(this.key.Object);
			this.activityCategories = new List<Mock<IActivityCategory>>() { this.activityCategory };
			
			this.modelFactory = new Mock<IModelFactory>();
			
			this.state = ConnectionState.Closed;
			this.repository = new Mock<IActivityCategoryRepository>();
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
		public virtual void Delete_ActivityCategory_Test_Calls_Delete_On_Repository()
		{
			//Arrange
			IActivityCategoryKey injected = activityCategory.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityCategoryKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Delete(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())); }
			catch { this.repository.Verify(r => r.Delete(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())); }
		}
		
		[TestMethod]
		public virtual void Delete_ActivityCategory_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			IActivityCategoryKey injected = activityCategory.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityCategoryKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<IActivityCategoryKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_ActivityCategory_Test_Throws_Error_When_ActivityCategoryKey_Is_Null()
		{
			//Arrange
			IActivityCategoryKey injected = null;
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public virtual void Delete_ActivityCategory_Test_Throws_Error_When_ActivityCategoryKey_Not_Found_In_Repository()
		{
			//Arrange
			IActivityCategoryKey injected = activityCategory.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityCategoryKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(new List<IActivityCategoryKey>());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IActivityCategoryKey>());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Delete_Multiple_ActivityCategories_Test_Calls_Delete_On_Repository()
		{
			//Arrange
			IList<IActivityCategoryKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityCategoryKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Delete(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>()), Times.Exactly(activityCategories.Count)); }
			catch { this.repository.Verify(r => r.Delete(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>()), Times.Exactly(1)); }
		}
		
		[TestMethod]
		public virtual void Delete_Multiple_ActivityCategories_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			IList<IActivityCategoryKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityCategoryKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<IActivityCategoryKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public virtual void Delete_Multiple_ActivityCategories_Test_Throws_Error_When_ActivityCategoryKey_Is_Not_Found_In_Repository()
		{
			//Arrange
			IList<IActivityCategoryKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityCategoryKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(new List<IActivityCategoryKey>());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IActivityCategoryKey>());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_Multiple_ActivityCategories_Test_Throws_Error_When_List_Of_ActivityCategoryKey_Is_Null()
		{
			//Arrange
			IList<IActivityCategoryKey> injected = null;
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Delete
		
		#region Exists
		
		[TestMethod]
		public virtual void Exists_ActivityCategories_Test_Returns_Empty_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			int actual;
			int expected = 0;
			IEnumerable<IActivityCategoryKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityCategoryKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(new List<IActivityCategoryKey>());
			
			//Act
			actual = this.service.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_ActivityCategories_Test_Returns_List_Of_Keys_When_They_Were_Found()
		{
			//Arrange
			int actual;
			int expected = 1;
			IEnumerable<IActivityCategoryKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityCategoryKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			actual = this.service.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_ActivityCategories_Test_Returns_Non_Null_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			IEnumerable<IActivityCategoryKey> actual;
			IEnumerable<IActivityCategoryKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityCategoryKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(new List<IActivityCategoryKey>());
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void Exists_ActivityCategory_Test_Returns_False_When_Key_Was_Not_Found()
		{
			//Arrange
			bool actual;
			bool expected = false;
			IActivityCategoryKey injected = this.key.Object;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityCategoryKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(new List<IActivityCategoryKey>());
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_ActivityCategory_Test_Returns_True_When_Key_Was_Found()
		{
			//Arrange
			bool actual;
			bool expected = true;
			IActivityCategoryKey injected = this.key.Object;
			
			this.repository.Setup(r => r.Exists(It.IsAny<IActivityCategoryKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		#endregion Exists
		
		#region Find
		
		[TestMethod]
		public virtual void Find_ActivityCategories_Test_Calls_Select_On_Repository()
		{
			//Arrange
			IActivityCategoriesSearch injected = this.searchMultiple.Object;
			
			this.repository.Setup(r => r.Select(It.IsAny<IActivityCategoriesSearch>())).Returns(new List<IActivityCategory>());
			
			//Act
			this.service.Find(injected);
			
			//Assert
			this.repository.Verify(r => r.Select(It.IsAny<IActivityCategoriesSearch>()), Times.Exactly(1));
		}
		
		[TestMethod]
		public virtual void Find_ActivityCategories_Test_Calls_Validate_On_Search()
		{
			//Arrange
			Mock<IActivityCategoriesSearch> expected = this.searchMultiple;
			IActivityCategoriesSearch injected = this.searchMultiple.Object;
			
			this.repository.Setup(r => r.Select(It.IsAny<IActivityCategoriesSearch>())).Returns(new List<IActivityCategory>());
			
			//Act
			this.service.Find(injected);
			
			//Assert
			expected.Verify(r => r.Validate());
		}
		
		#endregion Find
		
		#region Find Single
		
		[TestMethod]
		public virtual void FindSingle_ActivityCategory_Test_Calls_Select_On_Repository()
		{
			//Arrange
			IActivityCategorySearch injected = this.search.Object;
			
			
			this.repository.Setup(r => r.Select(It.IsAny<IActivityCategorySearch>())).Returns(this.activityCategory.Object);
			
			//Act
			this.service.FindSingle(injected);
			
			//Assert
			this.repository.Verify(r => r.Select(It.IsAny<IActivityCategorySearch>()), Times.Exactly(1));
		}
		
		[TestMethod]
		public virtual void FindSingle_ActivityCategory_Test_Calls_Validate_On_Search()
		{
			//Arrange
			Mock<IActivityCategorySearch> expected = this.search;
			IActivityCategorySearch injected = this.search.Object;
			
			
			this.repository.Setup(r => r.Select(It.IsAny<IActivityCategorySearch>())).Returns(this.activityCategory.Object);
			
			//Act
			this.service.FindSingle(injected);
			
			//Assert
			expected.Verify(e => e.Validate());
		}
		
		[TestMethod]
		public virtual void FindSingle_ActivityCategory_Test_Returns_ActivityCategory_When_Is_Found_In_Repository()
		{
			//Arrange
			IActivityCategory actual;
			IActivityCategorySearch injected = this.search.Object;
			
			
			this.repository.Setup(r => r.Select(It.IsAny<IActivityCategorySearch>())).Returns(this.activityCategory.Object);
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void FindSingle_ActivityCategory_Test_Returns_Null_When_ActivityCategory_Is_Not_Found_In_Repository()
		{
			//Arrange
			IActivityCategory actual;
			IActivityCategorySearch injected = this.search.Object;
			
			this.repository.Setup(r => r.Select(It.IsAny<IActivityCategorySearch>())).Returns((IActivityCategory)null);
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNull(actual);
		}
		
		#endregion Find Single
		
		#region Save
		
		[TestMethod]
		public virtual void Save_ActivityCategory_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			Mock<IActivityCategory> expected = this.activityCategory;
			IActivityCategory injected = this.activityCategory.Object;
			
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(new List<IActivityCategoryKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IActivityCategoryKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<IActivityCategoryKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_ActivityCategory_Test_Calls_Insert_On_Repository_For_New_ActivityCategories()
		{
			//Arrange
			Mock<IActivityCategory> expected = this.activityCategory;
			IActivityCategory injected = this.activityCategory.Object;
			
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(new List<IActivityCategoryKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IActivityCategoryKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Insert(It.IsAny<IActivityCategory>(), It.IsAny<IDbTransaction>())); }
			catch { this.repository.Verify(r => r.Insert(It.IsAny<IEnumerable<IActivityCategory>>(), It.IsAny<IDbTransaction>())); }
		}
		
		[TestMethod]
		public virtual void Save_ActivityCategory_Test_Calls_Update_On_Repository_For_Existing_ActivityCategories()
		{
			//Arrange
			Mock<IActivityCategory> expected = this.activityCategory;
			IActivityCategory injected = this.activityCategory.Object;
			
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Update(It.IsAny<IActivityCategory>(), It.IsAny<IDbTransaction>())); }
			catch { this.repository.Verify(r => r.Update(It.IsAny<IEnumerable<IActivityCategory>>(), It.IsAny<IDbTransaction>())); }
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Save_ActivityCategory_Test_Throws_Error_When_ActivityCategory_Is_Null()
		{
			//Arrange
			IActivityCategory injected = null;
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Save_Multiple_ActivityCategories_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			List<IActivityCategory> injected = this.activityCategories.Select(e => e.Object).ToList();
			
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(new List<IActivityCategoryKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IActivityCategoryKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<IActivityCategoryKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_Multiple_ActivityCategories_Test_Calls_Insert_On_Repository_For_New_ActivityCategories()
		{
			//Arrange
			List<IActivityCategory> injected = this.activityCategories.Select(e => e.Object).ToList();
			
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(new List<IActivityCategoryKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<IActivityCategoryKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.repository.Verify(r => r.Insert(It.IsAny<IEnumerable<IActivityCategory>>(), It.IsAny<IDbTransaction>()));
		}
		
		[TestMethod]
		public virtual void Save_Multiple_ActivityCategories_Test_Calls_Update_On_Repository_For_Existing_ActivityCategories()
		{
			//Arrange
			List<IActivityCategory> injected = this.activityCategories.Select(e => e.Object).ToList();
			
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IActivityCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<IActivityCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.repository.Verify(r => r.Update(It.IsAny<IEnumerable<IActivityCategory>>(), It.IsAny<IDbTransaction>()));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Save_Multiple_ActivityCategories_Test_Throws_Error_When_Any_ActivityCategory_Is_Null()
		{
			//Arrange
			List<IActivityCategory> injected = new List<IActivityCategory>() { new Mock<IActivityCategory>().Object, null, new Mock<IActivityCategory>().Object };
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Save
	}
}
