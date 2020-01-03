using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace DOAE.DAL 
{
    public    class SQLObject
    {
        public SqlConnection sqlConnectionMaster;
        public SqlConnection sqlConnectionClient;
        public SqlConnection sqlConnectionImport;
        const int itryCount = 10;
        public string MasterConnectionString = string.Empty;
        /// <summary>
        /// Execute Command on Master Database
        /// </summary>
        /// <param name="pCommand">SQL Command</param>        
        /// <returns>SqlDataReader object</returns>
        public SqlDataReader ExecuteMasterReader(SqlCommand pCommand, bool pisConOpen)
        {
            pCommand.CommandType = CommandType.StoredProcedure;
            SqlDataReader sqlreader = null;
            pCommand.CommandTimeout = 0;
            if (!pisConOpen)
            {
                pCommand.Connection.Open();
            }
            sqlreader = pCommand.ExecuteReader();

            //AuditTrail objLog = new AuditTrail();
            //objLog.LogSqlStatements(pCommand.CommandAsSql());

            return sqlreader;
        }

        /// <summary>
        /// Execute Command with Scalar method on Master Database
        /// </summary>
        /// <param name="pCommand">SQL Command</param>        
        /// <returns>Object</returns>
        public Object ExecuteMasterScalar(SqlCommand pCommand)
        {
            pCommand.CommandType = CommandType.StoredProcedure;
            Object objScalarReturn = null;
            int iTry = 0;
            using (SqlConnection sqlConnMaster = new SqlConnection(GetMasterDBConnString()))
            {
                pCommand.Connection = sqlConnMaster;
                while (sqlConnMaster.State != ConnectionState.Open && iTry < itryCount)
                {
                    if (sqlConnMaster.State != ConnectionState.Connecting)
                    {
                        sqlConnMaster.Open();
                    }
                    if (sqlConnMaster.State == ConnectionState.Open)
                    {
                        objScalarReturn = pCommand.ExecuteScalar();
                        break;
                    }
                    iTry++;
                }
            }

            //AuditTrail objLog = new AuditTrail();
            //objLog.LogSqlStatements(pCommand.CommandAsSql());

            return objScalarReturn;
        }

        /// <summary>
        /// Execute Command with Scalar method on Master Database
        /// </summary>
        /// <param name="pCommand">SQL Command</param>        
        /// <returns>Object</returns>
        public Object ExecuteScalar(SqlCommand pCommand, bool pIsConnOpen)
        {
            pCommand.CommandType = CommandType.StoredProcedure;
            Object objScalarReturn = null;

            if (!pIsConnOpen)
            {
                pCommand.Connection.Open();
            }
            objScalarReturn = pCommand.ExecuteScalar();


            //AuditTrail objLog = new AuditTrail();
            //objLog.LogSqlStatements(pCommand.CommandAsSql());

            return objScalarReturn;
        }


        /// <summary>
        /// Method with Client Database connectionstring 
        /// It is the responsibility of the caller to close reader when finished.
        /// </summary>
        /// <param name="pCommand">SQL Command</param>
        /// <param name="pIsConnOpen">Is Connection Open</param>
        /// <returns>SqlDataReader object</returns>
        public SqlDataReader SqlDataReader(SqlCommand pCommand, bool pIsConnOpen)
        {
            SqlDataReader sqlreader;
            pCommand.CommandType = CommandType.StoredProcedure;
            if (!pIsConnOpen)
            {
                pCommand.Connection.Open();
            }
            sqlreader = pCommand.ExecuteReader();

            //AuditTrail objLog = new AuditTrail();
            //objLog.LogSqlStatements(pCommand.CommandAsSql());

            return sqlreader;
        }

        /// <summary>
        /// Execute Command for Single Update
        /// </summary>
        /// <param name="pCommand">SQL Command</param>
        /// <param name="pConnString">Connection string</param>
        /// <returns>returns true if success</returns>
        public int ExecuteNonQuery(SqlCommand pCommand, string pConnString)
        {
            int i = 0;
            pCommand.CommandType = CommandType.StoredProcedure;
            pCommand.CommandTimeout = 0;
            using (SqlConnection sqlcon = new SqlConnection(pConnString))
            {
                sqlcon.Open();
                pCommand.Connection = sqlcon;
                i = pCommand.ExecuteNonQuery();
            }
            //AuditTrail objLog = new AuditTrail();
            //objLog.LogSqlStatements(pCommand.CommandAsSql());
            return i;
        }

        /// <summary>
        /// Execute Command for Single Update
        /// </summary>
        /// <param name="pCommand">SQL Command</param>
        /// <param name="pConnString">Connection string</param>
        /// <param name="pIsExecuted">boolean Is Executed</param>
        /// <returns>returns true if success</returns>
        public bool ExecuteNonQuery(SqlCommand pCommand, string pConnString, bool pIsExecuted)
        {
            pCommand.CommandType = CommandType.StoredProcedure;
            pIsExecuted = false;

            using (SqlConnection sqlcon = new SqlConnection(pConnString))
            {
                sqlcon.Open();
                pCommand.Connection = sqlcon;
                pCommand.ExecuteNonQuery();
            }
            pIsExecuted = true;

            //AuditTrail objLog = new AuditTrail();
            //objLog.LogSqlStatements(pCommand.CommandAsSql());

            return pIsExecuted;
        }

        /// <summary>
        /// To execute DML/DDL statements
        /// </summary>
        /// <param name="pCommand"></param>
        /// <returns>Returns true if success</returns>
        public bool ExecuteNonQuery(SqlCommand pCommand)
        {
            pCommand.CommandType = CommandType.StoredProcedure;
            bool bReturnVal = false;
            int i = pCommand.ExecuteNonQuery();
            bReturnVal = true;

            //AuditTrail objLog = new AuditTrail();
            //objLog.LogSqlStatements(pCommand.CommandAsSql());

            return bReturnVal;
        }
        /// <summary>
        /// To execute DML/DDL statements
        /// </summary>
        /// <param name="pCommand"></param>
        /// <returns>Returns true if success</returns>
        public int ExecuteNonQueryCount(SqlCommand pCommand)
        {
            pCommand.CommandType = CommandType.StoredProcedure;
            int i = 0;
            i = pCommand.ExecuteNonQuery();

            //AuditTrail objLog = new AuditTrail();
            //objLog.LogSqlStatements(pCommand.CommandAsSql());
            return i;
        }

        /// <summary>
        /// To create Master database connnection string
        /// </summary>
        /// <returns> connnection string</returns>
        public string GetMasterDBConnString()
        {
            string MasterConnectionString = ConfigurationManager.ConnectionStrings["SessionConn"].ToString();
            return MasterConnectionString;
        }

        /// <summary>
        /// To get requested client database connection string from Client
        /// </summary>
        /// <param name="pClient">Client</param>
        /// <returns>returns Client database connection string</returns>
        //public  string GetClientDBConnString(Client pClient)
        //{            
        //    //return "Data Source=" + pClient.DBIPAddress.Trim() + ";Initial Catalog=" + pClient.DatabaseName.Trim() + ";User ID=" + pClient.DBUID.Trim() + ";password=" + pClient.DBPassword.Trim() + ";Max Pool Size=10;Min Pool Size=4;";                       
        //    return "Data Source=" + pClient.DBIPAddress.Trim() + ",1433;Network Library=DBMSSOCN" + ";Initial Catalog=" + pClient.DatabaseName.Trim() + ";User ID=" + pClient.DBUID.Trim() + ";password=" + pClient.DBPassword.Trim() + ";";                       
        //}

        /// <summary>
        /// Returns DataSet result from Command
        /// </summary>
        /// <param name="pCommand">SQL Command</param>
        /// <param name="pConnString">Connection string</param>
        /// <returns>DataSet</returns>
        public DataSet SqlDataAdapter(SqlCommand pCommand, string pConnString)
        {
            DataSet dSet = null;
            pCommand.CommandType = CommandType.StoredProcedure;

            using (SqlConnection sqlconn = new SqlConnection(pConnString))
            {
                pCommand.Connection = sqlconn;
                dSet = SqlDataAdapter(pCommand);
            }

            //AuditTrail objLog = new AuditTrail();
            //objLog.LogSqlStatements(pCommand.CommandAsSql());

            return dSet;
        }


        public DataSet SqlDataAdapter(SqlCommand pCommand)
        {
            DataSet dSet = null;
            pCommand.CommandType = CommandType.StoredProcedure;
            pCommand.CommandTimeout = 0;
            dSet = new DataSet();
            using (SqlDataAdapter dataAdaptor = new SqlDataAdapter(pCommand))
            {
                dataAdaptor.Fill(dSet);
            }
            //AuditTrail objLog = new AuditTrail();
            //objLog.LogSqlStatements(pCommand.CommandAsSql());

            return dSet;
        }

        /// <summary>
        /// Check for Column 
        /// </summary>
        /// <param name="reader">Reader to Check</param>
        /// <param name="pColumnName">Name</param>
        /// <returns></returns>
        public bool ReaderHasColumn(IDataReader reader, string pColumnName)
        {
            reader.GetSchemaTable().DefaultView.RowFilter = "ColumnName= '" + pColumnName + "'";
            return (reader.GetSchemaTable().DefaultView.Count > 0);
        }
    }
}
