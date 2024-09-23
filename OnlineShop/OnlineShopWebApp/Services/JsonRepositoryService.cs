using Newtonsoft.Json;
using System.Collections.Generic;

namespace OnlineShopWebApp.Services
{
    public class JsonRepositoryService
    {
        private readonly FileService _fileService;

        public JsonRepositoryService(FileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Saves T repository changes to a file at the specified path
        /// </summary> 
        /// <param name="path">Path to the repository file</param>
        /// <param name="data">Saving data (List)</param>
        public void SaveChanges<T>(string path, List<T> data)
        {
            var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            _fileService.Save(path, jsonData);
        }

        /// <summary>
        /// Upload List of T from a json repository file at the specified path
        /// </summary>  
        /// <param name="path">Path to the repository file</param>
        /// <returns>List of T. Returns an empty List if the file does not exist </returns>
        public List<T> Upload<T>(string path)
        {
            var dataJson = _fileService.GetContent(path);

            if (dataJson is null)
            {
                var emptyData = new List<T>();
                return emptyData;
            }

            var data = JsonConvert.DeserializeObject<List<T>>(dataJson);
            return data;
        }
    }
}
