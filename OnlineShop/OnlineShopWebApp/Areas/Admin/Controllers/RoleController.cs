﻿using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;

namespace OnlineShopWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly RolesService _rolesService;
        private readonly IExcelService _excelService;

        public RoleController(RolesService rolesService, IExcelService excelService)
        {
            _rolesService = rolesService;
            _excelService = excelService;
        }

        /// <summary>
        /// Open Admin Roles Page
        /// </summary>
        /// <returns>Admin Roles View</returns>
        public IActionResult Index()
        {
            var roles = _rolesService.GetAll();
            return View(roles);
        }

        /// <summary>
        /// Add new role
        /// </summary>
        /// <returns>Admin Roles View</returns>
        /// <param name="role">Target role</param>  
        [HttpPost]
        public IActionResult Add(Role role)
        {
            var isModelValid = _rolesService.IsNewValid(ModelState, role);

            if (!isModelValid)
            {
                return PartialView("_AddForm", role);
            }

            _rolesService.Add(role);

            var redirectUrl = Url.Action("Index");

            return Json(new { redirectUrl });
        }

        /// <summary>
        /// Delete role by Id
        /// </summary>
        /// <returns>Admins roles View</returns>
        /// <param name="id">Target role Id</param>  
        public IActionResult Delete(Guid id)
        {
            _rolesService.Delete(id);
            return RedirectToAction("Index");
        }

        public IActionResult ExportToExcel()
        {
            var roles = _rolesService.GetAll();
            var excelStream = _excelService.ExportRoles(roles);
            return File(excelStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Roles.xlsx");
        }
    }
}
