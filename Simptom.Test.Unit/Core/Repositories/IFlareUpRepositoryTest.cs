using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Repository;
using Repository.Framework;

using Simptom.Framework;
using Simptom.Framework.Models;
using Simptom.Framework.Repositories;

namespace Simptom.Test.Unit.Core.Repositories
{
	public abstract class IFlareUpRepositoryTest
	{
		protected Mock<IFlareUp> flareUp;
		protected IList<Mock<IFlareUp>> flareUps;
		protected Mock<IFlareUpKey> key;
		protected IList<Mock<IFlareUpKey>> keys;
		protected Mock<IModelFactory> modelFactory;
		protected IFlareUpRepository repository;
		protected Mock<IFlareUpSearch> search;
		protected Mock<IFlareUpsSearch> searchMultiple;
		protected Mock<ISymptomCategory> symptomCategory;
		protected Mock<ISymptomCategoryKey> symptomCategoryKey;
		protected Mock<ISymptom> symptom;
		protected Mock<ISymptomKey> symptomKey;
		protected IList<Mock<ISymptom>> symptoms;
		protected TestContext testContext;
		protected Mock<IDbTransaction> transaction;
		protected Mock<IUser> user;
		protected Mock<IUserKey> userKey;
		protected IList<Mock<IUser>> users;
		
		public TestContext TestContext { get { return this.testContext; } set { this.testContext = value; } }
		
		[TestInitialize]
		public virtual void TestInitialize()
		{
			this.symptomCategoryKey = new Mock<ISymptomCategoryKey>();
			this.symptomCategoryKey.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.symptomCategory = new Mock<ISymptomCategory>();
			this.symptomCategory.SetupAllProperties();
			this.symptomCategory.Setup(_symptomCategory => _symptomCategory.Key).Returns(() => { return this.symptomCategoryKey.Object; });
			this.symptomCategory.Setup(_symptomCategory => _symptomCategory.Name).Returns("sPHVFFPRQmspouboOSKIwplqCUCIFeHUGuWaswzwPzCGr");
			this.symptomKey = new Mock<ISymptomKey>();
			this.symptomKey.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.symptom = new Mock<ISymptom>();
			this.symptom.SetupAllProperties();
			this.symptom.Setup(_model => _model.Key).Returns(() => { return this.symptomKey.Object; });
			this.symptom.Setup(_symptom => _symptom.Category).Returns(() => this.symptomCategory.Object);
			this.symptom.Setup(_symptom => _symptom.Name).Returns("sPHVFFPRQmspouboOSKIwplqCUCIFeHUGuWaswzwPzCGr");
			this.symptoms = new List<Mock<ISymptom>> { this.symptom };
			this.userKey = new Mock<IUserKey>();
			this.userKey.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.user = new Mock<IUser>();
			this.user.SetupAllProperties();
			this.user.Setup(_model => _model.Key).Returns(() => { return this.userKey.Object; });
			this.user.Setup(_user => _user.Name).Returns("PkUIFRRFzdDqsKdhlLFyIUxbVstoSXMFZDerNXyPcbVTwcdO");
			this.user.Setup(_user => _user.Password).Returns("InRbSqVMTMnyDMCqYqP");
			this.users = new List<Mock<IUser>> { this.user };
			
			this.key = new Mock<IFlareUpKey>();
			this.key.Setup(_key => _key.ID).Returns(Guid.NewGuid());
			this.keys = new List<Mock<IFlareUpKey>>() { this.key };
			
			this.flareUp = new Mock<IFlareUp>();
			this.flareUp.SetupAllProperties();
			this.flareUp.Setup(_model => _model.Key).Returns(() => { return this.key.Object; });
			this.flareUp.Setup(_model => _model.ExperiencedOn).Returns(new DateTime(2020, 5, 22, 5, 48, 51));
			this.flareUp.Setup(_model => _model.Symptom).Returns(() => { return this.symptom.Object; });
			this.flareUp.Setup(_model => _model.Severity).Returns(0.613772524806565d);
			this.flareUp.Setup(_model => _model.User).Returns(() => { return this.user.Object; });
			this.flareUps = new List<Mock<IFlareUp>>() { flareUp };
			
			this.modelFactory = new Mock<IModelFactory>();
			this.modelFactory.Setup(mf => mf.GenerateSymptomCategoryKey(It.IsAny<Guid>())).Returns(this.symptomCategoryKey.Object);
			this.modelFactory.Setup(mf => mf.GenerateSymptomCategory(It.IsAny<ISymptomCategoryKey>())).Returns(this.symptomCategory.Object);
			this.modelFactory.Setup(mf => mf.GenerateSymptomKey(It.IsAny<Guid>())).Returns(this.symptomKey.Object);
			this.modelFactory.Setup(mf => mf.GenerateSymptom(It.IsAny<ISymptomKey>())).Returns(this.symptom.Object);
			this.modelFactory.Setup(mf => mf.GenerateUserKey(It.IsAny<Guid>())).Returns(this.userKey.Object);
			this.modelFactory.Setup(mf => mf.GenerateUser(It.IsAny<IUserKey>())).Returns(this.user.Object);
			this.modelFactory.Setup(mf => mf.GenerateFlareUpKey(It.IsAny<Guid>())).Returns(this.key.Object);
			this.modelFactory.Setup(mf => mf.GenerateFlareUp(It.IsAny<IFlareUpKey>())).Returns(this.flareUp.Object);
			
			this.search = new Mock<IFlareUpSearch>();
			this.searchMultiple = new Mock<IFlareUpsSearch>();
			this.transaction = new Mock<IDbTransaction>();
		}
		
