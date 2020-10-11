using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Repository;
using Repository.Framework;

using Simptom.Framework.Models;
using Simptom.Framework.Repositories;
using Simptom.Framework.Services;

namespace Simptom.Services
{
	public partial class ParticipationService : IParticipationService
	{
		/// <summary>
		/// The Delete function is available in the non-designer portion of the class in case you need to customize it for any reason (e.g. dealing with AS400 Connections).  It is also being printed in the designer portion as it would if this class was being created new, so you may easily copy and paste to apply any needed, automated changes.
		/// </summary>
		/// </param name="IEnumerable<IParticipationKey>"></param>
		/// </param name="IDbTransaction"></param>
		public override void Delete(IEnumerable<IParticipationKey> keys, IDbTransaction transaction)
		{
			if(keys == null)
				throw new ArgumentNullException("Participations");
			foreach(IParticipationKey key in keys)
				if(key == null)
					throw new ArgumentNullException("Individual Participation");
			foreach(IParticipationKey key in keys)
				key.Validate();
			
			bool iOpenedTheConnection = false;
			bool iStartedTheTransaction = false;
			
			try
			{
				if (!this.repository.IsOpen)
				{
					iOpenedTheConnection = true;
			
					this.repository.Open();
				}
			
				if(transaction == null)
				{
					iStartedTheTransaction = true;
			
					transaction = this.participationRepository.StartTransaction();
				}
			
				IEnumerable<IParticipationKey> savedKeys = this.repository.Exists(keys, transaction);
				IEnumerable<IParticipationKey> notSavedKeys = keys.Where(_key => !savedKeys.Any(_savedKey => _savedKey.Equals(_key)));
			
				if(notSavedKeys.Count() > 0)
				{
					string spacer = "; ";
			
					StringBuilder stringBuilder = new StringBuilder();
					foreach (IParticipationKey key in notSavedKeys)
						stringBuilder.Append(key.ToString()).Append(spacer);
			
					string error = stringBuilder.Remove(stringBuilder.Length - spacer.Length, spacer.Length).ToString();
			
					throw new KeyNotFoundException(error);
				}
			
				this.repository.Delete(keys, transaction);
			
				if(iStartedTheTransaction)
					transaction.Commit();
			}
			catch
			{
				if(iStartedTheTransaction)
					transaction.Rollback();
			
				throw;
			}
			finally
			{
				if(iOpenedTheConnection)
					this.repository.Close();
			
				if(iStartedTheTransaction)
					transaction.Dispose();
			}
		}
		
		/// <summary>
		/// The Find Single function will most likely need to be customized to fit your needs.  If you need to find related Models when a single model is retrieved, you should do it here.  This can be done by specifying what fields to filter for the respective Searches.
		/// </summary>
		/// </param name="IParticipationSearch"></param>
		public override IParticipation FindSingle(IParticipationSearch participationSearch)
		{
			if(participationSearch == null)
				throw new ArgumentNullException("ParticipationSearch");
			
			IParticipation participation = base.FindSingle(participationSearch);
			
			if(participation != null)
			{
			}
			
			return participation;
		}
		
		/// <summary>
		/// The Save function is available in the non-designer portion of the class in case you need to customize it for any reason (e.g. dealing with AS400 Connections).  It is also being printed in the designer portion as it would if this class was being created new, so you may easily copy and paste to apply any needed, automated changes.
		/// </summary>
		/// </param name="IEnumerable"></param>
		/// </param name="IDbTransaction"></param>
		public override void Save(IEnumerable<IParticipation> participations, IDbTransaction transaction)
		{
			if(this.activityService == null)
				throw new ArgumentNullException("ActivityService was not provided when the ParticipationService was created.  This is required when saving a Participation.");
			
			if(participations == null)
				throw new ArgumentNullException("Participations");
			foreach(IParticipation participation in participations)
				if(participation == null)
					throw new ArgumentNullException("Individual Participation");
			foreach(IParticipation participation in participations)
				participation.Validate();
			
			bool iOpenedTheConnection = false;
			bool iStartedTheTransaction = false;
			
			try
			{
				if (!this.repository.IsOpen)
				{
					iOpenedTheConnection = true;
			
					this.repository.Open();
				}
			
				if(transaction == null)
				{
					iStartedTheTransaction = true;
			
					transaction = this.participationRepository.StartTransaction();
				}
			
				IEnumerable<IActivityKey> activityKeys = (from _participation in participations where _participation.Activity != null select _participation.Activity.Key).ToList();
				if(activityKeys.Count() > 0)
				{
					IEnumerable<IActivityKey> savedActivityKeys = this.activityService.Exists(activityKeys, transaction);
					IEnumerable<IActivityKey> missingActivityKeys = activityKeys.Where(_key => !savedActivityKeys.Any(_storedKey => _storedKey.Equals(_key)));
					if(missingActivityKeys.Count() > 0)
						throw new KeyNotFoundException("Activity: " + RepositoryHelper.ListKeys(missingActivityKeys));
				}

				IEnumerable<IUserKey> userKeys = (from _participation in participations where _participation.User != null select _participation.User.Key).ToList();
				if (userKeys.Count() > 0)
				{
					IEnumerable<IUserKey> savedUserKeys = this.userService.Exists(userKeys, transaction);
					IEnumerable<IUserKey> missingUserKeys = userKeys.Where(_key => !savedUserKeys.Any(_storedKey => _storedKey.Equals(_key)));
					if (missingUserKeys.Count() > 0)
						throw new KeyNotFoundException("User: " + RepositoryHelper.ListKeys(missingUserKeys));
				}

				IEnumerable<IParticipationKey> keys = participations.Select(_participation => _participation.Key);
				IEnumerable<IParticipationKey> savedKeys = this.repository.Exists(keys, transaction);
				IEnumerable<IParticipationKey> notSavedKeys = keys.Where(_key => !savedKeys.Any(_savedKey => _savedKey.Equals(_key)));
			
				this.repository.Insert(participations.Where(_participation => notSavedKeys.Any(_key => _participation.Key.Equals(_key))), transaction);
				this.repository.Update(participations.Where(_participation => savedKeys.Any(_key => _participation.Key.Equals(_key))), transaction);
			
				if(iStartedTheTransaction)
					transaction.Commit();
			}
			catch
			{
				if(iStartedTheTransaction)
					transaction.Rollback();
			
				throw;
			}
			finally
			{
				if(iOpenedTheConnection)
					this.repository.Close();
			
				if(iStartedTheTransaction)
					transaction.Dispose();
			}
		}
		
		public override void Save(IParticipation participation, IDbTransaction transaction)
		{
			if (participation == null)
				throw new ArgumentNullException("Participation");

			Save(new List<IParticipation>() { participation }, transaction);
		}
	}
}
