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
	public partial class SymptomService : ServiceBase<ISymptom, ISymptomKey, ISymptomSearch, ISymptomsSearch>, ISymptomService
	{
		protected IModelFactory modelFactory;
		protected ISymptomCategoryService symptomCategoryService;
		protected ISymptomRepository symptomRepository;
		
		public ISymptomCategoryService SymptomCategoryService { get { return this.symptomCategoryService; } }
		
		public SymptomService(ISymptomRepository symptomRepository, IModelFactory modelFactory)
			: base(symptomRepository)
		{
			this.modelFactory = modelFactory;
			this.symptomRepository = (ISymptomRepository)this.repository;
		}
		
		public SymptomService(ISymptomRepository repository, ISymptomCategoryService symptomCategoryService, IModelFactory modelFactory)
			: this(repository, modelFactory)
		{
			this.symptomCategoryService = symptomCategoryService;
		}
		
		/// <summary>
		/// The Delete function is available in the non-designer portion of the class in case you need to customize it for any reason (e.g. dealing with AS400 Connections).  It is also being printed in the designer portion as it would if this class was being created new, so you may easily copy and paste to apply any needed, automated changes.
		/// </summary>
		/// </param name="IEnumerable<ISymptomKey>"></param>
		/// </param name="IDbTransaction"></param>
		//public override void Delete(IEnumerable<ISymptomKey> keys, IDbTransaction transaction)
		//{
		//	if(keys == null)
		//		throw new ArgumentNullException("Symptoms");
		//	foreach(ISymptomKey key in keys)
		//		if(key == null)
		//			throw new ArgumentNullException("Individual Symptom");
		//	foreach(ISymptomKey key in keys)
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
		//			transaction = this.symptomRepository.StartTransaction();
		//		}
		//	
		//		IEnumerable<ISymptomKey> savedKeys = this.repository.Exists(keys);
		//		IEnumerable<ISymptomKey> notSavedKeys = keys.Where(_key => !savedKeys.Any(_savedKey => _savedKey.Equals(_key)));
		//	
		//		if(notSavedKeys.Count() > 0)
		//		{
		//			string spacer = "; ";
		//	
		//			StringBuilder stringBuilder = new StringBuilder();
		//			foreach (ISymptomKey key in notSavedKeys)
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
		//public override void Save(IEnumerable<ISymptom> symptoms, IDbTransaction transaction)
		//{
		//	if(this.symptomCategoryService == null)
		//		throw new ArgumentNullException("SymptomCategoryService was not provided when the SymptomService was created.  This is required when saving a Symptom.");
		//	
		//	if(symptoms == null)
		//		throw new ArgumentNullException("Symptoms");
		//	foreach(ISymptom symptom in symptoms)
		//		if(symptom == null)
		//			throw new ArgumentNullException("Individual Symptom");
		//	foreach(ISymptom symptom in symptoms)
		//		symptom.Validate();
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
		//			transaction = this.symptomRepository.StartTransaction();
		//		}
		//	
		//		IEnumerable<ISymptomCategoryKey> symptomCategoryKeys = (from _symptom in symptoms where _symptom.Category != null select _symptom.Category.Key).ToList();
		//		if(symptomCategoryKeys.Count() > 0)
		//		{
		//			IEnumerable<ISymptomCategoryKey> savedSymptomCategoryKeys = this.symptomCategoryService.Exists(symptomCategoryKeys);
		//			IEnumerable<ISymptomCategoryKey> missingSymptomCategoryKeys = symptomCategoryKeys.Where(_key => !savedSymptomCategoryKeys.Any(_storedKey => _storedKey.Equals(_key)));
		//			if(missingSymptomCategoryKeys.Count() > 0)
		//				throw new KeyNotFoundException("SymptomCategory: " + RepositoryHelper.ListKeys(missingSymptomCategoryKeys));
		//		}
		//	
		//		IEnumerable<ISymptomKey> keys = symptoms.Select(_symptom => _symptom.Key);
		//		IEnumerable<ISymptomKey> savedKeys = this.repository.Exists(keys);
		//		IEnumerable<ISymptomKey> notSavedKeys = keys.Where(_key => !savedKeys.Any(_savedKey => _savedKey.Equals(_key)));
		//	
		//		this.repository.Insert(symptoms.Where(_symptom => notSavedKeys.Any(_key => _symptom.Key.Equals(_key))), transaction);
		//		this.repository.Update(symptoms.Where(_symptom => savedKeys.Any(_key => _symptom.Key.Equals(_key))), transaction);
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
		
		//public override void Save(ISymptom symptom, IDbTransaction transaction)
		//{
		//	if(this.symptomCategoryService == null)
		//		throw new ArgumentNullException("SymptomCategoryService was not provided when the SymptomService was created.  This is required when saving Symptom.");
		//	
		//	if(symptom == null)
		//		throw new ArgumentNullException("Symptom");
		//	symptom.Validate();
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
		//			transaction = this.symptomRepository.StartTransaction();
		//		}
		//	
		//		if(symptom != null && !this.symptomCategoryService.Exists(symptom.Category.Key))
		//			throw new KeyNotFoundException("SymptomCategory: " + RepositoryHelper.ListKeys(new List<ISymptomCategoryKey>() { symptom.Category.Key }));
		//	
		//		if(!this.repository.Exists(symptom.Key))
		//			this.repository.Insert(symptom, transaction);
		//		else
		//			this.repository.Update(symptom, transaction);
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