		[TestCleanup]
		public virtual void TestCleanup()
		{
			this.symptomKey = null;
			this.symptom = null;
			this.symptoms = null;
			this.userKey = null;
			this.user = null;
			this.users = null;
			this.transaction = null;
		}
		
		public virtual void AddRow(IFlareUp flareUp)
		{
			throw new NotImplementedException();
		}
		
		public virtual void AddRows(IEnumerable<IFlareUp> flareUps)
		{
			foreach(IFlareUp flareUp in flareUps)
				AddRow(flareUp);
		}
		
		#region Delete
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_FlareUp_Test_Throws_Error_When_Key_Is_Null()
		{
			//Arrange
			IFlareUpKey injected = null;
			
			//Act
			this.repository.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Delete_Multiple_FlareUps_Test_Throws_Error_When_Any_Key_Is_Null()
		{
			//Arrange
			IList<IFlareUpKey> injected = new List<IFlareUpKey>() { new Mock<IFlareUpKey>().Object, null };
			
			//Act
			this.repository.Delete(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Delete
		
		#region Exists
		
		[TestMethod]
		public virtual void Exists_FlareUp_Test_Returns_False_When_Key_Was_Not_Found()
		{
			//Arrange
			bool actual;
			bool expected = false;
			IFlareUpKey injected = this.key.Object;
			
			//Act
			actual = this.repository.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_FlareUp_Test_Returns_True_When_Key_Was_Found()
		{
			//Arrange
			bool actual;
			bool expected = true;
			IFlareUpKey injected = this.key.Object;
			
			AddRow(this.flareUp.Object);
			
			//Act
			actual = this.repository.Exists(injected);
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_FlareUps_Test_Returns_Empty_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			int actual;
			int expected = 0;
			IEnumerable<IFlareUpKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			//Act
			actual = this.repository.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_FlareUps_Test_Returns_List_Of_Keys_When_They_Were_Found()
		{
			//Arrange
			int actual;
			int expected = 1;
			IEnumerable<IFlareUpKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			AddRow(this.flareUp.Object);
			
			//Act
			actual = this.repository.Exists(injected).Count();
			
			//Assert
			Assert.AreEqual(expected, actual);
		}
		
		[TestMethod]
		public virtual void Exists_FlareUps_Test_Returns_Non_Null_List_When_Keys_Were_Not_Found()
		{
			//Arrange
			IEnumerable<IFlareUpKey> actual;
			IEnumerable<IFlareUpKey> injected = this.keys.Select(_key => _key.Object).ToList();
			
			//Act
			actual = this.repository.Exists(injected);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		#endregion Exists
		
		#region Insert
		
		[TestMethod]
		public virtual void Insert_FlareUp_Test_Gets_FlareUp_ExperiencedOn()
		{
			//Arrange
			Mock<IFlareUp> expected = this.flareUp;
			
			//Act
			this.repository.Insert(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.ExperiencedOn, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Insert_FlareUp_Test_Gets_FlareUp_Severity()
		{
			//Arrange
			Mock<IFlareUp> expected = this.flareUp;
			
			//Act
			this.repository.Insert(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Severity, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Insert_FlareUp_Test_Gets_FlareUp_Symptom_Key()
		{
			//Arrange
			Mock<ISymptom> expected = this.symptom;
			Mock<IFlareUp> injected = this.flareUp;
			
			//Act
			this.repository.Insert(injected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Insert_FlareUp_Test_Gets_FlareUp_User_Key()
		{
			//Arrange
			Mock<IUser> expected = this.user;
			Mock<IFlareUp> injected = this.flareUp;
			
			//Act
			this.repository.Insert(injected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Insert_FlareUp_Test_Throws_Error_When_FlareUp_Is_Null()
		{
			//Arrange
			IFlareUp injected = null;
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Insert_Multiple_FlareUps_Test_Gets_ExperiencedOn_For_Every_FlareUp()
		{
			//Arrange
			IList<Mock<IFlareUp>> expected = new List<Mock<IFlareUp>>() { this.flareUp };
			IList<IFlareUp> injected = this.flareUps.Select(e => e.Object).ToList();
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<IFlareUp> flareUp in expected)
				flareUp.VerifyGet(_flareUp => _flareUp.ExperiencedOn, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Insert_Multiple_FlareUps_Test_Gets_Keys_For_FlareUp_Symptom()
		{
			//Arrange
			IList<Mock<ISymptom>> expected = this.symptoms;
			IList<IFlareUp> injected = this.flareUps.Select(e => e.Object).ToList();
			
			//Act
			this.repository.Insert(injected, this.transaction.Object );
			
			//Assert
			foreach(Mock<ISymptom> symptom in expected)
				symptom.VerifyGet(_symptom => _symptom.Key);
		}
		
		[TestMethod]
		public virtual void Insert_Multiple_FlareUps_Test_Gets_Keys_For_FlareUp_User()
		{
			//Arrange
			IList<Mock<IUser>> expected = this.users;
			IList<IFlareUp> injected = this.flareUps.Select(e => e.Object).ToList();
			
			//Act
			this.repository.Insert(injected, this.transaction.Object );
			
			//Assert
			foreach(Mock<IUser> user in expected)
				user.VerifyGet(_user => _user.Key);
		}
		
		[TestMethod]
		public virtual void Insert_Multiple_FlareUps_Test_Gets_Severity_For_Every_FlareUp()
		{
			//Arrange
			IList<Mock<IFlareUp>> expected = new List<Mock<IFlareUp>>() { this.flareUp };
			IList<IFlareUp> injected = this.flareUps.Select(e => e.Object).ToList();
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<IFlareUp> flareUp in expected)
				flareUp.VerifyGet(_flareUp => _flareUp.Severity, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Insert_Multiple_FlareUps_Test_Throws_Error_When_Any_FlareUp_Is_Null()
		{
			//Arrange
			IEnumerable<IFlareUp> injected = new List<IFlareUp>() { this.flareUp.Object, null };
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Insert_Multiple_FlareUps_Test_Throws_Error_When_FlareUp_Is_Null()
		{
			//Arrange
			IEnumerable<IFlareUp> injected = null;
			
			//Act
			this.repository.Insert(injected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Insert
		
		#region Select
		
		[TestMethod]
		public virtual void Select_FlareUps_Test_Calls_Generate_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IFlareUpsSearch> injected = this.searchMultiple;
			
			AddRow(this.flareUp.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateFlareUp(It.IsAny<IFlareUpKey>()));
		}
		
		[TestMethod]
		public virtual void Select_FlareUps_Test_Calls_GenerateFlareUpKey_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IFlareUpsSearch> injected = this.searchMultiple;
			
			AddRow(this.flareUp.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateFlareUpKey(It.IsAny<Guid>()));
		}
		
		#endregion Select
		
		#region Select Single
		
		[TestMethod]
		public virtual void SelectSingle_FlareUp_Test_Calls_Generate_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IFlareUpSearch> injected = this.search;
			
			AddRow(this.flareUp.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateFlareUp(It.IsAny<IFlareUpKey>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_FlareUp_Test_Calls_Generate_On_ModelFactory_For_Symptom()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IFlareUpSearch> injected = this.search;
			
			AddRow(this.flareUp.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateSymptom(It.IsAny<ISymptomKey>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_FlareUp_Test_Calls_Generate_On_ModelFactory_For_User()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IFlareUpSearch> injected = this.search;
			
			AddRow(this.flareUp.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateUser(It.IsAny<IUserKey>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_FlareUp_Test_Calls_GenerateFlareUpKey_On_ModelFactory()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IFlareUpSearch> injected = this.search;
			
			AddRow(this.flareUp.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateFlareUpKey(It.IsAny<Guid>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_FlareUp_Test_Calls_GenerateSymptomKey_On_ModelFactory_For_Symptom()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IFlareUpSearch> injected = this.search;
			
			AddRow(this.flareUp.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateSymptomKey(It.IsAny<Guid>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_FlareUp_Test_Calls_GenerateUserKey_On_ModelFactory_For_User()
		{
			//Arrange
			Mock<IModelFactory> expected = this.modelFactory;
			Mock<IFlareUpSearch> injected = this.search;
			
			AddRow(this.flareUp.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.Verify(e => e.GenerateUserKey(It.IsAny<Guid>()));
		}
		
		[TestMethod]
		public virtual void SelectSingle_FlareUp_Test_Returns_FlareUp_When_Is_Found()
		{
			//Arrange
			IFlareUp actual;
			Mock<IFlareUp> expected = this.flareUp;
			Mock<IFlareUpSearch> injected = this.search;
			
			AddRow(expected.Object);
			
			//Act
			actual = this.repository.Select(injected.Object);
			
			//Assert
			Assert.IsNotNull(actual);
		}
		
		[TestMethod]
		public virtual void SelectSingle_FlareUp_Test_Returns_Null_When_FlareUp_Is_Not_Found()
		{
			//Arrange
			IFlareUp actual;
			Mock<IFlareUpSearch> injected = this.search;
			
			//Act
			actual = this.repository.Select(injected.Object);
			
			//Assert
			Assert.IsNull(actual);
		}
		
		[TestMethod]
		public virtual void SelectSingle_FlareUp_Test_Sets_FlareUp_ExperiencedOn()
		{
			//Arrange
			Mock<IFlareUp> expected = this.flareUp;
			Mock<IFlareUpSearch> injected = this.search;
			
			AddRow(expected.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.VerifySet(e => e.ExperiencedOn = It.IsAny<DateTime>());
		}
		
		[TestMethod]
		public virtual void SelectSingle_FlareUp_Test_Sets_FlareUp_Severity()
		{
			//Arrange
			Mock<IFlareUp> expected = this.flareUp;
			Mock<IFlareUpSearch> injected = this.search;
			
			AddRow(expected.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.VerifySet(e => e.Severity = It.IsAny<double>());
		}
		
		[TestMethod]
		public virtual void SelectSingle_FlareUp_Test_Sets_FlareUp_Symptom_Properties()
		{
			//Arrange
			Mock<ISymptom> expected = this.symptom;
			Mock<IFlareUpSearch> injected = this.search;
			
			AddRow(this.flareUp.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.VerifySet(e => e.Category = It.IsAny<ISymptomCategory>(), "Symptom Category");
			expected.VerifySet(e => e.Name = It.IsAny<string>(), "Symptom Name");
		}
		
		[TestMethod]
		public virtual void SelectSingle_FlareUp_Test_Sets_FlareUp_User_Properties()
		{
			//Arrange
			Mock<IUser> expected = this.user;
			Mock<IFlareUpSearch> injected = this.search;
			
			AddRow(this.flareUp.Object);
			
			//Act
			this.repository.Select(injected.Object);
			
			//Assert
			expected.VerifySet(e => e.Name = It.IsAny<string>(), "User Name");
			expected.VerifySet(e => e.Password = It.IsAny<string>(), "User Password");
		}
		
		#endregion Select Single
		
		#region Update
		
		[TestMethod]
		public virtual void Update_FlareUp_Test_Gets_FlareUp_ExperiencedOn()
		{
			//Arrange
			Mock<IFlareUp> expected = this.flareUp;
			
			AddRow(expected.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.ExperiencedOn, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_FlareUp_Test_Gets_FlareUp_Key()
		{
			//Arrange
			Mock<IFlareUp> expected = this.flareUp;
			
			AddRow(expected.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_FlareUp_Test_Gets_FlareUp_Severity()
		{
			//Arrange
			Mock<IFlareUp> expected = this.flareUp;
			
			AddRow(expected.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(expected.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Severity, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_FlareUp_Test_Gets_FlareUp_Symptom_Key()
		{
			//Arrange
			Mock<ISymptom> expected = this.symptom;
			
			AddRow(this.flareUp.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(this.flareUp.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_FlareUp_Test_Gets_FlareUp_User_Key()
		{
			//Arrange
			Mock<IUser> expected = this.user;
			
			AddRow(this.flareUp.Object);
			
			//Act
			expected.Invocations.Clear();
			
			this.repository.Update(this.flareUp.Object, this.transaction.Object);
			
			//Assert
			expected.VerifyGet(e => e.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Update_FlareUp_Test_Throws_Error_When_FlareUp_Is_Null()
		{
			//Arrange
			IFlareUp injected = null;
			
			//Act
			this.repository.Update(injected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		public virtual void Update_Multiple_FlareUps_Test_Gets_ExperiencedOn_For_FlareUp()
		{
			//Arrange
			IList<Mock<IFlareUp>> expected = new List<Mock<IFlareUp>>() { this.flareUp };
			
			AddRow(this.flareUp.Object);
			
			//Act
			foreach(Mock<IFlareUp> flareUp in expected)
				flareUp.Invocations.Clear();
			
			this.repository.Update(expected.Select(e => e.Object).ToList(), this.transaction.Object);
			
			//Assert
			foreach(Mock<IFlareUp> flareUp in expected)
				flareUp.VerifyGet(m => m.ExperiencedOn, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Multiple_FlareUps_Test_Gets_Key_For_FlareUp()
		{
			//Arrange
			IList<Mock<IFlareUp>> expected = new List<Mock<IFlareUp>>() { this.flareUp };
			IList<IFlareUp> injected = expected.Select(e => e.Object).ToList();
			
			AddRow(this.flareUp.Object);
			
			//Act
			foreach(Mock<IFlareUp> flareUp in expected)
				flareUp.Invocations.Clear();
			
			this.repository.Update(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<IFlareUp> flareUp in expected)
				flareUp.VerifyGet(m => m.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Multiple_FlareUps_Test_Gets_Key_For_FlareUp_Symptom()
		{
			//Arrange
			IList<Mock<ISymptom>> expected = this.symptoms;
			IList<IFlareUp> injected = this.flareUps.Select(_mock => _mock.Object).ToList();
			
			AddRow(this.flareUp.Object);
			
			//Act
			foreach(Mock<ISymptom> symptom in expected)
				symptom.Invocations.Clear();
			
			this.repository.Update(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<ISymptom> symptom in expected)
				symptom.VerifyGet(m => m.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Multiple_FlareUps_Test_Gets_Key_For_FlareUp_User()
		{
			//Arrange
			IList<Mock<IUser>> expected = this.users;
			IList<IFlareUp> injected = this.flareUps.Select(_mock => _mock.Object).ToList();
			
			AddRow(this.flareUp.Object);
			
			//Act
			foreach(Mock<IUser> user in expected)
				user.Invocations.Clear();
			
			this.repository.Update(injected, this.transaction.Object);
			
			//Assert
			foreach(Mock<IUser> user in expected)
				user.VerifyGet(m => m.Key, Times.AtLeast(1));
		}
		
		[TestMethod]
		public virtual void Update_Multiple_FlareUps_Test_Gets_Severity_For_FlareUp()
		{
			//Arrange
			IList<Mock<IFlareUp>> expected = new List<Mock<IFlareUp>>() { this.flareUp };
			
			AddRow(this.flareUp.Object);
			
			//Act
			foreach(Mock<IFlareUp> flareUp in expected)
				flareUp.Invocations.Clear();
			
			this.repository.Update(expected.Select(e => e.Object).ToList(), this.transaction.Object);
			
			//Assert
			foreach(Mock<IFlareUp> flareUp in expected)
				flareUp.VerifyGet(m => m.Severity, Times.AtLeast(1));
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Update_Multiple_FlareUps_Test_Throws_Error_When_Any_FlareUp_Is_Null()
		{
			//Arrange
			IEnumerable<IFlareUp> expected = new List<IFlareUp>() { this.flareUp.Object, null };
			
			//Act
			this.repository.Update(expected, this.transaction.Object);
			
			//Assert
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public virtual void Update_Multiple_FlareUps_Test_Throws_Error_When_FlareUp_Is_Null()
		{
			//Arrange
			IEnumerable<IFlareUp> expected = null;
			
			//Act
			this.repository.Update(expected, this.transaction.Object);
			
			//Assert
		}
		
		#endregion Update
	}
}
