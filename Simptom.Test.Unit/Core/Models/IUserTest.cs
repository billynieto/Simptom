using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Simptom.Framework;
using Simptom.Framework.Models;

namespace Simptom.Test.Unit.Core.Models
{
	public abstract class IUserTest
	{
		protected Mock<IUserKey> key;
		protected IModelFactory modelFactory;
		protected TestContext testContext;
		protected IUser user;
		
		public TestContext TestContext { get { return this.testContext; } set { this.testContext = value; } }
		
		[TestCleanup]
		public virtual void TestCleanup()
		{
			this.user = null;
			this.key = null;
		}
		
		[TestInitialize]
		public virtual void TestInitialize()
		{
			this.key = new Mock<IUserKey>();
			this.key.Setup(_key => _key.ID).Returns(Guid.NewGuid());
		}
		
		#region Equals
		
		[TestMethod]
		public virtual void Equals_UserKey_Test_Returns_False_When_Key_Does_Not_Match()
		{
			//Arrange
			bool actual;
			bool expected = false;
			IUserKey injected = this.modelFactory.GenerateUserKey(Guid.NewGuid());
			IUserKey worked = this.modelFactory.GenerateUserKey(Guid.NewGuid());
			
			//Act
			actual = worked.Equals(injected);
			
			//Assert
			Assert.AreEqual(actual, expected);
		}
		
		[TestMethod]
		public virtual void Equals_UserKey_Test_Returns_True_When_Key_Matches()
		{
			//Arrange
			bool actual;
			bool expected = true;
			IUserKey injected = this.modelFactory.GenerateUserKey(Guid.NewGuid());
			
			//Act
			actual = injected.Equals(injected);
			
			//Assert
			Assert.AreEqual(actual, expected);
		}
		
		#endregion Equals
		
		#region Name
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Validate_User_Test_Name_Is_Null()
		{
			//Arrange
			IUser actual = this.modelFactory.GenerateUser(this.key.Object);
			
			actual.Name = null;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_User_Test_Name_Maximum()
		{
			//Arrange
			IUser actual = this.modelFactory.GenerateUser(this.key.Object);
			
			//             .........10........20........30........40........50........60
			actual.Name = "ucKGaHzfoAPueDhzjQetTQPeqRNMTfdbhSEWdfYUQjkIfPIHSB";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public virtual void Validate_User_Test_Name_Maximum_Plus_One()
		{
			//Arrange
			IUser actual = this.modelFactory.GenerateUser(this.key.Object);
			
			//             .........10........20........30........40........50........60
			actual.Name = "BwIHeiQMxGXfGOZNSTrbJhJYBYpbkAKEVHZMymnJErHWcDQaRlk";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_User_Test_Name_Maximum_With_WhiteSpace()
		{
			//Arrange
			IUser actual = this.modelFactory.GenerateUser(this.key.Object);
			
			//                     .........10........20........30........40........50........60........70
			actual.Name = " \t\r\n nIXbpDnFpiUMljwNnmZkweSkDJGhYBjgUsRvefCaJxPQZdYblr \t\r\n ";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_User_Test_Name_Minimum()
		{
			//Arrange
			IUser actual = this.modelFactory.GenerateUser(this.key.Object);
			
			
			actual.Name = "i";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public virtual void Validate_User_Test_Name_Minimum_Minus_One()
		{
			//Arrange
			IUser actual = this.modelFactory.GenerateUser(this.key.Object);
			
			actual.Name = string.Empty;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public virtual void Validate_User_Test_Name_WhiteSpace_Only()
		{
			//Arrange
			IUser actual = this.modelFactory.GenerateUser(this.key.Object);
			
			actual.Name = " \t\r\n ";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		#endregion Name
		
		#region Password
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Validate_User_Test_Password_Is_Null()
		{
			//Arrange
			IUser actual = this.modelFactory.GenerateUser(this.key.Object);
			
			actual.Password = null;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_User_Test_Password_Maximum()
		{
			//Arrange
			IUser actual = this.modelFactory.GenerateUser(this.key.Object);
			
			//                 .........10........20........30........40........50........60
			actual.Password = "KkHtNJfgTOBXVskZOmpFFHwsAMVYhokZqFUKZGYOhhPQPKbKsR";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public virtual void Validate_User_Test_Password_Maximum_Plus_One()
		{
			//Arrange
			IUser actual = this.modelFactory.GenerateUser(this.key.Object);
			
			//                 .........10........20........30........40........50........60
			actual.Password = "eLmTRdqufbkhLdpDBJCYzUkFDovqBypGurLaYMyYpIGdIPZIIxy";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_User_Test_Password_Maximum_With_WhiteSpace()
		{
			//Arrange
			IUser actual = this.modelFactory.GenerateUser(this.key.Object);
			
			//                         .........10........20........30........40........50........60........70
			actual.Password = " \t\r\n mEVEzcOoTbOhZEECGpxzZinByxRGDPLlFqcFdMfSWLMXaaillm \t\r\n ";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Validate_User_Test_Password_Minimum()
		{
			//Arrange
			IUser actual = this.modelFactory.GenerateUser(this.key.Object);
			
			
			actual.Password = "f";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public virtual void Validate_User_Test_Password_Minimum_Minus_One()
		{
			//Arrange
			IUser actual = this.modelFactory.GenerateUser(this.key.Object);
			
			actual.Password = string.Empty;
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public virtual void Validate_User_Test_Password_WhiteSpace_Only()
		{
			//Arrange
			IUser actual = this.modelFactory.GenerateUser(this.key.Object);
			
			actual.Password = " \t\r\n ";
			
			//Act
			actual.Validate();
			
			//Assert
		}
		
		#endregion Password
	}
}
