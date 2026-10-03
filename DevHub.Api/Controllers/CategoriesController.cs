using DevHub.Domain.Models;
using DevHub.DTOS.Categories;
using DevHub.EFCore.ErrorTypes;
using DevHub.Services.CategoriesService;
using Microsoft.AspNetCore.Mvc;
namespace DevHub.Controllers;
[ApiController]
[Route("api/v1/categories")]
public class CategoriesController(ICategoriesService _categoriesService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HashedCategoryResponse>>> GetCategoriesAsync()
    {
        var categories = await _categoriesService.GetCategoriesAsync();
        return Ok(categories.Value);
    }

    [HttpGet("{query:alpha}")]
    public async Task<ActionResult<IEnumerable<HashedCategoryResponse>>> GetCategoriesListAsync(string query)
    {
        var result = await _categoriesService.GetCategoriesAsync(query);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return NotFound();
    }

    [HttpGet("{categoryId:int}")]
    public async Task<ActionResult<Category>> GetCategoryById(int categoryId)
    {
        //By View
        // To be Edited
        return new Category { Id = categoryId };
    }

    [HttpPost]
    public async Task<ActionResult<Category>> PostAsync([FromBody] string categoryName)
    {
        var result = await _categoriesService.CreateAsync(categoryName);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetCategoryById), new { categoryId = result.Value.Id }, result.Value);
        }

        return BadRequest(result.Error);
    }

    [HttpPut]
    public async Task<ActionResult<Category>> PutAsync([FromBody]EditCategoryRequest request)
    {
        var result = await _categoriesService.EditCategoryAsync(request);

        if (result.IsSuccess)
        {
            return NoContent();
        }

        if (result.Error == DbErrors.NotFoundError.ToString())
        {
            return NotFound();
        }

        return BadRequest(result.Error);

    }

    [HttpPost("add-bulk")]
    public async Task<ActionResult> AddBulk(IEnumerable<string> categories)
    {
        var result = await _categoriesService.AddBulkAsync(categories);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpDelete]
    public async Task<ActionResult<int>> DeleteCategory(int categoryId)
    {
        var result = await _categoriesService.GetPostsCountPerCategoryAsync(categoryId);

        if(result.IsSuccess)
            return Ok(result.Value);

        if(result.Error == DbErrors.NotFoundError.ToString())
            return NotFound();

        return BadRequest(result.Error);
    }
}