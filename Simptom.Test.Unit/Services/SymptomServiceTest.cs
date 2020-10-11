using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Simptom.Services;
using Simptom.Test.Unit.Core.Services;

namespace Simptom.Test.Unit.Services
{
	[TestClass]
	public class SymptomServiceTest : ISymptomServiceTest
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
			
			this.service = new SymptomService(this.repository.Object, this.symptomCategoryService.Object, this.modelFactory.Object);
		}
	}
}
