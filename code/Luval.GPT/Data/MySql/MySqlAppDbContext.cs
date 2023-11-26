using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Data.MySql
{
    public class MySqlAppDbContext : AppDbContext
    {
        private string _connectionString;

        public MySqlAppDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            object value = optionsBuilder.UseMySQL(_connectionString);
        }
    }
}
