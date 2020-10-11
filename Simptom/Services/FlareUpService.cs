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
	public partial class FlareUpService : IFlareUpService
	{
		/// <summary>
		/// The Delete function is available in the non-designer portion of the class in case you need to customize it for any reason (e.g. dealing with AS400 Connections).  It is also being printed in the designer portion as it would if this class was being created new, so you may easily copy and paste to apply any needed, automated changes.
		/// </summary>
		/// </param name="IEnumerable<IFlareUpKey>"></param>
		/// </param name="IDbTransaction"></param>
		public override void Delete(IEnumerable<IFlareUpKey> keys, IDbTransaction transaction)
		{
			if(keys == null)
				throw new ArgumentNullException("FlareUps");
			foreach(IFlareUpKey key in keys)
				if(key == null)
					throw new ArgumentNullException("Individual FlareUp");
			foreach(IFlareUpKey key in keys)
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
			
					transaction = this.flareUpRepository.StartTransaction();
				}
			
				IEnumerable<IFlareUpKey> savedKeys = this.repository.Exists(keys, transaction);
				IEnumerable<IFlareUpKey> notSavedKeys = keys.Where(_key => !savedKeys.Any(_savedKey => _savedKey.Equals(_key)));
			
				if(notSavedKeys.Count() > 0)
				{
					string spacer = "; ";
			
					StringBuilder stringBuilder = new StringBuilder();
					foreach (IFlareUpKey key in notSavedKeys)
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
		/// </param name="IFlareUpSearch"></param>
		public override IFlareUp FindSingle(IFlareUpSearch flareUpSearch)
		{
			if(flareUpSearch == null)
				throw new ArgumentNullException("FlareUpSearch");
			
			IFlareUp flareUp = base.FindSingle(flareUpSearch);
			
			if(flareUp != null)
			{
			}
			
			return flareUp;
		}
		
		/// <summary>
		/// The Save function is available in the non-designer portion of the class in case you need to customize it for any reason (e.g. dealing with AS400 Connections).  It is also being printed in the designer portion as it would if this class was being created new, so you may easily copy and paste to apply any needed, automated changes.
		/// </summary>
		/// </param name="IEnumerable"></param>
		/// </param name="IDbTransaction"></param>
		public override void Save(IEnumerable<IFlareUp> flareUps, IDbTransaction transaction)
		{
			if(this.symptomService == null)
				throw new ArgumentNullException("SymptomService was not provided when the FlareUpService was created.  This is required when saving a FlareUp.");
			
			if(flareUps == null)
				throw new ArgumentNullException("FlareUps");
			foreach(IFlareUp flareUp in flareUps)
				if(flareUp == null)
					throw new ArgumentNullException("Individual FlareUp");
			foreach(IFlareUp flareUp in flareUps)
				flareUp.Validate();
			
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
			
					transaction = this.flareUpRepository.StartTransaction();
				}
			
				IEnumerable<ISymptomKey> symptomKeys = (from _flareUp in flareUps where _flareUp.Symptom != null select _flareUp.Symptom.Key).ToList();
				if(symptomKeys.Count() > 0)
				{
					IEnumerable<ISymptomKey> savedSymptomKeys = this.symptomService.Exists(symptomKeys, transaction);
					IEnumerable<ISymptomKey> missingSymptomKeys = symptomKeys.Where(_key => !savedSymptomKeys.Any(_storedKey => _storedKey.Equals(_key)));
					if(missingSymptomKeys.Count() > 0)
						throw new KeyNotFoundException("Symptom: " + RepositoryHelper.ListKeys(missingSymptomKeys));
				}

				IEnumerable<IUserKey> userKeys = (from _flareUp in flareUps where _flareUp.User != null select _flareUp.User.Key).ToList();
				if (userKeys.Count() > 0)
				{
					IEnumerable<IUserKey> savedUserKeys = this.userService.Exists(userKeys, transaction);
					IEnumerable<IUserKey> missingUserKeys = userKeys.Where(_key => !savedUserKeys.Any(_storedKey => _storedKey.Equals(_key)));
					if (missingUserKeys.Count() > 0)
						throw new KeyNotFoundException("Symptom: " + RepositoryHelper.ListKeys(missingUserKeys));
				}

				IEnumerable<IFlareUpKey> keys = flareUps.Select(_flareUp => _flareUp.Key);
				IEnumerable<IFlareUpKey> savedKeys = this.repository.Exists(keys, transaction);
				IEnumerable<IFlareUpKey> notSavedKeys = keys.Where(_key => !savedKeys.Any(_savedKey => _savedKey.Equals(_key)));
			
				this.repository.Insert(flareUps.Where(_flareUp => notSavedKeys.Any(_key => _flareUp.Key.Equals(_key))), transaction);
				this.repository.Update(flareUps.Where(_flareUp => savedKeys.Any(_key => _flareUp.Key.Equals(_key))), transaction);
			
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

		public override void Save(IFlareUp flareUp, IDbTransaction transaction)
		{
			if (flareUp == null)
				throw new ArgumentNullException("Flare Up");

			Save(new List<IFlareUp>() { flareUp }, transaction);
		}
	}
}
