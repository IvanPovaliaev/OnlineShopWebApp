using System;

namespace OnlineShopWebApp.Helpers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelExportGetSubfieldAttribute : Attribute
    {
        public string SubfieldName { get; }

        public ExcelExportGetSubfieldAttribute(string subfieldName)
        {
            SubfieldName = subfieldName;
        }
    }
}
