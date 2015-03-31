using System;
using System.Data;
using System.Data.SqlClient;

namespace EventSourcingDemo.Features
{
    public interface IRepository
    {
        IDbConnection Connection { get; }
    }

    public class Repository : IRepository
    {
        public Repository()
        {
            Connection = new SqlConnection("server=localhost;database=spike;trusted_connection=true");
            Connection.Open();
        }

        public IDbConnection Connection { get; private set; }
    }
}