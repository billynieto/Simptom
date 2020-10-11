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
	public class ActivityServiceTest : IActivityServiceTest
	{
		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();

			IDbConnection connection = new SqlConnection("Data Source='BADASS-TABLET-P\\SQLEXPRESS'; Initial Catalog = 'Simptom'; User ID = 'SystemUser'; Password = 'insecure'");

			IActivityCategoryRepository activityCategoryRepository = new ActivityCategoryRepository(connection, this.modelFactory);
			IActivityCategoryService activityCategoryService = new ActivityCategoryService(activityCategoryRepository, this.modelFactory);

			IActivityRepository repository = new ActivityRepository(connection, this.modelFactory);
			this.service = new ActivityService(repository, activityCategoryService, this.modelFactory);

			IActivitiesSearch search = this.modelFactory.GenerateActivitiesSearch();
			this.activity = this.service.Find(search).FirstOrDefault();
			
			Guid id = Guid.NewGuid();
			IActivityKey activityKeyNotFound = this.modelFactory.GenerateActivityKey(id);
			this.activityNotFound = this.modelFactory.GenerateActivity(activityKeyNotFound);
			this.activityNotFound.Category = this.activity.Category;
			this.activityNotFound.Name = "Doesn't Exist";

			this.searchMultipleThatFindsNothing = this.modelFactory.GenerateActivitiesSearch();
			this.searchMultipleThatFindsNothing.Name = this.activityNotFound.Name;

			this.searchMultipleThatFindsSomething = this.modelFactory.GenerateActivitiesSearch();
			this.searchMultipleThatFindsSomething.Name = this.activity.Name;

			this.searchThatReturnsNull = this.modelFactory.GenerateActivitySearch();
			this.searchThatReturnsNull.ID = this.activityNotFound.Key.ID;

			this.searchThatReturnsSomething = this.modelFactory.GenerateActivitySearch();
			this.searchThatReturnsSomething.ID = this.activity.Key.ID;
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
		
		public override IActivitySearch PrepareSearch(IActivity activity)
		{
			IActivitySearch search = this.modelFactory.GenerateActivitySearch();
			search.Name = activity.Name;

			return search;
		}
	}
}
