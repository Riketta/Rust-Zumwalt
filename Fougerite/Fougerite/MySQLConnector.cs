using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

//using MySql.Data.MySqlClient;

namespace Fougerite
{
    public class MySQLConnector
    {
        private static MySQLConnector _inst;

        private MySqlConnection connection;
        public string ServerAddress;
        public string DataBase;
        private string _username;
        private string _password;

        public MySqlConnection Connect(string ip, string database, string username, string passwd, string extraarg = "")
        {
            ServerAddress = ip;
            DataBase = database;
            _username = username;
            _password = passwd;
            string connectionString = "SERVER=" + ServerAddress + ";" + "DATABASE=" +
            DataBase + ";" + "UID=" + _username + ";" + "PASSWORD=" + _password + ";" + extraarg;

            connection = new MySqlConnection(connectionString);
            return connection;
        }

        public bool ExecuteQuery(string query)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = query;
                cmd.Connection = connection;
                cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                Logger.LogError("Failed to execute query " + ex);
                return false;
            }
            return true;
        }

        public bool OpenConnection()
        {
            try
            {
                if (connection != null)
                {
                    connection.Open();
                }
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        Logger.LogError("Cannot connect to server.");
                        break;

                    case 1045:
                        Logger.LogError("Invalid username/password, please try again");
                        break;
                    default:
                        Logger.LogError("Error: " + ex);
                        break;
                }
                return false;
            }
        }
        public bool CloseConnection()
        {
            try
            {
                if (connection != null) connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Logger.LogError("Failed to close connection " + ex.Message);
                return false;
            }
        }

        public MySqlCommand CreateMysqlCommand()
        {
            return new MySqlCommand();
        }

        public MySqlConnection Connection
        {
            get { return connection; }
        }

        public static MySQLConnector GetInstance
        {
            get
            {
                if (_inst == null)
                {
                    _inst = new MySQLConnector();
                }
                return _inst;
            }
        }
    }
}
