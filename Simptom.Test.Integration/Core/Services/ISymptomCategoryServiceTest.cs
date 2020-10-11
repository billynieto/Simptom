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
	public abstract class ISymptomCategoryServiceTest
	{
		protected ISymptomCategory initial;
		protected IModelFactory modelFactory;
		protected ISymptomCategoriesSearch searchMultipleThatFindsNothing;
		protected ISymptomCategoriesSearch searchMultipleThatFindsSomething;
		protected ISymptomCategorySearch searchThatReturnsNull;
		protected ISymptomCategorySearch searchThatReturnsSomething;
		protected ISymptomCategoryService service;
		protected ISymptomCategory symptomCategory;
		protected ISymptomCategory symptomCategoryNotFound;
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
			if(this.service.Exists(this.symptomCategoryNotFound.Key))
				this.service.Delete(this.symptomCategoryNotFound.Key, null);
			
			if(this.initial != null)
				this.service.Save(this.initial, null);
		}
		
		public virtual ISymptomCategorySearch PrepareSearch(ISymptomCategory symptomCategory)
		{
			throw new NotImplementedException();
		}
		
		#region Delete
		
		#endregion Delete
		
		#region Exists
		
		[TestMethod]
		public virtual void Exists_SymptomCategories_Test_Does_Not_Return_Null_List_When_Nothing_Matches()
		{
			//Arrange
			IEnumerable<ISymptomCategoryKey> actual = null;
			IEnumerable<ISymptomCategoryKey> injected = new List<ISymptomCategoryKey>() { this.symptomCategoryNotFound.Key };
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void Exists_SymptomCategories_Test_Returns_Empty_List_When_Nothing_Matches()
		{
			//Arrange
			int actual;
			int expected = 0;
			IEnumerable<ISymptomCategoryKey> injected = new List<ISymptomCategoryKey>() { this.symptomCategoryNotFound.Key };
			
			//Act
			actual = this.service.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_SymptomCategories_Test_Returns_List_Of_SymptomCategoryKeys_That_Match()
		{
			if(this.symptomCategory == null)
				throw new AssertInconclusiveException("You must provide a SymptomCategory that is already in the repository for this test to be effective.");
			
			//Arrange
			IEnumerable<ISymptomCategoryKey> actual = null;
			IEnumerable<ISymptomCategoryKey> injected = new List<ISymptomCategoryKey>() { this.symptomCategory.Key };
			IEnumerable<ISymptomCategoryKey> expected = injected;
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected.Count(), actual.Count());
			Assert.IsTrue(expected.All(_e => actual.Any(_a => _e.Equals(_a))));
			Assert.IsTrue(actual.All(_a => expected.Any(_e => _e.Equals(_a))));
		}
		
		[TestMethod]
		public virtual void Exists_SymptomCategory_Test_Returns_False_When_SymptomCategory_Is_Not_Found()
		{
			//Arrange
			bool actual;
			bool expected = false;
			ISymptomCategoryKey injected = this.symptomCategoryNotFound.Key;
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_SymptomCategory_Test_Returns_True_When_SymptomCategory_Is_Found()
		{
			if(this.symptomCategory == null)
				throw new AssertInconclusiveException("You must provide a SymptomCategory that is already in the repository for this test to be effective.");
			
			//Arrange
			bool actual;
			bool expected = true;
			ISymptomCategoryKey injected = this.symptomCategory.Key;
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		#endregion Exists
		
		#region Find
		
		[TestMethod]
		public virtual void Find_SymptomCategories_Test_Does_Not_Return_Null_List_When_Nothing_Matches()
		{
			//Arrange
			IEnumerable<ISymptomCategory> actual;
			ISymptomCategoriesSearch injected = this.searchMultipleThatFindsNothing;
			
			//Act
			actual = this.service.Find(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void Find_SymptomCategories_Test_Returns_Empty_List_When_Nothing_Matches()
		{
			//Arrange
			int actual;
			int expected = 0;
			ISymptomCategoriesSearch injected = this.searchMultipleThatFindsNothing;
			
			//Act
			actual = this.service.Find(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Find_SymptomCategories_Test_Throws_Error_When_Search_Is_Null()
		{
			//Arrange
			ISymptomCategoriesSearch injected = null;
			
			//Act
			this.service.Find(injected);
			
			//Assert
		}
		
		#endregion Find
		
		#region Find Single
		
		[TestMethod]
		public virtual void FindSingle_SymptomCategory_Test_Returns_Model_When_Found()
		{
			if(this.symptomCategory == null)
				throw new AssertInconclusiveException("You must provide a SymptomCategory that is already in the repository for this test to be effective.");
			
			//Arrange
			ISymptomCategory actual;
			ISymptomCategorySearch injected = this.searchThatReturnsSomething;
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void FindSingle_SymptomCategory_Test_Returns_Null_When_Not_Found()
		{
			//Arrange
			ISymptomCategory actual;
			ISymptomCategorySearch injected = this.searchThatReturnsNull;
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNull(actual);
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void FindSingle_SymptomCategory_Test_Throws_Error_When_Search_Is_Null()
		{
			//Arrange
			ISymptomCategorySearch injected = null;
			
			//Act
			this.service.FindSingle(injected);
			
			//Assert
		}
		
		#endregion Find Single
		
		#region Save
		
		[TestMethod]
		public virtual void Save_Multiple_SymptomCategories_Test_Preserves_Name_When_Is_Maxed_And_SymptomCategories_Already_Exists()
		{
			if(this.symptomCategory == null)
				throw new AssertInconclusiveException("You must provide a SymptomCategory that is already in the repository for this test to be effective.");
			
			//Arrange
			IEnumerable<ISymptomCategory> injected = new List<ISymptomCategory>() { this.symptomCategory };
			//                           .........10........20........30........40........50........60
			this.symptomCategory.Name = "enDqLgRLVkQdvGGrMvmsFTknyRWnpzJznHhcsOAmyCLtUjzozZ";
			
			ISymptomCategory actual = null;
			ISymptomCategory expected = this.symptomCategory;
			
			ISymptomCategorySearch search = PrepareSearch(expected);
			if(search == null)
				throw new AssertInconclusiveException("You must override the function that prepares a SymptomCategory Search model for this test to be effective.");
			
			this.initial = this.service.FindSingle(this.searchThatReturnsSomething);
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			actual = this.service.FindSingle(search);
			
			Assert.AreEqual(expected.Name, actual.Name);
		}
		
		[TestMethod]
		public virtual void Save_Multiple_SymptomCategories_Test_Preserves_SymptomCategories_When_They_Already_Exist()
		{
			if(this.symptomCategory == null)
				throw new AssertInconclusiveException("You must provide a SymptomCategory that is already in the repository for this test to be effective.");
			
			//Arrange
			List<ISymptomCategory> injected = new List<ISymptomCategory>() { this.symptomCategory };
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Save_Multiple_SymptomCategories_Test_Preserves_SymptomCategories_When_They_Are_New()
		{
			if(this.symptomCategory == null)
				throw new AssertInconclusiveException("You must provide a SymptomCategory that is already in the repository for this test to be effective.");
			
			//Arrange
			List<ISymptomCategory> injected = new List<ISymptomCategory>() { this.symptomCategoryNotFound };
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			ISymptomCategory expected = injected.First();
			ISymptomCategorySearch search = PrepareSearch(expected);
			if(search != null)
			{
				ISymptomCategory actual = this.symptomCategoryNotFound = this.service.FindSingle(search);
			
				Assert.IsNotNull(actual);
				Assert.AreEqual(expected.Name, actual.Name, "Name");
			}
		}
		
		[TestMethod]
		public virtual void Save_SymptomCategory_Test_Preserves_Name_When_Is_Maxed_And_SymptomCategories_Already_Exists()
		{
			if(this.symptomCategory == null)
				throw new AssertInconclusiveException("You must provide a SymptomCategory that is already in the repository for this test to be effective.");
			
			//Arrange
			ISymptomCategory injected = this.symptomCategory;
			//               .........10........20........30........40........50........60
			injected.Name = "CIKTKkcpSTjEvwAqPNzEYXVaTciRsosIOpWaPuWWZcVGcUhpSD";
			
			ISymptomCategory actual = null;
			ISymptomCategory expected = injected;
			
			ISymptomCategorySearch search = PrepareSearch(expected);
			if(search == null)
				throw new AssertInconclusiveException("You must override the function that prepares a SymptomCategory Search model for this test to be effective.");
			
			this.initial = this.service.FindSingle(this.searchThatReturnsSomething);
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			actual = this.service.FindSingle(search);
			
			Assert.AreEqual(expected.Name, actual.Name);
		}
		
		[TestMethod]
		public virtual void Save_SymptomCategory_Test_Preserves_SymptomCategory_When_It_Already_Exists()
		{
			if(this.symptomCategory == null)
				throw new AssertInconclusiveException("You must provide a SymptomCategory that is already in the repository for this test to be effective.");
			
			//Arrange
			ISymptomCategory injected = this.symptomCategory;
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Save_SymptomCategory_Test_Preserves_SymptomCategory_When_It_Is_New()
		{
			if(this.symptomCategory == null)
				throw new AssertInconclusiveException("You must provide a SymptomCategory that is already in the repository for this test to be effective.");
			
			//Arrange
			ISymptomCategory injected = this.symptomCategoryNotFound;
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			ISymptomCategory expected = injected;
			ISymptomCategorySearch search = PrepareSearch(expected);
			if(search != null)
			{
				ISymptomCategory actual = this.symptomCategoryNotFound = this.service.FindSingle(search);
			
				Assert.IsNotNull(actual);
				Assert.AreEqual(expected.Name, actual.Name, "Name");
			}
		}
		
		#endregion Save
	}
}
