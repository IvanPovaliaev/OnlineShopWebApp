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
        public static void Replace(string path, string content)
        {
            if (!Exists(path))
            {
                Create(path);
            }

            using (var streamWriter = new StreamWriter(path, false, System.Text.Encoding.Default))
            {
                streamWriter.WriteLine(content);
            }
        }
        public static string GetContent(string path)
        {
            if (!Exists(path))
            {
                Create(path);
            }

            return File.ReadAllText(path, System.Text.Encoding.Default);
        }
        public static void Clear(string path)
        {
            if (Exists(path))
            {
                File.WriteAllText(path, string.Empty);
            }
        }
        public static bool Exists(string path) => File.Exists(path);
        public static void Save(string path, string data) => Replace(path, data);
    }
}
