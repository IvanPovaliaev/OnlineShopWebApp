using System.IO;

namespace OnlineShopWebApp.Services
{
    public static class FileService
    {
        private static void Create(string path)
        {
            var dirPath = Path.GetDirectoryName(path) ?? ".";
            var dirInfo = new DirectoryInfo(dirPath);
            dirInfo.Create();
            File.Create(path).Close();
        }

        /// <summary>
        /// Get file content
        /// </summary>
        /// <returns>string if file exists; otherwise null</returns>
        public static string? GetContent(string path)
        {
            try
            {
                return File.ReadAllText(path, System.Text.Encoding.Default);
            }
            catch
            {
                return null;
            }
        }

        public static bool Exists(string path) => File.Exists(path);
        public static void Save(string path, string data)
        {
            if (!Exists(path))
            {
                Create(path);
            }

            using (var streamWriter = new StreamWriter(path, false, System.Text.Encoding.Default))
            {
                streamWriter.WriteLine(data);
            }
        }
    }
}
