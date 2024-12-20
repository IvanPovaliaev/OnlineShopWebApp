﻿using OnlineShop.Application.Models;
using OnlineShop.Application.Models.Admin;
using System.Collections.Generic;
using System.IO;

namespace OnlineShop.Application.Interfaces
{
    public interface IExcelService
    {
        /// <summary>
        /// Export all users info in collection to excel
        /// </summary>
        /// <returns>MemoryStream Excel file with users info</returns>
        /// <param name="users">Target users collection</param>
        MemoryStream ExportUsers(IEnumerable<UserViewModel> users);

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
        MemoryStream ExportRoles(IEnumerable<RoleViewModel> roles);

        /// <summary>
        /// Export all products info in collection to excel
        /// </summary>
        /// <returns>MemoryStream Excel file with products info</returns>
        /// <param name="products">Target products collection</param>
        MemoryStream ExportProducts(IEnumerable<ProductViewModel> products);
    }
}
