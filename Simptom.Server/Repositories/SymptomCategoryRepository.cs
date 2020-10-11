using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Simptom.Framework;
using Simptom.Framework.Models;
using Simptom.Framework.Repositories;

namespace Simptom.Server.Repositories
{
	public class SymptomCategoryRepository : RepositoryBase<ISymptomCategory, ISymptomCategoryKey, ISymptomCategorySearch, ISymptomCategoriesSearch>, ISymptomCategoryRepository
	{
		public SymptomCategoryRepository(IDbConnection connection, IModelFactory modelFactory)
			: base(connection, modelFactory)
		{
		}
		
		public override void Delete(IEnumerable<ISymptomCategoryKey> keys, IDbTransaction transaction)
		{
			if(keys == null)
				throw new ArgumentNullException("Keys");
			if(keys.Any(_key => _key == null))
				throw new ArgumentNullException("One of the provided Keys was NULL.");

			StringBuilder query = new StringBuilder()
				.Append("DELETE FROM SymptomCategories WHERE ID IN (");

			int counter = 1;
			foreach (ISymptomCategoryKey key in keys)
				query.Append("@ID" + counter++);

			query.Append(")");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				counter = 1;
				foreach (ISymptomCategoryKey key in keys)
					CreateParameter(command, "ID" + counter++, key.ID.ToString());

				command.ExecuteNonQuery();
			}
		}
		
		public override IEnumerable<ISymptomCategoryKey> Exists(IEnumerable<ISymptomCategoryKey> keys, IDbTransaction transaction)
		{
			List<ISymptomCategoryKey> foundKeys = new List<ISymptomCategoryKey>();

			if (keys == null)
				throw new ArgumentNullException("Keys");
			if(keys.Any(_key => _key == null))
				throw new ArgumentNullException("One of the provided Keys was NULL.");

			StringBuilder query = new StringBuilder()
				.Append("SELECT ID FROM SymptomCategories WHERE ID IN (");

			int counter = 1;
			foreach (ISymptomCategoryKey key in keys)
				query.Append("@ID" + counter++);

			query.Append(")");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				counter = 1;
				foreach (ISymptomCategoryKey key in keys)
					CreateParameter(command, "ID" + counter++, key.ID.ToString());

				using (IDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Guid id = new Guid(reader.GetValue(0).ToString());
						ISymptomCategoryKey key = this.modelFactory.GenerateSymptomCategoryKey(id);

						foundKeys.Add(key);
					}
				}
			}

			return foundKeys;
		}
		
		public override void Insert(IEnumerable<ISymptomCategory> symptomCategories, IDbTransaction transaction)
		{
			if(symptomCategories == null)
				throw new ArgumentNullException("symptomCategories");
			foreach(ISymptomCategory symptomCategory in symptomCategories)
				if(symptomCategory == null)
					throw new ArgumentNullException("Single ISymptomCategory");
			
			foreach(ISymptomCategory symptomCategory in symptomCategories)
				symptomCategory.Validate();

			StringBuilder query = new StringBuilder()
				.Append("INSERT INTO SymptomCategories (ID, Name)")
				.Append(" VALUES (@ID, @Name)");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				IDbDataParameter idParameter = CreateParameter(command, "ID", DbType.Guid);
				IDbDataParameter nameParameter = CreateParameter(command, "Name", DbType.String);

				foreach (ISymptomCategory symptomCategory in symptomCategories)
				{
					idParameter.Value = symptomCategory.Key.ID == Guid.Empty ? Guid.NewGuid() : symptomCategory.Key.ID;
					nameParameter.Value = symptomCategory.Name;

					command.ExecuteNonQuery();
				}
			}
		}
		
		public override IEnumerable<ISymptomCategory> Select(ISymptomCategoriesSearch search)
		{
			List<ISymptomCategory> symptomCategories = new List<ISymptomCategory>();

			if (search == null)
				throw new ArgumentNullException("ISymptomCategoriesSearch");
			search.Validate();

			StringBuilder query = new StringBuilder()
				.Append("SELECT ID, Name ")
				.Append("FROM SymptomCategories ")
				.Append("WHERE ")
				.Append("(@Name IS NULL OR Name LIKE @Name)");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();

				if (!string.IsNullOrEmpty(search.Name))
					CreateParameter(command, "Name", search.Name.Trim());
				else
					CreateParameter(command, "Name", DBNull.Value);

				using (IDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Guid id = new Guid(reader.GetValue(0).ToString());
						ISymptomCategoryKey key = this.modelFactory.GenerateSymptomCategoryKey(id);

						ISymptomCategory symptomCategory = this.modelFactory.GenerateSymptomCategory(key);
						symptomCategory.Name = reader.GetString(1).ToString();

						symptomCategories.Add(symptomCategory);
					}
				}
			}

			return symptomCategories;
		}
		
		public override ISymptomCategory Select(ISymptomCategorySearch search)
		{
			ISymptomCategory symptomCategory = null;

			if (search == null)
				throw new ArgumentNullException("ISymptomCategorySearch");
			search.Validate();

			StringBuilder query = new StringBuilder()
				.Append("SELECT ID, Name ")
				.Append("FROM SymptomCategories ")
				.Append("WHERE ")
				.Append("(@ID IS NULL OR ID = @ID) ")
				.Append("AND (@Name IS NULL OR Name LIKE @Name)");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();

				if (search.ID != Guid.Empty)
					CreateParameter(command, "ID", search.ID.ToString());
				else
					CreateParameter(command, "ID", DBNull.Value);

				if (!string.IsNullOrEmpty(search.Name))
					CreateParameter(command, "Name", search.Name.Trim());
				else
					CreateParameter(command, "Name", DBNull.Value);

				using (IDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Guid id = new Guid(reader.GetValue(0).ToString());
						ISymptomCategoryKey key = this.modelFactory.GenerateSymptomCategoryKey(id);

						symptomCategory = this.modelFactory.GenerateSymptomCategory(key);
						symptomCategory.Name = reader.GetString(1).ToString();
					}
				}
			}

			return symptomCategory;
		}
		
		public override void Update(IEnumerable<ISymptomCategory> symptomCategories, IDbTransaction transaction)
		{
			if(symptomCategories == null)
				throw new ArgumentNullException("symptomCategories");
			foreach(ISymptomCategory symptomCategory in symptomCategories)
				if(symptomCategory == null)
					throw new ArgumentNullException("Single SymptomCategory");
			
			foreach(ISymptomCategory symptomCategory in symptomCategories)
				symptomCategory.Validate();

			StringBuilder query = new StringBuilder()
				.Append("UPDATE SymptomCategories ")
				.Append("SET ")
				.Append("Name = @Name ")
				.Append("WHERE ")
				.Append("ID = @ID");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				IDbDataParameter idParameter = CreateParameter(command, "ID", DbType.Guid);
				IDbDataParameter nameParameter = CreateParameter(command, "Name", DbType.String);

				foreach (ISymptomCategory symptomCategory in symptomCategories)
				{
					idParameter.Value = symptomCategory.Key.ID;
					nameParameter.Value = symptomCategory.Name;

					command.ExecuteNonQuery();
				}
			}
		}
	}
}
