using System;

using Repository.Framework;

namespace Simptom.Framework.Models
{
	public partial interface ISymptom
	{
	}

	public interface ISymptomSearch : ISingleSearch
	{
		Guid ID { get; set; }
		string Name { get; set; }
	}

    public interface ISymptomsSearch : IMultipleSearch
    {
        string Name { get; set; }
    }
}
