using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Simptom.Framework.Models;
using Simptom.Framework.Repositories;
using Simptom.Framework.Services;
using Simptom.Server.Repositories;
using Simptom.Services;
using Simptom.Test.Integration.Core.Services;

namespace Simptom.Test.Integration.Services
{
	[TestClass]
	public class SymptomServiceTest : ISymptomServiceTest
	{
		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();

			IDbConnection connection = new SqlConnection("Data Source='BADASS-TABLET-P\\SQLEXPRESS'; Initial Catalog = 'Simptom'; User ID = 'SystemUser'; Password = 'insecure'");

			ISymptomCategoryRepository symptomCategoryRepository = new SymptomCategoryRepository(connection, this.modelFactory);
			ISymptomCategoryService symptomCategoryService = new SymptomCategoryService(symptomCategoryRepository, this.modelFactory);

			ISymptomRepository repository = new SymptomRepository(connection, this.modelFactory);
			this.service = new SymptomService(repository, symptomCategoryService, this.modelFactory);

			ISymptomsSearch search = this.modelFactory.GenerateSymptomsSearch();
			this.symptom = this.service.Find(search).FirstOrDefault();
			
			Guid id = Guid.NewGuid();
			ISymptomKey symptomKeyNotFound = this.modelFactory.GenerateSymptomKey(id);
			this.symptomNotFound = this.modelFactory.GenerateSymptom(symptomKeyNotFound);
			this.symptomNotFound.Category = this.symptom.Category;
			this.symptomNotFound.Name = "Doesn't Exist";

			this.searchMultipleThatFindsNothing = this.modelFactory.GenerateSymptomsSearch();
			this.searchMultipleThatFindsNothing.Name = this.symptomNotFound.Name;

			this.searchMultipleThatFindsSomething = this.modelFactory.GenerateSymptomsSearch();
			this.searchMultipleThatFindsSomething.Name = this.symptom.Name;

			this.searchThatReturnsNull = this.modelFactory.GenerateSymptomSearch();
			this.searchThatReturnsNull.ID = this.symptomNotFound.Key.ID;

			this.searchThatReturnsSomething = this.modelFactory.GenerateSymptomSearch();
			this.searchThatReturnsSomething.ID = this.symptom.Key.ID;
		}
		
		[TestCleanup]
		public override void TestCleanup()
		{
			base.TestCleanup();
			
			this.service = null;
			
			this.searchMultipleThatFindsNothing = null;
			this.searchMultipleThatFindsSomething = null;
			this.searchThatReturnsNull = null;
			this.searchThatReturnsSomething = null;
		}
		
		public override ISymptomSearch PrepareSearch(ISymptom symptom)
		{
			ISymptomSearch search = this.modelFactory.GenerateSymptomSearch();
			search.Name = symptom.Name;

			return search;
		}
	}
}
