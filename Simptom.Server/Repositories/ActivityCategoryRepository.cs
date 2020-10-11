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
	public class ActivityCategoryRepository : RepositoryBase<IActivityCategory, IActivityCategoryKey, IActivityCategorySearch, IActivityCategoriesSearch>, IActivityCategoryRepository
	{
		public ActivityCategoryRepository(IDbConnection connection, IModelFactory modelFactory)
			: base(connection, modelFactory)
		{
		}
		
		public override void Delete(IEnumerable<IActivityCategoryKey> keys, IDbTransaction transaction)
		{
			if(keys == null)
				throw new ArgumentNullException("Keys");
			if(keys.Any(_key => _key == null))
				throw new ArgumentNullException("One of the provided Keys was NULL.");

			StringBuilder query = new StringBuilder()
				.Append("DELETE FROM ActivityCategories WHERE ID IN (");

			int counter = 1;
			foreach (IActivityCategoryKey key in keys)
				query.Append("@ID" + counter++);

			query.Append(")");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				counter = 1;
				foreach (IActivityCategoryKey key in keys)
					CreateParameter(command, "ID" + counter++, key.ID.ToString());

				command.ExecuteNonQuery();
			}
		}
		
		public override IEnumerable<IActivityCategoryKey> Exists(IEnumerable<IActivityCategoryKey> keys, IDbTransaction transaction)
		{
			List<IActivityCategoryKey> foundKeys = new List<IActivityCategoryKey>();

			if (keys == null)
				throw new ArgumentNullException("Keys");
			if(keys.Any(_key => _key == null))
				throw new ArgumentNullException("One of the provided Keys was NULL.");

			StringBuilder query = new StringBuilder()
				.Append("SELECT ID FROM ActivityCategories WHERE ID IN (");

			int counter = 1;
			foreach (IActivityCategoryKey key in keys)
				query.Append("@ID" + counter++);

			query.Append(")");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				counter = 1;
				foreach (IActivityCategoryKey key in keys)
					CreateParameter(command, "ID" + counter++, key.ID.ToString());

				using (IDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Guid id = new Guid(reader.GetValue(0).ToString());
						IActivityCategoryKey key = this.modelFactory.GenerateActivityCategoryKey(id);

						foundKeys.Add(key);
					}
				}
			}

			return foundKeys;
		}

		public override void Insert(IEnumerable<IActivityCategory> activityCategories, IDbTransaction transaction)
		{
			if(activityCategories == null)
				throw new ArgumentNullException("activityCategories");
			foreach(IActivityCategory activityCategory in activityCategories)
				if(activityCategory == null)
					throw new ArgumentNullException("Single IActivityCategory");
			
			foreach(IActivityCategory activityCategory in activityCategories)
				activityCategory.Validate();

			StringBuilder query = new StringBuilder()
				.Append("INSERT INTO ActivityCategories (ID, Name)")
				.Append(" VALUES (@ID, @Name)");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				IDbDataParameter idParameter = CreateParameter(command, "ID", DbType.Guid);
				IDbDataParameter nameParameter = CreateParameter(command, "Name", DbType.String);

				foreach (IActivityCategory activityCategory in activityCategories)
				{
					idParameter.Value = activityCategory.Key.ID == Guid.Empty ? Guid.NewGuid() : activityCategory.Key.ID;
					nameParameter.Value = activityCategory.Name;

					command.ExecuteNonQuery();
				}
			}
		}
		
		public override IEnumerable<IActivityCategory> Select(IActivityCategoriesSearch search)
		{
			List<IActivityCategory> activityCategories = new List<IActivityCategory>();

			if (search == null)
				throw new ArgumentNullException("IActivityCategoriesSearch");
			search.Validate();

			StringBuilder query = new StringBuilder()
				.Append("SELECT ID, Name ")
				.Append("FROM ActivityCategories ")
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
						IActivityCategoryKey key = this.modelFactory.GenerateActivityCategoryKey(id);

						IActivityCategory activityCategory = this.modelFactory.GenerateActivityCategory(key);
						activityCategory.Name = reader.GetString(1).ToString();

						activityCategories.Add(activityCategory);
					}
				}
			}

			return activityCategories;
		}
		
		public override IActivityCategory Select(IActivityCategorySearch search)
		{
			IActivityCategory activityCategory = null;

			if (search == null)
				throw new ArgumentNullException("ActivityCategorySearch");
			search.Validate();

			StringBuilder query = new StringBuilder()
				.Append("SELECT ID, Name ")
				.Append("FROM ActivityCategories ")
				.Append("WHERE ")
				.Append("(@ID IS NULL OR ID = @ID)")
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
						IActivityCategoryKey key = this.modelFactory.GenerateActivityCategoryKey(id);

						activityCategory = this.modelFactory.GenerateActivityCategory(key);
						activityCategory.Name = reader.GetString(1).ToString();
					}
				}
			}

			return activityCategory;
		}
		
		public override void Update(IEnumerable<IActivityCategory> activityCategories, IDbTransaction transaction)
		{
			if(activityCategories == null)
				throw new ArgumentNullException("activityCategories");
			foreach(IActivityCategory activityCategory in activityCategories)
				if(activityCategory == null)
					throw new ArgumentNullException("Single ActivityCategory");
			
			foreach(IActivityCategory activityCategory in activityCategories)
				activityCategory.Validate();

			StringBuilder query = new StringBuilder()
				.Append("UPDATE ActivityCategories ")
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

				foreach (IActivityCategory activityCategory in activityCategories)
				{
					idParameter.Value = activityCategory.Key.ID;
					nameParameter.Value = activityCategory.Name;

					command.ExecuteNonQuery();
				}
			}
		}
	}
}
