using FirstSp.Entities;
using Bogus;
namespace FirstSp;

public delegate void DelegateContext(Context context);


public class DataBaseManager
    {
        private Context _context;
        public event DelegateContext OnContextChanged;
        private int usersCount = 0; 
        private object lockObject = new object();
        public event Action<int> OnUserCountChanged;

        public DataBaseManager()
        {
            Thread runConnection = new Thread(RunAsyncConnection);
            runConnection.Start();
            runConnection.Join();
        }

        private void RunAsyncConnection()
        {
            _context = new Context();
            _context.Users.Any();
            OnContextChanged?.Invoke(_context);
        }

        public void AddUsers(int n)
        {
            
            Thread[] threads = new Thread[n];
            
            for (int i = 0; i < n; i++)
            {
                string name = $"User {i + 1}";
                string surname = $"Surname {i + 1}";
                string photoName;

                threads[i] = new Thread(() =>
                {
                    using (var context = new Context())
                    {
                        photoName = DownloadUserPhoto(name, surname); 
                        AddAsyncUser(name, surname, photoName, context);
                        OnContextChanged?.Invoke(context);
                    }

                    lock (lockObject)
                    {
                        usersCount++;
                        OnUserCountChanged(usersCount);
                    }
                    
                });
                threads[i].Start();
            }
            
            

            Console.WriteLine($"Added {usersCount} users.");
        }

        private string DownloadUserPhoto(string name, string surname)
        {
            string url = "https://photoboom.ua/uploads/fotoalbom.jpg?1508829419312";
            HttpClient client = new HttpClient();
            
            var bytes = client.GetByteArrayAsync(url).Result; 

            string folder = "/Users/utereskovygmail.com/RiderProjects/FirstSP/FirstSP/users";
            string fileName = $"{name}_{surname}.jpg";
            string filePath = Path.Combine(folder, fileName);
            
            File.WriteAllBytes(filePath, bytes);
    
            return fileName;
        }


        private void AddAsyncUser(string Name, string Surname, string photo_name, Context context)
        {
            User user = new User
            {
                Name = Name,
                Surname = Surname,
                Photo = photo_name
            };

            context.Users.Add(user);
            context.SaveChanges(); 
        }
    }