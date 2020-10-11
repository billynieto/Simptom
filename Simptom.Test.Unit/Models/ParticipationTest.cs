using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Simptom.Framework.Models;
using Simptom.Models;
using Simptom.Test.Factories;

namespace Simptom.Test.Unit.Core.Models
{
	[TestClass]
	public class ParticipationTest : IParticipationTest
	{
		[TestCleanup]
		public override void TestCleanup()
		{
			base.TestCleanup();
			
			this.modelFactory = null;
		}
		
		[TestInitialize]
		public override void TestInitialize()
		{
			this.modelFactory = new ValidModelFactory();
			
			base.TestInitialize();
		}
	}
}
