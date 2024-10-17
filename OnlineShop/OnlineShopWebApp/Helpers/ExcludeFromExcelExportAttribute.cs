using System;

namespace OnlineShopWebApp.Helpers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcludeFromExcelExportAttribute : Attribute
    {
    }
}
