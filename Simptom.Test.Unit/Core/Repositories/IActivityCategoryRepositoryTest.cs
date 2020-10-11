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
	public abstract class IActivityCategoryRepositoryTest
	{
		protected IList<Mock<IActivityCategory>> activityCategories;
		protected Mock<IActivityCategory> activityCategory;
		protected Mock<IActivityCategoryKey> key;
		protected IList<Mock<IActivityCategoryKey>> keys;
		protected Mock<IModelFactory> modelFactory;
		protected IActivityCategoryRepository repository;
		protected Mock<IActivityCategorySearch> search;
		protected Mock<IActivityCategoriesSearch> searchMultiple;
		protected TestContext testContext;
		protected Mock<IDbTransaction> transaction;
		
		public TestContext TestContext { get { return this.testContext; } set { this.testContext = value; } }
		
		[TestInitialize]
		public virtual void TestInitialize()
		{
			this.key = new Mock<IActivityCategoryKey>();
			this.key.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.keys = new List<Mock<IActivityCategoryKey>>() { this.key };
			
			this.activityCategory = new Mock<IActivityCategory>();
			this.activityCategory.SetupAllProperties();
			this.activityCategory.Setup(_model => _model.Key).Returns(() => { return this.key.Object; });
			this.activityCategory.Setup(_model => _model.Name).Returns("rwVpoPZrecFqsSJodEVWuKCo");
			this.activityCategories = new List<Mock<IActivityCategory>>() { activityCategory };
			
			this.modelFactory = new Mock<IModelFactory>();
			this.modelFactory.Setup(mf => mf.GenerateActivityCategoryKey(It.IsAny<Guid>())).Returns(this.key.Object);
			this.modelFactory.Setup(mf => mf.GenerateActivityCategory(It.IsAny<IActivityCategoryKey>())).Returns(this.activityCategory.Object);
			
			this.search = new Mock<IActivityCategorySearch>();
			this.searchMultiple = new Mock<IActivityCategoriesSearch>();
			this.transaction = new Mock<IDbTransaction>();
		}
		
		[TestCleanup]
		public virtual void TestCleanup()
		{
			this.transaction = null;
		}
		
		public virtual void AddRow(IActivityCategory activityCategory)
		{
			throw new NotImplementedException();
		}
		
		public virtual void AddRows(IEnumerable<IActivityCategory> activityCategories)
		{
			foreach(IActivityCategory activityCategory in activityCategories)
				AddRow(activityCategory);
		}
		
		#region Delete
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_ActivityCategory_Test_Throws_Error_When_Key_Is_Null()
		{
			//Arrange
			IActivityCategoryKey injected = null;
			
			//Act
			this.repository.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_Multiple_ActivityCategories_Test_Throws_Error_When_Any_Key_Is_Null()
		{
			//Arrange
			IList<IActivityCategoryKey> injected = new List<IActivityCategoryKey>() { new Mock<IActivityCategoryKey>().Object, null };
			
			//Act
			this.repository.Delete(injected, this.transaction.Object);
			
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
			
			//Act
			actual = this.repository.Exists(injected).Count();
			
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
			
			AddRow(this.activityCategory.Object);
			
			//Act
			actual = this.repository.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_ActivityCategories_Test_Returns_Non_Null_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			IEnumerable<IActivityCategoryKey> actual;
			IEnumerable<IActivityCategoryKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			//Act
			actual = this.repository.Exists(injected);
			
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
			
			//Act
			actual = this.repository.Exists(injected);
			
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
			
			AddRow(this.activityCategory.Object);
			
			//Act
			actual = this.repository.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		#endregion Exists
		
		#region Insert
		
		[TestMethod]
		public virtual void Insert_ActivityCategory_Test_Gets_ActivityCategory_Name()
		{
			//Arrange
			Mock<IActivityCategory> expected = this.activityCategory;
			
			//Act
			this.repository.Insert(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Name, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Insert_ActivityCategory_Test_Throws_Error_When_ActivityCategory_Is_Null()
		{
			//Arrange
			IActivityCategory injected = null;
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Insert_Multiple_ActivityCategories_Test_Gets_Name_For_Every_ActivityCategory()
		{
			//Arrange
			IList<Mock<IActivityCategory>> expected = new List<Mock<IActivityCategory>>() { this.activityCategory };
			IList<IActivityCategory> injected = this.activityCategories.Select(e => e.Object).ToList();
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<IActivityCategory> activityCategory in expected)
				activityCategory.VerifyGet(_activityCategory => _activityCategory.Name, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Insert_Multiple_ActivityCategories_Test_Throws_Error_When_ActivityCategory_Is_Null()
		{
			//Arrange
			IEnumerable<IActivityCategory> injected = null;
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Insert_Multiple_ActivityCategories_Test_Throws_Error_When_Any_ActivityCategory_Is_Null()
		{
			//Arrange
			IEnumerable<IActivityCategory> injected = new List<IActivityCategory>() { this.activityCategory.Object, null };
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Insert
		
		#region Select
		
		[TestMethod]
		public virtual void Select_ActivityCategories_Test_Calls_Generate_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IActivityCategoriesSearch> injected = this.searchMultiple;
			
			AddRow(this.activityCategory.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateActivityCategory(It.IsAny<IActivityCategoryKey>()));
		}
		
		[TestMethod]
		public virtual void Select_ActivityCategories_Test_Calls_GenerateActivityCategoryKey_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IActivityCategoriesSearch> injected = this.searchMultiple;
			
			AddRow(this.activityCategory.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateActivityCategoryKey(It.IsAny<Guid>()));
		}
		
		#endregion Select
		
		#region Select Single
		
		[TestMethod]
		public virtual void SelectSingle_ActivityCategory_Test_Calls_Generate_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IActivityCategorySearch> injected = this.search;
			
			AddRow(this.activityCategory.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateActivityCategory(It.IsAny<IActivityCategoryKey>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_ActivityCategory_Test_Calls_GenerateActivityCategoryKey_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IActivityCategorySearch> injected = this.search;
			
			AddRow(this.activityCategory.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateActivityCategoryKey(It.IsAny<Guid>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_ActivityCategory_Test_Returns_ActivityCategory_When_Is_Found()
		{
			//Arrange
			IActivityCategory actual;
			Mock<IActivityCategory> expected = this.activityCategory;
			Mock<IActivityCategorySearch> injected = this.search;
			
			AddRow(expected.Object);
			
			//Act
			actual = this.repository.Select(injected.Object);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void SelectSingle_ActivityCategory_Test_Returns_Null_When_ActivityCategory_Is_Not_Found()
		{
			//Arrange
			IActivityCategory actual;
			Mock<IActivityCategorySearch> injected = this.search;
			
			//Act
			actual = this.repository.Select(injected.Object);
			
			//Assert
			Assert.IsNull(actual);
		}
		
		[TestMethod]
		public virtual void SelectSingle_ActivityCategory_Test_Sets_ActivityCategory_Name()
		{
			//Arrange
			Mock<IActivityCategory> expected = this.activityCategory;
			Mock<IActivityCategorySearch> injected = this.search;
			
			AddRow(expected.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.VerifySet(e => e.Name = It.IsAny<string>());
		}
		
		#endregion Select Single
		
		#region Update
		
		[TestMethod]
		public virtual void Update_ActivityCategory_Test_Gets_ActivityCategory_Key()
		{
			//Arrange
			Mock<IActivityCategory> expected = this.activityCategory;
			
			AddRow(expected.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_ActivityCategory_Test_Gets_ActivityCategory_Name()
		{
			//Arrange
			Mock<IActivityCategory> expected = this.activityCategory;
			
			AddRow(expected.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Name, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Update_ActivityCategory_Test_Throws_Error_When_ActivityCategory_Is_Null()
		{
			//Arrange
			IActivityCategory injected = null;
			
			//Act
			this.repository.Update(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Update_Multiple_ActivityCategories_Test_Gets_Key_For_ActivityCategory()
		{
			//Arrange
			IList<Mock<IActivityCategory>> expected = new List<Mock<IActivityCategory>>() { this.activityCategory };
			IList<IActivityCategory> injected = expected.Select(e => e.Object).ToList();
			
			AddRow(this.activityCategory.Object);
			
			//Act
			foreach(Mock<IActivityCategory> activityCategory in expected)
				activityCategory.Invocations.Clear();
			
			this.repository.Update(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<IActivityCategory> activityCategory in expected)
				activityCategory.VerifyGet(m => m.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Multiple_ActivityCategories_Test_Gets_Name_For_ActivityCategory()
		{
			//Arrange
			IList<Mock<IActivityCategory>> expected = new List<Mock<IActivityCategory>>() { this.activityCategory };
			
			AddRow(this.activityCategory.Object);
			
			//Act
			foreach(Mock<IActivityCategory> activityCategory in expected)
				activityCategory.Invocations.Clear();
			
			this.repository.Update(expected.Select(e => e.Object).ToList(), this.transaction.Object);
			
			//Assert
			foreach(Mock<IActivityCategory> activityCategory in expected)
				activityCategory.VerifyGet(m => m.Name, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Update_Multiple_ActivityCategories_Test_Throws_Error_When_ActivityCategory_Is_Null()
		{
			//Arrange
			IEnumerable<IActivityCategory> expected = null;
			
			//Act
			this.repository.Update(expected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Update_Multiple_ActivityCategories_Test_Throws_Error_When_Any_ActivityCategory_Is_Null()
		{
			//Arrange
			IEnumerable<IActivityCategory> expected = new List<IActivityCategory>() { this.activityCategory.Object, null };
			
			//Act
			this.repository.Update(expected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Update
	}
}
