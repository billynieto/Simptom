using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Simptom.Framework;
using Simptom.Framework.Models;

namespace Simptom.Test.Unit.Core.Models
{
	public abstract class ISymptomTest
	{
		protected Mock<ISymptomKey> key;
		protected IModelFactory modelFactory;
		protected ISymptom symptom;
		protected Mock<ISymptomCategory> symptomCategory;
		protected Mock<ISymptomCategoryKey> symptomCategoryKey;
		protected TestContext testContext;
		
		public TestContext TestContext { get { return this.testContext; } set { this.testContext = value; } }
		
		[TestCleanup]
		public virtual void TestCleanup()
		{
			this.symptomCategory = null;
			this.symptomCategoryKey = null;
			
			this.symptom = null;
			this.key = null;
		}
		
		[TestInitialize]
		public virtual void TestInitialize()
		{
			this.symptomCategory = new Mock<ISymptomCategory>();
			this.symptomCategory.SetupAllProperties();
			this.symptomCategory.Setup(_model => _model.Key).Returns(() => { return this.symptomCategoryKey.Object; });
			this.symptomCategoryKey = new Mock<ISymptomCategoryKey>();
			this.symptomCategoryKey.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			
			this.key = new Mock<ISymptomKey>();
			this.key.Setup(_key => _key.ID).Returns(Guid.NewGuid());
		}
		
		#region Equals
		
		[TestMethod]
		public virtual void Equals_SymptomKey_Test_Returns_False_When_Key_Does_Not_Match()
		{
			//Arrange
			bool actual;
			bool expected = false;
			ISymptomKey injected = this.modelFactory.GenerateSymptomKey(Guid.NewGuid());
			ISymptomKey worked = this.modelFactory.GenerateSymptomKey(Guid.NewGuid());
			
			this.symptomCategoryKey.Setup(_symptomCategoryKey => _symptomCategoryKey.Equals(It.IsAny<ISymptomCategoryKey>())).Returns(false);
			
			//Act
			actual = worked.Equals(injected);
			
			//Assert
			Assert.AreEqual(actual, expected);
		}
		
		[TestMethod]
		public virtual void Equals_SymptomKey_Test_Returns_True_When_Key_Matches()
		{
			//Arrange
			bool actual;
			bool expected = true;
			ISymptomKey injected = this.modelFactory.GenerateSymptomKey(Guid.NewGuid());
			
			this.symptomCategoryKey.Setup(_symptomCategoryKey => _symptomCategoryKey.Equals(It.IsAny<ISymptomCategoryKey>())).Returns(true);
			
			//Act
			actual = injected.Equals(injected);
			
			//Assert
			Assert.AreEqual(actual, expected);
		}
		
		#endregion Equals
		
		#region Category
		
		[TestMethod]
		public virtual void Validate_Symptom_Test_Category_Is_Null()
		{
			//Arrange
			ISymptom actual = this.modelFactory.GenerateSymptom(this.key.Object);
			
			actual.Category = null;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_Symptom_Test_Category_Is_Valid()
		{
			//Arrange
			ISymptom actual = this.modelFactory.GenerateSymptom(this.key.Object);
			Mock<ISymptomCategory> symptomCategory = new Mock<ISymptomCategory>();
			
			actual.Category = symptomCategory.Object;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		#endregion Category
		
		#region Name
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Validate_Symptom_Test_Name_Is_Null()
		{
			//Arrange
			ISymptom actual = this.modelFactory.GenerateSymptom(this.key.Object);
			
			actual.Name = null;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_Symptom_Test_Name_Maximum()
		{
			//Arrange
			ISymptom actual = this.modelFactory.GenerateSymptom(this.key.Object);
			
			//             .........10........20........30........40........50........60
			actual.Name = "roNvMjJmdlzeWMemzIZPyecYlhXIKsUKJNOSczqYqcvwcCPFyc";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public virtual void Validate_Symptom_Test_Name_Maximum_Plus_One()
		{
			//Arrange
			ISymptom actual = this.modelFactory.GenerateSymptom(this.key.Object);
			
			//             .........10........20........30........40........50........60
			actual.Name = "hGHnToZzCLEESCnUQUHOFMDvEVLGojVYCIBLJWNODJgUXdvMySy";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_Symptom_Test_Name_Maximum_With_WhiteSpace()
		{
			//Arrange
			ISymptom actual = this.modelFactory.GenerateSymptom(this.key.Object);
			
			//                     .........10........20........30........40........50........60........70
			actual.Name = " \t\r\n orAbhnlOkFmaCLrPvjXljasbzvIGVIrrjHdCjIGBHooLMpzKen \t\r\n ";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_Symptom_Test_Name_Minimum()
		{
			//Arrange
			ISymptom actual = this.modelFactory.GenerateSymptom(this.key.Object);
			
			
			actual.Name = "F";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public virtual void Validate_Symptom_Test_Name_Minimum_Minus_One()
		{
			//Arrange
			ISymptom actual = this.modelFactory.GenerateSymptom(this.key.Object);
			
			actual.Name = string.Empty;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public virtual void Validate_Symptom_Test_Name_WhiteSpace_Only()
		{
			//Arrange
			ISymptom actual = this.modelFactory.GenerateSymptom(this.key.Object);
			
			actual.Name = " \t\r\n ";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		#endregion Name
	}
}
