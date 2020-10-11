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
	public abstract class ISymptomCategoryServiceTest
	{
		protected Mock<ISymptomCategoryKey> key;
		protected IList<Mock<ISymptomCategoryKey>> keys;
		protected Mock<IModelFactory> modelFactory;
		protected Mock<ISymptomCategoryRepository> repository;
		protected Mock<ISymptomCategorySearch> search;
		protected Mock<ISymptomCategoriesSearch> searchMultiple;
		protected ISymptomCategoryService service;
		protected ConnectionState state;
		protected IList<Mock<ISymptomCategory>> symptomCategories;
		protected Mock<ISymptomCategory> symptomCategory;
		protected TestContext testContext;
		protected Mock<IDbTransaction> transaction;
		
		public TestContext TestContext { get { return this.testContext; } set { this.testContext = value; } }
		
		[TestInitialize]
		public virtual void TestInitialize()
		{
			this.key = new Mock<ISymptomCategoryKey>();
			this.key.SetupAllProperties();
			this.key.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.keys = new List<Mock<ISymptomCategoryKey>>() { this.key };
			this.search = new Mock<ISymptomCategorySearch>();
			this.searchMultiple = new Mock<ISymptomCategoriesSearch>();
			
			this.symptomCategory = new Mock<ISymptomCategory>();
			this.symptomCategory.SetupAllProperties();
			this.symptomCategory.Setup(m => m.Key).Returns(this.key.Object);
			this.symptomCategories = new List<Mock<ISymptomCategory>>() { this.symptomCategory };
			
			this.modelFactory = new Mock<IModelFactory>();
			
			this.state = ConnectionState.Closed;
			this.repository = new Mock<ISymptomCategoryRepository>();
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
		public virtual void Delete_Multiple_SymptomCategories_Test_Calls_Delete_On_Repository()
		{
			//Arrange
			IList<ISymptomCategoryKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Delete(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>()), Times.Exactly(symptomCategories.Count)); }
			catch { this.repository.Verify(r => r.Delete(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>()), Times.Exactly(1)); }
		}
		
		[TestMethod]
		public virtual void Delete_Multiple_SymptomCategories_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			IList<ISymptomCategoryKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<ISymptomCategoryKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_Multiple_SymptomCategories_Test_Throws_Error_When_List_Of_SymptomCategoryKey_Is_Null()
		{
			//Arrange
			IList<ISymptomCategoryKey> injected = null;
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public virtual void Delete_Multiple_SymptomCategories_Test_Throws_Error_When_SymptomCategoryKey_Is_Not_Found_In_Repository()
		{
			//Arrange
			IList<ISymptomCategoryKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(new List<ISymptomCategoryKey>());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<ISymptomCategoryKey>());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Delete_SymptomCategory_Test_Calls_Delete_On_Repository()
		{
			//Arrange
			ISymptomCategoryKey injected = symptomCategory.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Delete(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())); }
			catch { this.repository.Verify(r => r.Delete(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())); }
		}
		
		[TestMethod]
		public virtual void Delete_SymptomCategory_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			ISymptomCategoryKey injected = symptomCategory.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<ISymptomCategoryKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_SymptomCategory_Test_Throws_Error_When_SymptomCategoryKey_Is_Null()
		{
			//Arrange
			ISymptomCategoryKey injected = null;
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public virtual void Delete_SymptomCategory_Test_Throws_Error_When_SymptomCategoryKey_Not_Found_In_Repository()
		{
			//Arrange
			ISymptomCategoryKey injected = symptomCategory.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(new List<ISymptomCategoryKey>());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<ISymptomCategoryKey>());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
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
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(new List<ISymptomCategoryKey>());
			
			//Act
			actual = this.service.Exists(injected).Count();
			
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
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			actual = this.service.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_SymptomCategories_Test_Returns_Non_Null_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			IEnumerable<ISymptomCategoryKey> actual;
			IEnumerable<ISymptomCategoryKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(new List<ISymptomCategoryKey>());
			
			//Act
			actual = this.service.Exists(injected);
			
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
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(new List<ISymptomCategoryKey>());
			
			//Act
			actual = this.service.Exists(injected);
			
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
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		#endregion Exists
		
		#region Find
		
		[TestMethod]
		public virtual void Find_SymptomCategories_Test_Calls_Select_On_Repository()
		{
			//Arrange
			ISymptomCategoriesSearch injected = this.searchMultiple.Object;
			
			this.repository.Setup(r => r.Select(It.IsAny<ISymptomCategoriesSearch>())).Returns(new List<ISymptomCategory>());
			
			//Act
			this.service.Find(injected);
			
			//Assert
			this.repository.Verify(r => r.Select(It.IsAny<ISymptomCategoriesSearch>()), Times.Exactly(1));
		}
		
		[TestMethod]
		public virtual void Find_SymptomCategories_Test_Calls_Validate_On_Search()
		{
			//Arrange
			Mock<ISymptomCategoriesSearch> expected = this.searchMultiple;
			ISymptomCategoriesSearch injected = this.searchMultiple.Object;
			
			this.repository.Setup(r => r.Select(It.IsAny<ISymptomCategoriesSearch>())).Returns(new List<ISymptomCategory>());
			
			//Act
			this.service.Find(injected);
			
			//Assert
			expected.Verify(r => r.Validate());
		}
		
		#endregion Find
		
		#region Find Single
		
		[TestMethod]
		public virtual void FindSingle_SymptomCategory_Test_Calls_Select_On_Repository()
		{
			//Arrange
			ISymptomCategorySearch injected = this.search.Object;
			
			
			this.repository.Setup(r => r.Select(It.IsAny<ISymptomCategorySearch>())).Returns(this.symptomCategory.Object);
			
			//Act
			this.service.FindSingle(injected);
			
			//Assert
			this.repository.Verify(r => r.Select(It.IsAny<ISymptomCategorySearch>()), Times.Exactly(1));
		}
		
		[TestMethod]
		public virtual void FindSingle_SymptomCategory_Test_Calls_Validate_On_Search()
		{
			//Arrange
			Mock<ISymptomCategorySearch> expected = this.search;
			ISymptomCategorySearch injected = this.search.Object;
			
			
			this.repository.Setup(r => r.Select(It.IsAny<ISymptomCategorySearch>())).Returns(this.symptomCategory.Object);
			
			//Act
			this.service.FindSingle(injected);
			
			//Assert
			expected.Verify(e => e.Validate());
		}
		
		[TestMethod]
		public virtual void FindSingle_SymptomCategory_Test_Returns_Null_When_SymptomCategory_Is_Not_Found_In_Repository()
		{
			//Arrange
			ISymptomCategory actual;
			ISymptomCategorySearch injected = this.search.Object;
			
			this.repository.Setup(r => r.Select(It.IsAny<ISymptomCategorySearch>())).Returns((ISymptomCategory)null);
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNull(actual);
		}
		
		[TestMethod]
		public virtual void FindSingle_SymptomCategory_Test_Returns_SymptomCategory_When_Is_Found_In_Repository()
		{
			//Arrange
			ISymptomCategory actual;
			ISymptomCategorySearch injected = this.search.Object;
			
			
			this.repository.Setup(r => r.Select(It.IsAny<ISymptomCategorySearch>())).Returns(this.symptomCategory.Object);
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		#endregion Find Single
		
		#region Save
		
		[TestMethod]
		public virtual void Save_Multiple_SymptomCategories_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			List<ISymptomCategory> injected = this.symptomCategories.Select(e => e.Object).ToList();
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(new List<ISymptomCategoryKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<ISymptomCategoryKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<ISymptomCategoryKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_Multiple_SymptomCategories_Test_Calls_Insert_On_Repository_For_New_SymptomCategories()
		{
			//Arrange
			List<ISymptomCategory> injected = this.symptomCategories.Select(e => e.Object).ToList();
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(new List<ISymptomCategoryKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<ISymptomCategoryKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.repository.Verify(r => r.Insert(It.IsAny<IEnumerable<ISymptomCategory>>(), It.IsAny<IDbTransaction>()));
		}
		
		[TestMethod]
		public virtual void Save_Multiple_SymptomCategories_Test_Calls_Update_On_Repository_For_Existing_SymptomCategories()
		{
			//Arrange
			List<ISymptomCategory> injected = this.symptomCategories.Select(e => e.Object).ToList();
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.repository.Verify(r => r.Update(It.IsAny<IEnumerable<ISymptomCategory>>(), It.IsAny<IDbTransaction>()));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Save_Multiple_SymptomCategories_Test_Throws_Error_When_Any_SymptomCategory_Is_Null()
		{
			//Arrange
			List<ISymptomCategory> injected = new List<ISymptomCategory>() { new Mock<ISymptomCategory>().Object, null, new Mock<ISymptomCategory>().Object };
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Save_SymptomCategory_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			Mock<ISymptomCategory> expected = this.symptomCategory;
			ISymptomCategory injected = this.symptomCategory.Object;
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(new List<ISymptomCategoryKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<ISymptomCategoryKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<ISymptomCategoryKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_SymptomCategory_Test_Calls_Insert_On_Repository_For_New_SymptomCategories()
		{
			//Arrange
			Mock<ISymptomCategory> expected = this.symptomCategory;
			ISymptomCategory injected = this.symptomCategory.Object;
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(new List<ISymptomCategoryKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<ISymptomCategoryKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Insert(It.IsAny<ISymptomCategory>(), It.IsAny<IDbTransaction>())); }
			catch { this.repository.Verify(r => r.Insert(It.IsAny<IEnumerable<ISymptomCategory>>(), It.IsAny<IDbTransaction>())); }
		}
		
		[TestMethod]
		public virtual void Save_SymptomCategory_Test_Calls_Update_On_Repository_For_Existing_SymptomCategories()
		{
			//Arrange
			Mock<ISymptomCategory> expected = this.symptomCategory;
			ISymptomCategory injected = this.symptomCategory.Object;
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Update(It.IsAny<ISymptomCategory>(), It.IsAny<IDbTransaction>())); }
			catch { this.repository.Verify(r => r.Update(It.IsAny<IEnumerable<ISymptomCategory>>(), It.IsAny<IDbTransaction>())); }
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Save_SymptomCategory_Test_Throws_Error_When_SymptomCategory_Is_Null()
		{
			//Arrange
			ISymptomCategory injected = null;
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Save
	}
}
