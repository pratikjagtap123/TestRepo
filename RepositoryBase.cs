using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace DOAE.DAL
{
    public   class RepositoryBase<T> : IRepository<T>
    {
        #region Public Methods
        public int ExecuteNonQuery(T obj, string[] param, string spName)
        {
            PropertyInfo[] Props;
            SqlCommand sqlCmd = new SqlCommand();
            SQLObject sqlObj = new SQLObject();
            int iRowsAffected = 0;
            string sqlConnString;
            try
            {
                Props = typeof(T).GetProperties();
                sqlConnString = sqlObj.GetMasterDBConnString();
                sqlCmd.CommandText = spName;
                List<string> lstParas = param.ToList();
                //var f = (from c in Props
                //         where (lstParas.Contains(c.Name))
                //         select new { PropName = c.Name, PropVal = Convert.ToString(c.GetValue(obj, null)) }).ToList();

                var f = (from c in Props
                         where (lstParas.Contains(c.Name))
                         select new { PropName = c.Name, PropVal = c.GetValue(obj, null), PropType = c.PropertyType }).ToList();


                f.ForEach(x => AddParametersToCmd(x.PropName, x.PropVal, x.PropType, ref sqlCmd, false));
                iRowsAffected = sqlObj.ExecuteNonQuery(sqlCmd, sqlConnString);
                return iRowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
            }
        }
        public T Get(T obj, string[] param, string spName)
        {
            SqlCommand sqlCmd = new SqlCommand();
            SqlConnection sqlconn = new SqlConnection();
            SQLObject sqlObj = new SQLObject();
            SqlDataReader rd = null;
            PropertyInfo[] Props;
            string sqlConnString;
            try
            {
                Props = typeof(T).GetProperties();
                sqlConnString = sqlObj.GetMasterDBConnString();
                sqlconn.ConnectionString = sqlConnString;

                sqlCmd.Connection = sqlconn;
                sqlCmd.CommandText = spName;
                if (param != null)
                {
                    List<string> lstParas = param.ToList();
                    var f = (from c in Props
                             where (lstParas.Contains(c.Name))
                             select new { PropName = c.Name, PropVal = Convert.ToString(c.GetValue(obj, null)), PropType = c.PropertyType }).ToList();

                    f.ForEach(x => AddParametersToCmd(x.PropName, x.PropVal, x.PropType, ref sqlCmd, true));
                }
                rd = sqlObj.ExecuteMasterReader(sqlCmd, false);
                return AutoMapReaderSingle<T>(rd);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (rd != null && !rd.IsClosed) rd.Close();
                if (sqlconn != null && sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
            }
        }

        public IEnumerable<T> GetAll(T obj, string[] param, string spName)
        {

            PropertyInfo[] Props;
            SqlCommand sqlCmd = new SqlCommand();
            SqlConnection sqlconn = new SqlConnection();
            SQLObject sqlObj = new SQLObject();
            SqlDataReader rd = null;
            string sqlConnString;

            try
            {
                Props = typeof(T).GetProperties();
                sqlConnString = sqlObj.GetMasterDBConnString();
                sqlconn.ConnectionString = sqlConnString;
                sqlCmd.Connection = sqlconn;
                sqlCmd.CommandText = spName;
                if (param != null)
                {
                    List<string> lstParas = param.ToList();
                    var f = (from c in Props
                             where (lstParas.Contains(c.Name))
                             select new { PropName = c.Name, PropVal = Convert.ToString(c.GetValue(obj, null)), PropType = c.PropertyType }).ToList();

                    f.ForEach(x => AddParametersToCmd(x.PropName, x.PropVal, x.PropType, ref sqlCmd, true));
                }
                rd = sqlObj.ExecuteMasterReader(sqlCmd, false);
                List<T> lst = new List<T>();
                lst = AutoMapReaderAll<T>(rd).ToList();
                return lst;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (rd != null && !rd.IsClosed) rd.Close();
                if (sqlconn != null && sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
            }
        }
        public IEnumerable<T> GetAll(object obj, string[] param, string spName)
        {
            PropertyInfo[] Props;
            SqlCommand sqlCmd = new SqlCommand();
            SqlConnection sqlconn = new SqlConnection();
            SQLObject sqlObj = new SQLObject();
            SqlDataReader rd = null;
            string sqlConnString;
            try
            {
                Props = typeof(T).GetProperties();

                sqlConnString = sqlObj.GetMasterDBConnString();

                sqlconn.ConnectionString = sqlConnString;

                sqlCmd.Connection = sqlconn;

                sqlCmd.CommandText = spName;
                if (param != null)
                {
                    List<string> lstParas = param.ToList();

                    var f = (from c in Props
                             where (lstParas.Contains(c.Name))
                             select new { PropName = c.Name, PropVal = Convert.ToString(c.GetValue(obj, null)), PropType = c.PropertyType }).ToList();

                    f.ForEach(x => AddParametersToCmd(x.PropName, x.PropVal, x.PropType, ref sqlCmd, true));


                }
                rd = sqlObj.ExecuteMasterReader(sqlCmd, false);
                List<T> lst = new List<T>();
                lst = AutoMapReaderAll<T>(rd).ToList();
                return lst;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (rd != null && !rd.IsClosed) rd.Close();
                if (sqlconn != null && sqlconn.State != ConnectionState.Closed)
                    sqlconn.Close();
            }
        }
        public DataTable GetAlDataTablel(T obj, string[] param, string spName)
        {
            PropertyInfo[] Props;
            SqlCommand sqlCmd = new SqlCommand();
            SQLObject sqlObj = new SQLObject();
            DataSet ds = null;
            string sqlConnString;
            try
            {
                Props = typeof(T).GetProperties();
                sqlConnString = sqlObj.GetMasterDBConnString();
                sqlCmd.CommandText = spName;
                if (param != null)
                {
                    List<string> lstParas = param.ToList();

                    var f = (from c in Props
                             where (lstParas.Contains(c.Name))
                             select new { PropName = c.Name, PropVal = Convert.ToString(c.GetValue(obj, null)), PropType = c.PropertyType }).ToList();

                    f.ForEach(x => AddParametersToCmd(x.PropName, x.PropVal, x.PropType, ref sqlCmd, true));

                }

                ds = sqlObj.SqlDataAdapter(sqlCmd, sqlConnString);
                return (ds.Tables.Count > 0) ? ds.Tables[0] : null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {

            }
        }
        public object GetScalar(T obj, string[] param, string spName)
        {
            PropertyInfo[] Props;
            SqlCommand sqlCmd = new SqlCommand();
            SQLObject sqlObj = new SQLObject();
            string sqlConnString;
            try
            {
                Props = typeof(T).GetProperties();
                sqlConnString = sqlObj.GetMasterDBConnString();
                sqlCmd.CommandText = spName;
                if (param != null)
                {
                    List<string> lstParas = param.ToList();

                    var f = (from c in Props
                             where (lstParas.Contains(c.Name))
                             select new { PropName = c.Name, PropVal = Convert.ToString(c.GetValue(obj, null)), PropType = c.PropertyType }).ToList();

                    f.ForEach(x => AddParametersToCmd(x.PropName, x.PropVal, x.PropType, ref sqlCmd, true));
                }
                return sqlObj.ExecuteMasterScalar(sqlCmd);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {

            }
        }

        #endregion

        #region Private Methods
        private void AddParametersToCmd(string paraName, object paraValue, Type propType, ref SqlCommand pSqlcmd, bool IsGetCall)
        {
            try
            {
                SqlParameter _sqlPara;
                string paraValueDateString = string.Empty;

                //code change amit 21/08/14
                //handle single qoute
                if (paraValue is string && IsGetCall)
                {
                    if (Convert.ToString(paraValue).Contains("'") && !paraName.ToUpper().Contains("WHERE"))
                    {
                        paraValue = Convert.ToString(paraValue).Replace("'", "''");
                    }
                }

                if (propType == typeof(DateTime?) || propType == typeof(DateTime))
                {
                    DateTime dt;
                    if (DateTime.TryParse(Convert.ToString(paraValue), out dt))
                    {
                        paraValueDateString = dt.ToString("yyyy-MM-dd");
                        paraValue = paraValueDateString;
                    }
                }

                _sqlPara = new SqlParameter(paraName, paraValue);
                pSqlcmd.Parameters.Add(_sqlPara);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private T AutoMapReaderSingle<T>(SqlDataReader rd)
        {
            try
            {
                T objInstance = Activator.CreateInstance<T>();
                while (rd.Read())
                {
                    var prop = from c in typeof(T).GetProperties()
                               select new { Property = c, PropertyName = c.Name };

                    prop.ToList().ForEach(x => x.Property.SetValue(objInstance, TryGetValue(rd, x.PropertyName), null));
                }
                return objInstance;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private IEnumerable<T> AutoMapReaderAll<T>(SqlDataReader rd)
        {
            List<T> objTCollections = new List<T>();
            while (rd.Read())
            {
                T objInstance = Activator.CreateInstance<T>();
                var prop = from c in typeof(T).GetProperties()
                           select new { Property = c, PropertyName = c.Name };

                prop.ToList().ForEach(x => x.Property.SetValue(objInstance, TryGetValue(rd, x.PropertyName), null));
                objTCollections.Add(objInstance);
            }
            return objTCollections;
        }
        private object TryGetValue(SqlDataReader reader, string name)
        {
            try
            {
                DataTable table = reader.GetSchemaTable();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    if (table.Rows[i][0].ToString().ToLower() == name.ToLower())
                    {
                        if (DBNull.Value.Equals(reader[i]))
                            return null;
                        else
                            return reader[i];
                    }
                }
                // Return null if the column is not in the result.
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }



        public DataSet GetAllDataSet(T obj, string[] param, string spName)
        {
            PropertyInfo[] Props;
            SqlCommand sqlCmd = new SqlCommand();
            SQLObject sqlObj = new SQLObject();
            DataSet ds = null;
            string sqlConnString;
            try
            {
                Props = typeof(T).GetProperties();
                sqlConnString = sqlObj.GetMasterDBConnString();
                sqlCmd.CommandText = spName;
                if (param != null)
                {
                    List<string> lstParas = param.ToList();

                    var f = (from c in Props
                             where (lstParas.Contains(c.Name))
                             select new { PropName = c.Name, PropVal = Convert.ToString(c.GetValue(obj, null)), PropType = c.PropertyType }).ToList();

                    f.ForEach(x => AddParametersToCmd(x.PropName, x.PropVal, x.PropType, ref sqlCmd, true));

                }

                ds = sqlObj.SqlDataAdapter(sqlCmd, sqlConnString);
                return (ds.Tables.Count > 0) ? ds : null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {

            }
        }
        #endregion
    }
}
