using OnlineShopWebApp.Models;
using System.Collections.Generic;
using System.IO;

namespace OnlineShopWebApp.Interfaces
{
    public interface IExcelService
    {
        MemoryStream ExportUsers(IEnumerable<User> users);
        MemoryStream ExportOrders(IEnumerable<Order> orders);
        MemoryStream ExportRoles(IEnumerable<Role> roles);
        MemoryStream ExportProducts(IEnumerable<Product> products);
    }
}
