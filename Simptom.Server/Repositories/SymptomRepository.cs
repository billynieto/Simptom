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
	public class SymptomRepository : RepositoryBase<ISymptom, ISymptomKey, ISymptomSearch, ISymptomsSearch>, ISymptomRepository
	{
		public SymptomRepository(IDbConnection connection, IModelFactory modelFactory)
			: base(connection, modelFactory)
		{
		}
		
		public override void Delete(IEnumerable<ISymptomKey> keys, IDbTransaction transaction)
		{
			if(keys == null)
				throw new ArgumentNullException("Keys");
			if(keys.Any(_key => _key == null))
				throw new ArgumentNullException("One of the provided Keys was NULL.");

			StringBuilder query = new StringBuilder()
				.Append("DELETE FROM Symptoms WHERE ID IN (");

			int counter = 1;
			foreach (ISymptomKey key in keys)
				query.Append("@ID" + counter++);

			query.Append(")");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				counter = 1;
				foreach (ISymptomKey key in keys)
					CreateParameter(command, "ID" + counter++, key.ID.ToString());

				command.ExecuteNonQuery();
			}
		}
		
		public override IEnumerable<ISymptomKey> Exists(IEnumerable<ISymptomKey> keys, IDbTransaction transaction)
		{
			List<ISymptomKey> foundKeys = new List<ISymptomKey>();

			if (keys == null)
				throw new ArgumentNullException("Keys");
			if(keys.Any(_key => _key == null))
				throw new ArgumentNullException("One of the provided Keys was NULL.");

			StringBuilder query = new StringBuilder()
				.Append("SELECT ID FROM Symptoms WHERE ID IN (");

			int counter = 1;
			foreach (ISymptomKey key in keys)
				query.Append("@ID" + counter++);

			query.Append(")");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				counter = 1;
				foreach (ISymptomKey key in keys)
					CreateParameter(command, "ID" + counter++, key.ID.ToString());

				using (IDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Guid id = new Guid(reader.GetValue(0).ToString());
						ISymptomKey key = this.modelFactory.GenerateSymptomKey(id);

						foundKeys.Add(key);
					}
				}
			}

			return foundKeys;
		}
		
		public override void Insert(IEnumerable<ISymptom> symptoms, IDbTransaction transaction)
		{
			if(symptoms == null)
				throw new ArgumentNullException("symptoms");
			foreach(ISymptom symptom in symptoms)
				if(symptom == null)
					throw new ArgumentNullException("Single ISymptom");
			
			foreach(ISymptom symptom in symptoms)
				symptom.Validate();

			StringBuilder query = new StringBuilder()
				.Append("INSERT INTO Symptoms (ID, CategoryID, Name)")
				.Append(" VALUES (@ID, @CategoryID, @Name)");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				IDbDataParameter idParameter = CreateParameter(command, "ID", DbType.Guid);
				IDbDataParameter categoryIDParameter = CreateParameter(command, "CategoryID", DbType.Guid);
				IDbDataParameter nameParameter = CreateParameter(command, "Name", DbType.String);

				foreach (ISymptom symptom in symptoms)
				{
					idParameter.Value = symptom.Key.ID == Guid.Empty ? Guid.NewGuid() : symptom.Key.ID;
					categoryIDParameter.Value = symptom.Category.Key.ID;
					nameParameter.Value = symptom.Name.Trim();

					command.ExecuteNonQuery();
				}
			}
		}
		
		public override ISymptom Select(ISymptomSearch search)
		{
			ISymptom symptom = null;

			if (search == null)
				throw new ArgumentNullException("ISymptomSearch");
			search.Validate();

			StringBuilder query = new StringBuilder()
				.Append("SELECT Symptoms.ID, Categories.ID AS CategoryID, Categories.Name AS CategoryName, Symptoms.Name ")
				.Append("FROM Symptoms ")
				.Append("INNER JOIN SymptomCategories Categories ")
				.Append("ON Symptoms.CategoryID = Categories.ID ")
				.Append("WHERE ")
				.Append("(@ID IS NULL OR Symptoms.ID = @ID) ")
				.Append("AND (@Name IS NULL OR Symptoms.Name LIKE @Name)");

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
						Guid symptomCategoryId = new Guid(reader.GetValue(1).ToString());
						ISymptomCategoryKey symptomCategoryKey = this.modelFactory.GenerateSymptomCategoryKey(symptomCategoryId);

						ISymptomCategory symptomCategory = this.modelFactory.GenerateSymptomCategory(symptomCategoryKey);
						symptomCategory.Name = reader.GetString(2).ToString();

						Guid id = new Guid(reader.GetValue(0).ToString());
						ISymptomKey key = this.modelFactory.GenerateSymptomKey(id);

						symptom = this.modelFactory.GenerateSymptom(key);
						symptom.Category = symptomCategory;
						symptom.Name = reader.GetString(3).ToString();
					}
				}
			}

			return symptom;
		}
		
		public override IEnumerable<ISymptom> Select(ISymptomsSearch search)
		{
			List<ISymptom> symptoms = new List<ISymptom>();

			if (search == null)
				throw new ArgumentNullException("ISymptomSearch");
			search.Validate();

			StringBuilder query = new StringBuilder()
				.Append("SELECT Symptoms.ID, Categories.ID AS CategoryID, Categories.Name AS CategoryName, Symptoms.Name ")
				.Append("FROM Symptoms ")
				.Append("INNER JOIN SymptomCategories Categories ")
				.Append("ON Symptoms.CategoryID = Categories.ID ")
				.Append("WHERE ")
				.Append("(@Name IS NULL OR Symptoms.Name LIKE @Name)");

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
						Guid symptomCategoryId = new Guid(reader.GetValue(1).ToString());
						ISymptomCategoryKey symptomCategoryKey = this.modelFactory.GenerateSymptomCategoryKey(symptomCategoryId);

						ISymptomCategory symptomCategory = this.modelFactory.GenerateSymptomCategory(symptomCategoryKey);
						symptomCategory.Name = reader.GetString(2).ToString();

						Guid id = new Guid(reader.GetValue(0).ToString());
						ISymptomKey key = this.modelFactory.GenerateSymptomKey(id);

						ISymptom symptom = this.modelFactory.GenerateSymptom(key);
						symptom.Category = symptomCategory;
						symptom.Name = reader.GetString(3).ToString();

						symptoms.Add(symptom);
					}
				}
			}

			return symptoms;
		}
		
		public override void Update(IEnumerable<ISymptom> symptoms, IDbTransaction transaction)
		{
			if(symptoms == null)
				throw new ArgumentNullException("symptoms");
			foreach(ISymptom symptom in symptoms)
				if(symptom == null)
					throw new ArgumentNullException("Single Symptom");
			
			foreach(ISymptom symptom in symptoms)
				symptom.Validate();

			StringBuilder query = new StringBuilder()
				.Append("UPDATE Symptoms ")
				.Append("SET ")
				.Append("CategoryID = @CategoryID, ")
				.Append("Name = @Name ")
				.Append("WHERE ")
				.Append("ID = @ID");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				IDbDataParameter idParameter = CreateParameter(command, "ID", DbType.Guid);
				IDbDataParameter categoryIDParameter = CreateParameter(command, "CategoryID", DbType.Guid);
				IDbDataParameter nameParameter = CreateParameter(command, "Name", DbType.String);

				foreach (ISymptom symptom in symptoms)
				{
					idParameter.Value = symptom.Key.ID;
					categoryIDParameter.Value = symptom.Category.Key.ID;
					nameParameter.Value = symptom.Name.Trim();

					command.ExecuteNonQuery();
				}
			}
		}
	}
}
