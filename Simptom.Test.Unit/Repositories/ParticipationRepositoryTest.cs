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
	public class ParticipationRepositoryTest : IParticipationRepositoryTest
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
			table.Columns.Add("ActivityID", typeof(Guid));
			table.Columns.Add("ActivityCategoryID", typeof(Guid));
			table.Columns.Add("ActivityCategoryName", typeof(string));
			table.Columns.Add("ActivityName", typeof(string));
			table.Columns.Add("PerformedOn", typeof(DateTime));
			table.Columns.Add("Severity", typeof(double));
			table.Columns.Add("UserID", typeof(Guid));
			table.Columns.Add("UserName", typeof(string));

			this.reader = new Mock<IDataReader>();
			this.reader.Setup(_reader => _reader.Read())
				.Callback(() => this.row = (this.dataSet.Tables[0].Rows.Count > index) ? this.dataSet.Tables[0].Rows[index++] : (DataRow)null)
				.Returns(() => this.row != null);
			this.reader.Setup(_reader => _reader.GetValue(0)).Returns(() => this.row["ID"].ToString());
			this.reader.Setup(_reader => _reader.GetValue(1)).Returns(() => this.row["ActivityID"].ToString());
			this.reader.Setup(_reader => _reader.GetValue(2)).Returns(() => this.row["ActivityCategoryID"].ToString());
			this.reader.Setup(_reader => _reader.GetString(3)).Returns(() => this.row["ActivityCategoryName"].ToString());
			this.reader.Setup(_reader => _reader.GetString(4)).Returns(() => this.row["ActivityName"].ToString());
			this.reader.Setup(_reader => _reader.GetDateTime(5)).Returns(() => (DateTime)this.row["PerformedOn"]);
			this.reader.Setup(_reader => _reader.GetDouble(6)).Returns(() => (double)this.row["Severity"]);
			this.reader.Setup(_reader => _reader.GetValue(7)).Returns(() => this.row["UserID"].ToString());
			this.reader.Setup(_reader => _reader.GetString(8)).Returns(() => this.row["UserName"].ToString());

			this.parameter = new Mock<IDbDataParameter>();
			this.parameters = new Mock<IDataParameterCollection>();

			this.command = new Mock<IDbCommand>();
			this.command.Setup(_command => _command.CreateParameter()).Returns(this.parameter.Object);
			this.command.Setup(_command => _command.ExecuteReader()).Returns(this.reader.Object);
			this.command.Setup(_command => _command.Parameters).Returns(this.parameters.Object);

			this.connection = new Mock<IDbConnection>();
			this.connection.Setup(_connection => _connection.CreateCommand()).Returns(this.command.Object);

			this.repository = new ParticipationRepository(this.connection.Object, this.modelFactory.Object);
		}
		
		[TestCleanup]
		public override void TestCleanup()
		{
			base.TestCleanup();
			
			this.dataSet = null;
			this.repository = null;
		}
		
		public override void AddRow(IParticipation participation)
		{
			DataRow row = this.dataSet.Tables[0].NewRow();
			
			row["ID"] = participation.Key.ID;
			
			row["ActivityID"] = participation.Activity.Key.ID;
			row["ActivityCategoryID"] = participation.Activity.Category.Key.ID;
			row["ActivityCategoryName"] = participation.Activity.Category.Name;
			row["ActivityName"] = participation.Activity.Name;
			row["PerformedOn"] = participation.PerformedOn;
			row["Severity"] = participation.Severity;
			row["UserID"] = participation.User.Key.ID;
			row["UserName"] = participation.User.Name;

			this.dataSet.Tables[0].Rows.Add(row);
		}
	}
}
