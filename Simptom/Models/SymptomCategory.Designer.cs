using System;
using System.Collections.Generic;

using Repository;
using Repository.Framework;

using Simptom.Framework;
using Simptom.Framework.Models;

namespace Simptom.Models
{
	public partial class SymptomCategory : Model<ISymptomCategoryKey>, ISymptomCategory
	{
		public string Name { get; set; }

		public SymptomCategory(ISymptomCategoryKey symptomCategoryKey)
			: base(symptomCategoryKey)
		{
		}
		
		#region IModel
		
		public override void Validate()
		{
			base.Validate();
			
			RepositoryHelper.ValidateNotNull("Name", this.Name);
			RepositoryHelper.ValidateBounds("Name", this.Name, 1, 50);
		}
		
		#endregion IModel
	}

	public partial class SymptomCategoryKey : ISymptomCategoryKey
	{
		public Guid ID { get; set; }
		
		public SymptomCategoryKey(Guid id)
		{
			this.ID = id;
		}
		
		public int CompareTo(object obj)
		{
			throw new NotImplementedException();
		}
		
		public override bool Equals(object obj)
		{
			if(obj == null)
				throw new ArgumentNullException("Object was null when comparing SymptomCategory keys");
			
			if(!(obj is ISymptomCategoryKey))
				return false;
			
			ISymptomCategoryKey symptomCategoryKey = (ISymptomCategoryKey)obj;
			
			return this.ID == symptomCategoryKey.ID;
		}
		
		public void Validate()
		{
		}
	}
}
