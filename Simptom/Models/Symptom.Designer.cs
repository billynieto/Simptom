using System;
using System.Collections.Generic;

using Repository;
using Repository.Framework;

using Simptom.Framework;
using Simptom.Framework.Models;

namespace Simptom.Models
{
	public partial class Symptom : Model<ISymptomKey>, ISymptom
	{
		public ISymptomCategory Category { get; set; }
		public string Name { get; set; }
		
		public Symptom(ISymptomKey symptomKey)
			: base(symptomKey)
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

	public partial class SymptomKey : ISymptomKey
	{
		public Guid ID { get; set; }
		
		public SymptomKey(Guid id)
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
				throw new ArgumentNullException("Object was null when comparing Symptom keys");
			
			if(!(obj is ISymptomKey))
				return false;
			
			ISymptomKey symptomKey = (ISymptomKey)obj;
			
			return this.ID == symptomKey.ID;
		}
		
		public void Validate()
		{
		}
	}
}
