using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Repository;

using Simptom.Framework.Models;
using Simptom.Framework.Repositories;
using Simptom.Framework.Services;

namespace Simptom.Services
{
	public partial class ActivityService : ServiceBase<IActivity, IActivityKey, IActivitySearch, IActivitiesSearch>, IActivityService
	{
		protected IActivityCategoryService activityCategoryService;
		protected IActivityRepository activityRepository;
		protected IModelFactory modelFactory;
		
		public IActivityCategoryService ActivityCategoryService { get { return this.activityCategoryService; } }
		
		public ActivityService(IActivityRepository activityRepository, IModelFactory modelFactory)
			: base(activityRepository)
		{
			this.modelFactory = modelFactory;
			this.activityRepository = (IActivityRepository)this.repository;
		}
		
		public ActivityService(IActivityRepository repository, IActivityCategoryService activityCategoryService, IModelFactory modelFactory)
			: this(repository, modelFactory)
		{
			this.activityCategoryService = activityCategoryService;
		}
		
		/// <summary>
		/// The Delete function is available in the non-designer portion of the class in case you need to customize it for any reason (e.g. dealing with AS400 Connections).  It is also being printed in the designer portion as it would if this class was being created new, so you may easily copy and paste to apply any needed, automated changes.
		/// </summary>
		/// </param name="IEnumerable<IActivityKey>"></param>
		/// </param name="IDbTransaction"></param>
		//public override void Delete(IEnumerable<IActivityKey> keys, IDbTransaction transaction)
		//{
		//	if(keys == null)
		//		throw new ArgumentNullException("Activities");
		//	foreach(IActivityKey key in keys)
		//		if(key == null)
		//			throw new ArgumentNullException("Individual Activity");
		//	foreach(IActivityKey key in keys)
		//		key.Validate();
		//	
		//	bool iOpenedTheConnection = false;
		//	bool iStartedTheTransaction = false;
		//	
		//	try
		//	{
		//		if (!this.repository.IsOpen)
		//		{
		//			iOpenedTheConnection = true;
		//	
		//			this.repository.Open();
		//		}
		//	
		//		if(transaction == null)
		//		{
		//			iStartedTheTransaction = true;
		//	
		//			transaction = this.activityRepository.StartTransaction();
		//		}
		//	
		//		IEnumerable<IActivityKey> savedKeys = this.repository.Exists(keys);
		//		IEnumerable<IActivityKey> notSavedKeys = keys.Where(_key => !savedKeys.Any(_savedKey => _savedKey.Equals(_key)));
		//	
		//		if(notSavedKeys.Count() > 0)
		//		{
		//			string spacer = "; ";
		//	
		//			StringBuilder stringBuilder = new StringBuilder();
		//			foreach (IActivityKey key in notSavedKeys)
		//				stringBuilder.Append(key.ToString()).Append(spacer);
		//	
		//			string error = stringBuilder.Remove(stringBuilder.Length - spacer.Length, spacer.Length).ToString();
		//	
		//			throw new KeyNotFoundException(error);
		//		}
		//	
		//		this.repository.Delete(keys, transaction);
		//	
		//		if(iStartedTheTransaction)
		//			transaction.Commit();
		//	}
		//	catch
		//	{
		//		if(iStartedTheTransaction)
		//			transaction.Rollback();
		//	
		//		throw;
		//	}
		//	finally
		//	{
		//		if(iOpenedTheConnection)
		//			this.repository.Close();
		//	
		//		if(iStartedTheTransaction)
		//			transaction.Dispose();
		//	}
		//}
		
		//public override void Save(IActivity activity, IDbTransaction transaction)
		//{
		//	if(activity == null)
		//		throw new ArgumentNullException("Activity");
		//	activity.Validate();
		//	
		//	bool iOpenedTheConnection = false;
		//	bool iStartedTheTransaction = false;
		//	
		//	try
		//	{
		//		if (!this.repository.IsOpen)
		//		{
		//			iOpenedTheConnection = true;
		//	
		//			this.repository.Open();
		//		}
		//	
		//		if(transaction == null)
		//		{
		//			iStartedTheTransaction = true;
		//	
		//			transaction = this.activityRepository.StartTransaction();
		//		}
		//	
		//		if(!this.repository.Exists(activity.Key))
		//			this.repository.Insert(activity, transaction);
		//		else
		//			this.repository.Update(activity, transaction);
		//	
		//		if(iStartedTheTransaction)
		//			transaction.Commit();
		//	}
		//	catch
		//	{
		//		if(iStartedTheTransaction)
		//			transaction.Rollback();
		//	
		//		throw;
		//	}
		//	finally
		//	{
		//		if(iOpenedTheConnection)
		//			this.repository.Close();
		//	
		//		if(iStartedTheTransaction)
		//			transaction.Dispose();
		//	}
		//}
		
		/// <summary>
		/// The Save function is available in the non-designer portion of the class in case you need to customize it for any reason (e.g. dealing with AS400 Connections).  It is also being printed in the designer portion as it would if this class was being created new, so you may easily copy and paste to apply any needed, automated changes.
		/// </summary>
		/// </param name="IEnumerable"></param>
		/// </param name="IDbTransaction"></param>
		//public override void Save(IEnumerable<IActivity> activities, IDbTransaction transaction)
		//{
		//	if(activities == null)
		//		throw new ArgumentNullException("Activities");
		//	foreach(IActivity activity in activities)
		//		if(activity == null)
		//			throw new ArgumentNullException("Individual Activity");
		//	foreach(IActivity activity in activities)
		//		activity.Validate();
		//	
		//	bool iOpenedTheConnection = false;
		//	bool iStartedTheTransaction = false;
		//	
		//	try
		//	{
		//		if (!this.repository.IsOpen)
		//		{
		//			iOpenedTheConnection = true;
		//	
		//			this.repository.Open();
		//		}
		//	
		//		if(transaction == null)
		//		{
		//			iStartedTheTransaction = true;
		//	
		//			transaction = this.activityRepository.StartTransaction();
		//		}
		//	
		//		IEnumerable<IActivityKey> keys = activities.Select(_activity => _activity.Key);
		//		IEnumerable<IActivityKey> savedKeys = this.repository.Exists(keys);
		//		IEnumerable<IActivityKey> notSavedKeys = keys.Where(_key => !savedKeys.Any(_savedKey => _savedKey.Equals(_key)));
		//	
		//		this.repository.Insert(activities.Where(_activity => notSavedKeys.Any(_key => _activity.Key.Equals(_key))), transaction);
		//		this.repository.Update(activities.Where(_activity => savedKeys.Any(_key => _activity.Key.Equals(_key))), transaction);
		//	
		//		if(iStartedTheTransaction)
		//			transaction.Commit();
		//	}
		//	catch
		//	{
		//		if(iStartedTheTransaction)
		//			transaction.Rollback();
		//	
		//		throw;
		//	}
		//	finally
		//	{
		//		if(iOpenedTheConnection)
		//			this.repository.Close();
		//	
		//		if(iStartedTheTransaction)
		//			transaction.Dispose();
		//	}
		//}
	}
}
