using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace Fougerite
{
    public class SQLiteConnector
    {
        private static SQLiteConnector _inst;
        public string SQLitePath = Directory.GetCurrentDirectory() + "\\Save\\FougeriteSQL.sqlite";
        private SQLiteConnection _dbConnection;

        internal void Setup()
        {
            string extDir = Directory.GetCurrentDirectory() + "\\rust_server_Data\\Managed";
            File.WriteAllText(Path.Combine(extDir, "System.Data.SQLite.dll.config"), $"<configuration>\n<dllmap dll=\"sqlite3\" target=\"{extDir}\\x86\\libsqlite3.so\" os=\"!windows,osx\" cpu=\"x86\" />\n<dllmap dll=\"sqlite3\" target=\"{extDir}\\x64\\libsqlite3.so\" os=\"!windows,osx\" cpu=\"x86-64\" />\n</configuration>");
            if (!File.Exists(SQLitePath))
            {
                SQLiteConnection.CreateFile(SQLitePath);
            }
        }

        public SQLiteConnection Connect(string extraarguments = ";Version=3;New=False;Compress=True;Foreign Keys=True;")
        {
            _dbConnection = new SQLiteConnection("Data Source=" + SQLitePath + extraarguments);
            return _dbConnection;
        }

        public SQLiteConnection Connection
        {
            get { return _dbConnection; }
        }

        public SQLiteCommand CreateSQLiteCommand(string command)
        {
            return new SQLiteCommand(command);
        }

        public SQLiteCommand CreateSQLiteCommand(string command, SQLiteConnection con)
        {
            return new SQLiteCommand(command, con);
        }

        public static SQLiteConnector GetInstance
        {
            get
            {
                if (_inst == null)
                {
                    _inst = new SQLiteConnector();
                }
                return _inst;
            }
        }
    }
}
