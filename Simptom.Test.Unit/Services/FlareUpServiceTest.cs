using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Simptom.Services;
using Simptom.Test.Unit.Core.Services;

namespace Simptom.Test.Unit.Services
{
	[TestClass]
	public class FlareUpServiceTest : IFlareUpServiceTest
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
			
			this.service = new FlareUpService(this.repository.Object, this.symptomService.Object, this.userService.Object, this.modelFactory.Object);
		}
	}
}
