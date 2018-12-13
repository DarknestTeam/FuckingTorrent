using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitTorrent.Enteties
{
  public  class SqlDBConnection
    {
        string _hash { get; set; }
       string _currentdowl { get; set; }
        string _ip { get; set; }
       
      public  SqlDBConnection()
        {
           
        }
      
       public void PutHash(string hash, string dowlm, string ip)
        {
            string connectionString = @"Data Source=DESKTOP-EA677QJ\SQLEXPRESS;initial catalog=TorrentDB;integrated security=True";
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                string sqlExpression = String.Format("INSERT INTO psy_trance_fm_bittorrent_announces  (Info_hash,ip,downloaded) VALUES ('{0}','{1}','{2}')", _hash, _ip,_currentdowl);
                try
                {
                    //Открыть подключение
                    cn.Open();

                    // Создание объекта команды с помощью конструктора
                   
                    SqlCommand myCommand = new SqlCommand(sqlExpression, cn);
                    myCommand.ExecuteNonQuery();
                   
                }
                catch (SqlException ex)
                {
                    // Протоколировать исключение
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    // Гарантировать освобождение подключения
                    cn.Close();
                }
            }

        }

    }
}
