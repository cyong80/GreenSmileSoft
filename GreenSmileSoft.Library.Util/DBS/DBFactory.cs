using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenSmileSoft.Library.Util.DBS
{
    public enum DBServerType
    {
        DEFAULT = MSSQL,
        MSSQL = 1,
        MYSQL = 2
    }
    public class DBFactory :IDisposable
    {
        private IDbCommand command = null;
        private IDbConnection connection = null;
        private DBServerType DBServer = DBServerType.DEFAULT;
        private bool isDisposing;

        public IDbCommand Command
        {
            get
            {
                return command;
            }
        }
        public IDbDataAdapter DbDataAdapter
        {
            get
            {
                if(this.command == null)
                {
                    throw new NullReferenceException("DBFactory Connection is null");
                }
                switch(DBServer)
                {
                    case DBServerType.MYSQL:
                        return new MySqlDataAdapter(this.command as MySqlCommand);
                    default:
                        return new SqlDataAdapter(this.command as SqlCommand);
                }
            }
        }

        public DBFactory(IDbConnection conn)
        {
            
            this.connection = conn;
            this.setDBType();
            this.setCommand();
        }

        private void setDBType()
        {
            if(connection is MySqlConnection)
            {
                DBServer = DBServerType.MYSQL;
            }
        }

        private void setCommand()
        {
            if(this.connection == null)
            {
                throw new NullReferenceException("DBFactory Connection is null");
            }

            switch(DBServer)
            { 
                case DBServerType.MYSQL:
                    this.command = new MySql.Data.MySqlClient.MySqlCommand();
                    break;
                default:
                    this.command = new SqlCommand();
                    break;
            }
            this.command.CommandTimeout = 0;
            this.command.Connection = this.connection;
        }

        public void SetParameters(List<KeyValuePair<string, object>> parameters)
        { 
            if(parameters == null)
            {
                return;
            }
            this.command.Parameters.Clear();

            if(this.command.CommandType == CommandType.StoredProcedure)
            {
                foreach(var p in parameters)
                {
                    addProcParameter(p.Key, p.Value);
                }
            }
            else if(this.command.CommandType == CommandType.Text)
            {
                if(parameters.Count == 0)
                {
                    return;
                }
                var p = from x in parameters select x.Value;
                this.command.CommandText = string.Format(this.command.CommandText, p.ToArray());
            }

        }

        private void addProcParameter(string name, object value)
        {
            IDbDataParameter param = null;

            switch(DBServer)
            {
                case DBServerType.MYSQL:
                    param = new MySqlParameter(name, value);
                    break;
                default:
                    param = new SqlParameter(name, value);
                    break;
            }

            this.command.Parameters.Add(param);
        }

        public void Dispose()
        {
            if(!this.isDisposing)
            {
                this.isDisposing = true;
                if(this.command != null)
                {
                    this.command.Dispose();
                }
            }
            this.isDisposing = true;
        }
    }
}
