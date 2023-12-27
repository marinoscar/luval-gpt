using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace Luval.WebGPT.Presenter
{
    public class SqlPresenter
    {

        public SqlPresenter()
        {
            QueryResult = CreateTable();
        }

        public void GetSqlQueryResult(string sql)
        {
            QueryResult.Clear();
            using (var conn = CreateConnection())
            {
                try
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = tran;
                            cmd.CommandText = sql;
                            cmd.CommandType = CommandType.Text;
                            using (var reader = cmd.ExecuteReader())
                            {
                                var first = true;
                                while (reader.Read())
                                {
                                    var row = new Dictionary<string, string>();
                                    if (first)
                                    {
                                        first = false;
                                        var header = new Dictionary<string, string>();
                                        for (int i = 0; i < reader.FieldCount; i++)
                                        {
                                            header[reader.GetName(i)] = reader.GetName(i);
                                        }
                                        QueryResult.Add(header);
                                    }
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        row[reader.GetName(i)] = GetFormattedValue(reader.GetValue(i));
                                    }
                                    QueryResult.Add(row);
                                }
                            }
                            tran.Commit();
                        }
                    }
                    conn.Close();
                }
                catch (Exception e)
                {
                    QueryResult = ReturnError(e);
                }
            }
        }

        public string GetFormattedValue(object o)
        {
            if (o == DBNull.Value || o == null) return "Null";
            if (o is DateTime)
                return ((DateTime)o).ToString("s");
            if (o is DateTimeOffset)
                return ((DateTime)o).ToString("s");
            if (o is TimeSpan)
                return ((TimeSpan)o).ToString("t");
            if (o is double)
                return ((double)o).ToString("N2");
            if (o is float)
                return ((float)o).ToString("N2");
            if (o is decimal)
                return ((decimal)o).ToString("N2");
            return Convert.ToString(o);

        }

        public List<Dictionary<string, string>> QueryResult { get; set; }

        private List<Dictionary<string, string>> CreateTable()
        {
            return new List<Dictionary<string, string>> {
                new Dictionary<string, string>() { { "Results", "Results"} } ,
                new Dictionary<string, string>() { { "Results", "Empty"} } ,
            };
        }

        private List<Dictionary<string, string>> ReturnError(Exception exception)
        {
            return new List<Dictionary<string, string>> {
                new Dictionary<string, string>() { { "Error Message", "Error Message"} } ,
                new Dictionary<string, string>() { { "Error Message", exception.Message} } ,
            };
        }

        private IDbConnection CreateConnection()
        {
            var connString = ServiceExtensions.GetConnectionString();
            return new MySqlConnection(connString);
        }
    }
}
