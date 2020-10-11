using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Simptom.Framework;
using Simptom.Framework.Models;

namespace Simptom.Test.Unit.Core.Models
{
	public abstract class ISymptomCategoryTest
	{
		protected Mock<ISymptomCategoryKey> key;
		protected IModelFactory modelFactory;
		protected ISymptomCategory symptomCategory;
		protected TestContext testContext;
		
		public TestContext TestContext { get { return this.testContext; } set { this.testContext = value; } }
		
		[TestCleanup]
		public virtual void TestCleanup()
		{
			this.symptomCategory = null;
			this.key = null;
		}
		
		[TestInitialize]
		public virtual void TestInitialize()
		{
			this.key = new Mock<ISymptomCategoryKey>();
			this.key.Setup(_key => _key.ID).Returns(Guid.NewGuid());
		}
		
		#region Equals
		
		[TestMethod]
		public virtual void Equals_SymptomCategoryKey_Test_Returns_False_When_Key_Does_Not_Match()
		{
			//Arrange
			bool actual;
			bool expected = false;
			ISymptomCategoryKey injected = this.modelFactory.GenerateSymptomCategoryKey(Guid.NewGuid());
			ISymptomCategoryKey worked = this.modelFactory.GenerateSymptomCategoryKey(Guid.NewGuid());
			
			//Act
			actual = worked.Equals(injected);
			
			//Assert
			Assert.AreEqual(actual, expected);
		}
		
		[TestMethod]
		public virtual void Equals_SymptomCategoryKey_Test_Returns_True_When_Key_Matches()
		{
			//Arrange
			bool actual;
			bool expected = true;
			ISymptomCategoryKey injected = this.modelFactory.GenerateSymptomCategoryKey(Guid.NewGuid());
			
			//Act
			actual = injected.Equals(injected);
			
			//Assert
			Assert.AreEqual(actual, expected);
		}
		
		#endregion Equals
		
		#region Name
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Validate_SymptomCategory_Test_Name_Is_Null()
		{
			//Arrange
			ISymptomCategory actual = this.modelFactory.GenerateSymptomCategory(this.key.Object);
			
			actual.Name = null;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_SymptomCategory_Test_Name_Maximum()
		{
			//Arrange
			ISymptomCategory actual = this.modelFactory.GenerateSymptomCategory(this.key.Object);
			
			//             .........10........20........30........40........50........60
			actual.Name = "cqHucmJYCslsPGckbBkUhTkWGIyAAZiwLqVlagJxnPlkyNeqbk";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public virtual void Validate_SymptomCategory_Test_Name_Maximum_Plus_One()
		{
			//Arrange
			ISymptomCategory actual = this.modelFactory.GenerateSymptomCategory(this.key.Object);
			
			//             .........10........20........30........40........50........60
			actual.Name = "gFFkaQdTXfBgnVPGJioXZAFYLUCDeCbYZNLYBppyiVjNkZbxzcv";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_SymptomCategory_Test_Name_Maximum_With_WhiteSpace()
		{
			//Arrange
			ISymptomCategory actual = this.modelFactory.GenerateSymptomCategory(this.key.Object);
			
			//                     .........10........20........30........40........50........60........70
			actual.Name = " \t\r\n KWHExjfLOtOiyDkOHyKQjUSNHsHZxSEhzpomPgTMfpspJeqIaJ \t\r\n ";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_SymptomCategory_Test_Name_Minimum()
		{
			//Arrange
			ISymptomCategory actual = this.modelFactory.GenerateSymptomCategory(this.key.Object);
			
			
			actual.Name = "E";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public virtual void Validate_SymptomCategory_Test_Name_Minimum_Minus_One()
		{
			//Arrange
			ISymptomCategory actual = this.modelFactory.GenerateSymptomCategory(this.key.Object);
			
			actual.Name = string.Empty;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public virtual void Validate_SymptomCategory_Test_Name_WhiteSpace_Only()
		{
			//Arrange
			ISymptomCategory actual = this.modelFactory.GenerateSymptomCategory(this.key.Object);
			
			actual.Name = " \t\r\n ";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		#endregion Name
	}
}
