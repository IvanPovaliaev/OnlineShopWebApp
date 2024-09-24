using OnlineShopWebApp.Models;

namespace OnlineShopWebApp.Interfaces
{
    public interface IComparisonRepository
    {
        /// <summary>
        /// Create a new ComparisonProduct
        /// </summary>    
        void Create(ComparisonProduct product);
    }
}
