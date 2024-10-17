using ClosedXML.Excel;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OnlineShopWebApp.Services
{
    public class ClosedXMLExcelService : IExcelService
    {
        public MemoryStream ExportUsers(IEnumerable<User> users)
        {
            var stream = new MemoryStream();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Users");

                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Email";
                worksheet.Cell(1, 3).Value = "Name";
                worksheet.Cell(1, 4).Value = "Phone";
                worksheet.Cell(1, 5).Value = "Role";

                var rowNumber = 2;
                foreach (var user in users)
                {
                    worksheet.Cell(rowNumber, 1).Value = user.Id.ToString();
                    worksheet.Cell(rowNumber, 2).Value = user.Email;
                    worksheet.Cell(rowNumber, 3).Value = user.Name;
                    worksheet.Cell(rowNumber, 4).Value = user.Phone;
                    worksheet.Cell(rowNumber, 5).Value = user.Role.Name;
                    rowNumber++;
                }

                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(stream);
            }

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public MemoryStream ExportOrders(IEnumerable<Order> orders)
        {
            var stream = new MemoryStream();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Orders");

                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Article";
                worksheet.Cell(1, 3).Value = "UserID";
                worksheet.Cell(1, 4).Value = "CreationDate";
                worksheet.Cell(1, 5).Value = "Status";
                worksheet.Cell(1, 6).Value = "City";
                worksheet.Cell(1, 7).Value = "Entrance";
                worksheet.Cell(1, 8).Value = "Floor";
                worksheet.Cell(1, 9).Value = "Apartment";
                worksheet.Cell(1, 10).Value = "PostCode";
                worksheet.Cell(1, 11).Value = "FullName";
                worksheet.Cell(1, 12).Value = "Email";
                worksheet.Cell(1, 13).Value = "Phone";
                worksheet.Cell(1, 14).Value = "ReservePhone";
                worksheet.Cell(1, 15).Value = "AdditionalInfo";
                worksheet.Cell(1, 16).Value = "Positions";
                worksheet.Cell(1, 17).Value = "TotalCost, rub";

                var rowNumber = 2;
                foreach (var order in orders)
                {
                    worksheet.Cell(rowNumber, 1).Value = order.Id.ToString();
                    worksheet.Cell(rowNumber, 2).Value = order.Article;
                    worksheet.Cell(rowNumber, 3).Value = order.UserId.ToString();
                    worksheet.Cell(rowNumber, 4).Value = order.CreationDate;
                    worksheet.Cell(rowNumber, 5).Value = order.Status.ToString();
                    worksheet.Cell(rowNumber, 6).Value = order.City;
                    worksheet.Cell(rowNumber, 7).Value = order.Entrance;
                    worksheet.Cell(rowNumber, 8).Value = order.Floor;
                    worksheet.Cell(rowNumber, 9).Value = order.Apartment;
                    worksheet.Cell(rowNumber, 10).Value = order.PostCode;
                    worksheet.Cell(rowNumber, 11).Value = order.FullName;
                    worksheet.Cell(rowNumber, 12).Value = order.Email;
                    worksheet.Cell(rowNumber, 13).Value = order.Phone;
                    worksheet.Cell(rowNumber, 14).Value = order.ReservePhone;
                    worksheet.Cell(rowNumber, 15).Value = order.AdditionalInfo;

                    var formattedPositions = order.Positions.Select(p => $"{p.Product.Name}........{p.Quantity}........{p.Cost}");
                    worksheet.Cell(rowNumber, 16).Value = string.Join("\n", formattedPositions);

                    worksheet.Cell(rowNumber, 17).Value = order.TotalCost;

                    rowNumber++;
                }

                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(stream);
            }

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public MemoryStream ExportProducts(IEnumerable<Product> products)
        {
            var stream = new MemoryStream();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Orders");

                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Name";
                worksheet.Cell(1, 3).Value = "Article";
                worksheet.Cell(1, 4).Value = "Category";
                worksheet.Cell(1, 5).Value = "Cost";
                worksheet.Cell(1, 6).Value = "Description";
                worksheet.Cell(1, 7).Value = "ImageUrl";
                worksheet.Cell(1, 8).Value = "Specifications";

                var rowNumber = 2;
                foreach (var product in products)
                {
                    worksheet.Cell(rowNumber, 1).Value = product.Id.ToString();
                    worksheet.Cell(rowNumber, 2).Value = product.Name;
                    worksheet.Cell(rowNumber, 3).Value = product.Article;
                    worksheet.Cell(rowNumber, 4).Value = product.Category.ToString();
                    worksheet.Cell(rowNumber, 5).Value = product.Cost;
                    worksheet.Cell(rowNumber, 6).Value = product.Description;
                    worksheet.Cell(rowNumber, 7).Value = product.ImageUrl;

                    var formattedSpecifications = product.Specifications.Select(s => $"{s.Key} ........ {s.Value}");
                    worksheet.Cell(rowNumber, 8).Value = string.Join("\n", formattedSpecifications);

                    rowNumber++;
                }

                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(stream);
            }

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public MemoryStream ExportRoles(IEnumerable<Role> roles)
        {
            var stream = new MemoryStream();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Orders");

                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Name";
                worksheet.Cell(1, 3).Value = "CanBeDeleted";

                var rowNumber = 2;
                foreach (var role in roles)
                {
                    worksheet.Cell(rowNumber, 1).Value = role.Id.ToString();
                    worksheet.Cell(rowNumber, 2).Value = role.Name;
                    worksheet.Cell(rowNumber, 3).Value = role.CanBeDeleted;
                    rowNumber++;
                }

                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(stream);
            }

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
