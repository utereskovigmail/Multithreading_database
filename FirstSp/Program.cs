using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.SqlServer;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace FirstSp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            DataBaseManager db = new DataBaseManager();
            // db.OnContextChanged += DataBaseManager_GetConnectionEvent;

            int numebr_of_users = 100;
            
            db.AddUsers(numebr_of_users);
            
            
            
            
            
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);
        }

        private static void DataBaseManager_GetConnectionEvent(Context context)
        {
            Console.WriteLine("Successful connection event");
        }
        
        
    }
    
    
}