using DevHub.Domain.Models;
using DevHub.Domain.Result;
using DevHub.DTOS.Categories;
using System.Runtime.InteropServices;
namespace DevHub.Services.CategoriesService;
public interface ICategoriesService
{
    Task<Result<Category>> CreateAsync(string categoryName);
    Task<Result<IEnumerable<Category>>> AddBulkAsync(IEnumerable<string> categoryList);
    Task<Result<IEnumerable<HashedCategoryResponse>>> GetCategoriesAsync([Optional]string? query);
    Task<SimpleResult<int>> GetPostsCountPerCategoryAsync(int categoryId);
    Task<SimpleResult<bool>> IsCategoryExistAsync(string categoryName);
    Task<SimpleResult<bool>> EditCategoryAsync(EditCategoryRequest request);
}