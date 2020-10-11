using Repository.Framework;

using Simptom.Framework.Models;

namespace Simptom.Framework.Repositories
{
	public partial interface IParticipationRepository : IRepository<IParticipation, IParticipationKey, IParticipationSearch, IParticipationsSearch>
	{
	}
}
