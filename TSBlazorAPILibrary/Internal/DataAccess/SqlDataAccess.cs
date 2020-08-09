using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

//This uses Dapper. Is a mirco ORM (Object Relation Mapper). allows you to talk to database to get information
//and then map it to an object. Works really well with StoredProcedure

namespace TSBlazorAPILibrary.Internal.DataAccess
{
    internal class SqlDataAccess //: IDisposable
    {
        private readonly IConfiguration _config;

        public SqlDataAccess(IConfiguration config)
        {
            _config = config;
  
        }

        //public void Dispose()
        //{
        //    throw new NotImplementedException();
        //}

        public string GetConnectionString(string name)
        {
            //look in the system configuration for a ConnectionString with a matching name
            //and then return that connection string.
            //Since this is a library there is no config file. It has to be ran through a web or program
            //This will use the web config for the API
            return _config.GetConnectionString(name);
        }

        public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                List<T> rows = connection.Query<T>(storedProcedure, parameters,
                    commandType: CommandType.StoredProcedure).ToList();

                return rows;
            }
        }

        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Execute(storedProcedure, parameters,
                   commandType: CommandType.StoredProcedure);
            }
        }

       


    }
}
