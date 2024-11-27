namespace OnlineShop.Application.Interfaces
{
    public interface IHostingEnvironmentService
    {
        /// <summary>
        /// Return a web root path for current project
        /// </summary>        
        /// <returns>Web root url path</returns>
        string GetWebRootPath();
    }
}
