using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Simptom.Framework;
using Simptom.Framework.Models;

namespace Simptom.Test.Unit.Core.Models
{
	public abstract class IActivityTest
	{
		protected IActivity activity;
		protected Mock<IActivityCategory> activityCategory;
		protected Mock<IActivityCategoryKey> activityCategoryKey;
		protected Mock<IActivityKey> key;
		protected IModelFactory modelFactory;
		protected TestContext testContext;
		
		public TestContext TestContext { get { return this.testContext; } set { this.testContext = value; } }
		
		[TestCleanup]
		public virtual void TestCleanup()
		{
			this.activityCategory = null;
			this.activityCategoryKey = null;
			
			this.activity = null;
			this.key = null;
		}
		
		[TestInitialize]
		public virtual void TestInitialize()
		{
			this.activityCategory = new Mock<IActivityCategory>();
			this.activityCategory.SetupAllProperties();
			this.activityCategory.Setup(_model => _model.Key).Returns(() => { return this.activityCategoryKey.Object; });
			this.activityCategoryKey = new Mock<IActivityCategoryKey>();
			this.activityCategoryKey.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			
			this.key = new Mock<IActivityKey>();
			this.key.Setup(_key => _key.ID).Returns(Guid.NewGuid());
		}
		
		#region Equals
		
		[TestMethod]
		public virtual void Equals_ActivityKey_Test_Returns_False_When_Key_Does_Not_Match()
		{
			//Arrange
			bool actual;
			bool expected = false;
			IActivityKey injected = this.modelFactory.GenerateActivityKey(Guid.NewGuid());
			IActivityKey worked = this.modelFactory.GenerateActivityKey(Guid.NewGuid());
			
			this.activityCategoryKey.Setup(_activityCategoryKey => _activityCategoryKey.Equals(It.IsAny<IActivityCategoryKey>())).Returns(false);
			
			//Act
			actual = worked.Equals(injected);
			
			//Assert
			Assert.AreEqual(actual, expected);
		}
		
		[TestMethod]
		public virtual void Equals_ActivityKey_Test_Returns_True_When_Key_Matches()
		{
			//Arrange
			bool actual;
			bool expected = true;
			IActivityKey injected = this.modelFactory.GenerateActivityKey(Guid.NewGuid());
			
			this.activityCategoryKey.Setup(_activityCategoryKey => _activityCategoryKey.Equals(It.IsAny<IActivityCategoryKey>())).Returns(true);
			
			//Act
			actual = injected.Equals(injected);
			
			//Assert
			Assert.AreEqual(actual, expected);
		}
		
		#endregion Equals
		
		#region Category
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Validate_Activity_Test_Category_Is_Null()
		{
			//Arrange
			IActivity actual = this.modelFactory.GenerateActivity(this.key.Object);
			
			actual.Category = null;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_Activity_Test_Category_Is_Valid()
		{
			//Arrange
			IActivity actual = this.modelFactory.GenerateActivity(this.key.Object);
			Mock<IActivityCategory> activityCategory = new Mock<IActivityCategory>();
			
			actual.Category = activityCategory.Object;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		#endregion Category
		
		#region Name
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Validate_Activity_Test_Name_Is_Null()
		{
			//Arrange
			IActivity actual = this.modelFactory.GenerateActivity(this.key.Object);
			actual.Category = this.activityCategory.Object;
			
			actual.Name = null;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_Activity_Test_Name_Maximum()
		{
			//Arrange
			IActivity actual = this.modelFactory.GenerateActivity(this.key.Object);
			actual.Category = this.activityCategory.Object;
			
			//             .........10........20........30........40........50........60
			actual.Name = "tjcCGdrQqBtqwjHyFmyBGIrqPOTgFCWintUSvoZbvgHzXZBKdQ";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public virtual void Validate_Activity_Test_Name_Maximum_Plus_One()
		{
			//Arrange
			IActivity actual = this.modelFactory.GenerateActivity(this.key.Object);
			actual.Category = this.activityCategory.Object;
			
			//             .........10........20........30........40........50........60
			actual.Name = "WyvoNoffVHJatWEbhwRbERQePuPeFWEgXFSPMObHyAqFAYpEDQc";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_Activity_Test_Name_Maximum_With_WhiteSpace()
		{
			//Arrange
			IActivity actual = this.modelFactory.GenerateActivity(this.key.Object);
			actual.Category = this.activityCategory.Object;
			
			//                     .........10........20........30........40........50........60........70
			actual.Name = " \t\r\n GsCchKSMMSjklNFkcdoOdrmwgzaRsPnidOWDGlmkZIBhRkVUUN \t\r\n ";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_Activity_Test_Name_Minimum()
		{
			//Arrange
			IActivity actual = this.modelFactory.GenerateActivity(this.key.Object);
			actual.Category = this.activityCategory.Object;
			
			
			actual.Name = "N";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public virtual void Validate_Activity_Test_Name_Minimum_Minus_One()
		{
			//Arrange
			IActivity actual = this.modelFactory.GenerateActivity(this.key.Object);
			actual.Category = this.activityCategory.Object;
			
			actual.Name = string.Empty;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public virtual void Validate_Activity_Test_Name_WhiteSpace_Only()
		{
			//Arrange
			IActivity actual = this.modelFactory.GenerateActivity(this.key.Object);
			actual.Category = this.activityCategory.Object;
			
			actual.Name = " \t\r\n ";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		#endregion Name
	}
}
