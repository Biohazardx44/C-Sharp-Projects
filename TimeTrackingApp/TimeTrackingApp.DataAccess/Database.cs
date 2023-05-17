using Newtonsoft.Json;
using TimeTrackingApp.DataAccess.Interfaces;
using TimeTrackingApp.Domain.Entities;

namespace TimeTrackingApp.DataAccess
{
    public abstract class Database<T> : IDatabase<T> where T : BaseEntity
    {
        private readonly string _filePath;

        protected List<T> Items = new List<T>();

        private const string DATABASE_FOLDER_NAME = "../../../../Database";
        private const string DATABASE_FILE_EXTENSION = ".json";

        public Database()
        {
            if (!Directory.Exists(DATABASE_FOLDER_NAME))
            {
                Directory.CreateDirectory(DATABASE_FOLDER_NAME);
            }

            _filePath = Path.Combine(DATABASE_FOLDER_NAME, typeof(T).Name + DATABASE_FILE_EXTENSION);

            if (!File.Exists(_filePath))
            {
                File.Create(_filePath).Close();
            }

            LoadItemsAsync().Wait();
        }

        /// <summary>
        /// Inserts the specified data into the database.
        /// </summary>
        /// <param name="data">The data to be inserted.</param>
        public async Task InsertAsync(T data)
        {
            data.Id = Items.Count > 0 ? Items.Max(i => i.Id) + 1 : 1;
            Items.Add(data);
            await WriteToFileAsync();
        }

        /// <summary>
        /// Updates the specified data in the database.
        /// </summary>
        /// <param name="data">The data to be updated.</param>
        public async Task UpdateAsync(T data)
        {
            await WriteToFileAsync();
        }

        /// <summary>
        /// Deletes the specified data from the database.
        /// </summary>
        /// <param name="data">The data to be deleted.</param>
        public async Task DeleteAsync(T data)
        {
            Items.RemoveAll(i => i.Id == data.Id);
            await WriteToFileAsync();
        }

        /// <summary>
        /// Gets the data with the specified ID from the database.
        /// </summary>
        /// <param name="id">The ID of the data to get.</param>
        /// <returns>The data with the specified ID, or null if not found.</returns>
        public async Task<T> GetByIdAsync(int id)
        {
            await Task.Yield();
            return Items.FirstOrDefault(i => i.Id == id);
        }

        /// <summary>
        /// Loads the items from the file asynchronously.
        /// </summary>
        private async Task LoadItemsAsync()
        {
            using (StreamReader sr = new StreamReader(_filePath))
            {
                string jsonData = await sr.ReadToEndAsync();

                if (!string.IsNullOrEmpty(jsonData))
                {
                    Items = JsonConvert.DeserializeObject<List<T>>(jsonData);
                }
            }
        }

        /// <summary>
        /// Writes the items to the file asynchronously.
        /// </summary>
        private async Task WriteToFileAsync()
        {
            using (StreamWriter sw = new StreamWriter(_filePath))
            {
                string jsonData = JsonConvert.SerializeObject(Items, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                await sw.WriteAsync(jsonData);
            }
        }
    }
}