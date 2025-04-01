using FirstSp.Entities;
using Bogus;
namespace FirstSp;
using Microsoft.EntityFrameworkCore.Storage;

public delegate void DelegateContext(Context context);


public class DataBaseManager
{
    private Context _threadAppContext;

    public event Action<int> DataInserted;
    public event Action<Context> GetConnectionEvent;

    //Спеціальний обєкт, який вміє призупитняти довільний потік
    public static ManualResetEvent mre = new ManualResetEvent(false);

    //public event DeletageContextConnection GetConnectionEvent;

    public DataBaseManager()
    {
        Thread runConection = new Thread(RunAsyncConnection);
        runConection.Start();
    }

    private void RunAsyncConnection()
    {
        _threadAppContext = new Context();
        _threadAppContext.Users.Any();
        if (GetConnectionEvent != null)
            GetConnectionEvent(_threadAppContext);
    }

    public void AddBanansAsync(int count, CancellationToken? token = null)
    {
        Thread thread = new Thread(()=>AddBanans(0, count, token));
        thread.Start();
    }

    public void AddBanansReallyAsync(int count, CancellationToken? token = null)
    {
        int processorCount = Environment.ProcessorCount;
        int CountPerThread = (count + processorCount - 1) / processorCount;
        Thread[] threads = new Thread[processorCount];
        
        for (int i = 0; i < processorCount; i++)
        {
            mre.Set();
            int range = CountPerThread;
            if (i == processorCount - 1) range = count - i * CountPerThread;
            
            Thread thread = new Thread(() => AddBanans(i, range, token));
            threads[i] = thread;
            threads[i].Start();
            
        }

        for (int i = 0; i < threads.Length; i++)
        {
            if(threads[i].IsAlive) threads[i].Join();
        }

        
    }

    public Faker<User> CreateUserFaker()
    {
        string url = "https://picsum.photos/1200/800?grayscale";
        var faker = new Faker<User>("en")
            .RuleFor(b => b.Name, f => f.Name.FirstName())
            .RuleFor(b => b.Surname, f => f.Name.LastName())
            .RuleFor(b => b.Photo, url);
        return faker;
    }

    public void Download(int idx, string Name)
    {
        string url = "https://picsum.photos/1200/800?grayscale";
        string FolderPath = "../../../photos";
        HttpClient client = new HttpClient();
        var bytes = client.GetByteArrayAsync(url).Result;
        string name = $"{Name}{idx}" + ".jpg";
        var to = Path.Combine(FolderPath, name);
        File.WriteAllBytes(to, bytes);
    }
    

    public void AddBanans(int start, int count, CancellationToken? token = null)
    {
        
        
        var faker = CreateUserFaker();
        for (int i = 0; i < count; i++)
        {
            var b = faker.Generate(1);
            using (var context = new Context())
            {
                context.Add(b[0]);
                context.SaveChanges();
                Download(i+start, b[0].Name);
            }
            
            
            
            DataInserted?.Invoke(i + 1);

            mre.WaitOne(Timeout.Infinite);

            if (token != null)
            {
                // Перевіряємо, чи був отриманий сигнал на скасування завдання  
                if (token.Value.IsCancellationRequested)
                {
                    return; // Виходимо з методу, завершуючи виконання завдання  
                }
            }

        }
        
    }
    

}