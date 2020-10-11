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
	public class FlareUpServiceTest : IFlareUpServiceTest
	{
		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();

			IDbConnection connection = new SqlConnection("Data Source='BADASS-TABLET-P\\SQLEXPRESS'; Initial Catalog = 'Simptom'; User ID = 'SystemUser'; Password = 'insecure'");

			ISymptomRepository symptomRepository = new SymptomRepository(connection, this.modelFactory);
			ISymptomService symptomService = new SymptomService(symptomRepository, this.modelFactory);

			IUserRepository userRepository = new UserRepository(connection, this.modelFactory);
			IUserService userService = new UserService(userRepository, this.modelFactory);

			IFlareUpRepository repository = new FlareUpRepository(connection, this.modelFactory);
			this.service = new FlareUpService(repository, symptomService, userService, this.modelFactory);

			IFlareUpsSearch search = this.modelFactory.GenerateFlareUpsSearch();
			this.flareUp = this.service.Find(search).FirstOrDefault();
			
			Guid id = Guid.NewGuid();
			IFlareUpKey flareUpKeyNotFound = this.modelFactory.GenerateFlareUpKey(id);
			this.flareUpNotFound = this.modelFactory.GenerateFlareUp(flareUpKeyNotFound);
			this.flareUpNotFound.ExperiencedOn = DateTime.Now;
			this.flareUpNotFound.Severity = 0.4d;
			this.flareUpNotFound.Symptom = this.flareUp.Symptom;
			this.flareUpNotFound.User = this.flareUp.User;

			this.searchMultipleThatFindsNothing = this.modelFactory.GenerateFlareUpsSearch();
			this.searchMultipleThatFindsNothing.Start = DateTime.Now.AddDays(1);
			this.searchMultipleThatFindsNothing.UserID = this.flareUpNotFound.User.Key.ID;

			this.searchMultipleThatFindsSomething = this.modelFactory.GenerateFlareUpsSearch();
			this.searchMultipleThatFindsSomething.UserID = this.flareUp.User.Key.ID;

			this.searchThatReturnsNull = this.modelFactory.GenerateFlareUpSearch();
			this.searchThatReturnsNull.ExperiencedOn = this.flareUpNotFound.ExperiencedOn;
			this.searchThatReturnsNull.ID = this.flareUpNotFound.Key.ID;

			this.searchThatReturnsSomething = this.modelFactory.GenerateFlareUpSearch();
			this.searchThatReturnsSomething.ExperiencedOn = this.flareUp.ExperiencedOn;
			this.searchThatReturnsSomething.ID = this.flareUp.Key.ID;
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

		public override IFlareUpSearch PrepareSearch(IFlareUp flareUp)
		{
			IFlareUpSearch search = this.modelFactory.GenerateFlareUpSearch();

			if (flareUp.Key.ID != Guid.Empty)
			{
				search.ID = flareUp.Key.ID;
			}
			else
			{
				search.ExperiencedOn = flareUp.ExperiencedOn;
				search.UserID = flareUp.User.Key.ID;
			}

			return search;
		}
	}
}
