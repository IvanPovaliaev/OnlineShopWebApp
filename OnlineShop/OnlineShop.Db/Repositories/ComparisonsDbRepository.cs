using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShop.Db.Repositories
{
    public class ComparisonsDbRepository : IComparisonsRepository
    {
        private readonly DatabaseContext _databaseContext;

        public ComparisonsDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public List<ComparisonProduct> GetAll() => _databaseContext.ComparisonProducts.ToList();

        public ComparisonProduct Get(Guid id) => _databaseContext.ComparisonProducts.FirstOrDefault(c => c.Id == id)!;

        public void Create(ComparisonProduct product)
        {
            _databaseContext.ComparisonProducts.Add(product);
            _databaseContext.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var comparison = Get(id);
            _databaseContext.ComparisonProducts.Remove(comparison);
            _databaseContext.SaveChanges();
        }

        public void DeleteAll(Guid userId)
        {
            var comparisons = _databaseContext.ComparisonProducts.Where(c => c.UserId == userId);
            _databaseContext.ComparisonProducts.RemoveRange(comparisons);
            _databaseContext.SaveChanges();
        }
    }
}
