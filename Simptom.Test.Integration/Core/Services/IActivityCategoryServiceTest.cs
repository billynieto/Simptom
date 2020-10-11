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
	public abstract class IActivityCategoryServiceTest
	{
		protected IActivityCategory activityCategory;
		protected IActivityCategory activityCategoryNotFound;
		protected IActivityCategory initial;
		protected IModelFactory modelFactory;
		protected IActivityCategoriesSearch searchMultipleThatFindsNothing;
		protected IActivityCategoriesSearch searchMultipleThatFindsSomething;
		protected IActivityCategorySearch searchThatReturnsNull;
		protected IActivityCategorySearch searchThatReturnsSomething;
		protected IActivityCategoryService service;
		protected TestContext testContext;
		
		public TestContext TestContext { get { return this.testContext; } set { this.testContext = value; } }
		
		[TestInitialize]
		public virtual void TestInitialize()
		{
			this.modelFactory = new ValidModelFactory();
		}
		
		[TestCleanup]
		public virtual void TestCleanup()
		{
			if(this.service.Exists(this.activityCategoryNotFound.Key))
				this.service.Delete(this.activityCategoryNotFound.Key, null);
			
			if(this.initial != null)
				this.service.Save(this.initial, null);
		}
		
		public virtual IActivityCategorySearch PrepareSearch(IActivityCategory activityCategory)
		{
			throw new NotImplementedException();
		}
		
		#region Delete
		
		#endregion Delete
		
		#region Exists
		
		[TestMethod]
		public virtual void Exists_ActivityCategories_Test_Does_Not_Return_Null_List_When_Nothing_Matches()
		{
			//Arrange
			IEnumerable<IActivityCategoryKey> actual = null;
			IEnumerable<IActivityCategoryKey> injected = new List<IActivityCategoryKey>() { this.activityCategoryNotFound.Key };
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void Exists_ActivityCategories_Test_Returns_Empty_List_When_Nothing_Matches()
		{
			//Arrange
			int actual;
			int expected = 0;
			IEnumerable<IActivityCategoryKey> injected = new List<IActivityCategoryKey>() { this.activityCategoryNotFound.Key };
			
			//Act
			actual = this.service.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_ActivityCategories_Test_Returns_List_Of_ActivityCategoryKeys_That_Match()
		{
			if(this.activityCategory == null)
				throw new AssertInconclusiveException("You must provide a ActivityCategory that is already in the repository for this test to be effective.");
			
			//Arrange
			IEnumerable<IActivityCategoryKey> actual = null;
			IEnumerable<IActivityCategoryKey> injected = new List<IActivityCategoryKey>() { this.activityCategory.Key };
			IEnumerable<IActivityCategoryKey> expected = injected;
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected.Count(), actual.Count());
			Assert.IsTrue(expected.All(_e => actual.Any(_a => _e.Equals(_a))));
			Assert.IsTrue(actual.All(_a => expected.Any(_e => _e.Equals(_a))));
		}
		
		[TestMethod]
		public virtual void Exists_ActivityCategory_Test_Returns_False_When_ActivityCategory_Is_Not_Found()
		{
			//Arrange
			bool actual;
			bool expected = false;
			IActivityCategoryKey injected = this.activityCategoryNotFound.Key;
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_ActivityCategory_Test_Returns_True_When_ActivityCategory_Is_Found()
		{
			if(this.activityCategory == null)
				throw new AssertInconclusiveException("You must provide a ActivityCategory that is already in the repository for this test to be effective.");
			
			//Arrange
			bool actual;
			bool expected = true;
			IActivityCategoryKey injected = this.activityCategory.Key;
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		#endregion Exists
		
		#region Find
		
		[TestMethod]
		public virtual void Find_ActivityCategories_Test_Does_Not_Return_Null_List_When_Nothing_Matches()
		{
			//Arrange
			IEnumerable<IActivityCategory> actual;
			IActivityCategoriesSearch injected = this.searchMultipleThatFindsNothing;
			
			//Act
			actual = this.service.Find(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void Find_ActivityCategories_Test_Returns_Empty_List_When_Nothing_Matches()
		{
			//Arrange
			int actual;
			int expected = 0;
			IActivityCategoriesSearch injected = this.searchMultipleThatFindsNothing;
			
			//Act
			actual = this.service.Find(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Find_ActivityCategories_Test_Throws_Error_When_Search_Is_Null()
		{
			//Arrange
			IActivityCategoriesSearch injected = null;
			
			//Act
			this.service.Find(injected);
			
			//Assert
		}
		
		#endregion Find
		
		#region Find Single
		
		[TestMethod]
		public virtual void FindSingle_ActivityCategory_Test_Returns_Model_When_Found()
		{
			if(this.activityCategory == null)
				throw new AssertInconclusiveException("You must provide a ActivityCategory that is already in the repository for this test to be effective.");
			
			//Arrange
			IActivityCategory actual;
			IActivityCategorySearch injected = this.searchThatReturnsSomething;
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void FindSingle_ActivityCategory_Test_Returns_Null_When_Not_Found()
		{
			//Arrange
			IActivityCategory actual;
			IActivityCategorySearch injected = this.searchThatReturnsNull;
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNull(actual);
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void FindSingle_ActivityCategory_Test_Throws_Error_When_Search_Is_Null()
		{
			//Arrange
			IActivityCategorySearch injected = null;
			
			//Act
			this.service.FindSingle(injected);
			
			//Assert
		}
		
		#endregion Find Single
		
		#region Save
		
		[TestMethod]
		public virtual void Save_ActivityCategory_Test_Preserves_ActivityCategory_When_It_Already_Exists()
		{
			if(this.activityCategory == null)
				throw new AssertInconclusiveException("You must provide a ActivityCategory that is already in the repository for this test to be effective.");
			
			//Arrange
			IActivityCategory injected = this.activityCategory;
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Save_ActivityCategory_Test_Preserves_ActivityCategory_When_It_Is_New()
		{
			if(this.activityCategory == null)
				throw new AssertInconclusiveException("You must provide a ActivityCategory that is already in the repository for this test to be effective.");
			
			//Arrange
			IActivityCategory injected = this.activityCategoryNotFound;
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			IActivityCategory expected = injected;
			IActivityCategorySearch search = PrepareSearch(expected);
			if(search != null)
			{
				IActivityCategory actual = this.activityCategoryNotFound = this.service.FindSingle(search);
			
				Assert.IsNotNull(actual);
				Assert.AreEqual(expected.Name, actual.Name, "Name");
			}
		}
		
		[TestMethod]
		public virtual void Save_ActivityCategory_Test_Preserves_Name_When_Is_Maxed_And_ActivityCategories_Already_Exists()
		{
			if(this.activityCategory == null)
				throw new AssertInconclusiveException("You must provide a ActivityCategory that is already in the repository for this test to be effective.");
			
			//Arrange
			IActivityCategory injected = this.activityCategory;
			//               .........10........20........30........40........50........60
			injected.Name = "FGvQsGNmhnJaKzZAyBMnHVIfUhiWXkNKwpoebzgGxqyBwCFCFw";
			
			IActivityCategory actual = null;
			IActivityCategory expected = injected;
			
			IActivityCategorySearch search = PrepareSearch(expected);
			if(search == null)
				throw new AssertInconclusiveException("You must override the function that prepares a ActivityCategory Search model for this test to be effective.");
			
			this.initial = this.service.FindSingle(this.searchThatReturnsSomething);
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			actual = this.service.FindSingle(search);
			
			Assert.AreEqual(expected.Name, actual.Name);
		}
		
		[TestMethod]
		public virtual void Save_Multiple_ActivityCategories_Test_Preserves_ActivityCategories_When_They_Already_Exist()
		{
			if(this.activityCategory == null)
				throw new AssertInconclusiveException("You must provide a ActivityCategory that is already in the repository for this test to be effective.");
			
			//Arrange
			List<IActivityCategory> injected = new List<IActivityCategory>() { this.activityCategory };
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Save_Multiple_ActivityCategories_Test_Preserves_ActivityCategories_When_They_Are_New()
		{
			if(this.activityCategory == null)
				throw new AssertInconclusiveException("You must provide a ActivityCategory that is already in the repository for this test to be effective.");
			
			//Arrange
			List<IActivityCategory> injected = new List<IActivityCategory>() { this.activityCategoryNotFound };
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			IActivityCategory expected = injected.First();
			IActivityCategorySearch search = PrepareSearch(expected);
			if(search != null)
			{
				IActivityCategory actual = this.activityCategoryNotFound = this.service.FindSingle(search);
			
				Assert.IsNotNull(actual);
				Assert.AreEqual(expected.Name, actual.Name, "Name");
			}
		}
		
		[TestMethod]
		public virtual void Save_Multiple_ActivityCategories_Test_Preserves_Name_When_Is_Maxed_And_ActivityCategories_Already_Exists()
		{
			if(this.activityCategory == null)
				throw new AssertInconclusiveException("You must provide a ActivityCategory that is already in the repository for this test to be effective.");
			
			//Arrange
			IEnumerable<IActivityCategory> injected = new List<IActivityCategory>() { this.activityCategory };
			//                            .........10........20........30........40........50........60
			this.activityCategory.Name = "QohbAEtVtLXOZZylAxmxxkBVUnUskULwqynkDFPgDqcEXKzkWV";
			
			IActivityCategory actual = null;
			IActivityCategory expected = this.activityCategory;
			
			IActivityCategorySearch search = PrepareSearch(expected);
			if(search == null)
				throw new AssertInconclusiveException("You must override the function that prepares a ActivityCategory Search model for this test to be effective.");
			
			this.initial = this.service.FindSingle(this.searchThatReturnsSomething);
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			actual = this.service.FindSingle(search);
			
			Assert.AreEqual(expected.Name, actual.Name);
		}
		
		#endregion Save
	}
}
