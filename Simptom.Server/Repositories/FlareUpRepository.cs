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
	public class FlareUpRepository : RepositoryBase<IFlareUp, IFlareUpKey, IFlareUpSearch, IFlareUpsSearch>, IFlareUpRepository
	{
		public FlareUpRepository(IDbConnection connection, IModelFactory modelFactory)
			: base(connection, modelFactory)
		{
		}
		
		public override void Delete(IEnumerable<IFlareUpKey> keys, IDbTransaction transaction)
		{
			if(keys == null)
				throw new ArgumentNullException("Keys");
			if(keys.Any(_key => _key == null))
				throw new ArgumentNullException("One of the provided Keys was NULL.");

			StringBuilder query = new StringBuilder()
				.Append("DELETE FROM FlareUps WHERE ID IN (");

			int counter = 1;
			foreach (IFlareUpKey key in keys)
				query.Append("@ID" + counter++);

			query.Append(")");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				counter = 1;
				foreach (IFlareUpKey key in keys)
					CreateParameter(command, "ID" + counter++, key.ID.ToString());

				command.ExecuteNonQuery();
			}
		}
		
		public override IEnumerable<IFlareUpKey> Exists(IEnumerable<IFlareUpKey> keys, IDbTransaction transaction)
		{
			List<IFlareUpKey> foundKeys = new List<IFlareUpKey>();

			if (keys == null)
				throw new ArgumentNullException("Keys");
			if(keys.Any(_key => _key == null))
				throw new ArgumentNullException("One of the provided Keys was NULL.");

			StringBuilder query = new StringBuilder()
				.Append("SELECT ID FROM FlareUps WHERE ID IN (");

			int counter = 1;
			foreach (IFlareUpKey key in keys)
				query.Append("@ID" + counter++);

			query.Append(")");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				counter = 1;
				foreach (IFlareUpKey key in keys)
					CreateParameter(command, "ID" + counter++, key.ID.ToString());

				using (IDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Guid id = new Guid(reader.GetValue(0).ToString());
						IFlareUpKey key = this.modelFactory.GenerateFlareUpKey(id);

						foundKeys.Add(key);
					}
				}
			}

			return foundKeys;
		}
		
		public override void Insert(IEnumerable<IFlareUp> flareUps, IDbTransaction transaction)
		{
			if(flareUps == null)
				throw new ArgumentNullException("flareUps");
			foreach(IFlareUp flareUp in flareUps)
				if(flareUp == null)
					throw new ArgumentNullException("Single IFlareUp");
			
			foreach(IFlareUp flareUp in flareUps)
				flareUp.Validate();

			StringBuilder query = new StringBuilder()
				.Append("INSERT INTO FlareUps (ID, ExperiencedOn, Severity, SymptomID, UserID)")
				.Append(" VALUES (@ID, @ExperiencedOn, @Severity, @SymptomID, @UserID)");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				IDbDataParameter idParameter = CreateParameter(command, "ID", DbType.Guid);
				IDbDataParameter experiencedOnParameter = CreateParameter(command, "ExperiencedOn", DbType.DateTime2);
				IDbDataParameter severityParameter = CreateParameter(command, "Severity", DbType.Decimal);
				IDbDataParameter symptomIDParameter = CreateParameter(command, "SymptomID", DbType.Guid);
				IDbDataParameter userIDParameter = CreateParameter(command, "UserID", DbType.Guid);

				foreach (IFlareUp flareUp in flareUps)
				{
					idParameter.Value = flareUp.Key.ID == Guid.Empty ? Guid.NewGuid() : flareUp.Key.ID;
					experiencedOnParameter.Value = flareUp.ExperiencedOn;
					severityParameter.Value = flareUp.Severity;
					symptomIDParameter.Value = flareUp.Symptom.Key.ID;
					userIDParameter.Value = flareUp.User.Key.ID;

					command.ExecuteNonQuery();
				}
			}
		}
		
		public override IFlareUp Select(IFlareUpSearch search)
		{
			IFlareUp flareUp = null;

			if (search == null)
				throw new ArgumentNullException("IFlareUpSearch");
			search.Validate();

			StringBuilder query = new StringBuilder()
				.Append("SELECT FlareUps.ID, ExperiencedOn, Severity, Symptoms.ID AS SymptomID, Categories.ID AS SymptomCategoryID, Categories.Name AS SymptomCategoryName, Symptoms.Name AS SymptomName, Users.ID AS UserID, Users.Name AS UserName ")
				.Append("FROM FlareUps ")
				.Append("INNER JOIN Symptoms ")
				.Append("ON FlareUps.SymptomID = Symptoms.ID ")
				.Append("INNER JOIN SymptomCategories AS Categories ")
				.Append("ON Symptoms.CategoryID = Categories.ID ")
				.Append("INNER JOIN Users ")
				.Append("ON FlareUps.UserID = Users.ID ")
				.Append("WHERE ")
				.Append("(@ID IS NULL OR FlareUps.ID = @ID)")
				.Append("AND (@ExperiencedOn IS NULL OR FlareUps.ExperiencedOn = @ExperiencedOn)")
				.Append("AND (@UserID IS NULL OR FlareUps.UserID = @UserID)");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();

				if (search.ID != Guid.Empty)
					CreateParameter(command, "ID", search.ID.ToString());
				else
					CreateParameter(command, "ID", DBNull.Value);

				if (search.ExperiencedOn.HasValue)
					CreateParameter(command, "ExperiencedOn", search.ExperiencedOn.Value);
				else
					CreateParameter(command, "ExperiencedOn", DBNull.Value);

				if (search.UserID != Guid.Empty)
					CreateParameter(command, "UserID", search.UserID.ToString());
				else
					CreateParameter(command, "UserID", DBNull.Value);

				using (IDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Guid symptomCategoryId = new Guid(reader.GetValue(4).ToString());
						ISymptomCategoryKey symptomCategoryKey = this.modelFactory.GenerateSymptomCategoryKey(symptomCategoryId);

						ISymptomCategory symptomCategory = this.modelFactory.GenerateSymptomCategory(symptomCategoryKey);
						symptomCategory.Name = reader.GetString(5);

						Guid symptomId = new Guid(reader.GetValue(3).ToString());
						ISymptomKey symptomKey = this.modelFactory.GenerateSymptomKey(symptomId);

						ISymptom symptom = this.modelFactory.GenerateSymptom(symptomKey);
						symptom.Category = symptomCategory;
						symptom.Name = reader.GetString(6);

						Guid userId = new Guid(reader.GetValue(7).ToString());
						IUserKey userKey = this.modelFactory.GenerateUserKey(userId);

						IUser user = this.modelFactory.GenerateUser(userKey);
						user.Name = reader.GetString(8);
						user.Password = null;

						Guid id = new Guid(reader.GetValue(0).ToString());
						IFlareUpKey key = this.modelFactory.GenerateFlareUpKey(id);

						flareUp = this.modelFactory.GenerateFlareUp(key);
						flareUp.ExperiencedOn = reader.GetDateTime(1);
						flareUp.Severity = reader.GetDouble(2);
						flareUp.Symptom = symptom;
						flareUp.User = user;
					}
				}
			}

			return flareUp;
		}
		
		public override IEnumerable<IFlareUp> Select(IFlareUpsSearch search)
		{
			List<IFlareUp> flareUps = new List<IFlareUp>();

			if (search == null)
				throw new ArgumentNullException("IFlareUpsSearch");
			search.Validate();

			StringBuilder query = new StringBuilder()
				.Append("SELECT FlareUps.ID, ExperiencedOn, Severity, Symptoms.ID AS SymptomID, Categories.ID AS SymptomCategoryID, Categories.Name AS SymptomCategoryName, Symptoms.Name AS SymptomName, Users.ID AS UserID, Users.Name AS UserName ")
				.Append("FROM FlareUps ")
				.Append("INNER JOIN Symptoms ")
				.Append("ON FlareUps.SymptomID = Symptoms.ID ")
				.Append("INNER JOIN SymptomCategories AS Categories ")
				.Append("ON Symptoms.CategoryID = Categories.ID ")
				.Append("INNER JOIN Users ")
				.Append("ON FlareUps.UserID = Users.ID ")
				.Append("WHERE ")
				.Append("(@Start IS NULL OR FlareUps.ExperiencedOn > @Start)")
				.Append("AND (@End IS NULL OR FlareUps.ExperiencedOn < @End)")
				.Append("AND (@UserID IS NULL OR FlareUps.UserID = @UserID)");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();

				if (search.End.HasValue)
					CreateParameter(command, "End", search.End.Value);
				else
					CreateParameter(command, "End", DBNull.Value);

				if (search.Start.HasValue)
					CreateParameter(command, "Start", search.Start.Value);
				else
					CreateParameter(command, "Start", DBNull.Value);

				if (search.UserID != Guid.Empty)
					CreateParameter(command, "UserID", search.UserID.ToString());
				else
					CreateParameter(command, "UserID", DBNull.Value);

				using (IDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Guid symptomCategoryId = new Guid(reader.GetValue(4).ToString());
						ISymptomCategoryKey symptomCategoryKey = this.modelFactory.GenerateSymptomCategoryKey(symptomCategoryId);

						ISymptomCategory symptomCategory = this.modelFactory.GenerateSymptomCategory(symptomCategoryKey);
						symptomCategory.Name = reader.GetString(5);

						Guid symptomId = new Guid(reader.GetValue(3).ToString());
						ISymptomKey symptomKey = this.modelFactory.GenerateSymptomKey(symptomId);

						ISymptom symptom = this.modelFactory.GenerateSymptom(symptomKey);
						symptom.Category = symptomCategory;
						symptom.Name = reader.GetString(6);

						Guid userId = new Guid(reader.GetValue(7).ToString());
						IUserKey userKey = this.modelFactory.GenerateUserKey(userId);

						IUser user = this.modelFactory.GenerateUser(userKey);
						user.Name = reader.GetString(8);
						user.Password = null;

						Guid id = new Guid(reader.GetValue(0).ToString());
						IFlareUpKey key = this.modelFactory.GenerateFlareUpKey(id);

						IFlareUp flareUp = this.modelFactory.GenerateFlareUp(key);
						flareUp.ExperiencedOn = reader.GetDateTime(1);
						flareUp.Severity = reader.GetDouble(2);
						flareUp.Symptom = symptom;
						flareUp.User = user;

						flareUps.Add(flareUp);
					}
				}
			}

			return flareUps;
		}
		
		public override void Update(IEnumerable<IFlareUp> flareUps, IDbTransaction transaction)
		{
			if(flareUps == null)
				throw new ArgumentNullException("flareUps");
			foreach(IFlareUp flareUp in flareUps)
				if(flareUp == null)
					throw new ArgumentNullException("Single FlareUp");
			
			foreach(IFlareUp flareUp in flareUps)
				flareUp.Validate();

			StringBuilder query = new StringBuilder()
				.Append("UPDATE FlareUps ")
				.Append("SET ")
				.Append("ExperiencedOn = @ExperiencedOn, ")
				.Append("Severity = @Severity, ")
				.Append("SymptomID = @SymptomID, ")
				.Append("UserID = @UserID ")
				.Append("WHERE ")
				.Append("ID = @ID");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				IDbDataParameter idParameter = CreateParameter(command, "ID", DbType.Guid);
				IDbDataParameter experiencedOnParameter = CreateParameter(command, "ExperiencedOn", DbType.DateTime2);
				IDbDataParameter severityParameter = CreateParameter(command, "Severity", DbType.Decimal);
				IDbDataParameter symptomIDParameter = CreateParameter(command, "SymptomID", DbType.Guid);
				IDbDataParameter userIDParameter = CreateParameter(command, "UserID", DbType.Guid);

				foreach (IFlareUp flareUp in flareUps)
				{
					idParameter.Value = flareUp.Key.ID;
					experiencedOnParameter.Value = flareUp.ExperiencedOn;
					severityParameter.Value = flareUp.Severity;
					symptomIDParameter.Value = flareUp.Symptom.Key.ID;
					userIDParameter.Value = flareUp.User.Key.ID;

					command.ExecuteNonQuery();
				}
			}
		}
	}
}
