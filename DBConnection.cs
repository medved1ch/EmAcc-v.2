using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace uchet
{
    class DBConnection
    {
        public SQLiteConnection myConn;
        public SQLiteCommand myComm;
        public SQLiteDataReader myReader;

        public void openConnection(string query)
        {
            myConn = new SQLiteConnection("Data Source=uchet.db;Version=3;");
            myConn.Open();
            myComm = new SQLiteCommand(query, myConn);
            myComm.ExecuteNonQuery();
            myReader = myComm.ExecuteReader();
        }
    }
}
