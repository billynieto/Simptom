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
	public class ActivityCategoryServiceTest : IActivityCategoryServiceTest
	{
		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();

			IDbConnection connection = new SqlConnection("Data Source='BADASS-TABLET-P\\SQLEXPRESS'; Initial Catalog = 'Simptom'; User ID = 'SystemUser'; Password = 'insecure'");

			IActivityCategoryRepository repository = new ActivityCategoryRepository(connection, this.modelFactory);
			this.service = new ActivityCategoryService(repository, this.modelFactory);

			IActivityCategoriesSearch search = this.modelFactory.GenerateActivityCategoriesSearch();
			this.activityCategory = this.service.Find(search).FirstOrDefault();
			
			Guid id = Guid.NewGuid();
			IActivityCategoryKey activityCategoryKeyNotFound = this.modelFactory.GenerateActivityCategoryKey(id);
			this.activityCategoryNotFound = this.modelFactory.GenerateActivityCategory(activityCategoryKeyNotFound);
			this.activityCategoryNotFound.Name = "Doesn't Exist";

			this.searchMultipleThatFindsNothing = this.modelFactory.GenerateActivityCategoriesSearch();
			this.searchMultipleThatFindsNothing.Name = this.activityCategoryNotFound.Name;

			this.searchMultipleThatFindsSomething = this.modelFactory.GenerateActivityCategoriesSearch();
			this.searchMultipleThatFindsSomething.Name = this.activityCategory.Name;

			this.searchThatReturnsNull = this.modelFactory.GenerateActivityCategorySearch();
			this.searchThatReturnsNull.ID = this.activityCategoryNotFound.Key.ID;

			this.searchThatReturnsSomething = this.modelFactory.GenerateActivityCategorySearch();
			this.searchThatReturnsSomething.ID = this.activityCategory.Key.ID;
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
		
		public override IActivityCategorySearch PrepareSearch(IActivityCategory activityCategory)
		{
			IActivityCategorySearch search = this.modelFactory.GenerateActivityCategorySearch();
			search.Name = activityCategory.Name;

			return search;
		}
	}
}
