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
	public abstract class IFlareUpServiceTest
	{
		protected IFlareUp flareUp;
		protected IFlareUp flareUpNotFound;
		protected IFlareUp initial;
		protected IModelFactory modelFactory;
		protected IFlareUpsSearch searchMultipleThatFindsNothing;
		protected IFlareUpsSearch searchMultipleThatFindsSomething;
		protected IFlareUpSearch searchThatReturnsNull;
		protected IFlareUpSearch searchThatReturnsSomething;
		protected IFlareUpService service;
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
			if(this.service.Exists(this.flareUpNotFound.Key))
				this.service.Delete(this.flareUpNotFound.Key, null);
			
			if(this.initial != null)
				this.service.Save(this.initial, null);
		}
		
		public virtual IFlareUpSearch PrepareSearch(IFlareUp flareUp)
		{
			throw new NotImplementedException();
		}
		
		#region Delete
		
		#endregion Delete
		
		#region Exists
		
		[TestMethod]
		public virtual void Exists_FlareUp_Test_Returns_False_When_FlareUp_Is_Not_Found()
		{
			//Arrange
			bool actual;
			bool expected = false;
			IFlareUpKey injected = this.flareUpNotFound.Key;
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_FlareUp_Test_Returns_True_When_FlareUp_Is_Found()
		{
			if(this.flareUp == null)
				throw new AssertInconclusiveException("You must provide a FlareUp that is already in the repository for this test to be effective.");
			
			//Arrange
			bool actual;
			bool expected = true;
			IFlareUpKey injected = this.flareUp.Key;
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_FlareUps_Test_Does_Not_Return_Null_List_When_Nothing_Matches()
		{
			//Arrange
			IEnumerable<IFlareUpKey> actual = null;
			IEnumerable<IFlareUpKey> injected = new List<IFlareUpKey>() { this.flareUpNotFound.Key };
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void Exists_FlareUps_Test_Returns_Empty_List_When_Nothing_Matches()
		{
			//Arrange
			int actual;
			int expected = 0;
			IEnumerable<IFlareUpKey> injected = new List<IFlareUpKey>() { this.flareUpNotFound.Key };
			
			//Act
			actual = this.service.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_FlareUps_Test_Returns_List_Of_FlareUpKeys_That_Match()
		{
			if(this.flareUp == null)
				throw new AssertInconclusiveException("You must provide a FlareUp that is already in the repository for this test to be effective.");
			
			//Arrange
			IEnumerable<IFlareUpKey> actual = null;
			IEnumerable<IFlareUpKey> injected = new List<IFlareUpKey>() { this.flareUp.Key };
			IEnumerable<IFlareUpKey> expected = injected;
			
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
		public virtual void Find_FlareUps_Test_Does_Not_Return_Null_List_When_Nothing_Matches()
		{
			//Arrange
			IEnumerable<IFlareUp> actual;
			IFlareUpsSearch injected = this.searchMultipleThatFindsNothing;
			
			//Act
			actual = this.service.Find(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void Find_FlareUps_Test_Returns_Empty_List_When_Nothing_Matches()
		{
			//Arrange
			int actual;
			int expected = 0;
			IFlareUpsSearch injected = this.searchMultipleThatFindsNothing;
			
			//Act
			actual = this.service.Find(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Find_FlareUps_Test_Throws_Error_When_Search_Is_Null()
		{
			//Arrange
			IFlareUpsSearch injected = null;
			
			//Act
			this.service.Find(injected);
			
			//Assert
		}
		
		#endregion Find
		
		#region Find Single
		
		[TestMethod]
		public virtual void FindSingle_FlareUp_Test_Returns_Model_When_Found()
		{
			if(this.flareUp == null)
				throw new AssertInconclusiveException("You must provide a FlareUp that is already in the repository for this test to be effective.");
			
			//Arrange
			IFlareUp actual;
			IFlareUpSearch injected = this.searchThatReturnsSomething;
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void FindSingle_FlareUp_Test_Returns_Null_When_Not_Found()
		{
			//Arrange
			IFlareUp actual;
			IFlareUpSearch injected = this.searchThatReturnsNull;
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNull(actual);
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void FindSingle_FlareUp_Test_Throws_Error_When_Search_Is_Null()
		{
			//Arrange
			IFlareUpSearch injected = null;
			
			//Act
			this.service.FindSingle(injected);
			
			//Assert
		}
		
		#endregion Find Single
		
		#region Save
		
		[TestMethod]
		public virtual void Save_FlareUp_Test_Preserves_ExperiencedOn_When_Is_Maxed_And_FlareUps_Already_Exists()
		{
			if(this.flareUp == null)
				throw new AssertInconclusiveException("You must provide a FlareUp that is already in the repository for this test to be effective.");
			
			//Arrange
			IFlareUp injected = this.flareUp;
			injected.ExperiencedOn = new DateTime(9999, 12, 31, 23, 59, 59);
			
			IFlareUp actual = null;
			IFlareUp expected = injected;
			
			IFlareUpSearch search = PrepareSearch(expected);
			if(search == null)
				throw new AssertInconclusiveException("You must override the function that prepares a FlareUp Search model for this test to be effective.");
			
			this.initial = this.service.FindSingle(this.searchThatReturnsSomething);
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			actual = this.service.FindSingle(search);
			
			Assert.AreEqual(expected.ExperiencedOn, actual.ExperiencedOn);
		}
		
		[TestMethod]
		public virtual void Save_FlareUp_Test_Preserves_FlareUp_When_It_Already_Exists()
		{
			if(this.flareUp == null)
				throw new AssertInconclusiveException("You must provide a FlareUp that is already in the repository for this test to be effective.");
			
			//Arrange
			IFlareUp injected = this.flareUp;
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Save_FlareUp_Test_Preserves_FlareUp_When_It_Is_New()
		{
			if(this.flareUp == null)
				throw new AssertInconclusiveException("You must provide a FlareUp that is already in the repository for this test to be effective.");
			
			//Arrange
			IFlareUp injected = this.flareUpNotFound;
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			IFlareUp expected = injected;
			IFlareUpSearch search = PrepareSearch(expected);
			if(search != null)
			{
				IFlareUp actual = this.flareUpNotFound = this.service.FindSingle(search);
			
				Assert.IsNotNull(actual);
				Assert.AreEqual(expected.ExperiencedOn, actual.ExperiencedOn, "ExperiencedOn");
				Assert.AreEqual(expected.Severity, actual.Severity, "Severity");
			}
		}
		
		[TestMethod]
		public virtual void Save_FlareUp_Test_Preserves_Severity_When_Is_Maxed_And_FlareUps_Already_Exists()
		{
			if(this.flareUp == null)
				throw new AssertInconclusiveException("You must provide a FlareUp that is already in the repository for this test to be effective.");
			
			//Arrange
			IFlareUp injected = this.flareUp;
			injected.Severity = 1d;
			
			IFlareUp actual = null;
			IFlareUp expected = injected;
			
			IFlareUpSearch search = PrepareSearch(expected);
			if(search == null)
				throw new AssertInconclusiveException("You must override the function that prepares a FlareUp Search model for this test to be effective.");
			
			this.initial = this.service.FindSingle(this.searchThatReturnsSomething);
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			actual = this.service.FindSingle(search);
			
			Assert.AreEqual(expected.Severity, actual.Severity);
		}
		
		[TestMethod]
		public virtual void Save_Multiple_FlareUps_Test_Preserves_ExperiencedOn_When_Is_Maxed_And_FlareUps_Already_Exists()
		{
			if(this.flareUp == null)
				throw new AssertInconclusiveException("You must provide a FlareUp that is already in the repository for this test to be effective.");
			
			//Arrange
			IEnumerable<IFlareUp> injected = new List<IFlareUp>() { this.flareUp };
			this.flareUp.ExperiencedOn = new DateTime(9999, 12, 31, 23, 59, 59);
			
			IFlareUp actual = null;
			IFlareUp expected = this.flareUp;
			
			IFlareUpSearch search = PrepareSearch(expected);
			if(search == null)
				throw new AssertInconclusiveException("You must override the function that prepares a FlareUp Search model for this test to be effective.");
			
			this.initial = this.service.FindSingle(this.searchThatReturnsSomething);
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			actual = this.service.FindSingle(search);
			
			Assert.AreEqual(expected.ExperiencedOn, actual.ExperiencedOn);
		}
		
		[TestMethod]
		public virtual void Save_Multiple_FlareUps_Test_Preserves_FlareUps_When_They_Already_Exist()
		{
			if(this.flareUp == null)
				throw new AssertInconclusiveException("You must provide a FlareUp that is already in the repository for this test to be effective.");
			
			//Arrange
			List<IFlareUp> injected = new List<IFlareUp>() { this.flareUp };
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Save_Multiple_FlareUps_Test_Preserves_FlareUps_When_They_Are_New()
		{
			if(this.flareUp == null)
				throw new AssertInconclusiveException("You must provide a FlareUp that is already in the repository for this test to be effective.");
			
			//Arrange
			List<IFlareUp> injected = new List<IFlareUp>() { this.flareUpNotFound };
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			IFlareUp expected = injected.First();
			IFlareUpSearch search = PrepareSearch(expected);
			if(search != null)
			{
				IFlareUp actual = this.flareUpNotFound = this.service.FindSingle(search);
			
				Assert.IsNotNull(actual);
				Assert.AreEqual(expected.ExperiencedOn, actual.ExperiencedOn, "ExperiencedOn");
				Assert.AreEqual(expected.Severity, actual.Severity, "Severity");
			}
		}
		
		[TestMethod]
		public virtual void Save_Multiple_FlareUps_Test_Preserves_Severity_When_Is_Maxed_And_FlareUps_Already_Exists()
		{
			if(this.flareUp == null)
				throw new AssertInconclusiveException("You must provide a FlareUp that is already in the repository for this test to be effective.");
			
			//Arrange
			IEnumerable<IFlareUp> injected = new List<IFlareUp>() { this.flareUp };
			this.flareUp.Severity = 1d;
			
			IFlareUp actual = null;
			IFlareUp expected = this.flareUp;
			
			IFlareUpSearch search = PrepareSearch(expected);
			if(search == null)
				throw new AssertInconclusiveException("You must override the function that prepares a FlareUp Search model for this test to be effective.");
			
			this.initial = this.service.FindSingle(this.searchThatReturnsSomething);
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			actual = this.service.FindSingle(search);
			
			Assert.AreEqual(expected.Severity, actual.Severity);
		}
		
		#endregion Save
	}
}
