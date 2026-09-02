using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/tags")]
public class TagsController(IDataStore _dataStore) : ControllerBase
{
    /// <summary>
    ///     Returns List of Tags
    /// </summary>
    /// <param name="tagId"></param>
    /// <returns></returns>
    [HttpGet("{tagId}")]
    public async Task<ActionResult<Tag>> GetTag(int tagId)
    {
        var tag = await _dataStore.Tags.GetByIdAsync(tagId);

        if (tag is null)
            return NotFound();
        
        return Ok(tag);
    }

    [HttpGet("count/{tagId}")]
    public async Task<ActionResult<int>> GetTagCount(int tagId)
    {
        return Ok(await _dataStore.Tags.GetCountWithCriteriaAsync(t => t.Id == tagId));
    }


    [HttpGet("tag-post")]
    public async Task<ActionResult> GetTagsCount()
    {
        return Ok(0);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tag>>> GetTags([FromHeader(Name = "X-Page-Size")] int pageSize,
                                                              [FromHeader(Name = "X-Page-Number")] int pageNumber)
    {
        var result = await _dataStore.Tags.GetPaginatedAsync(pageSize, pageNumber, x => x.Id);

        return Ok(result);
    }

    [HttpGet("{query-tag}")]
    public async Task<ActionResult<IEnumerable<Tag>>> SearchTags([FromQuery]string queryTag)
    {
        queryTag = queryTag.Trim();
        
        if (queryTag.Length == 0)
            return BadRequest();

        var result = await _dataStore.Tags.GetByCriteriaAsync(t => t.Name == queryTag, t => t.Id);

        if (result.ValueList.Any())
            return Ok(result);

        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult> AddTag([FromBody]string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName) || tagName.Length == 1)
            return BadRequest();

        if (await _dataStore.Tags.GetCountWithCriteriaAsync(t => t.Name == tagName) > 0)
            return BadRequest(new { error = $"tag name {tagName} already exists." });

        Tag tag = GenerateTag(tagName);

        await _dataStore.Tags.AddAsync(tag);

        await _dataStore.CompleteAsync();

        return CreatedAtAction(nameof(GetTag), new { tagId = tag.Id }, tag);
    }

    [HttpPut]
    public async Task<ActionResult> EditTag(string tagName, int tagId)
    {
        tagName = tagName.Trim();

        if (!(tagName.Length > 0))
            return BadRequest();

        var oldTag = await _dataStore.Tags.GetByIdAsync(tagId);

        if (oldTag is null)
            return NotFound();

        oldTag.Name = tagName;

        _dataStore.Tags.UpdateItem(oldTag);

        await _dataStore.CompleteAsync();

        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> RemoveTag(int tagId) 
    {
        var oldTag = await _dataStore.Tags.GetByIdAsync(tagId);

        if (oldTag is null)
            return BadRequest();

        _dataStore.Tags.RemoveItem(oldTag);

        await _dataStore.CompleteAsync();
            return NoContent();

        return StatusCode(500);
    }

    [HttpPost("bulk")]
    public async Task<ActionResult> AddBulk()
    {
        throw new NotImplementedException();
    }

    [NonAction]
    public Tag GenerateTag(string tag)
    {
        return new Tag { Name = tag };
    }
}
