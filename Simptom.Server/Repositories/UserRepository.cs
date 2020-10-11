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
	public class UserRepository : RepositoryBase<IUser, IUserKey, IUserSearch, IUsersSearch>, IUserRepository
	{
		public UserRepository(IDbConnection connection, IModelFactory modelFactory)
			: base(connection, modelFactory)
		{
		}
		
		public override void Delete(IEnumerable<IUserKey> keys, IDbTransaction transaction)
		{
			if(keys == null)
				throw new ArgumentNullException("Keys");
			if(keys.Any(_key => _key == null))
				throw new ArgumentNullException("One of the provided Keys was NULL.");

			StringBuilder query = new StringBuilder()
				.Append("DELETE FROM Users WHERE ID IN (");

			int counter = 1;
			foreach (IUserKey key in keys)
				query.Append("@ID" + counter++);

			query.Append(")");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				counter = 1;
				foreach (IUserKey key in keys)
					CreateParameter(command, "ID" + counter++, key.ID.ToString());

				command.ExecuteNonQuery();
			}
		}
		
		public override IEnumerable<IUserKey> Exists(IEnumerable<IUserKey> keys, IDbTransaction transaction)
		{
			List<IUserKey> foundKeys = new List<IUserKey>();

			if (keys == null)
				throw new ArgumentNullException("Keys");
			if(keys.Any(_key => _key == null))
				throw new ArgumentNullException("One of the provided Keys was NULL.");

			StringBuilder query = new StringBuilder()
				.Append("SELECT ID FROM Users WHERE ID IN (");

			int counter = 1;
			foreach (IUserKey key in keys)
				query.Append("@ID" + counter++);

			query.Append(")");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				counter = 1;
				foreach (IUserKey key in keys)
					CreateParameter(command, "ID" + counter++, key.ID.ToString());

				using (IDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Guid id = new Guid(reader.GetValue(0).ToString());
						IUserKey key = this.modelFactory.GenerateUserKey(id);

						foundKeys.Add(key);
					}
				}
			}

			return foundKeys;
		}
		
		public override void Insert(IEnumerable<IUser> users, IDbTransaction transaction)
		{
			if(users == null)
				throw new ArgumentNullException("users");
			foreach(IUser user in users)
				if(user == null)
					throw new ArgumentNullException("Single IUser");
			
			foreach(IUser user in users)
				user.Validate();

			StringBuilder query = new StringBuilder()
				.Append("INSERT INTO Users (ID, Name, Password)")
				.Append(" VALUES (@ID, @Name, @Password)");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				IDbDataParameter idParameter = CreateParameter(command, "ID", DbType.Guid);
				IDbDataParameter nameParameter = CreateParameter(command, "Name", DbType.String);
				IDbDataParameter passwordParameter = CreateParameter(command, "Password", DbType.String);

				foreach (IUser user in users)
				{
					idParameter.Value = user.Key.ID == Guid.Empty ? Guid.NewGuid() : user.Key.ID;
					nameParameter.Value = user.Name.Trim();
					passwordParameter.Value = user.Password.Trim();

					command.ExecuteNonQuery();
				}
			}
		}
		
		public override IUser Select(IUserSearch search)
		{
			IUser user = null;

			if (search == null)
				throw new ArgumentNullException("IUserSearch");
			search.Validate();

			StringBuilder query = new StringBuilder()
				.Append("SELECT ID, Name, Password ")
				.Append("FROM Users ")
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
						IUserKey key = this.modelFactory.GenerateUserKey(id);

						user = this.modelFactory.GenerateUser(key);
						user.Name = reader.GetString(1).ToString();
						user.Password = reader.GetString(2).ToString();
					}
				}
			}

			return user;
		}
		
		public override IEnumerable<IUser> Select(IUsersSearch search)
		{
			List<IUser> users = new List<IUser>();

			if (search == null)
				throw new ArgumentNullException("IUsersSearch");
			search.Validate();

			StringBuilder query = new StringBuilder()
				.Append("SELECT ID, Name, Password ")
				.Append("FROM Users ")
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
						IUserKey key = this.modelFactory.GenerateUserKey(id);

						IUser user = this.modelFactory.GenerateUser(key);
						user.Name = reader.GetString(1).ToString();
						user.Password = reader.GetString(2).ToString();

						users.Add(user);
					}
				}
			}

			return users;
		}
		
		public override void Update(IEnumerable<IUser> users, IDbTransaction transaction)
		{
			if(users == null)
				throw new ArgumentNullException("users");
			foreach(IUser user in users)
				if(user == null)
					throw new ArgumentNullException("Single User");
			
			foreach(IUser user in users)
				user.Validate();

			StringBuilder query = new StringBuilder()
				.Append("UPDATE Users ")
				.Append("SET ")
				.Append("Name = @Name, ")
				.Append("Password = @Password ")
				.Append("WHERE ")
				.Append("ID = @ID");

			using (IDbCommand command = this.connection.CreateCommand())
			{
				command.CommandText = query.ToString();
				command.Transaction = transaction;

				IDbDataParameter idParameter = CreateParameter(command, "ID", DbType.Guid);
				IDbDataParameter nameParameter = CreateParameter(command, "Name", DbType.String);
				IDbDataParameter passwordParameter = CreateParameter(command, "Password", DbType.String);

				foreach (IUser user in users)
				{
					idParameter.Value = user.Key.ID;
					nameParameter.Value = user.Name.Trim();
					passwordParameter.Value = user.Password.Trim();

					command.ExecuteNonQuery();
				}
			}
		}
	}
}
