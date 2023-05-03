using Newtonsoft.Json;
using TimeTrackingApp.DataAccess.Interfaces;
using TimeTrackingApp.Domain.Entities;

namespace TimeTrackingApp.DataAccess
{
    public abstract class Database<T> : IDatabase<T> where T : BaseEntity
    {
        protected List<T> Items;

        private const string DATABASE_FOLDER_NAME = "Database";
        private const string DATABASE_FILE_EXTENSION = ".json";

        private readonly string _filePath = $"{DATABASE_FOLDER_NAME}/{typeof(T).Name}{DATABASE_FILE_EXTENSION}";

        public Database()
        {
            Items = new List<T>();

            if (!Directory.Exists(DATABASE_FOLDER_NAME))
            {
                Directory.CreateDirectory(DATABASE_FOLDER_NAME);
            }

            if (!File.Exists(_filePath))
            {
                var file = File.Create(_filePath);
                file.Close();
            }

            Task.Run(async () => Items = await ReadFromFileAsync()).Wait();
        }

        public async Task DeleteAsync(T Data)
        {
            Items = Items.Where(item => item.Id == Data.Id).ToList();

            await WriteToFileAsync();
        }

        public List<T> GetAll()
        {
            return Items;
        }

        public T GetById(int Id)
        {
            return Items.FirstOrDefault(item => item.Id == Id);
        }

        public async Task InsertAsync(T Data)
        {
            Items.Add(AutoIncrementId(Data));
            await WriteToFileAsync();
        }

        public async Task UpdateAsync(T Data)
        {
            await WriteToFileAsync();
        }

        private async Task<List<T>> ReadFromFileAsync()
        {
            using (StreamReader sr = new StreamReader(_filePath))
            {
                string data = await sr.ReadToEndAsync();

                return JsonConvert.DeserializeObject<List<T>>(data) ?? new List<T>();
            }
        }

        private async Task WriteToFileAsync()
        {
            using (StreamWriter sw = new StreamWriter(_filePath))
            {
                string data = JsonConvert.SerializeObject(Items, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                });

                await sw.WriteLineAsync(data);
            }
        }

        private T AutoIncrementId(T item)
        {
            int currentId = 0;

            if (Items.Count > 0)
            {
                currentId = Items.OrderByDescending(item => item.Id).First().Id;
            }

            item.Id = ++currentId;

            return item;
        }
    }
}