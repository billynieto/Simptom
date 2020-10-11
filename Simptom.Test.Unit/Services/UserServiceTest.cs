using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Simptom.Services;
using Simptom.Test.Unit.Core.Services;

namespace Simptom.Test.Unit.Services
{
	[TestClass]
	public class UserServiceTest : IUserServiceTest
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
			
			this.service = new UserService(this.repository.Object, this.modelFactory.Object);
		}
	}
}
