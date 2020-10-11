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
	public class ParticipationRepository : RepositoryBase<IParticipation, IParticipationKey, IParticipationSearch, IParticipationsSearch>, IParticipationRepository
	{
		public ParticipationRepository(IDbConnection connection, IModelFactory modelFactory)
			: base(connection, modelFactory)
		{
		}
		
		public override void Delete(IEnumerable<IParticipationKey> keys, IDbTransaction transaction)
		{
			if(keys == null)
				throw new ArgumentNullException("Keys");
			if(keys.Any(_key => _key == null))
				throw new ArgumentNullException("One of the provided Keys was NULL.");

			StringBuilder query = new StringBuilder()
				.Append("DELETE FROM Participations WHERE ID IN (");

			int counter = 1;
			foreach (IParticipationKey key in keys)
				query.Append("@ID" + counter++);

			query.Append(")");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				counter = 1;
				foreach (IParticipationKey key in keys)
					CreateParameter(command, "ID" + counter++, key.ID.ToString());

				command.ExecuteNonQuery();
			}
		}
		
		public override IEnumerable<IParticipationKey> Exists(IEnumerable<IParticipationKey> keys, IDbTransaction transaction)
		{
			List<IParticipationKey> foundKeys = new List<IParticipationKey>();

			if (keys == null)
				throw new ArgumentNullException("Keys");
			if(keys.Any(_key => _key == null))
				throw new ArgumentNullException("One of the provided Keys was NULL.");

			StringBuilder query = new StringBuilder()
				.Append("SELECT ID FROM Participations WHERE ID IN (");

			int counter = 1;
			foreach (IParticipationKey key in keys)
				query.Append("@ID" + counter++);

			query.Append(")");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				counter = 1;
				foreach (IParticipationKey key in keys)
					CreateParameter(command, "ID" + counter++, key.ID.ToString());

				using (IDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Guid id = new Guid(reader.GetValue(0).ToString());
						IParticipationKey key = this.modelFactory.GenerateParticipationKey(id);

						foundKeys.Add(key);
					}
				}
			}

			return foundKeys;
		}
		
		public override void Insert(IEnumerable<IParticipation> participations, IDbTransaction transaction)
		{
			if(participations == null)
				throw new ArgumentNullException("participations");
			foreach(IParticipation participation in participations)
				if(participation == null)
					throw new ArgumentNullException("Single IParticipation");
			
			foreach(IParticipation participation in participations)
				participation.Validate();

			StringBuilder query = new StringBuilder()
				.Append("INSERT INTO Participations (ID, ActivityID, PerformedOn, Severity, UserID)")
				.Append(" VALUES (@ID, @ActivityID, @PerformedOn, @Severity, @UserID)");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				IDbDataParameter idParameter = CreateParameter(command, "ID", DbType.Guid);
				IDbDataParameter activityIDParameter = CreateParameter(command, "ActivityID", DbType.Guid);
				IDbDataParameter performedOnParameter = CreateParameter(command, "PerformedOn", DbType.DateTime2);
				IDbDataParameter severityParameter = CreateParameter(command, "Severity", DbType.Decimal);
				IDbDataParameter userIDParameter = CreateParameter(command, "UserID", DbType.Guid);

				foreach (IParticipation participation in participations)
				{
					idParameter.Value = participation.Key.ID == Guid.Empty ? Guid.NewGuid() : participation.Key.ID;
					activityIDParameter.Value = participation.Activity.Key.ID;
					performedOnParameter.Value = participation.PerformedOn;
					severityParameter.Value = participation.Severity;
					userIDParameter.Value = participation.User.Key.ID;

					command.ExecuteNonQuery();
				}
			}
		}
		
		public override IParticipation Select(IParticipationSearch search)
		{
			IParticipation participation = null;

			if (search == null)
				throw new ArgumentNullException("IParticipationSearch");
			search.Validate();

			StringBuilder query = new StringBuilder()
				.Append("SELECT Participations.ID, Activities.ID AS ActivityID, ActivityCategories.ID AS ActivityCategoryID, ActivityCategories.Name AS ActivityCategoryName, Activities.Name AS ActivityName, PerformedOn, Severity, Users.ID AS UserID, Users.Name AS UserName ")
				.Append("FROM Participations ")
				.Append("INNER JOIN Activities ")
				.Append("ON Participations.ActivityID = Activities.ID ")
				.Append("INNER JOIN ActivityCategories ")
				.Append("ON Activities.CategoryID = ActivityCategories.ID ")
				.Append("INNER JOIN Users ")
				.Append("ON Participations.UserID = Users.ID ")
				.Append("WHERE ")
				.Append("(@ID IS NULL OR Participations.ID = @ID)")
				.Append("AND (@PerformedOn IS NULL OR Participations.PerformedOn = @PerformedOn)")
				.Append("AND (@UserID IS NULL OR Participations.UserID = @UserID)");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();

				if (search.ID != Guid.Empty)
					CreateParameter(command, "ID", search.ID.ToString());
				else
					CreateParameter(command, "ID", DBNull.Value);

				if (search.PerformedOn.HasValue)
					CreateParameter(command, "PerformedOn", search.PerformedOn.Value);
				else
					CreateParameter(command, "PerformedOn", DBNull.Value);

				if (search.UserID != Guid.Empty)
					CreateParameter(command, "UserID", search.UserID.ToString());
				else
					CreateParameter(command, "UserID", DBNull.Value);

				using (IDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Guid activityCategoryId = new Guid(reader.GetValue(2).ToString());
						IActivityCategoryKey activityCategoryKey = this.modelFactory.GenerateActivityCategoryKey(activityCategoryId);

						IActivityCategory activityCategory = this.modelFactory.GenerateActivityCategory(activityCategoryKey);
						activityCategory.Name = reader.GetString(3);

						Guid activityId = new Guid(reader.GetValue(1).ToString());
						IActivityKey activityKey = this.modelFactory.GenerateActivityKey(activityId);

						IActivity activity = this.modelFactory.GenerateActivity(activityKey);
						activity.Category = activityCategory;
						activity.Name = reader.GetString(4);

						Guid userId = new Guid(reader.GetValue(7).ToString());
						IUserKey userKey = this.modelFactory.GenerateUserKey(userId);

						IUser user = this.modelFactory.GenerateUser(userKey);
						user.Name = reader.GetString(8);
						user.Password = null;

						Guid id = new Guid(reader.GetValue(0).ToString());
						IParticipationKey key = this.modelFactory.GenerateParticipationKey(id);

						participation = this.modelFactory.GenerateParticipation(key);
						participation.Activity = activity;
						participation.PerformedOn = reader.GetDateTime(5);
						participation.Severity = reader.GetDouble(6);
						participation.User = user;
					}
				}
			}

			return participation;
		}
		
		public override IEnumerable<IParticipation> Select(IParticipationsSearch search)
		{
			List<IParticipation> participations = new List<IParticipation>();

			if (search == null)
				throw new ArgumentNullException("IParticipationsSearch");
			search.Validate();

			StringBuilder query = new StringBuilder()
				.Append("SELECT Participations.ID, Activities.ID AS ActivityID, ActivityCategories.ID AS ActivityCategoryID, ActivityCategories.Name AS ActivityCategoryName, Activities.Name AS ActivityName, PerformedOn, Severity, Users.ID AS UserID, Users.Name AS UserName ")
				.Append("FROM Participations ")
				.Append("INNER JOIN Activities ")
				.Append("ON Participations.ActivityID = Activities.ID ")
				.Append("INNER JOIN ActivityCategories ")
				.Append("ON Activities.CategoryID = ActivityCategories.ID ")
				.Append("INNER JOIN Users ")
				.Append("ON Participations.UserID = Users.ID ")
				.Append("WHERE ")
				.Append("(@Start IS NULL OR Participations.PerformedOn > @Start)")
				.Append("AND (@End IS NULL OR Participations.PerformedOn < @End)")
				.Append("AND (@UserID IS NULL OR Participations.UserID = @UserID)");

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
						Guid activityCategoryId = new Guid(reader.GetValue(2).ToString());
						IActivityCategoryKey activityCategoryKey = this.modelFactory.GenerateActivityCategoryKey(activityCategoryId);

						IActivityCategory activityCategory = this.modelFactory.GenerateActivityCategory(activityCategoryKey);
						activityCategory.Name = reader.GetString(3);

						Guid activityId = new Guid(reader.GetValue(1).ToString());
						IActivityKey activityKey = this.modelFactory.GenerateActivityKey(activityId);

						IActivity activity = this.modelFactory.GenerateActivity(activityKey);
						activity.Category = activityCategory;
						activity.Name = reader.GetString(4);

						Guid userId = new Guid(reader.GetValue(7).ToString());
						IUserKey userKey = this.modelFactory.GenerateUserKey(userId);

						IUser user = this.modelFactory.GenerateUser(userKey);
						user.Name = reader.GetString(8);
						user.Password = null;

						Guid id = new Guid(reader.GetValue(0).ToString());
						IParticipationKey key = this.modelFactory.GenerateParticipationKey(id);

						IParticipation participation = this.modelFactory.GenerateParticipation(key);
						participation.Activity = activity;
						participation.PerformedOn = reader.GetDateTime(5);
						participation.Severity = reader.GetDouble(6);
						participation.User = user;

						participations.Add(participation);
					}
				}
			}

			return participations;
		}
		
		public override void Update(IEnumerable<IParticipation> participations, IDbTransaction transaction)
		{
			if(participations == null)
				throw new ArgumentNullException("participations");
			foreach(IParticipation participation in participations)
				if(participation == null)
					throw new ArgumentNullException("Single Participation");
			
			foreach(IParticipation participation in participations)
				participation.Validate();

			StringBuilder query = new StringBuilder()
				.Append("UPDATE Participations ")
				.Append("SET ")
				.Append("ActivityID = @ActivityID, ")
				.Append("PerformedOn = @PerformedOn, ")
				.Append("Severity = @Severity, ")
				.Append("UserID = @UserID ")
				.Append("WHERE ")
				.Append("ID = @ID");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				IDbDataParameter idParameter = CreateParameter(command, "ID", DbType.Guid);
				IDbDataParameter activityIDParameter = CreateParameter(command, "ActivityID", DbType.Guid);
				IDbDataParameter performedOnParameter = CreateParameter(command, "PerformedOn", DbType.DateTime2);
				IDbDataParameter severityParameter = CreateParameter(command, "Severity", DbType.Decimal);
				IDbDataParameter userIDParameter = CreateParameter(command, "UserID", DbType.Guid);

				foreach (IParticipation participation in participations)
				{
					idParameter.Value = participation.Key.ID;
					activityIDParameter.Value = participation.Activity.Key.ID;
					performedOnParameter.Value = participation.PerformedOn;
					severityParameter.Value = participation.Severity;
					userIDParameter.Value = participation.User.Key.ID;

					command.ExecuteNonQuery();
				}
			}
		}
	}
}
