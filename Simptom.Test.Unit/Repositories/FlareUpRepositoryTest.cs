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
	public class FlareUpRepositoryTest : IFlareUpRepositoryTest
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
			table.Columns.Add("ExperiencedOn", typeof(DateTime));
			table.Columns.Add("Severity", typeof(double));
			table.Columns.Add("SymptomID", typeof(Guid));
			table.Columns.Add("SymptomCategoryID", typeof(Guid));
			table.Columns.Add("SymptomCategoryName", typeof(string));
			table.Columns.Add("SymptomName", typeof(string));
			table.Columns.Add("UserID", typeof(Guid));
			table.Columns.Add("UserName", typeof(string));

			this.reader = new Mock<IDataReader>();
			this.reader.Setup(_reader => _reader.Read())
				.Callback(() => this.row = (this.dataSet.Tables[0].Rows.Count > index) ? this.dataSet.Tables[0].Rows[index++] : (DataRow)null)
				.Returns(() => this.row != null);
			this.reader.Setup(_reader => _reader.GetValue(0)).Returns(() => this.row["ID"].ToString());
			this.reader.Setup(_reader => _reader.GetDateTime(1)).Returns(() => (DateTime)this.row["ExperiencedOn"]);
			this.reader.Setup(_reader => _reader.GetDouble(2)).Returns(() => (double)this.row["Severity"]);
			this.reader.Setup(_reader => _reader.GetValue(3)).Returns(() => this.row["SymptomID"].ToString());
			this.reader.Setup(_reader => _reader.GetValue(4)).Returns(() => this.row["SymptomCategoryID"].ToString());
			this.reader.Setup(_reader => _reader.GetString(5)).Returns(() => this.row["SymptomCategoryName"].ToString());
			this.reader.Setup(_reader => _reader.GetString(6)).Returns(() => this.row["SymptomName"].ToString());
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

			this.repository = new FlareUpRepository(this.connection.Object, this.modelFactory.Object);
		}
		
		[TestCleanup]
		public override void TestCleanup()
		{
			base.TestCleanup();
			
			this.dataSet = null;
			this.repository = null;
		}
		
		public override void AddRow(IFlareUp flareUp)
		{
			DataRow row = this.dataSet.Tables[0].NewRow();
			
			row["ID"] = flareUp.Key.ID;
			
			row["ExperiencedOn"] = flareUp.ExperiencedOn;
			row["Severity"] = flareUp.Severity;
			row["SymptomID"] = flareUp.Symptom.Key.ID;
			row["SymptomCategoryID"] = flareUp.Symptom.Category.Key.ID;
			row["SymptomCategoryName"] = flareUp.Symptom.Category.Name;
			row["SymptomName"] = flareUp.Symptom.Name;
			row["UserID"] = flareUp.User.Key.ID;
			row["UserName"] = flareUp.User.Name;

			this.dataSet.Tables[0].Rows.Add(row);
		}
	}
}
