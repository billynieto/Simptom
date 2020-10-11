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
	public abstract class ISymptomServiceTest
	{
		protected Mock<ISymptomKey> key;
		protected IList<Mock<ISymptomKey>> keys;
		protected Mock<IModelFactory> modelFactory;
		protected Mock<ISymptomRepository> repository;
		protected Mock<ISymptomSearch> search;
		protected Mock<ISymptomsSearch> searchMultiple;
		protected ISymptomService service;
		protected ConnectionState state;
		protected Mock<ISymptom> symptom;
		protected IList<Mock<ISymptomCategory>> symptomCategories;
		protected Mock<ISymptomCategoriesSearch> symptomCategoriesSearch;
		protected Mock<ISymptomCategory> symptomCategory;
		protected Mock<ISymptomCategoryKey> symptomCategoryKey;
		protected IList<Mock<ISymptomCategoryKey>> symptomCategoryKeys;
		protected Mock<ISymptomCategoryService> symptomCategoryService;
		protected IList<Mock<ISymptom>> symptoms;
		protected TestContext testContext;
		protected Mock<IDbTransaction> transaction;
		
		public TestContext TestContext { get { return this.testContext; } set { this.testContext = value; } }
		
		[TestInitialize]
		public virtual void TestInitialize()
		{
			this.symptomCategoryKey = new Mock<ISymptomCategoryKey>();
			this.symptomCategoryKey.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.symptomCategoryKeys = new List<Mock<ISymptomCategoryKey>>() { this.symptomCategoryKey };
			this.symptomCategory = new Mock<ISymptomCategory>();
			this.symptomCategory.SetupAllProperties();
			this.symptomCategory.Setup(_model => _model.Key).Returns(() => { return this.symptomCategoryKey.Object; });
			this.symptomCategory.Setup(_model => _model.Name).Returns("HtmxAOsxzGiOxCUAMLPrWvkTQGAAFdfRZbRli");
			this.symptomCategories = new List<Mock<ISymptomCategory>> { this.symptomCategory };
			this.symptomCategoryService = new Mock<ISymptomCategoryService>();
			this.symptomCategoryService.Setup(_service => _service.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(() => { return this.symptomCategoryKeys.Select(_symptomCategoryKey => _symptomCategoryKey.Object); });
			this.symptomCategoriesSearch = new Mock<ISymptomCategoriesSearch>();
			
			this.key = new Mock<ISymptomKey>();
			this.key.SetupAllProperties();
			this.key.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.keys = new List<Mock<ISymptomKey>>() { this.key };
			this.search = new Mock<ISymptomSearch>();
			this.searchMultiple = new Mock<ISymptomsSearch>();
			
			this.symptom = new Mock<ISymptom>();
			this.symptom.SetupAllProperties();
			this.symptom.Setup(m => m.Key).Returns(this.key.Object);
			this.symptom.Setup(m => m.Category).Returns(() => { return this.symptomCategory.Object; });
			this.symptoms = new List<Mock<ISymptom>>() { this.symptom };
			
			this.modelFactory = new Mock<IModelFactory>();
			this.modelFactory.Setup(_modelFactory => _modelFactory.GenerateSymptomCategoriesSearch()).Returns(() => { return this.symptomCategoriesSearch.Object; });
			
			this.state = ConnectionState.Closed;
			this.repository = new Mock<ISymptomRepository>();
			this.repository.Setup(_repository => _repository.Open()).Callback(() => { this.state = ConnectionState.Open; });
			this.repository.Setup(_repository => _repository.Close()).Callback(() => { this.state = ConnectionState.Closed; });
			this.repository.Setup(_repository => _repository.IsOpen).Returns(() => { return this.state == ConnectionState.Open; });
			this.transaction = new Mock<IDbTransaction>();
		}
		
		[TestCleanup]
		public virtual void TestCleanup()
		{
			this.symptomCategoryKey = null;
			this.symptomCategory = null;
			this.symptomCategories = null;
			
			this.repository = null;
		}
		
		#region Delete
		
		[TestMethod]
		public virtual void Delete_Multiple_Symptoms_Test_Calls_Delete_On_Repository()
		{
			//Arrange
			IList<ISymptomKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Delete(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>()), Times.Exactly(symptoms.Count)); }
			catch { this.repository.Verify(r => r.Delete(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>()), Times.Exactly(1)); }
		}
		
		[TestMethod]
		public virtual void Delete_Multiple_Symptoms_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			IList<ISymptomKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<ISymptomKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Delete_Multiple_Symptoms_Test_Does_Not_Call_Delete_On_SymptomCategoryService()
		{
			//Arrange
			IList<ISymptomKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			this.symptomCategoryService.Verify(s => s.Delete(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.symptomCategoryService.Verify(s => s.Delete(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_Multiple_Symptoms_Test_Throws_Error_When_List_Of_SymptomKey_Is_Null()
		{
			//Arrange
			IList<ISymptomKey> injected = null;
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public virtual void Delete_Multiple_Symptoms_Test_Throws_Error_When_SymptomKey_Is_Not_Found_In_Repository()
		{
			//Arrange
			IList<ISymptomKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(new List<ISymptomKey>());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<ISymptomKey>());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Delete_Symptom_Test_Calls_Delete_On_Repository()
		{
			//Arrange
			ISymptomKey injected = symptom.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Delete(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())); }
			catch { this.repository.Verify(r => r.Delete(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())); }
		}
		
		[TestMethod]
		public virtual void Delete_Symptom_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			ISymptomKey injected = symptom.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<ISymptomKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Delete_Symptom_Test_Does_Not_Call_Delete_On_SymptomCategoryService()
		{
			//Arrange
			ISymptomKey injected = symptom.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
			this.symptomCategoryService.Verify(s => s.Delete(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.symptomCategoryService.Verify(s => s.Delete(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_Symptom_Test_Throws_Error_When_SymptomKey_Is_Null()
		{
			//Arrange
			ISymptomKey injected = null;
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public virtual void Delete_Symptom_Test_Throws_Error_When_SymptomKey_Not_Found_In_Repository()
		{
			//Arrange
			ISymptomKey injected = symptom.Object.Key;
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(new List<ISymptomKey>());
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<ISymptomKey>());
			
			//Act
			this.service.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Delete
		
		#region Exists
		
		[TestMethod]
		public virtual void Exists_Symptom_Test_Returns_False_When_Key_Was_Not_Found()
		{
			//Arrange
			bool actual;
			bool expected = false;
			ISymptomKey injected = this.key.Object;
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(new List<ISymptomKey>());
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Symptom_Test_Returns_True_When_Key_Was_Found()
		{
			//Arrange
			bool actual;
			bool expected = true;
			ISymptomKey injected = this.key.Object;
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Symptoms_Test_Returns_Empty_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			int actual;
			int expected = 0;
			IEnumerable<ISymptomKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(new List<ISymptomKey>());
			
			//Act
			actual = this.service.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Symptoms_Test_Returns_List_Of_Keys_When_They_Were_Found()
		{
			//Arrange
			int actual;
			int expected = 1;
			IEnumerable<ISymptomKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomKey>())).Returns(true);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(this.keys.Select(_key => _key.Object).ToList());
			
			//Act
			actual = this.service.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Symptoms_Test_Returns_Non_Null_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			IEnumerable<ISymptomKey> actual;
			IEnumerable<ISymptomKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			this.repository.Setup(r => r.Exists(It.IsAny<ISymptomKey>())).Returns(false);
			this.repository.Setup(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(new List<ISymptomKey>());
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		#endregion Exists
		
		#region Find
		
		[TestMethod]
		public virtual void Find_Symptoms_Test_Calls_Select_On_Repository()
		{
			//Arrange
			ISymptomsSearch injected = this.searchMultiple.Object;
			
			this.repository.Setup(r => r.Select(It.IsAny<ISymptomsSearch>())).Returns(new List<ISymptom>());
			
			//Act
			this.service.Find(injected);
			
			//Assert
			this.repository.Verify(r => r.Select(It.IsAny<ISymptomsSearch>()), Times.Exactly(1));
		}
		
		[TestMethod]
		public virtual void Find_Symptoms_Test_Calls_Validate_On_Search()
		{
			//Arrange
			Mock<ISymptomsSearch> expected = this.searchMultiple;
			ISymptomsSearch injected = this.searchMultiple.Object;
			
			this.repository.Setup(r => r.Select(It.IsAny<ISymptomsSearch>())).Returns(new List<ISymptom>());
			
			//Act
			this.service.Find(injected);
			
			//Assert
			expected.Verify(r => r.Validate());
		}
		
		[TestMethod]
		public virtual void Find_Symptoms_Test_Does_Not_Call_Find_On_ISymptomCategory_Service()
		{
			//Arrange
			ISymptomsSearch injected = this.searchMultiple.Object;
			
			//Act
			this.service.Find(injected);
			
			//Assert
			this.symptomCategoryService.Verify(r => r.Find(It.IsAny<ISymptomCategoriesSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Find_Symptoms_Test_Does_Not_Call_FindSingle_On_ISymptomCategory_Service()
		{
			//Arrange
			ISymptomsSearch injected = this.searchMultiple.Object;
			
			//Act
			this.service.Find(injected);
			
			//Assert
			this.symptomCategoryService.Verify(r => r.FindSingle(It.IsAny<ISymptomCategorySearch>()), Times.Never());
		}
		
		#endregion Find
		
		#region Find Single
		
		[TestMethod]
		public virtual void FindSingle_Symptom_Test_Calls_Select_On_Repository()
		{
			//Arrange
			ISymptomSearch injected = this.search.Object;
			
			
			this.repository.Setup(r => r.Select(It.IsAny<ISymptomSearch>())).Returns(this.symptom.Object);
			
			//Act
			this.service.FindSingle(injected);
			
			//Assert
			this.repository.Verify(r => r.Select(It.IsAny<ISymptomSearch>()), Times.Exactly(1));
		}
		
		[TestMethod]
		public virtual void FindSingle_Symptom_Test_Calls_Validate_On_Search()
		{
			//Arrange
			Mock<ISymptomSearch> expected = this.search;
			ISymptomSearch injected = this.search.Object;
			
			
			this.repository.Setup(r => r.Select(It.IsAny<ISymptomSearch>())).Returns(this.symptom.Object);
			
			//Act
			this.service.FindSingle(injected);
			
			//Assert
			expected.Verify(e => e.Validate());
		}
		
		[TestMethod]
		public virtual void FindSingle_Symptom_Test_Returns_Null_When_Symptom_Is_Not_Found_In_Repository()
		{
			//Arrange
			ISymptom actual;
			ISymptomSearch injected = this.search.Object;
			
			this.repository.Setup(r => r.Select(It.IsAny<ISymptomSearch>())).Returns((ISymptom)null);
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNull(actual);
		}
		
		[TestMethod]
		public virtual void FindSingle_Symptom_Test_Returns_Symptom_When_Is_Found_In_Repository()
		{
			//Arrange
			ISymptom actual;
			ISymptomSearch injected = this.search.Object;
			
			
			this.repository.Setup(r => r.Select(It.IsAny<ISymptomSearch>())).Returns(this.symptom.Object);
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		#endregion Find Single
		
		#region Save
		
		[TestMethod]
		public virtual void Save_Multiple_Symptoms_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			List<ISymptom> injected = this.symptoms.Select(e => e.Object).ToList();
			
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(new List<ISymptomKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<ISymptomKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<ISymptomKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Symptoms_Test_Calls_Exists_On_SymptomCategoryService()
		{
			//Arrange
			List<ISymptom> injected = this.symptoms.Select(e => e.Object).ToList();
			
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.symptomCategoryService.Verify(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())); }
			catch {
				try { this.symptomCategoryService.Verify(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.symptomCategoryService.Verify(s => s.Exists(It.IsAny<ISymptomCategoryKey>())); }
					catch { this.symptomCategoryService.Verify(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Symptoms_Test_Calls_Insert_On_Repository_For_New_Symptoms()
		{
			//Arrange
			List<ISymptom> injected = this.symptoms.Select(e => e.Object).ToList();
			
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(new List<ISymptomKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<ISymptomKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.repository.Verify(r => r.Insert(It.IsAny<IEnumerable<ISymptom>>(), It.IsAny<IDbTransaction>()));
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Symptoms_Test_Calls_Update_On_Repository_For_Existing_Symptoms()
		{
			//Arrange
			List<ISymptom> injected = this.symptoms.Select(e => e.Object).ToList();
			
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.repository.Verify(r => r.Update(It.IsAny<IEnumerable<ISymptom>>(), It.IsAny<IDbTransaction>()));
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Symptoms_Test_Does_Not_Call_Delete_On_SymptomCategoryService()
		{
			//Arrange
			IList<Mock<ISymptom>> expected = new List<Mock<ISymptom>>() { this.symptom };
			List<ISymptom> injected = this.symptoms.Select(e => e.Object).ToList();
			
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.symptomCategoryService.Verify(s => s.Delete(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.symptomCategoryService.Verify(s => s.Delete(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Symptoms_Test_Does_Not_Call_Find_On_SymptomCategoryService()
		{
			//Arrange
			IList<Mock<ISymptom>> expected = new List<Mock<ISymptom>>() { this.symptom };
			List<ISymptom> injected = this.symptoms.Select(e => e.Object).ToList();
			
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.symptomCategoryService.Verify(s => s.Find(It.IsAny<ISymptomCategoriesSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Symptoms_Test_Does_Not_Call_FindSingle_On_SymptomCategoryService()
		{
			//Arrange
			IList<Mock<ISymptom>> expected = new List<Mock<ISymptom>>() { this.symptom };
			List<ISymptom> injected = this.symptoms.Select(e => e.Object).ToList();
			
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.symptomCategoryService.Verify(s => s.FindSingle(It.IsAny<ISymptomCategorySearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Symptoms_Test_Does_Not_Call_Save_On_SymptomCategoryService()
		{
			//Arrange
			IList<Mock<ISymptom>> expected = new List<Mock<ISymptom>>() { this.symptom };
			List<ISymptom> injected = this.symptoms.Select(e => e.Object).ToList();
			
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.symptomCategoryService.Verify(s => s.Save(It.IsAny<ISymptomCategory>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.symptomCategoryService.Verify(s => s.Save(It.IsAny<IEnumerable<ISymptomCategory>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Save_Multiple_Symptoms_Test_Throws_Error_When_Any_Symptom_Is_Null()
		{
			//Arrange
			List<ISymptom> injected = new List<ISymptom>() { new Mock<ISymptom>().Object, null, new Mock<ISymptom>().Object };
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Save_Symptom_Test_Calls_Exists_On_Repository()
		{
			//Arrange
			Mock<ISymptom> expected = this.symptom;
			ISymptom injected = this.symptom.Object;
			
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(new List<ISymptomKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<ISymptomKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Exists(It.IsAny<ISymptomKey>())); }
			catch {
				try { this.repository.Verify(r => r.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>())); }
					catch { this.repository.Verify(r => r.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_Symptom_Test_Calls_Exists_On_SymptomCategoryService()
		{
			//Arrange
			Mock<ISymptom> expected = this.symptom;
			ISymptom injected = this.symptom.Object;
			
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(new List<ISymptomKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<ISymptomKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.symptomCategoryService.Verify(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())); }
			catch {
				try { this.symptomCategoryService.Verify(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())); }
				catch {
					try { this.symptomCategoryService.Verify(s => s.Exists(It.IsAny<ISymptomCategoryKey>())); }
					catch { this.symptomCategoryService.Verify(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())); }
				}
			}
		}
		
		[TestMethod]
		public virtual void Save_Symptom_Test_Calls_Insert_On_Repository_For_New_Symptoms()
		{
			//Arrange
			Mock<ISymptom> expected = this.symptom;
			ISymptom injected = this.symptom.Object;
			
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(false);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(new List<ISymptomKey>());
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(new List<ISymptomKey>());
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Insert(It.IsAny<ISymptom>(), It.IsAny<IDbTransaction>())); }
			catch { this.repository.Verify(r => r.Insert(It.IsAny<IEnumerable<ISymptom>>(), It.IsAny<IDbTransaction>())); }
		}
		
		[TestMethod]
		public virtual void Save_Symptom_Test_Calls_Update_On_Repository_For_Existing_Symptoms()
		{
			//Arrange
			Mock<ISymptom> expected = this.symptom;
			ISymptom injected = this.symptom.Object;
			
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			try { this.repository.Verify(r => r.Update(It.IsAny<ISymptom>(), It.IsAny<IDbTransaction>())); }
			catch { this.repository.Verify(r => r.Update(It.IsAny<IEnumerable<ISymptom>>(), It.IsAny<IDbTransaction>())); }
		}
		
		[TestMethod]
		public virtual void Save_Symptom_Test_Does_Not_Call_Delete_On_SymptomCategoryService()
		{
			//Arrange
			Mock<ISymptom> expected = this.symptom;
			ISymptom injected = this.symptom.Object;
			
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.symptomCategoryService.Verify(s => s.Delete(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>()), Times.Never());
			this.symptomCategoryService.Verify(s => s.Delete(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Symptom_Test_Does_Not_Call_Find_On_SymptomCategoryService()
		{
			//Arrange
			Mock<ISymptom> expected = this.symptom;
			ISymptom injected = expected.Object;
			
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.symptomCategoryService.Verify(s => s.Find(It.IsAny<ISymptomCategoriesSearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Symptom_Test_Does_Not_Call_FindSingle_On_SymptomCategoryService()
		{
			//Arrange
			Mock<ISymptom> expected = this.symptom;
			ISymptom injected = expected.Object;
			
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.symptomCategoryService.Verify(s => s.FindSingle(It.IsAny<ISymptomCategorySearch>()), Times.Never());
		}
		
		[TestMethod]
		public virtual void Save_Symptom_Test_Does_Not_Call_Save_On_SymptomCategoryService()
		{
			//Arrange
			Mock<ISymptom> expected = this.symptom;
			ISymptom injected = this.symptom.Object;
			
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<ISymptomCategoryKey>(), It.IsAny<IDbTransaction>())).Returns(() => { return true; });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			this.symptomCategoryService.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomCategoryKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.symptomCategoryKeys.Select(_key => _key.Object); });
			
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<ISymptomKey>(), It.IsAny<IDbTransaction>())).Returns(true);
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			this.repository.Setup(s => s.Exists(It.IsAny<IEnumerable<ISymptomKey>>(), It.IsAny<IDbTransaction>())).Returns(() => { return this.keys.Select(_key => _key.Object); });
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
			this.symptomCategoryService.Verify(s => s.Save(It.IsAny<IEnumerable<ISymptomCategory>>(), It.IsAny<IDbTransaction>()), Times.Never());
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Save_Symptom_Test_Throws_Error_When_Symptom_Is_Null()
		{
			//Arrange
			ISymptom injected = null;
			
			//Act
			this.service.Save(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Save
	}
}
