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
	public class ParticipationServiceTest : IParticipationServiceTest
	{
		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();

			IDbConnection connection = new SqlConnection("Data Source='BADASS-TABLET-P\\SQLEXPRESS'; Initial Catalog = 'Simptom'; User ID = 'SystemUser'; Password = 'insecure'");

			IActivityRepository activityRepository = new ActivityRepository(connection, this.modelFactory);
			IActivityService activityService = new ActivityService(activityRepository, this.modelFactory);

			IUserRepository userRepository = new UserRepository(connection, this.modelFactory);
			IUserService userService = new UserService(userRepository, this.modelFactory);

			IParticipationRepository repository = new ParticipationRepository(connection, this.modelFactory);
			this.service = new ParticipationService(repository, activityService, userService, this.modelFactory);

			IParticipationsSearch search = this.modelFactory.GenerateParticipationsSearch();
			this.participation = this.service.Find(search).FirstOrDefault();
			
			Guid id = Guid.NewGuid();
			IParticipationKey participationKeyNotFound = this.modelFactory.GenerateParticipationKey(id);
			this.participationNotFound = this.modelFactory.GenerateParticipation(participationKeyNotFound);
			this.participationNotFound.Activity = this.participation.Activity;
			this.participationNotFound.PerformedOn = DateTime.Now;
			this.participationNotFound.Severity = 0.3d;
			this.participationNotFound.User = this.participation.User;

			this.searchMultipleThatFindsNothing = this.modelFactory.GenerateParticipationsSearch();
			this.searchMultipleThatFindsNothing.Start = DateTime.Now.AddDays(1);
			this.searchMultipleThatFindsNothing.UserID = this.participationNotFound.User.Key.ID;

			this.searchMultipleThatFindsSomething = this.modelFactory.GenerateParticipationsSearch();
			this.searchMultipleThatFindsSomething.UserID = this.participation.User.Key.ID;

			this.searchThatReturnsNull = this.modelFactory.GenerateParticipationSearch();
			this.searchThatReturnsNull.PerformedOn = this.participationNotFound.PerformedOn;
			this.searchThatReturnsNull.ID = this.participationNotFound.Key.ID;

			this.searchThatReturnsSomething = this.modelFactory.GenerateParticipationSearch();
			this.searchThatReturnsSomething.PerformedOn = this.participation.PerformedOn;
			this.searchThatReturnsSomething.ID = this.participation.Key.ID;
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
		
		public override IParticipationSearch PrepareSearch(IParticipation participation)
		{
			IParticipationSearch search = this.modelFactory.GenerateParticipationSearch();

			if (participation.Key.ID != Guid.Empty)
			{
				search.ID = participation.Key.ID;
			}
			else
			{
				search.PerformedOn = participation.PerformedOn;
				search.UserID = participation.User.Key.ID;
			}

			return search;
		}
	}
}
