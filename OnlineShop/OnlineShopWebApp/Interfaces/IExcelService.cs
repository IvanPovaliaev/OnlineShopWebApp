using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Models;
using System.Collections.Generic;
using System.IO;

namespace OnlineShopWebApp.Interfaces
{
    public interface IExcelService
    {
        /// <summary>
        /// Export all users info in collection to excel
        /// </summary>
        /// <returns>MemoryStream Excel file with users info</returns>
        /// <param name="users">Target users collection</param>
        MemoryStream ExportUsers(IEnumerable<User> users);

        /// <summary>
        /// Export all orders info in collection to excel
        /// </summary>
        /// <returns>MemoryStream Excel file with orders info</returns>
        /// <param name="orders">Target orders collection</param>
        MemoryStream ExportOrders(IEnumerable<OrderViewModel> orders);

        /// <summary>
        /// Export all roles info in collection to excel
        /// </summary>
        /// <returns>MemoryStream Excel file with roles info</returns>
        /// <param name="roles">Target roles collection</param>
        MemoryStream ExportRoles(IEnumerable<Role> roles);

        /// <summary>
        /// Export all products info in collection to excel
        /// </summary>
        /// <returns>MemoryStream Excel file with products info</returns>
        /// <param name="products">Target products collection</param>
        MemoryStream ExportProducts(IEnumerable<ProductViewModel> products);
    }
}
