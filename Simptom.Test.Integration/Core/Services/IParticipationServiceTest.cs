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
	public abstract class IParticipationServiceTest
	{
		protected IParticipation initial;
		protected IModelFactory modelFactory;
		protected IParticipation participation;
		protected IParticipation participationNotFound;
		protected IParticipationsSearch searchMultipleThatFindsNothing;
		protected IParticipationsSearch searchMultipleThatFindsSomething;
		protected IParticipationSearch searchThatReturnsNull;
		protected IParticipationSearch searchThatReturnsSomething;
		protected IParticipationService service;
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
			if(this.service.Exists(this.participationNotFound.Key))
				this.service.Delete(this.participationNotFound.Key, null);
			
			if(this.initial != null)
				this.service.Save(this.initial, null);
		}
		
		public virtual IParticipationSearch PrepareSearch(IParticipation participation)
		{
			throw new NotImplementedException();
		}
		
		#region Delete
		
		#endregion Delete
		
		#region Exists
		
		[TestMethod]
		public virtual void Exists_Participation_Test_Returns_False_When_Participation_Is_Not_Found()
		{
			//Arrange
			bool actual;
			bool expected = false;
			IParticipationKey injected = this.participationNotFound.Key;
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Participation_Test_Returns_True_When_Participation_Is_Found()
		{
			if(this.participation == null)
				throw new AssertInconclusiveException("You must provide a Participation that is already in the repository for this test to be effective.");
			
			//Arrange
			bool actual;
			bool expected = true;
			IParticipationKey injected = this.participation.Key;
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Participations_Test_Does_Not_Return_Null_List_When_Nothing_Matches()
		{
			//Arrange
			IEnumerable<IParticipationKey> actual = null;
			IEnumerable<IParticipationKey> injected = new List<IParticipationKey>() { this.participationNotFound.Key };
			
			//Act
			actual = this.service.Exists(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void Exists_Participations_Test_Returns_Empty_List_When_Nothing_Matches()
		{
			//Arrange
			int actual;
			int expected = 0;
			IEnumerable<IParticipationKey> injected = new List<IParticipationKey>() { this.participationNotFound.Key };
			
			//Act
			actual = this.service.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_Participations_Test_Returns_List_Of_ParticipationKeys_That_Match()
		{
			if(this.participation == null)
				throw new AssertInconclusiveException("You must provide a Participation that is already in the repository for this test to be effective.");
			
			//Arrange
			IEnumerable<IParticipationKey> actual = null;
			IEnumerable<IParticipationKey> injected = new List<IParticipationKey>() { this.participation.Key };
			IEnumerable<IParticipationKey> expected = injected;
			
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
		public virtual void Find_Participations_Test_Does_Not_Return_Null_List_When_Nothing_Matches()
		{
			//Arrange
			IEnumerable<IParticipation> actual;
			IParticipationsSearch injected = this.searchMultipleThatFindsNothing;
			
			//Act
			actual = this.service.Find(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void Find_Participations_Test_Returns_Empty_List_When_Nothing_Matches()
		{
			//Arrange
			int actual;
			int expected = 0;
			IParticipationsSearch injected = this.searchMultipleThatFindsNothing;
			
			//Act
			actual = this.service.Find(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Find_Participations_Test_Throws_Error_When_Search_Is_Null()
		{
			//Arrange
			IParticipationsSearch injected = null;
			
			//Act
			this.service.Find(injected);
			
			//Assert
		}
		
		#endregion Find
		
		#region Find Single
		
		[TestMethod]
		public virtual void FindSingle_Participation_Test_Returns_Model_When_Found()
		{
			if(this.participation == null)
				throw new AssertInconclusiveException("You must provide a Participation that is already in the repository for this test to be effective.");
			
			//Arrange
			IParticipation actual;
			IParticipationSearch injected = this.searchThatReturnsSomething;
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void FindSingle_Participation_Test_Returns_Null_When_Not_Found()
		{
			//Arrange
			IParticipation actual;
			IParticipationSearch injected = this.searchThatReturnsNull;
			
			//Act
			actual = this.service.FindSingle(injected);
			
			//Assert
			Assert.IsNull(actual);
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void FindSingle_Participation_Test_Throws_Error_When_Search_Is_Null()
		{
			//Arrange
			IParticipationSearch injected = null;
			
			//Act
			this.service.FindSingle(injected);
			
			//Assert
		}
		
		#endregion Find Single
		
		#region Save
		
		[TestMethod]
		public virtual void Save_Multiple_Participations_Test_Preserves_Participations_When_They_Already_Exist()
		{
			if(this.participation == null)
				throw new AssertInconclusiveException("You must provide a Participation that is already in the repository for this test to be effective.");
			
			//Arrange
			List<IParticipation> injected = new List<IParticipation>() { this.participation };
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Participations_Test_Preserves_Participations_When_They_Are_New()
		{
			if(this.participation == null)
				throw new AssertInconclusiveException("You must provide a Participation that is already in the repository for this test to be effective.");
			
			//Arrange
			List<IParticipation> injected = new List<IParticipation>() { this.participationNotFound };
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			IParticipation expected = injected.First();
			IParticipationSearch search = PrepareSearch(expected);
			if(search != null)
			{
				IParticipation actual = this.participationNotFound = this.service.FindSingle(search);
			
				Assert.IsNotNull(actual);
				Assert.AreEqual(expected.PerformedOn, actual.PerformedOn, "PerformedOn");
				Assert.AreEqual(expected.Severity, actual.Severity, "Severity");
			}
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Participations_Test_Preserves_PerformedOn_When_Is_Maxed_And_Participations_Already_Exists()
		{
			if(this.participation == null)
				throw new AssertInconclusiveException("You must provide a Participation that is already in the repository for this test to be effective.");
			
			//Arrange
			IEnumerable<IParticipation> injected = new List<IParticipation>() { this.participation };
			this.participation.PerformedOn = new DateTime(9999, 12, 31, 23, 59, 59);
			
			IParticipation actual = null;
			IParticipation expected = this.participation;
			
			IParticipationSearch search = PrepareSearch(expected);
			if(search == null)
				throw new AssertInconclusiveException("You must override the function that prepares a Participation Search model for this test to be effective.");
			
			this.initial = this.service.FindSingle(this.searchThatReturnsSomething);
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			actual = this.service.FindSingle(search);
			
			Assert.AreEqual(expected.PerformedOn, actual.PerformedOn);
		}
		
		[TestMethod]
		public virtual void Save_Multiple_Participations_Test_Preserves_Severity_When_Is_Maxed_And_Participations_Already_Exists()
		{
			if(this.participation == null)
				throw new AssertInconclusiveException("You must provide a Participation that is already in the repository for this test to be effective.");
			
			//Arrange
			IEnumerable<IParticipation> injected = new List<IParticipation>() { this.participation };
			this.participation.Severity = 1d;
			
			IParticipation actual = null;
			IParticipation expected = this.participation;
			
			IParticipationSearch search = PrepareSearch(expected);
			if(search == null)
				throw new AssertInconclusiveException("You must override the function that prepares a Participation Search model for this test to be effective.");
			
			this.initial = this.service.FindSingle(this.searchThatReturnsSomething);
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			actual = this.service.FindSingle(search);
			
			Assert.AreEqual(expected.Severity, actual.Severity);
		}
		
		[TestMethod]
		public virtual void Save_Participation_Test_Preserves_Participation_When_It_Already_Exists()
		{
			if(this.participation == null)
				throw new AssertInconclusiveException("You must provide a Participation that is already in the repository for this test to be effective.");
			
			//Arrange
			IParticipation injected = this.participation;
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Save_Participation_Test_Preserves_Participation_When_It_Is_New()
		{
			if(this.participation == null)
				throw new AssertInconclusiveException("You must provide a Participation that is already in the repository for this test to be effective.");
			
			//Arrange
			IParticipation injected = this.participationNotFound;
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			IParticipation expected = injected;
			IParticipationSearch search = PrepareSearch(expected);
			if(search != null)
			{
				IParticipation actual = this.participationNotFound = this.service.FindSingle(search);
			
				Assert.IsNotNull(actual);
				Assert.AreEqual(expected.PerformedOn, actual.PerformedOn, "PerformedOn");
				Assert.AreEqual(expected.Severity, actual.Severity, "Severity");
			}
		}
		
		[TestMethod]
		public virtual void Save_Participation_Test_Preserves_PerformedOn_When_Is_Maxed_And_Participations_Already_Exists()
		{
			if(this.participation == null)
				throw new AssertInconclusiveException("You must provide a Participation that is already in the repository for this test to be effective.");
			
			//Arrange
			IParticipation injected = this.participation;
			injected.PerformedOn = new DateTime(9999, 12, 31, 23, 59, 59);
			
			IParticipation actual = null;
			IParticipation expected = injected;
			
			IParticipationSearch search = PrepareSearch(expected);
			if(search == null)
				throw new AssertInconclusiveException("You must override the function that prepares a Participation Search model for this test to be effective.");
			
			this.initial = this.service.FindSingle(this.searchThatReturnsSomething);
			
			//Act
			this.service.Save(injected, null);
			
			//Assert
			actual = this.service.FindSingle(search);
			
			Assert.AreEqual(expected.PerformedOn, actual.PerformedOn);
		}
		
		[TestMethod]
		public virtual void Save_Participation_Test_Preserves_Severity_When_Is_Maxed_And_Participations_Already_Exists()
		{
			if(this.participation == null)
				throw new AssertInconclusiveException("You must provide a Participation that is already in the repository for this test to be effective.");
			
			//Arrange
			IParticipation injected = this.participation;
			injected.Severity = 1d;
			
			IParticipation actual = null;
			IParticipation expected = injected;
			
			IParticipationSearch search = PrepareSearch(expected);
			if(search == null)
				throw new AssertInconclusiveException("You must override the function that prepares a Participation Search model for this test to be effective.");
			
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
