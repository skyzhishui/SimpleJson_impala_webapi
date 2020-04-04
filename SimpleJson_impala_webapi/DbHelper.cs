using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleJson_impala_webapi
{
    public static class DbHelper
    {
        private static OdbcConnection connection;
        public static string connectionString = "DSN=myimpala";

        public static void InitConnect()
        {
            connection = new OdbcConnection(connectionString);
        }
        private static void openConnected() 
        {
            if (connection.State != System.Data.ConnectionState.Open) 
            {
                connection.Open();
            }
        }
        public static DataTable ExecuteDataTable(string SQLString)
        {
            
            DataSet ds = new DataSet();
            try
            {
                openConnected();
                using (OdbcDataAdapter command = new OdbcDataAdapter(SQLString, connection)) 
                {
                    command.Fill(ds, "ds");
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                throw new Exception(ex.Message);
            }
            return ds.Tables[0];

        }
    }
}
