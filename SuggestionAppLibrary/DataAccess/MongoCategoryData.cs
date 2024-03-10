using Microsoft.Extensions.Caching.Memory;

namespace SuggestionAppLibrary.DataAccess
{
    public class MongoCategoryData : ICategoryData
    {
        private readonly IMemoryCache _cache;
        private readonly IMongoCollection<CategoryModel> _categories;
        private const string cacheName = "CategoryData";
        public MongoCategoryData(IDbConnection db, IMemoryCache cache)
        {
            _cache = cache;
            _categories = db.CategoryCollection;
        }

        public async Task<List<CategoryModel>> GetCategoryModels()
        {
            var output = _cache.Get<List<CategoryModel>>(cacheName);
            try
            {
                if (output == null)
                {
                    var results = await _categories.FindAsync(_ => true);
                    output = results.ToList();
                    _cache.Set(cacheName, output, TimeSpan.FromDays(1));
                }
            }
            catch (Exception ex)
            {

                 Console.WriteLine(ex.Message);
            }
            return output;
        }

        public Task CreateCategory(CategoryModel category)
        {
            return _categories.InsertOneAsync(category);
        }
    }
}
