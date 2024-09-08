using System.IO;

namespace OnlineShopWebApp.Models
{
    public static class FileProvider
    {
        private static void CreateFile(string filePath)
        {
            var dirPath = Path.GetDirectoryName(filePath) ?? ".";
            var dirInfo = new DirectoryInfo(dirPath);
            dirInfo.Create();
            File.Create(filePath).Close();
        }
        public static void Replace(string filePath, string content)
        {
            if (!Exists(filePath)) CreateFile(filePath);
            using (var sw = new StreamWriter(filePath, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(content);
            }
        }
        public static string GetContent(string filePath)
        {
            if (!Exists(filePath)) CreateFile(filePath);
            return File.ReadAllText(filePath, System.Text.Encoding.Default);
        }
        public static void Clear(string filePath)
        {
            if (Exists(filePath))
                File.WriteAllText(filePath, string.Empty);
        }
        public static bool Exists(string filePath) => File.Exists(filePath);
        public static void Save(string filePath, string fileData) => Replace(filePath, fileData);
    }
}
