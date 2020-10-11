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
	public abstract class IActivityServiceTest
	{
		protected IActivity activity;
		protected IActivity activityNotFound;
		protected IActivity initial;
		protected IModelFactory modelFactory;
		protected IActivitiesSearch searchMultipleThatFindsNothing;
		protected IActivitiesSearch searchMultipleThatFindsSomething;
		protected IActivitySearch searchThatReturnsNull;
		protected IActivitySearch searchThatReturnsSomething;
		protected IActivityService service;
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
			if(this.service.Exists(this.activityNotFound.Key))
				this.service.Delete(this.activityNotFound.Key, null);
			
			if(this.initial != null)
				this.service.Save(this.initial, null);
		}
		
		public virtual IActivitySearch PrepareSearch(IActivity activity)
		{
			throw new NotImplementedException();
		}
		
		#region Delete
		
		#endregion Delete
		
		#region Exists
		
		[TestMethod]
		public virtual void Exists_Activities_Test_Does_Not_Return_Null_List_When_Nothing_Matches()
		{
			//Arrange
			IEnumerable<IActivityKey> actual = null;
			IEnumerable<IActivityKey> injected = new List<IActivityKey>() { this.activityNotFound.Key };
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void Exists_Activities_Test_Returns_Empty_List_When_Nothing_Matches()
		{
			//Arrange
			int actual;
			int expected = 0;
			IEnumerable<IActivityKey> injected = new List<IActivityKey>() { this.activityNotFound.Key };
			
			//Act
			actual = this.service.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Activities_Test_Returns_List_Of_ActivityKeys_That_Match()
		{
			if(this.activity == null)
				throw new AssertInconclusiveException("You must provide a Activity that is already in the repository for this test to be effective.");
			
			//Arrange
			IEnumerable<IActivityKey> actual = null;
			IEnumerable<IActivityKey> injected = new List<IActivityKey>() { this.activity.Key };
			IEnumerable<IActivityKey> expected = injected;
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected.Count(), actual.Count());
			Assert.IsTrue(expected.All(_e => actual.Any(_a => _e.Equals(_a))));
			Assert.IsTrue(actual.All(_a => expected.Any(_e => _e.Equals(_a))));
		}
		
		[TestMethod]
		public virtual void Exists_Activity_Test_Returns_False_When_Activity_Is_Not_Found()
		{
			//Arrange
			bool actual;
			bool expected = false;
			IActivityKey injected = this.activityNotFound.Key;
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Activity_Test_Returns_True_When_Activity_Is_Found()
		{
			if(this.activity == null)
				throw new AssertInconclusiveException("You must provide a Activity that is already in the repository for this test to be effective.");
			
			//Arrange
			bool actual;
			bool expected = true;
			IActivityKey injected = this.activity.Key;
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		#endregion Exists
		
		#region Find
		
		[TestMethod]
		public virtual void Find_Activities_Test_Does_Not_Return_Null_List_When_Nothing_Matches()
		{
			//Arrange
			IEnumerable<IActivity> actual;
			IActivitiesSearch injected = this.searchMultipleThatFindsNothing;
			
			//Act
			actual = this.service.Find(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void Find_Activities_Test_Returns_Empty_List_When_Nothing_Matches()
		{
			//Arrange
			int actual;
			int expected = 0;
			IActivitiesSearch injected = this.searchMultipleThatFindsNothing;
			
			//Act
			actual = this.service.Find(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Find_Activities_Test_Throws_Error_When_Search_Is_Null()
		{
			//Arrange
			IActivitiesSearch injected = null;
			
			//Act
			this.service.Find(injected);
			
			//Assert
		}
		
		#endregion Find
		
		#region Find Single
		
		[TestMethod]
		public virtual void FindSingle_Activity_Test_Returns_Model_When_Found()
		{
			if(this.activity == null)
				throw new AssertInconclusiveException("You must provide a Activity that is already in the repository for this test to be effective.");
			
			//Arrange
			IActivity actual;
			IActivitySearch injected = this.searchThatReturnsSomething;
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void FindSingle_Activity_Test_Returns_Null_When_Not_Found()
		{
			//Arrange
			IActivity actual;
			IActivitySearch injected = this.searchThatReturnsNull;
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNull(actual);
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void FindSingle_Activity_Test_Throws_Error_When_Search_Is_Null()
		{
			//Arrange
			IActivitySearch injected = null;
			
			//Act
			this.service.FindSingle(injected);
			
			//Assert
		}
		
		#endregion Find Single
		
		#region Save
		
		[TestMethod]
		public virtual void Save_Activity_Test_Preserves_Activity_When_It_Already_Exists()
		{
			if(this.activity == null)
				throw new AssertInconclusiveException("You must provide a Activity that is already in the repository for this test to be effective.");
			
			//Arrange
			IActivity injected = this.activity;
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Save_Activity_Test_Preserves_Activity_When_It_Is_New()
		{
			if(this.activity == null)
				throw new AssertInconclusiveException("You must provide a Activity that is already in the repository for this test to be effective.");
			
			//Arrange
			IActivity injected = this.activityNotFound;
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			IActivity expected = injected;
			IActivitySearch search = PrepareSearch(expected);
			if(search != null)
			{
				IActivity actual = this.activityNotFound = this.service.FindSingle(search);
			
				Assert.IsNotNull(actual);
				Assert.AreEqual(expected.Name, actual.Name, "Name");
			}
		}
		
		[TestMethod]
		public virtual void Save_Activity_Test_Preserves_Name_When_Is_Maxed_And_Activities_Already_Exists()
		{
			if(this.activity == null)
				throw new AssertInconclusiveException("You must provide a Activity that is already in the repository for this test to be effective.");
			
			//Arrange
			IActivity injected = this.activity;
			//               .........10........20........30........40........50........60
			injected.Name = "JimtBctneYWhNXvJGRPtMeOqZQeEnezLHpLXlvqUyzjvgfeHoP";
			
			IActivity actual = null;
			IActivity expected = injected;
			
			IActivitySearch search = PrepareSearch(expected);
			if(search == null)
				throw new AssertInconclusiveException("You must override the function that prepares a Activity Search model for this test to be effective.");
			
			this.initial = this.service.FindSingle(this.searchThatReturnsSomething);
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			actual = this.service.FindSingle(search);
			
			Assert.AreEqual(expected.Name, actual.Name);
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Activities_Test_Preserves_Activities_When_They_Already_Exist()
		{
			if(this.activity == null)
				throw new AssertInconclusiveException("You must provide a Activity that is already in the repository for this test to be effective.");
			
			//Arrange
			List<IActivity> injected = new List<IActivity>() { this.activity };
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Activities_Test_Preserves_Activities_When_They_Are_New()
		{
			if(this.activity == null)
				throw new AssertInconclusiveException("You must provide a Activity that is already in the repository for this test to be effective.");
			
			//Arrange
			List<IActivity> injected = new List<IActivity>() { this.activityNotFound };
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			IActivity expected = injected.First();
			IActivitySearch search = PrepareSearch(expected);
			if(search != null)
			{
				IActivity actual = this.activityNotFound = this.service.FindSingle(search);
			
				Assert.IsNotNull(actual);
				Assert.AreEqual(expected.Name, actual.Name, "Name");
			}
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Activities_Test_Preserves_Name_When_Is_Maxed_And_Activities_Already_Exists()
		{
			if(this.activity == null)
				throw new AssertInconclusiveException("You must provide a Activity that is already in the repository for this test to be effective.");
			
			//Arrange
			IEnumerable<IActivity> injected = new List<IActivity>() { this.activity };
			//                    .........10........20........30........40........50........60
			this.activity.Name = "MHaFEFjpsAgvKMkysitnHKoMBGJbCufxPSWRMotcnRFJyxgXum";
			
			IActivity actual = null;
			IActivity expected = this.activity;
			
			IActivitySearch search = PrepareSearch(expected);
			if(search == null)
				throw new AssertInconclusiveException("You must override the function that prepares a Activity Search model for this test to be effective.");
			
			this.initial = this.service.FindSingle(this.searchThatReturnsSomething);
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			actual = this.service.FindSingle(search);
			
			Assert.AreEqual(expected.Name, actual.Name);
		}
		
		#endregion Save
	}
}
