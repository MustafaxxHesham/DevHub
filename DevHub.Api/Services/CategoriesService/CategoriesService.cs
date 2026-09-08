using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Helpers;
using DevHub.Domain.Models;
using DevHub.Domain.Result;
using DevHub.DTOS.Categories;
using DevHub.EFCore.ErrorTypes;
using DevHub.Responses;
using DevHub.Utilities;
using Microsoft.AspNetCore.DataProtection;
using System.Runtime.InteropServices;
namespace DevHub.Services.CategoriesService;
public class CategoriesService(IDataStore _dataStore, IDataProtectionProvider _provider, ILogger<CategoriesService> _logger) 
    : ICategoriesService
{
    private readonly IDataProtector _protector = _provider.CreateProtector(ProtectionPurposes.CATEGORY_ID_PURPOSE);
    public async Task<Result<Category>> CreateAsync(string categoryName)
    {
        var categoryNameCleanedUp = categoryName.Trim();

        var validationResult = await validateCategoryNameAsync(categoryNameCleanedUp);

        if (validationResult.Length > 0)
            return Result<Category>.Failure(validationResult);

        var category = generateCategory(categoryNameCleanedUp);

        await _dataStore.Categories.AddAsync(category);

        await _dataStore.CompleteAsync();

        return Result<Category>.Success(category);
    }

    public async Task<Result<IEnumerable<HashedCategoryResponse>>> GetCategoriesAsync([Optional] string? query)
    {
        IPagedList<Category> categoriesList;

        if (!string.IsNullOrEmpty(query))
        {
            categoriesList = await _dataStore.Categories.GetByCriteriaAsync(c => c.Name.Contains(query), x => x.Id);
        }
        else
        {
            categoriesList = new EFCore.EFCorePaginationHelper.PagedResponse<Category>(new List<Category>(), 5, 4, 5);
//            categoriesList = await _dataStore.Categories.GetAllAsync();
        }

        if (categoriesList.ValueList.Any())
        {
            var response = new List<HashedCategoryResponse>();

            foreach (var category in categoriesList.ValueList)
            {
                response.Add(new HashedCategoryResponse(CategoryName: category.Name, Id: _protector.Protect(category.Id.ToString())));
            }

            return Result<IEnumerable<HashedCategoryResponse>>.Success(response);
        }
        return Result<IEnumerable<HashedCategoryResponse>>.Failure(DbErrors.NotFoundError.ToString());

    }

    public async Task<SimpleResult<bool>> EditCategoryAsync(EditCategoryRequest request)
    {
        int id = 0;
        try
        {
            id = int.Parse(_protector.Unprotect(request?.categoryId ?? ""));
        }
        catch (Exception ex) {
            _logger.LogError($"Unprotecting error while unprotecting category id of {request.newCategoryName} for updating.");
            
        }

        var category = await _dataStore.Categories.GetByIdAsync(id);
        
        if (category is null)
            return SimpleResult<bool>.Failure(DbErrors.NotFoundError.ToString());

        var newCategoryNameCleanedUp = request.newCategoryName.Trim();

        var validationResult = await validateCategoryNameAsync(newCategoryNameCleanedUp);

        if (!string.IsNullOrEmpty(validationResult))
            return SimpleResult<bool>.Failure(validationResult);

        category.Name = newCategoryNameCleanedUp;

        _dataStore.Categories.UpdateItem(category);

        await _dataStore.CompleteAsync();

        return SimpleResult<bool>.Success(true);
    }

    public async Task<SimpleResult<int>> GetPostsCountPerCategoryAsync(int categoryId)
    {
        var category = await _dataStore.Categories.GetByIdAsync(categoryId);
        if (category == null)
        {
            return SimpleResult<int>.Failure(DbErrors.NotFoundError.ToString());
        }

        var counts = await _dataStore.Posts.GetCountWithCriteriaAsync(p => p.CategoryId == categoryId);

        return SimpleResult<int>.Success(counts);
    }

    public async Task<SimpleResult<bool>> IsCategoryExistAsync(string categoryName)
    {
        var count = await _dataStore.Categories.GetCountWithCriteriaAsync(c => c.Name == categoryName);
        
        if (count > 0)
            return SimpleResult<bool>.Success(true);

        return SimpleResult<bool>.Failure(ResponseMessages.CATEGORY_NOT_FOUND);
    }

    private async Task<string> validateCategoryNameAsync(string categoryName)
    {
        if (string.IsNullOrEmpty(categoryName) || categoryName.Length <= 1 || categoryName.Length >= 50)
            return "category name is not valid";

        if ((await IsCategoryExistAsync(categoryName)).IsSuccess)
            return "category is already exists.";

        return string.Empty;
    }

    private Category generateCategory(string categoryName)
    {
        return new Category
        {
            Name = categoryName
        };
    }

    public async Task<Result<IEnumerable<Category>>> AddBulkAsync(IEnumerable<string> categoryList)
    {
        var categories = categoryList.Select(name => new Category { Name = name.Trim() }).ToList();
        
        await _dataStore.Categories.AddBulkAsync(categories);
        
        await _dataStore.CompleteAsync();

        return Result<IEnumerable<Category>>.Success(categories);
    }
}
