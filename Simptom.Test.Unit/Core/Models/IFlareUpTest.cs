using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Simptom.Framework;
using Simptom.Framework.Models;

namespace Simptom.Test.Unit.Core.Models
{
	public abstract class IFlareUpTest
	{
		protected IFlareUp flareUp;
		protected Mock<IFlareUpKey> key;
		protected IModelFactory modelFactory;
		protected Mock<ISymptom> symptom;
		protected Mock<ISymptomKey> symptomKey;
		protected TestContext testContext;
		protected Mock<IUser> user;
		protected Mock<IUserKey> userKey;
		
		public TestContext TestContext { get { return this.testContext; } set { this.testContext = value; } }
		
		[TestCleanup]
		public virtual void TestCleanup()
		{
			this.symptom = null;
			this.symptomKey = null;
			this.user = null;
			this.userKey = null;
			
			this.flareUp = null;
			this.key = null;
		}
		
		[TestInitialize]
		public virtual void TestInitialize()
		{
			this.symptom = new Mock<ISymptom>();
			this.symptom.SetupAllProperties();
			this.symptom.Setup(_model => _model.Key).Returns(() => { return this.symptomKey.Object; });
			this.symptomKey = new Mock<ISymptomKey>();
			this.symptomKey.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.user = new Mock<IUser>();
			this.user.SetupAllProperties();
			this.user.Setup(_model => _model.Key).Returns(() => { return this.userKey.Object; });
			this.userKey = new Mock<IUserKey>();
			this.userKey.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			
			this.key = new Mock<IFlareUpKey>();
			this.key.Setup(_key => _key.ID).Returns(Guid.NewGuid());
		}
		
		#region Equals
		
		[TestMethod]
		public virtual void Equals_FlareUpKey_Test_Returns_False_When_Key_Does_Not_Match()
		{
			//Arrange
			bool actual;
			bool expected = false;
			IFlareUpKey injected = this.modelFactory.GenerateFlareUpKey(Guid.NewGuid());
			IFlareUpKey worked = this.modelFactory.GenerateFlareUpKey(Guid.NewGuid());
			
			this.symptomKey.Setup(_symptomKey => _symptomKey.Equals(It.IsAny<ISymptomKey>())).Returns(false);
			
			this.userKey.Setup(_userKey => _userKey.Equals(It.IsAny<IUserKey>())).Returns(false);
			
			//Act
			actual = worked.Equals(injected);
			
			//Assert
			Assert.AreEqual(actual, expected);
		}
		
		[TestMethod]
		public virtual void Equals_FlareUpKey_Test_Returns_True_When_Key_Matches()
		{
			//Arrange
			bool actual;
			bool expected = true;
			IFlareUpKey injected = this.modelFactory.GenerateFlareUpKey(Guid.NewGuid());
			
			this.symptomKey.Setup(_symptomKey => _symptomKey.Equals(It.IsAny<ISymptomKey>())).Returns(true);
			
			this.userKey.Setup(_userKey => _userKey.Equals(It.IsAny<IUserKey>())).Returns(true);
			
			//Act
			actual = injected.Equals(injected);
			
			//Assert
			Assert.AreEqual(actual, expected);
		}
		
		#endregion Equals
		
		#region ExperiencedOn
		
		[TestMethod]
		public virtual void Validate_FlareUp_Test_ExperiencedOn_Maximum()
		{
			//Arrange
			IFlareUp actual = this.modelFactory.GenerateFlareUp(this.key.Object);
			
			actual.ExperiencedOn = new DateTime(9999, 12, 31, 23, 59, 59);
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_FlareUp_Test_ExperiencedOn_Minimum()
		{
			//Arrange
			IFlareUp actual = this.modelFactory.GenerateFlareUp(this.key.Object);
			
			actual.ExperiencedOn = new DateTime(1, 1, 1, 0, 0, 0);
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		#endregion ExperiencedOn
		
		#region Symptom
		
		[TestMethod]
		public virtual void Validate_FlareUp_Test_Symptom_Is_Null()
		{
			//Arrange
			IFlareUp actual = this.modelFactory.GenerateFlareUp(this.key.Object);
			
			actual.Symptom = null;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_FlareUp_Test_Symptom_Is_Valid()
		{
			//Arrange
			IFlareUp actual = this.modelFactory.GenerateFlareUp(this.key.Object);
			Mock<ISymptom> symptom = new Mock<ISymptom>();
			
			actual.Symptom = symptom.Object;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		#endregion Symptom
		
		#region Severity
		
		[TestMethod]
		public virtual void Validate_FlareUp_Test_Severity_Maximum()
		{
			//Arrange
			IFlareUp actual = this.modelFactory.GenerateFlareUp(this.key.Object);
			
			actual.Severity = 1d;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public virtual void Validate_FlareUp_Test_Severity_Maximum_Plus_One()
		{
			//Arrange
			IFlareUp actual = this.modelFactory.GenerateFlareUp(this.key.Object);
			
			actual.Severity = 1.00000000001d;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_FlareUp_Test_Severity_Minimum()
		{
			//Arrange
			IFlareUp actual = this.modelFactory.GenerateFlareUp(this.key.Object);
			
			actual.Severity = 0.0d;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public virtual void Validate_FlareUp_Test_Severity_Minimum_Minus_One()
		{
			//Arrange
			IFlareUp actual = this.modelFactory.GenerateFlareUp(this.key.Object);
			
			actual.Severity = -1E-11d;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		#endregion Severity
		
		#region User
		
		[TestMethod]
		public virtual void Validate_FlareUp_Test_User_Is_Null()
		{
			//Arrange
			IFlareUp actual = this.modelFactory.GenerateFlareUp(this.key.Object);
			
			actual.User = null;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_FlareUp_Test_User_Is_Valid()
		{
			//Arrange
			IFlareUp actual = this.modelFactory.GenerateFlareUp(this.key.Object);
			Mock<IUser> user = new Mock<IUser>();
			
			actual.User = user.Object;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		#endregion User
	}
}
