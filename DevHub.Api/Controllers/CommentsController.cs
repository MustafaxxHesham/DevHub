using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Models;
using DevHub.DTOS.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/comments/{postId?}")]
public class CommentsController(IDataStore _dataStore, ILogger<CommentsController> _logger) : ControllerBase
{
    [HttpGet("comments")]
    public async Task<ActionResult<IEnumerable<Comment>>> GetAllMainComments([FromRoute]int postId)
    {
        var result = await _dataStore.Comments.GetByCriteriaAsync(p => p.PostId == postId, x => x.Id);
        
        if (result.ValueList.Any())
            return Ok(result);

        return BadRequest();
    }

    [HttpGet("replies")]
    public async Task<ActionResult<IEnumerable<Comment>>> GetAllRepliesToComment([FromQuery]CommentRepliesRequest request)
    {

        var result = await _dataStore.Comments
            .GetByCriteriaAsync(p => p.PostId == request.PostId && p.ParentCommentId == request.CommentId, x => x.Id);

        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Comment(SubmitCommentRequest request) 
    {
        if (!await _dataStore.Posts.IsExistAsync(request.PostId))
            return BadRequest();

        if (request.ParentCommentId.HasValue && !await _dataStore.Comments.IsExistAsync(request.ParentCommentId.Value))
            return BadRequest();

        var comment = new Comment
        {
            Content = request.Content,
            PostId = request.PostId,
            ParentCommentId = request.ParentCommentId,
            UserId = request.UserId,
            WrittenAt = DateTime.UtcNow.ToLocalTime(),
        };

        await _dataStore.Comments.AddAsync(comment);
        await _dataStore.CompleteAsync();


//            await _notificationService.NotifyAuthor(authorId);

        return Ok();
    }

    [Authorize]
    [HttpDelete("{commentId}")]
    public async Task<ActionResult> DeleteComment(int commentId)
    {
        var result = await _dataStore.Comments.GetByIdAsync(commentId);

        if (result is null)
            return NotFound();

        _dataStore.Comments.RemoveItem(result);

        await _dataStore.CompleteAsync();

        return NoContent();

        _logger.LogError("Exception happened about deleting comment with Id = " +  commentId);

        throw new Exception("Error Happended about deleting post.");
    }
}
