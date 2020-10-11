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
	public class ActivityRepository : RepositoryBase<IActivity, IActivityKey, IActivitySearch, IActivitiesSearch>, IActivityRepository
	{
		public ActivityRepository(IDbConnection connection, IModelFactory modelFactory)
			: base(connection, modelFactory)
		{
		}
		
		public override void Delete(IEnumerable<IActivityKey> keys, IDbTransaction transaction)
		{
			if(keys == null)
				throw new ArgumentNullException("Keys");
			if(keys.Any(_key => _key == null))
				throw new ArgumentNullException("One of the provided Keys was NULL.");

			StringBuilder query = new StringBuilder()
				.Append("DELETE FROM Activities WHERE ID IN (");

			int counter = 1;
			foreach (IActivityKey key in keys)
				query.Append("@ID" + counter++);

			query.Append(")");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				counter = 1;
				foreach (IActivityKey key in keys)
					CreateParameter(command, "ID" + counter++, key.ID.ToString());

				command.ExecuteNonQuery();
			}
		}
		
		public override IEnumerable<IActivityKey> Exists(IEnumerable<IActivityKey> keys, IDbTransaction transaction)
		{
			List<IActivityKey> foundKeys = new List<IActivityKey>();

			if (keys == null)
				throw new ArgumentNullException("Keys");
			if(keys.Any(_key => _key == null))
				throw new ArgumentNullException("One of the provided Keys was NULL.");

			StringBuilder query = new StringBuilder()
				.Append("SELECT ID FROM Activities WHERE ID IN (");

			int counter = 1;
			foreach (IActivityKey key in keys)
				query.Append("@ID" + counter++);

			query.Append(")");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				counter = 1;
				foreach (IActivityKey key in keys)
					CreateParameter(command, "ID" + counter++, key.ID.ToString());

				using (IDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Guid id = new Guid(reader.GetValue(0).ToString());
						IActivityKey key = this.modelFactory.GenerateActivityKey(id);

						foundKeys.Add(key);
					}
				}
			}

			return foundKeys;
		}
		
		public override void Insert(IEnumerable<IActivity> activities, IDbTransaction transaction)
		{
			if(activities == null)
				throw new ArgumentNullException("activities");
			foreach(IActivity activity in activities)
				if(activity == null)
					throw new ArgumentNullException("Single IActivity");
			
			foreach(IActivity activity in activities)
				activity.Validate();

			StringBuilder query = new StringBuilder()
				.Append("INSERT INTO Activities (ID, CategoryID, Name)")
				.Append(" VALUES (@ID, @CategoryID, @Name)");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				IDbDataParameter idParameter = CreateParameter(command, "ID", DbType.Guid);
				IDbDataParameter categoryIDParameter = CreateParameter(command, "CategoryID", DbType.Guid);
				IDbDataParameter nameParameter = CreateParameter(command, "Name", DbType.String);

				foreach (IActivity activity in activities)
				{
					idParameter.Value = activity.Key.ID == Guid.Empty ? Guid.NewGuid() : activity.Key.ID;
					categoryIDParameter.Value = activity.Category.Key.ID;
					nameParameter.Value = activity.Name.Trim();

					command.ExecuteNonQuery();
				}
			}
		}
		
		public override IEnumerable<IActivity> Select(IActivitiesSearch search)
		{
			List<IActivity> activities = new List<IActivity>();

			if (search == null)
				throw new ArgumentNullException("IActivitiesSearch");
			search.Validate();

			StringBuilder query = new StringBuilder()
				.Append("SELECT Activities.ID, Categories.ID AS CategoryID, Categories.Name AS CategoryName, Activities.Name ")
				.Append("FROM Activities ")
				.Append("INNER JOIN ActivityCategories AS Categories ")
				.Append("ON Activities.CategoryID = Categories.ID ")
				.Append("WHERE ")
				.Append("(@Name IS NULL OR Activities.Name LIKE @Name)");

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
						Guid activityCategoryId = new Guid(reader.GetValue(1).ToString());
						IActivityCategoryKey activityCategoryKey = this.modelFactory.GenerateActivityCategoryKey(activityCategoryId);

						IActivityCategory activityCategory = this.modelFactory.GenerateActivityCategory(activityCategoryKey);
						activityCategory.Name = reader.GetString(2).ToString();

						Guid id = new Guid(reader.GetValue(0).ToString());
						IActivityKey key = this.modelFactory.GenerateActivityKey(id);

						IActivity activity = this.modelFactory.GenerateActivity(key);
						activity.Category = activityCategory;
						activity.Name = reader.GetString(3).ToString();

						activities.Add(activity);
					}
				}
			}

			return activities;
		}
		
		public override IActivity Select(IActivitySearch search)
		{
			IActivity activity = null;

			if (search == null)
				throw new ArgumentNullException("IActivitySearch");
			search.Validate();

			StringBuilder query = new StringBuilder()
				.Append("SELECT Activities.ID, Categories.ID AS CategoryID, Categories.Name AS CategoryName, Activities.Name ")
				.Append("FROM Activities ")
				.Append("INNER JOIN ActivityCategories AS Categories ")
				.Append("ON Activities.CategoryID = Categories.ID ")
				.Append("WHERE ")
				.Append("(@ID IS NULL OR Activities.ID = @ID)")
				.Append("AND (@Name IS NULL OR Activities.Name LIKE @Name)");

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
						Guid activityCategoryId = new Guid(reader.GetValue(1).ToString());
						IActivityCategoryKey activityCategoryKey = this.modelFactory.GenerateActivityCategoryKey(activityCategoryId);

						IActivityCategory activityCategory = this.modelFactory.GenerateActivityCategory(activityCategoryKey);
						activityCategory.Name = reader.GetString(2).ToString();

						Guid id = new Guid(reader.GetValue(0).ToString());
						IActivityKey key = this.modelFactory.GenerateActivityKey(id);

						activity = this.modelFactory.GenerateActivity(key);
						activity.Category = activityCategory;
						activity.Name = reader.GetString(3).ToString();
					}
				}
			}

			return activity;
		}
		
		public override void Update(IEnumerable<IActivity> activities, IDbTransaction transaction)
		{
			if(activities == null)
				throw new ArgumentNullException("activities");
			foreach(IActivity activity in activities)
				if(activity == null)
					throw new ArgumentNullException("Single Activity");
			
			foreach(IActivity activity in activities)
				activity.Validate();

			StringBuilder query = new StringBuilder()
				.Append("UPDATE Activities ")
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

				foreach (IActivity activity in activities)
				{
					idParameter.Value = activity.Key.ID;
					categoryIDParameter.Value = activity.Category.Key.ID;
					nameParameter.Value = activity.Name.Trim();

					command.ExecuteNonQuery();
				}
			}
		}
	}
}
