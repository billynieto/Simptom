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
	public abstract class ISymptomRepositoryTest
	{
		protected Mock<ISymptomKey> key;
		protected IList<Mock<ISymptomKey>> keys;
		protected Mock<IModelFactory> modelFactory;
		protected ISymptomRepository repository;
		protected Mock<ISymptomSearch> search;
		protected Mock<ISymptomsSearch> searchMultiple;
		protected Mock<ISymptom> symptom;
		protected IList<Mock<ISymptomCategory>> symptomCategories;
		protected Mock<ISymptomCategory> symptomCategory;
		protected Mock<ISymptomCategoryKey> symptomCategoryKey;
		protected IList<Mock<ISymptom>> symptoms;
		protected TestContext testContext;
		protected Mock<IDbTransaction> transaction;
		
		public TestContext TestContext { get { return this.testContext; } set { this.testContext = value; } }
		
		[TestInitialize]
		public virtual void TestInitialize()
		{
			this.symptomCategoryKey = new Mock<ISymptomCategoryKey>();
			this.symptomCategoryKey.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.symptomCategory = new Mock<ISymptomCategory>();
			this.symptomCategory.SetupAllProperties();
			this.symptomCategory.Setup(_model => _model.Key).Returns(() => { return this.symptomCategoryKey.Object; });
			this.symptomCategory.Setup(_symptomCategory => _symptomCategory.Name).Returns("geANMAPVEOBpqhiKXyvFhgbBW");
			this.symptomCategories = new List<Mock<ISymptomCategory>> { this.symptomCategory };
			
			this.key = new Mock<ISymptomKey>();
			this.key.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.keys = new List<Mock<ISymptomKey>>() { this.key };
			
			this.symptom = new Mock<ISymptom>();
			this.symptom.SetupAllProperties();
			this.symptom.Setup(_model => _model.Key).Returns(() => { return this.key.Object; });
			this.symptom.Setup(_model => _model.Category).Returns(() => { return this.symptomCategory.Object; });
			this.symptom.Setup(_model => _model.Name).Returns("LTLMbfGfU");
			this.symptoms = new List<Mock<ISymptom>>() { symptom };
			
			this.modelFactory = new Mock<IModelFactory>();
			this.modelFactory.Setup(mf => mf.GenerateSymptomCategoryKey(It.IsAny<Guid>())).Returns(this.symptomCategoryKey.Object);
			this.modelFactory.Setup(mf => mf.GenerateSymptomCategory(It.IsAny<ISymptomCategoryKey>())).Returns(this.symptomCategory.Object);
			this.modelFactory.Setup(mf => mf.GenerateSymptomKey(It.IsAny<Guid>())).Returns(this.key.Object);
			this.modelFactory.Setup(mf => mf.GenerateSymptom(It.IsAny<ISymptomKey>())).Returns(this.symptom.Object);
			
			this.search = new Mock<ISymptomSearch>();
			this.searchMultiple = new Mock<ISymptomsSearch>();
			this.transaction = new Mock<IDbTransaction>();
		}
		
		[TestCleanup]
		public virtual void TestCleanup()
		{
			this.symptomCategoryKey = null;
			this.symptomCategory = null;
			this.symptomCategories = null;
			this.transaction = null;
		}
		
		public virtual void AddRow(ISymptom symptom)
		{
			throw new NotImplementedException();
		}
		
		public virtual void AddRows(IEnumerable<ISymptom> symptoms)
		{
			foreach(ISymptom symptom in symptoms)
				AddRow(symptom);
		}
		
		#region Delete
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_Multiple_Symptoms_Test_Throws_Error_When_Any_Key_Is_Null()
		{
			//Arrange
			IList<ISymptomKey> injected = new List<ISymptomKey>() { new Mock<ISymptomKey>().Object, null };
			
			//Act
			this.repository.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_Symptom_Test_Throws_Error_When_Key_Is_Null()
		{
			//Arrange
			ISymptomKey injected = null;
			
			//Act
			this.repository.Delete(injected, this.transaction.Object);
			
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
			
			//Act
			actual = this.repository.Exists(injected);
			
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
			
			AddRow(this.symptom.Object);
			
			//Act
			actual = this.repository.Exists(injected);
			
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
			
			//Act
			actual = this.repository.Exists(injected).Count();
			
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
			
			AddRow(this.symptom.Object);
			
			//Act
			actual = this.repository.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Symptoms_Test_Returns_Non_Null_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			IEnumerable<ISymptomKey> actual;
			IEnumerable<ISymptomKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			//Act
			actual = this.repository.Exists(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		#endregion Exists
		
		#region Insert
		
		[TestMethod]
		public virtual void Insert_Multiple_Symptoms_Test_Gets_Keys_For_Symptom_Category()
		{
			//Arrange
			IList<Mock<ISymptomCategory>> expected = this.symptomCategories;
			IList<ISymptom> injected = this.symptoms.Select(e => e.Object).ToList();
			
			//Act
			this.repository.Insert(injected, this.transaction.Object );
			
			//Assert
			foreach(Mock<ISymptomCategory> symptomCategory in expected)
				symptomCategory.VerifyGet(_symptomCategory => _symptomCategory.Key);
		}
		
		[TestMethod]
		public virtual void Insert_Multiple_Symptoms_Test_Gets_Name_For_Every_Symptom()
		{
			//Arrange
			IList<Mock<ISymptom>> expected = new List<Mock<ISymptom>>() { this.symptom };
			IList<ISymptom> injected = this.symptoms.Select(e => e.Object).ToList();
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<ISymptom> symptom in expected)
				symptom.VerifyGet(_symptom => _symptom.Name, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Insert_Multiple_Symptoms_Test_Throws_Error_When_Any_Symptom_Is_Null()
		{
			//Arrange
			IEnumerable<ISymptom> injected = new List<ISymptom>() { this.symptom.Object, null };
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Insert_Multiple_Symptoms_Test_Throws_Error_When_Symptom_Is_Null()
		{
			//Arrange
			IEnumerable<ISymptom> injected = null;
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Insert_Symptom_Test_Gets_Symptom_Category_Key()
		{
			//Arrange
			Mock<ISymptomCategory> expected = this.symptomCategory;
			Mock<ISymptom> injected = this.symptom;
			
			//Act
			this.repository.Insert(injected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Insert_Symptom_Test_Gets_Symptom_Name()
		{
			//Arrange
			Mock<ISymptom> expected = this.symptom;
			
			//Act
			this.repository.Insert(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Name, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Insert_Symptom_Test_Throws_Error_When_Symptom_Is_Null()
		{
			//Arrange
			ISymptom injected = null;
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Insert
		
		#region Select
		
		[TestMethod]
		public virtual void Select_Symptoms_Test_Calls_Generate_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<ISymptomsSearch> injected = this.searchMultiple;
			
			AddRow(this.symptom.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateSymptom(It.IsAny<ISymptomKey>()));
		}
		
		[TestMethod]
		public virtual void Select_Symptoms_Test_Calls_GenerateSymptomKey_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<ISymptomsSearch> injected = this.searchMultiple;
			
			AddRow(this.symptom.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateSymptomKey(It.IsAny<Guid>()));
		}
		
		#endregion Select
		
		#region Select Single
		
		[TestMethod]
		public virtual void SelectSingle_Symptom_Test_Calls_Generate_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<ISymptomSearch> injected = this.search;
			
			AddRow(this.symptom.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateSymptom(It.IsAny<ISymptomKey>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_Symptom_Test_Calls_Generate_On_ModelFactory_For_Category()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<ISymptomSearch> injected = this.search;
			
			AddRow(this.symptom.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateSymptomCategory(It.IsAny<ISymptomCategoryKey>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_Symptom_Test_Calls_GenerateSymptomCategoryKey_On_ModelFactory_For_Category()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<ISymptomSearch> injected = this.search;
			
			AddRow(this.symptom.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateSymptomCategoryKey(It.IsAny<Guid>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_Symptom_Test_Calls_GenerateSymptomKey_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<ISymptomSearch> injected = this.search;
			
			AddRow(this.symptom.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateSymptomKey(It.IsAny<Guid>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_Symptom_Test_Returns_Null_When_Symptom_Is_Not_Found()
		{
			//Arrange
			ISymptom actual;
			Mock<ISymptomSearch> injected = this.search;
			
			//Act
			actual = this.repository.Select(injected.Object);
			
			//Assert
			Assert.IsNull(actual);
		}
		
		[TestMethod]
		public virtual void SelectSingle_Symptom_Test_Returns_Symptom_When_Is_Found()
		{
			//Arrange
			ISymptom actual;
			Mock<ISymptom> expected = this.symptom;
			Mock<ISymptomSearch> injected = this.search;
			
			AddRow(expected.Object);
			
			//Act
			actual = this.repository.Select(injected.Object);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void SelectSingle_Symptom_Test_Sets_Symptom_Category_Properties()
		{
			//Arrange
			Mock<ISymptomCategory> expected = this.symptomCategory;
			Mock<ISymptomSearch> injected = this.search;
			
			AddRow(this.symptom.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.VerifySet(e => e.Name = It.IsAny<string>(), "SymptomCategory Name");
		}
		
		[TestMethod]
		public virtual void SelectSingle_Symptom_Test_Sets_Symptom_Name()
		{
			//Arrange
			Mock<ISymptom> expected = this.symptom;
			Mock<ISymptomSearch> injected = this.search;
			
			AddRow(expected.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.VerifySet(e => e.Name = It.IsAny<string>());
		}
		
		#endregion Select Single
		
		#region Update
		
		[TestMethod]
		public virtual void Update_Multiple_Symptoms_Test_Gets_Key_For_Symptom()
		{
			//Arrange
			IList<Mock<ISymptom>> expected = new List<Mock<ISymptom>>() { this.symptom };
			IList<ISymptom> injected = expected.Select(e => e.Object).ToList();
			
			AddRow(this.symptom.Object);
			
			//Act
			foreach(Mock<ISymptom> symptom in expected)
				symptom.Invocations.Clear();
			
			this.repository.Update(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<ISymptom> symptom in expected)
				symptom.VerifyGet(m => m.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Multiple_Symptoms_Test_Gets_Key_For_Symptom_Category()
		{
			//Arrange
			IList<Mock<ISymptomCategory>> expected = this.symptomCategories;
			IList<ISymptom> injected = this.symptoms.Select(_mock => _mock.Object).ToList();
			
			AddRow(this.symptom.Object);
			
			//Act
			foreach(Mock<ISymptomCategory> symptomCategory in expected)
				symptomCategory.Invocations.Clear();
			
			this.repository.Update(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<ISymptomCategory> symptomCategory in expected)
				symptomCategory.VerifyGet(m => m.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Multiple_Symptoms_Test_Gets_Name_For_Symptom()
		{
			//Arrange
			IList<Mock<ISymptom>> expected = new List<Mock<ISymptom>>() { this.symptom };
			
			AddRow(this.symptom.Object);
			
			//Act
			foreach(Mock<ISymptom> symptom in expected)
				symptom.Invocations.Clear();
			
			this.repository.Update(expected.Select(e => e.Object).ToList(), this.transaction.Object);
			
			//Assert
			foreach(Mock<ISymptom> symptom in expected)
				symptom.VerifyGet(m => m.Name, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Update_Multiple_Symptoms_Test_Throws_Error_When_Any_Symptom_Is_Null()
		{
			//Arrange
			IEnumerable<ISymptom> expected = new List<ISymptom>() { this.symptom.Object, null };
			
			//Act
			this.repository.Update(expected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Update_Multiple_Symptoms_Test_Throws_Error_When_Symptom_Is_Null()
		{
			//Arrange
			IEnumerable<ISymptom> expected = null;
			
			//Act
			this.repository.Update(expected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Update_Symptom_Test_Gets_Symptom_Category_Key()
		{
			//Arrange
			Mock<ISymptomCategory> expected = this.symptomCategory;
			
			AddRow(this.symptom.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(this.symptom.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Symptom_Test_Gets_Symptom_Key()
		{
			//Arrange
			Mock<ISymptom> expected = this.symptom;
			
			AddRow(expected.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Symptom_Test_Gets_Symptom_Name()
		{
			//Arrange
			Mock<ISymptom> expected = this.symptom;
			
			AddRow(expected.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Name, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Update_Symptom_Test_Throws_Error_When_Symptom_Is_Null()
		{
			//Arrange
			ISymptom injected = null;
			
			//Act
			this.repository.Update(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Update
	}
}
