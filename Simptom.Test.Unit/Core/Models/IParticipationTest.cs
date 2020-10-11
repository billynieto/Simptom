using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Simptom.Framework;
using Simptom.Framework.Models;

namespace Simptom.Test.Unit.Core.Models
{
	public abstract class IParticipationTest
	{
		protected Mock<IActivity> activity;
		protected Mock<IActivityKey> activityKey;
		protected Mock<IParticipationKey> key;
		protected IModelFactory modelFactory;
		protected IParticipation participation;
		protected TestContext testContext;
		protected Mock<IUser> user;
		protected Mock<IUserKey> userKey;
		
		public TestContext TestContext { get { return this.testContext; } set { this.testContext = value; } }
		
		[TestCleanup]
		public virtual void TestCleanup()
		{
			this.activity = null;
			this.activityKey = null;
			this.user = null;
			this.userKey = null;
			
			this.participation = null;
			this.key = null;
		}
		
		[TestInitialize]
		public virtual void TestInitialize()
		{
			this.activity = new Mock<IActivity>();
			this.activity.SetupAllProperties();
			this.activity.Setup(_model => _model.Key).Returns(() => { return this.activityKey.Object; });
			this.activityKey = new Mock<IActivityKey>();
			this.activityKey.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.user = new Mock<IUser>();
			this.user.SetupAllProperties();
			this.user.Setup(_model => _model.Key).Returns(() => { return this.userKey.Object; });
			this.userKey = new Mock<IUserKey>();
			this.userKey.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			
			this.key = new Mock<IParticipationKey>();
			this.key.Setup(_key => _key.ID).Returns(Guid.NewGuid());
		}
		
		#region Equals
		
		[TestMethod]
		public virtual void Equals_ParticipationKey_Test_Returns_False_When_Key_Does_Not_Match()
		{
			//Arrange
			bool actual;
			bool expected = false;
			IParticipationKey injected = this.modelFactory.GenerateParticipationKey(Guid.NewGuid());
			IParticipationKey worked = this.modelFactory.GenerateParticipationKey(Guid.NewGuid());
			
			this.activityKey.Setup(_activityKey => _activityKey.Equals(It.IsAny<IActivityKey>())).Returns(false);
			
			this.userKey.Setup(_userKey => _userKey.Equals(It.IsAny<IUserKey>())).Returns(false);
			
			//Act
			actual = worked.Equals(injected);
			
			//Assert
			Assert.AreEqual(actual, expected);
		}
		
		[TestMethod]
		public virtual void Equals_ParticipationKey_Test_Returns_True_When_Key_Matches()
		{
			//Arrange
			bool actual;
			bool expected = true;
			IParticipationKey injected = this.modelFactory.GenerateParticipationKey(Guid.NewGuid());
			
			this.activityKey.Setup(_activityKey => _activityKey.Equals(It.IsAny<IActivityKey>())).Returns(true);
			
			this.userKey.Setup(_userKey => _userKey.Equals(It.IsAny<IUserKey>())).Returns(true);
			
			//Act
			actual = injected.Equals(injected);
			
			//Assert
			Assert.AreEqual(actual, expected);
		}
		
		#endregion Equals
		
		#region Activity
		
		[TestMethod]
		public virtual void Validate_Participation_Test_Activity_Is_Null()
		{
			//Arrange
			IParticipation actual = this.modelFactory.GenerateParticipation(this.key.Object);
			
			actual.Activity = null;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_Participation_Test_Activity_Is_Valid()
		{
			//Arrange
			IParticipation actual = this.modelFactory.GenerateParticipation(this.key.Object);
			Mock<IActivity> activity = new Mock<IActivity>();
			
			actual.Activity = activity.Object;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		#endregion Activity
		
		#region PerformedOn
		
		[TestMethod]
		public virtual void Validate_Participation_Test_PerformedOn_Maximum()
		{
			//Arrange
			IParticipation actual = this.modelFactory.GenerateParticipation(this.key.Object);
			
			actual.PerformedOn = new DateTime(9999, 12, 31, 23, 59, 59);
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_Participation_Test_PerformedOn_Minimum()
		{
			//Arrange
			IParticipation actual = this.modelFactory.GenerateParticipation(this.key.Object);
			
			actual.PerformedOn = new DateTime(1, 1, 1, 0, 0, 0);
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		#endregion PerformedOn
		
		#region Severity
		
		[TestMethod]
		public virtual void Validate_Participation_Test_Severity_Maximum()
		{
			//Arrange
			IParticipation actual = this.modelFactory.GenerateParticipation(this.key.Object);
			
			actual.Severity = 1d;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public virtual void Validate_Participation_Test_Severity_Maximum_Plus_One()
		{
			//Arrange
			IParticipation actual = this.modelFactory.GenerateParticipation(this.key.Object);
			
			actual.Severity = 1.00000000001d;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_Participation_Test_Severity_Minimum()
		{
			//Arrange
			IParticipation actual = this.modelFactory.GenerateParticipation(this.key.Object);
			
			actual.Severity = 0.0d;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public virtual void Validate_Participation_Test_Severity_Minimum_Minus_One()
		{
			//Arrange
			IParticipation actual = this.modelFactory.GenerateParticipation(this.key.Object);
			
			actual.Severity = -1E-11d;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		#endregion Severity
		
		#region User
		
		[TestMethod]
		public virtual void Validate_Participation_Test_User_Is_Null()
		{
			//Arrange
			IParticipation actual = this.modelFactory.GenerateParticipation(this.key.Object);
			
			actual.User = null;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_Participation_Test_User_Is_Valid()
		{
			//Arrange
			IParticipation actual = this.modelFactory.GenerateParticipation(this.key.Object);
			Mock<IUser> user = new Mock<IUser>();
			
			actual.User = user.Object;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		#endregion User
	}
}
