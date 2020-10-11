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
	public abstract class ISymptomServiceTest
	{
		protected ISymptom initial;
		protected IModelFactory modelFactory;
		protected ISymptomsSearch searchMultipleThatFindsNothing;
		protected ISymptomsSearch searchMultipleThatFindsSomething;
		protected ISymptomSearch searchThatReturnsNull;
		protected ISymptomSearch searchThatReturnsSomething;
		protected ISymptomService service;
		protected ISymptom symptom;
		protected ISymptom symptomNotFound;
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
			if(this.service.Exists(this.symptomNotFound.Key))
				this.service.Delete(this.symptomNotFound.Key, null);
			
			if(this.initial != null)
				this.service.Save(this.initial, null);
		}
		
		public virtual ISymptomSearch PrepareSearch(ISymptom symptom)
		{
			throw new NotImplementedException();
		}
		
		#region Delete
		
		#endregion Delete
		
		#region Exists
		
		[TestMethod]
		public virtual void Exists_Symptom_Test_Returns_False_When_Symptom_Is_Not_Found()
		{
			//Arrange
			bool actual;
			bool expected = false;
			ISymptomKey injected = this.symptomNotFound.Key;
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Symptom_Test_Returns_True_When_Symptom_Is_Found()
		{
			if(this.symptom == null)
				throw new AssertInconclusiveException("You must provide a Symptom that is already in the repository for this test to be effective.");
			
			//Arrange
			bool actual;
			bool expected = true;
			ISymptomKey injected = this.symptom.Key;
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Symptoms_Test_Does_Not_Return_Null_List_When_Nothing_Matches()
		{
			//Arrange
			IEnumerable<ISymptomKey> actual = null;
			IEnumerable<ISymptomKey> injected = new List<ISymptomKey>() { this.symptomNotFound.Key };
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void Exists_Symptoms_Test_Returns_Empty_List_When_Nothing_Matches()
		{
			//Arrange
			int actual;
			int expected = 0;
			IEnumerable<ISymptomKey> injected = new List<ISymptomKey>() { this.symptomNotFound.Key };
			
			//Act
			actual = this.service.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Symptoms_Test_Returns_List_Of_SymptomKeys_That_Match()
		{
			if(this.symptom == null)
				throw new AssertInconclusiveException("You must provide a Symptom that is already in the repository for this test to be effective.");
			
			//Arrange
			IEnumerable<ISymptomKey> actual = null;
			IEnumerable<ISymptomKey> injected = new List<ISymptomKey>() { this.symptom.Key };
			IEnumerable<ISymptomKey> expected = injected;
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected.Count(), actual.Count());
			Assert.IsTrue(expected.All(_e => actual.Any(_a => _e.Equals(_a))));
			Assert.IsTrue(actual.All(_a => expected.Any(_e => _e.Equals(_a))));
		}
		
		#endregion Exists
		
		#region Find
		
		[TestMethod]
		public virtual void Find_Symptoms_Test_Does_Not_Return_Null_List_When_Nothing_Matches()
		{
			//Arrange
			IEnumerable<ISymptom> actual;
			ISymptomsSearch injected = this.searchMultipleThatFindsNothing;
			
			//Act
			actual = this.service.Find(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void Find_Symptoms_Test_Returns_Empty_List_When_Nothing_Matches()
		{
			//Arrange
			int actual;
			int expected = 0;
			ISymptomsSearch injected = this.searchMultipleThatFindsNothing;
			
			//Act
			actual = this.service.Find(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Find_Symptoms_Test_Throws_Error_When_Search_Is_Null()
		{
			//Arrange
			ISymptomsSearch injected = null;
			
			//Act
			this.service.Find(injected);
			
			//Assert
		}
		
		#endregion Find
		
		#region Find Single
		
		[TestMethod]
		public virtual void FindSingle_Symptom_Test_Returns_Model_When_Found()
		{
			if(this.symptom == null)
				throw new AssertInconclusiveException("You must provide a Symptom that is already in the repository for this test to be effective.");
			
			//Arrange
			ISymptom actual;
			ISymptomSearch injected = this.searchThatReturnsSomething;
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void FindSingle_Symptom_Test_Returns_Null_When_Not_Found()
		{
			//Arrange
			ISymptom actual;
			ISymptomSearch injected = this.searchThatReturnsNull;
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNull(actual);
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void FindSingle_Symptom_Test_Throws_Error_When_Search_Is_Null()
		{
			//Arrange
			ISymptomSearch injected = null;
			
			//Act
			this.service.FindSingle(injected);
			
			//Assert
		}
		
		#endregion Find Single
		
		#region Save
		
		[TestMethod]
		public virtual void Save_Multiple_Symptoms_Test_Preserves_Name_When_Is_Maxed_And_Symptoms_Already_Exists()
		{
			if(this.symptom == null)
				throw new AssertInconclusiveException("You must provide a Symptom that is already in the repository for this test to be effective.");
			
			//Arrange
			IEnumerable<ISymptom> injected = new List<ISymptom>() { this.symptom };
			//                   .........10........20........30........40........50........60
			this.symptom.Name = "mYxInKYpWkyRoWAZEvkUzYvfQrxgIrBFrAYPmNbEgLCViFbBbx";
			
			ISymptom actual = null;
			ISymptom expected = this.symptom;
			
			ISymptomSearch search = PrepareSearch(expected);
			if(search == null)
				throw new AssertInconclusiveException("You must override the function that prepares a Symptom Search model for this test to be effective.");
			
			this.initial = this.service.FindSingle(this.searchThatReturnsSomething);
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			actual = this.service.FindSingle(search);
			
			Assert.AreEqual(expected.Name, actual.Name);
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Symptoms_Test_Preserves_Symptoms_When_They_Already_Exist()
		{
			if(this.symptom == null)
				throw new AssertInconclusiveException("You must provide a Symptom that is already in the repository for this test to be effective.");
			
			//Arrange
			List<ISymptom> injected = new List<ISymptom>() { this.symptom };
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Symptoms_Test_Preserves_Symptoms_When_They_Are_New()
		{
			if(this.symptom == null)
				throw new AssertInconclusiveException("You must provide a Symptom that is already in the repository for this test to be effective.");
			
			//Arrange
			List<ISymptom> injected = new List<ISymptom>() { this.symptomNotFound };
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			ISymptom expected = injected.First();
			ISymptomSearch search = PrepareSearch(expected);
			if(search != null)
			{
				ISymptom actual = this.symptomNotFound = this.service.FindSingle(search);
			
				Assert.IsNotNull(actual);
				Assert.AreEqual(expected.Name, actual.Name, "Name");
			}
		}
		
		[TestMethod]
		public virtual void Save_Symptom_Test_Preserves_Name_When_Is_Maxed_And_Symptoms_Already_Exists()
		{
			if(this.symptom == null)
				throw new AssertInconclusiveException("You must provide a Symptom that is already in the repository for this test to be effective.");
			
			//Arrange
			ISymptom injected = this.symptom;
			//               .........10........20........30........40........50........60
			injected.Name = "YkrKXnCsSWnRGFJSAOYKMRtGONwzXlpVflsqnHDLmUUfrpeWmp";
			
			ISymptom actual = null;
			ISymptom expected = injected;
			
			ISymptomSearch search = PrepareSearch(expected);
			if(search == null)
				throw new AssertInconclusiveException("You must override the function that prepares a Symptom Search model for this test to be effective.");
			
			this.initial = this.service.FindSingle(this.searchThatReturnsSomething);
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			actual = this.service.FindSingle(search);
			
			Assert.AreEqual(expected.Name, actual.Name);
		}
		
		[TestMethod]
		public virtual void Save_Symptom_Test_Preserves_Symptom_When_It_Already_Exists()
		{
			if(this.symptom == null)
				throw new AssertInconclusiveException("You must provide a Symptom that is already in the repository for this test to be effective.");
			
			//Arrange
			ISymptom injected = this.symptom;
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Save_Symptom_Test_Preserves_Symptom_When_It_Is_New()
		{
			if(this.symptom == null)
				throw new AssertInconclusiveException("You must provide a Symptom that is already in the repository for this test to be effective.");
			
			//Arrange
			ISymptom injected = this.symptomNotFound;
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			ISymptom expected = injected;
			ISymptomSearch search = PrepareSearch(expected);
			if(search != null)
			{
				ISymptom actual = this.symptomNotFound = this.service.FindSingle(search);
			
				Assert.IsNotNull(actual);
				Assert.AreEqual(expected.Name, actual.Name, "Name");
			}
		}
		
		#endregion Save
	}
}
