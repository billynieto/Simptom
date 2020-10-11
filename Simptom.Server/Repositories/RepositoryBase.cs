using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Repository.Framework;

using Simptom.Framework;
using Simptom.Framework.Models;
using Simptom.Framework.Repositories;

namespace Simptom.Server.Repositories
{
	public class RepositoryBase<TModel, TKey, TSingleSearch, TMultipleSearch> : IRepository<TModel, TKey, TSingleSearch, TMultipleSearch>
		where TModel : IModel<TKey>
		where TKey : IKey
		where TSingleSearch : ISingleSearch
		where TMultipleSearch : IMultipleSearch
	{
		protected IDbConnection connection;
		protected IModelFactory modelFactory;
		
		public bool IsOpen { get { return State == ConnectionState.Open; } }
		public ConnectionState State { get { return this.connection.State; } }
		
		public RepositoryBase(IDbConnection connection, IModelFactory modelFactory)
		{
			this.connection = connection;
			this.modelFactory = modelFactory;
		}

		public IDbDataParameter CreateParameter(IDbCommand command, string name, DBNull value)
		{
			IDbDataParameter parameter = command.CreateParameter();
			parameter.ParameterName = name;
			parameter.Value = value;

			command.Parameters.Add(parameter);

			return parameter;
		}

		public IDbDataParameter CreateParameter(IDbCommand command, string name, DateTime value)
		{
			IDbDataParameter parameter = command.CreateParameter();
			parameter.ParameterName = name;
			parameter.Value = value;

			command.Parameters.Add(parameter);

			return parameter;
		}

		public IDbDataParameter CreateParameter(IDbCommand command, string name, double value)
		{
			IDbDataParameter parameter = command.CreateParameter();
			parameter.ParameterName = name;
			parameter.Value = value;

			command.Parameters.Add(parameter);

			return parameter;
		}

		public IDbDataParameter CreateParameter(IDbCommand command, string name, string value)
		{
			IDbDataParameter parameter = command.CreateParameter();
			parameter.ParameterName = name;
			parameter.Value = value;

			command.Parameters.Add(parameter);

			return parameter;
		}

		public IDbDataParameter CreateParameter(IDbCommand command, string name, DbType dbType)
		{
			IDbDataParameter parameter = command.CreateParameter();
			parameter.DbType = dbType;
			parameter.ParameterName = name;

			command.Parameters.Add(parameter);

			return parameter;
		}

		public virtual void Close()
		{
			this.connection.Close();
		}
		
		public virtual void Delete(IEnumerable<TKey> keys, IDbTransaction transaction)
		{
			throw new NotImplementedException("Delete (multiple) must be overridden!");
		}
		
		public virtual void Delete(TKey key, IDbTransaction transaction)
		{
			if(key == null)
				throw new ArgumentNullException("Key");
			
			Delete(new List<TKey>(){ key }, transaction);
		}
		
		public virtual IEnumerable<TKey> Exists(IEnumerable<TKey> keys)
		{
			return Exists(keys, null);
		}
		
		public virtual IEnumerable<TKey> Exists(IEnumerable<TKey> keys, IDbTransaction transaction)
		{
			throw new NotImplementedException("Exists (multiple) must be overridden!");
		}
		
		public virtual bool Exists(TKey key)
		{
			if(key == null)
				throw new ArgumentNullException("Key");
			
			return Exists(new List<TKey>(){ key }).FirstOrDefault() != null;
		}
		
		public virtual bool Exists(TKey key, IDbTransaction transaction)
		{
			if(key == null)
				throw new ArgumentNullException("Key");
			
			return Exists(new List<TKey>(){ key }, transaction).FirstOrDefault() != null;
		}
		
		public virtual void Insert(IEnumerable<TModel> models, IDbTransaction transaction)
		{
			throw new NotImplementedException("Insert (multiple) must be overridden!");
		}
		
		public virtual void Insert(TModel model, IDbTransaction transaction)
		{
			if(model == null)
				throw new ArgumentNullException("Model");
			
			Insert(new List<TModel>(){ model }, transaction);
		}
		
		public virtual void Open()
		{
			this.connection.Open();
		}
		
		public virtual IEnumerable<TModel> Select(TMultipleSearch search)
		{
			throw new NotImplementedException("Select (multiple) must be overridden!");
		}
		
		public virtual IEnumerable<TModel> Select(TMultipleSearch search, IDbTransaction transaction)
		{
			throw new NotImplementedException("Select (multiple) must be overridden!");
		}
		
		public virtual TModel Select(TSingleSearch search)
		{
			throw new NotImplementedException("Select must be overridden!");
		}
		
		public virtual TModel Select(TSingleSearch search, IDbTransaction transaction)
		{
			throw new NotImplementedException("Select must be overridden!");
		}
		
		public virtual IDbTransaction StartTransaction()
		{
			return this.connection.BeginTransaction();
		}
		
		public virtual void Update(IEnumerable<TModel> model, IDbTransaction transaction)
		{
			throw new NotImplementedException("Update (multiple) must be overridden!");
		}
		
		public virtual void Update(TModel model, IDbTransaction transaction)
		{
			if (model == null)
				throw new ArgumentNullException("Model");

			Update(new List<TModel>() { model }, transaction);
		}
	}
}
