using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Simptom.Services;
using Simptom.Test.Unit.Core.Services;

namespace Simptom.Test.Unit.Services
{
	[TestClass]
	public class ParticipationServiceTest : IParticipationServiceTest
	{
		[TestCleanup]
		public override void TestCleanup()
		{
			base.TestCleanup();
			
			this.service = null;
		}
		
		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();
			
			this.service = new ParticipationService(this.repository.Object, this.activityService.Object, this.userService.Object, this.modelFactory.Object);
		}
	}
}
