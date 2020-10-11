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
	public class SymptomCategoryServiceTest : ISymptomCategoryServiceTest
	{
		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();

			IDbConnection connection = new SqlConnection("Data Source='BADASS-TABLET-P\\SQLEXPRESS'; Initial Catalog = 'Simptom'; User ID = 'SystemUser'; Password = 'insecure'");

			ISymptomCategoryRepository repository = new SymptomCategoryRepository(connection, this.modelFactory);
			this.service = new SymptomCategoryService(repository, this.modelFactory);

			ISymptomCategoriesSearch search = this.modelFactory.GenerateSymptomCategoriesSearch();
			this.symptomCategory = this.service.Find(search).FirstOrDefault();
			
			Guid id = Guid.NewGuid();
			ISymptomCategoryKey symptomCategoryKeyNotFound = this.modelFactory.GenerateSymptomCategoryKey(id);
			this.symptomCategoryNotFound = this.modelFactory.GenerateSymptomCategory(symptomCategoryKeyNotFound);
			this.symptomCategoryNotFound.Name = "Doesn't Exist";

			this.searchMultipleThatFindsNothing = this.modelFactory.GenerateSymptomCategoriesSearch();
			this.searchMultipleThatFindsNothing.Name = this.symptomCategoryNotFound.Name;

			this.searchMultipleThatFindsSomething = this.modelFactory.GenerateSymptomCategoriesSearch();
			this.searchMultipleThatFindsSomething.Name = this.symptomCategory.Name;
			
			this.searchThatReturnsNull = this.modelFactory.GenerateSymptomCategorySearch();
			this.searchThatReturnsNull.ID = this.symptomCategoryNotFound.Key.ID;

			this.searchThatReturnsSomething = this.modelFactory.GenerateSymptomCategorySearch();
			this.searchThatReturnsSomething.ID = this.symptomCategory.Key.ID;
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
		
		public override ISymptomCategorySearch PrepareSearch(ISymptomCategory symptomCategory)
		{
			ISymptomCategorySearch search = this.modelFactory.GenerateSymptomCategorySearch();
			search.Name = symptomCategory.Name;

			return search;
		}
	}
}
