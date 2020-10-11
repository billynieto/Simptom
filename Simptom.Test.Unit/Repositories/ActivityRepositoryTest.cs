using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Simptom.Framework;
using Simptom.Framework.Models;
using Simptom.Server.Repositories;
using Simptom.Test.Unit.Core.Repositories;

namespace Simptom.Test.Unit.Repositories
{
	[TestClass]
	public class ActivityRepositoryTest : IActivityRepositoryTest
	{
		protected Mock<IDbCommand> command;
		protected Mock<IDbConnection> connection;
		protected DataSet dataSet;
		protected Mock<IDbDataParameter> parameter;
		protected Mock<IDataParameterCollection> parameters;
		protected Mock<IDataReader> reader;
		protected DataRow row = null;

		[TestInitialize]
		public override void TestInitialize()
		{
			base.TestInitialize();

			int index = 0;

			this.dataSet = new DataSet();
			DataTable table = this.dataSet.Tables.Add();
			table.Columns.Add("ID", typeof(Guid));
			table.Columns.Add("CategoryID", typeof(Guid));
			table.Columns.Add("CategoryName", typeof(string));
			table.Columns.Add("Name", typeof(string));

			this.reader = new Mock<IDataReader>();
			this.reader.Setup(_reader => _reader.Read())
				.Callback(() => this.row = (this.dataSet.Tables[0].Rows.Count > index) ? this.dataSet.Tables[0].Rows[index++] : (DataRow)null)
				.Returns(() => this.row != null);
			this.reader.Setup(_reader => _reader.GetValue(0)).Returns(() => this.row["ID"].ToString());
			this.reader.Setup(_reader => _reader.GetValue(1)).Returns(() => this.row["CategoryID"].ToString());
			this.reader.Setup(_reader => _reader.GetString(2)).Returns(() => this.row["CategoryName"].ToString());
			this.reader.Setup(_reader => _reader.GetString(3)).Returns(() => this.row["Name"].ToString());

			this.parameter = new Mock<IDbDataParameter>();
			this.parameters = new Mock<IDataParameterCollection>();

			this.command = new Mock<IDbCommand>();
			this.command.Setup(_command => _command.CreateParameter()).Returns(this.parameter.Object);
			this.command.Setup(_command => _command.ExecuteReader()).Returns(this.reader.Object);
			this.command.Setup(_command => _command.Parameters).Returns(this.parameters.Object);

			this.connection = new Mock<IDbConnection>();
			this.connection.Setup(_connection => _connection.CreateCommand()).Returns(this.command.Object);

			this.repository = new ActivityRepository(this.connection.Object, this.modelFactory.Object);
		}
		
		[TestCleanup]
		public override void TestCleanup()
		{
			base.TestCleanup();
			
			this.dataSet = null;
			this.repository = null;
		}
		
		public override void AddRow(IActivity activity)
		{
			DataRow row = this.dataSet.Tables[0].NewRow();
			
			row["ID"] = activity.Key.ID;

			row["CategoryID"] = activity.Category.Key.ID;
			row["CategoryName"] = activity.Category.Name;
			row["Name"] = activity.Name;
			
			this.dataSet.Tables[0].Rows.Add(row);
		}
	}
}
