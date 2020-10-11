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
	public partial class SymptomCategoryService : ServiceBase<ISymptomCategory, ISymptomCategoryKey, ISymptomCategorySearch, ISymptomCategoriesSearch>, ISymptomCategoryService
	{
		protected IModelFactory modelFactory;
		protected ISymptomCategoryRepository symptomCategoryRepository;
		
		public SymptomCategoryService(ISymptomCategoryRepository symptomCategoryRepository, IModelFactory modelFactory)
			: base(symptomCategoryRepository)
		{
			this.modelFactory = modelFactory;
			this.symptomCategoryRepository = (ISymptomCategoryRepository)this.repository;
		}
		
		/// <summary>
		/// The Delete function is available in the non-designer portion of the class in case you need to customize it for any reason (e.g. dealing with AS400 Connections).  It is also being printed in the designer portion as it would if this class was being created new, so you may easily copy and paste to apply any needed, automated changes.
		/// </summary>
		/// </param name="IEnumerable<ISymptomCategoryKey>"></param>
		/// </param name="IDbTransaction"></param>
		//public override void Delete(IEnumerable<ISymptomCategoryKey> keys, IDbTransaction transaction)
		//{
		//	if(keys == null)
		//		throw new ArgumentNullException("SymptomCategories");
		//	foreach(ISymptomCategoryKey key in keys)
		//		if(key == null)
		//			throw new ArgumentNullException("Individual SymptomCategory");
		//	foreach(ISymptomCategoryKey key in keys)
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
		//			transaction = this.symptomCategoryRepository.StartTransaction();
		//		}
		//	
		//		IEnumerable<ISymptomCategoryKey> savedKeys = this.repository.Exists(keys);
		//		IEnumerable<ISymptomCategoryKey> notSavedKeys = keys.Where(_key => !savedKeys.Any(_savedKey => _savedKey.Equals(_key)));
		//	
		//		if(notSavedKeys.Count() > 0)
		//		{
		//			string spacer = "; ";
		//	
		//			StringBuilder stringBuilder = new StringBuilder();
		//			foreach (ISymptomCategoryKey key in notSavedKeys)
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
		
		/// <summary>
		/// The Save function is available in the non-designer portion of the class in case you need to customize it for any reason (e.g. dealing with AS400 Connections).  It is also being printed in the designer portion as it would if this class was being created new, so you may easily copy and paste to apply any needed, automated changes.
		/// </summary>
		/// </param name="IEnumerable"></param>
		/// </param name="IDbTransaction"></param>
		//public override void Save(IEnumerable<ISymptomCategory> symptomCategories, IDbTransaction transaction)
		//{
		//	if(symptomCategories == null)
		//		throw new ArgumentNullException("SymptomCategories");
		//	foreach(ISymptomCategory symptomCategory in symptomCategories)
		//		if(symptomCategory == null)
		//			throw new ArgumentNullException("Individual SymptomCategory");
		//	foreach(ISymptomCategory symptomCategory in symptomCategories)
		//		symptomCategory.Validate();
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
		//			transaction = this.symptomCategoryRepository.StartTransaction();
		//		}
		//	
		//		IEnumerable<ISymptomCategoryKey> keys = symptomCategories.Select(_symptomCategory => _symptomCategory.Key);
		//		IEnumerable<ISymptomCategoryKey> savedKeys = this.repository.Exists(keys);
		//		IEnumerable<ISymptomCategoryKey> notSavedKeys = keys.Where(_key => !savedKeys.Any(_savedKey => _savedKey.Equals(_key)));
		//	
		//		this.repository.Insert(symptomCategories.Where(_symptomCategory => notSavedKeys.Any(_key => _symptomCategory.Key.Equals(_key))), transaction);
		//		this.repository.Update(symptomCategories.Where(_symptomCategory => savedKeys.Any(_key => _symptomCategory.Key.Equals(_key))), transaction);
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
		
		//public override void Save(ISymptomCategory symptomCategory, IDbTransaction transaction)
		//{
		//	if(symptomCategory == null)
		//		throw new ArgumentNullException("SymptomCategory");
		//	symptomCategory.Validate();
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
		//			transaction = this.symptomCategoryRepository.StartTransaction();
		//		}
		//	
		//		if(!this.repository.Exists(symptomCategory.Key))
		//			this.repository.Insert(symptomCategory, transaction);
		//		else
		//			this.repository.Update(symptomCategory, transaction);
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
