﻿using PricingCalculator.Models;

namespace PricingCalculator.Data.Repositories.CategoryRepositories
{
    public interface ICategoryRepository : IGenericRepository<int, Category>
    {
        public Task<List<Category>> GetAllCategoriesWithProducts();
    }
}
