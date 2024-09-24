using System.IO;

namespace OnlineShopWebApp.Services
{
    public class FileService
    {
        /// <summary>
        /// Get file content
        /// </summary>
        /// <returns>string if file exists; otherwise null</returns>
        public string? GetContent(string path)
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

        /// <summary>
        /// Determines whether specified file exists.
        /// </summary>
        /// <returns>true if the caller has the required permissions and path contains the name of an existing file; otherwise, false. This method also returns false if path is null, an invalid path, or a zero-length string. If the caller does not have sufficient permissions to read the specified file, no exception is thrown and the method returns false regardless of the existence of path.</returns>
        public bool Exists(string path) => File.Exists(path);

        /// <summary>
        /// Saves (rewrites) data to the specified file.
        /// </summary>
        public void Save(string path, string data)
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

        /// <summary>
        /// Creates a file at the specified path.
        /// </summary>
        /// <param name="path">Path to the file</param>
        private void Create(string path)
        {
            var dirPath = Path.GetDirectoryName(path) ?? ".";
            var dirInfo = new DirectoryInfo(dirPath);
            dirInfo.Create();
            File.Create(path).Close();
        }
    }
}
