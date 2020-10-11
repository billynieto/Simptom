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
	public abstract class ISymptomCategoryRepositoryTest
	{
		protected Mock<ISymptomCategoryKey> key;
		protected IList<Mock<ISymptomCategoryKey>> keys;
		protected Mock<IModelFactory> modelFactory;
		protected ISymptomCategoryRepository repository;
		protected Mock<ISymptomCategorySearch> search;
		protected Mock<ISymptomCategoriesSearch> searchMultiple;
		protected IList<Mock<ISymptomCategory>> symptomCategories;
		protected Mock<ISymptomCategory> symptomCategory;
		protected TestContext testContext;
		protected Mock<IDbTransaction> transaction;
		
		public TestContext TestContext { get { return this.testContext; } set { this.testContext = value; } }
		
		[TestInitialize]
		public virtual void TestInitialize()
		{
			this.key = new Mock<ISymptomCategoryKey>();
			this.key.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.keys = new List<Mock<ISymptomCategoryKey>>() { this.key };
			
			this.symptomCategory = new Mock<ISymptomCategory>();
			this.symptomCategory.SetupAllProperties();
			this.symptomCategory.Setup(_model => _model.Key).Returns(() => { return this.key.Object; });
			this.symptomCategory.Setup(_model => _model.Name).Returns("Hn");
			this.symptomCategories = new List<Mock<ISymptomCategory>>() { symptomCategory };
			
			this.modelFactory = new Mock<IModelFactory>();
			this.modelFactory.Setup(mf => mf.GenerateSymptomCategoryKey(It.IsAny<Guid>())).Returns(this.key.Object);
			this.modelFactory.Setup(mf => mf.GenerateSymptomCategory(It.IsAny<ISymptomCategoryKey>())).Returns(this.symptomCategory.Object);
			
			this.search = new Mock<ISymptomCategorySearch>();
			this.searchMultiple = new Mock<ISymptomCategoriesSearch>();
			this.transaction = new Mock<IDbTransaction>();
		}
		
		[TestCleanup]
		public virtual void TestCleanup()
		{
			this.transaction = null;
		}
		
		public virtual void AddRow(ISymptomCategory symptomCategory)
		{
			throw new NotImplementedException();
		}
		
		public virtual void AddRows(IEnumerable<ISymptomCategory> symptomCategories)
		{
			foreach(ISymptomCategory symptomCategory in symptomCategories)
				AddRow(symptomCategory);
		}
		
		#region Delete
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_Multiple_SymptomCategories_Test_Throws_Error_When_Any_Key_Is_Null()
		{
			//Arrange
			IList<ISymptomCategoryKey> injected = new List<ISymptomCategoryKey>() { new Mock<ISymptomCategoryKey>().Object, null };
			
			//Act
			this.repository.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_SymptomCategory_Test_Throws_Error_When_Key_Is_Null()
		{
			//Arrange
			ISymptomCategoryKey injected = null;
			
			//Act
			this.repository.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Delete
		
		#region Exists
		
		[TestMethod]
		public virtual void Exists_SymptomCategories_Test_Returns_Empty_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			int actual;
			int expected = 0;
			IEnumerable<ISymptomCategoryKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			//Act
			actual = this.repository.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_SymptomCategories_Test_Returns_List_Of_Keys_When_They_Were_Found()
		{
			//Arrange
			int actual;
			int expected = 1;
			IEnumerable<ISymptomCategoryKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			AddRow(this.symptomCategory.Object);
			
			//Act
			actual = this.repository.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_SymptomCategories_Test_Returns_Non_Null_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			IEnumerable<ISymptomCategoryKey> actual;
			IEnumerable<ISymptomCategoryKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			//Act
			actual = this.repository.Exists(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void Exists_SymptomCategory_Test_Returns_False_When_Key_Was_Not_Found()
		{
			//Arrange
			bool actual;
			bool expected = false;
			ISymptomCategoryKey injected = this.key.Object;
			
			//Act
			actual = this.repository.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_SymptomCategory_Test_Returns_True_When_Key_Was_Found()
		{
			//Arrange
			bool actual;
			bool expected = true;
			ISymptomCategoryKey injected = this.key.Object;
			
			AddRow(this.symptomCategory.Object);
			
			//Act
			actual = this.repository.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		#endregion Exists
		
		#region Insert
		
		[TestMethod]
		public virtual void Insert_Multiple_SymptomCategories_Test_Gets_Name_For_Every_SymptomCategory()
		{
			//Arrange
			IList<Mock<ISymptomCategory>> expected = new List<Mock<ISymptomCategory>>() { this.symptomCategory };
			IList<ISymptomCategory> injected = this.symptomCategories.Select(e => e.Object).ToList();
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<ISymptomCategory> symptomCategory in expected)
				symptomCategory.VerifyGet(_symptomCategory => _symptomCategory.Name, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Insert_Multiple_SymptomCategories_Test_Throws_Error_When_Any_SymptomCategory_Is_Null()
		{
			//Arrange
			IEnumerable<ISymptomCategory> injected = new List<ISymptomCategory>() { this.symptomCategory.Object, null };
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Insert_Multiple_SymptomCategories_Test_Throws_Error_When_SymptomCategory_Is_Null()
		{
			//Arrange
			IEnumerable<ISymptomCategory> injected = null;
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Insert_SymptomCategory_Test_Gets_SymptomCategory_Name()
		{
			//Arrange
			Mock<ISymptomCategory> expected = this.symptomCategory;
			
			//Act
			this.repository.Insert(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Name, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Insert_SymptomCategory_Test_Throws_Error_When_SymptomCategory_Is_Null()
		{
			//Arrange
			ISymptomCategory injected = null;
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Insert
		
		#region Select
		
		[TestMethod]
		public virtual void Select_SymptomCategories_Test_Calls_Generate_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<ISymptomCategoriesSearch> injected = this.searchMultiple;
			
			AddRow(this.symptomCategory.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateSymptomCategory(It.IsAny<ISymptomCategoryKey>()));
		}
		
		[TestMethod]
		public virtual void Select_SymptomCategories_Test_Calls_GenerateSymptomCategoryKey_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<ISymptomCategoriesSearch> injected = this.searchMultiple;
			
			AddRow(this.symptomCategory.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateSymptomCategoryKey(It.IsAny<Guid>()));
		}
		
		#endregion Select
		
		#region Select Single
		
		[TestMethod]
		public virtual void SelectSingle_SymptomCategory_Test_Calls_Generate_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<ISymptomCategorySearch> injected = this.search;
			
			AddRow(this.symptomCategory.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateSymptomCategory(It.IsAny<ISymptomCategoryKey>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_SymptomCategory_Test_Calls_GenerateSymptomCategoryKey_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<ISymptomCategorySearch> injected = this.search;
			
			AddRow(this.symptomCategory.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateSymptomCategoryKey(It.IsAny<Guid>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_SymptomCategory_Test_Returns_Null_When_SymptomCategory_Is_Not_Found()
		{
			//Arrange
			ISymptomCategory actual;
			Mock<ISymptomCategorySearch> injected = this.search;
			
			//Act
			actual = this.repository.Select(injected.Object);
			
			//Assert
			Assert.IsNull(actual);
		}
		
		[TestMethod]
		public virtual void SelectSingle_SymptomCategory_Test_Returns_SymptomCategory_When_Is_Found()
		{
			//Arrange
			ISymptomCategory actual;
			Mock<ISymptomCategory> expected = this.symptomCategory;
			Mock<ISymptomCategorySearch> injected = this.search;
			
			AddRow(expected.Object);
			
			//Act
			actual = this.repository.Select(injected.Object);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void SelectSingle_SymptomCategory_Test_Sets_SymptomCategory_Name()
		{
			//Arrange
			Mock<ISymptomCategory> expected = this.symptomCategory;
			Mock<ISymptomCategorySearch> injected = this.search;
			
			AddRow(expected.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.VerifySet(e => e.Name = It.IsAny<string>());
		}
		
		#endregion Select Single
		
		#region Update
		
		[TestMethod]
		public virtual void Update_Multiple_SymptomCategories_Test_Gets_Key_For_SymptomCategory()
		{
			//Arrange
			IList<Mock<ISymptomCategory>> expected = new List<Mock<ISymptomCategory>>() { this.symptomCategory };
			IList<ISymptomCategory> injected = expected.Select(e => e.Object).ToList();
			
			AddRow(this.symptomCategory.Object);
			
			//Act
			foreach(Mock<ISymptomCategory> symptomCategory in expected)
				symptomCategory.Invocations.Clear();
			
			this.repository.Update(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<ISymptomCategory> symptomCategory in expected)
				symptomCategory.VerifyGet(m => m.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Multiple_SymptomCategories_Test_Gets_Name_For_SymptomCategory()
		{
			//Arrange
			IList<Mock<ISymptomCategory>> expected = new List<Mock<ISymptomCategory>>() { this.symptomCategory };
			
			AddRow(this.symptomCategory.Object);
			
			//Act
			foreach(Mock<ISymptomCategory> symptomCategory in expected)
				symptomCategory.Invocations.Clear();
			
			this.repository.Update(expected.Select(e => e.Object).ToList(), this.transaction.Object);
			
			//Assert
			foreach(Mock<ISymptomCategory> symptomCategory in expected)
				symptomCategory.VerifyGet(m => m.Name, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Update_Multiple_SymptomCategories_Test_Throws_Error_When_Any_SymptomCategory_Is_Null()
		{
			//Arrange
			IEnumerable<ISymptomCategory> expected = new List<ISymptomCategory>() { this.symptomCategory.Object, null };
			
			//Act
			this.repository.Update(expected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Update_Multiple_SymptomCategories_Test_Throws_Error_When_SymptomCategory_Is_Null()
		{
			//Arrange
			IEnumerable<ISymptomCategory> expected = null;
			
			//Act
			this.repository.Update(expected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Update_SymptomCategory_Test_Gets_SymptomCategory_Key()
		{
			//Arrange
			Mock<ISymptomCategory> expected = this.symptomCategory;
			
			AddRow(expected.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_SymptomCategory_Test_Gets_SymptomCategory_Name()
		{
			//Arrange
			Mock<ISymptomCategory> expected = this.symptomCategory;
			
			AddRow(expected.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Name, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Update_SymptomCategory_Test_Throws_Error_When_SymptomCategory_Is_Null()
		{
			//Arrange
			ISymptomCategory injected = null;
			
			//Act
			this.repository.Update(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Update
	}
}
