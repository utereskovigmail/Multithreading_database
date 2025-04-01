using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using FirstSp;

namespace ThreadAndDatabase
{
   
    internal class Program
    {
        private static DataBaseManager _dataBaseManager;
        //Токен для переривання процесу роботи потоку.
        private static CancellationTokenSource cancellationToken;
        private static CancellationToken token;
        

        static void Main(string[] args)
        {
            Context context = new Context();
            _dataBaseManager = new DataBaseManager();
            Stopwatch stopwatch = Stopwatch.StartNew();
            
            
            Console.WriteLine("Addinig one user - ");
            context.Add(_dataBaseManager.CreateUserFaker().Generate(1)[0]);
            _dataBaseManager.Download(888,"ytytyytyty");
            stopwatch.Stop();
            
            Console.WriteLine("Finished");
            int TimeForOne = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
            Console.WriteLine("Total time: {0}", TimeForOne + " milliseconds");
            Console.WriteLine();

            int NumberOfUsers = 15;
            double PredictedTime = TimeForOne * NumberOfUsers / 1000.0;
            Console.WriteLine("Predicted time for : "+ NumberOfUsers+ " users using one thread is --------" + PredictedTime + " seconds");
            
            stopwatch = Stopwatch.StartNew();
            
            
            Console.WriteLine($"Adding {NumberOfUsers} users with multi threading -----");
            Console.WriteLine("Time starts");
            
            _dataBaseManager.AddBanansReallyAsync(NumberOfUsers, token);
            
            stopwatch.Stop();
            Console.WriteLine("Time --"+ stopwatch.Elapsed);
            
            
            
            
        }

        private static void _dataBaseManager_DataInserted(int obj)
        {
            Console.WriteLine($"Insert data --{obj}--");
        }

        private static void DataBaseManager_GetConnectionEvent(Context threadAppContext )
        {
            cancellationToken = new CancellationTokenSource();
            CancellationToken token = cancellationToken.Token;

            DataBaseManager.mre.Set(); //Потік буде працювати у стандартному режимі 
            //Console.WriteLine("Зєднання з БД успішно кількість бананів {0}", threadAppContext.Banans.Count());
            Console.WriteLine("Вкажіть кількість користувачів");
            int count = int.Parse(Console.ReadLine());
            //_dataBaseManager.AddBanans(count);
            _dataBaseManager.AddBanansAsync(count, token);
            var isTrue=true;
            while (isTrue)
            {
                Console.WriteLine("Назміть p - пауза, r - відновити, q - вихід");
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.P)
                {
                    DataBaseManager.mre.Reset(); //Призупинити виконання
                    Console.WriteLine("Пауза ...");
                }

                else if (key == ConsoleKey.R)
                {
                    DataBaseManager.mre.Set();
                    Console.WriteLine("Віновлено ...");
                }

                else if (key == ConsoleKey.Q)
                {
                    Console.WriteLine("Вихід");
                    cancellationToken.Cancel();
                    DataBaseManager.mre.Set();
                    
                    isTrue =false;
                }
            }
        }
    }
    
    
}
