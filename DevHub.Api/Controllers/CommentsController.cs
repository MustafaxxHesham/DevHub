using DevHub.AuthorizationPolicies;
using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Models;
using DevHub.DTOS.Comments;
using DevHub.Responses;
using DevHub.Services.CommentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/posts/{postId}/comments")]
public class CommentsController(IDataStore _dataStore,
    ICommentService _commentService,
    ILogger<CommentsController> _logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Comment>>> GetAllMainComments([FromRoute]int postId)
    {
        var result = await _dataStore.Comments.GetByCriteriaAsync(p => p.PostId == postId, x => x.Id);
        
        if (result.ValueList.Any())
            return Ok(result);

        return BadRequest();
    }

    [HttpGet("{commentId}/replies")]
    public async Task<ActionResult<IEnumerable<Comment>>> GetAllRepliesToComment([FromQuery]CommentRepliesRequest request)
    {

        var result = await _dataStore.Comments
            .GetByCriteriaAsync(p => p.PostId == request.PostId && p.ParentCommentId == request.CommentId, x => x.Id);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.AddCommentPolicy)]
    public async Task<ActionResult> Comment(SubmitCommentRequest request) 
    {
        //if (!await _dataStore.Posts.IsExistAsync(request.PostId))
        //    return BadRequest("Post Not Found!");

        //if ((request.ParentCommentId.HasValue && request.ParentCommentId > 0) && !await _dataStore.Comments.IsExistAsync(request.ParentCommentId.Value))
        //    return BadRequest("No Parent Comment to nested!");

        //var result = await _commentService.AddCommentAsync(request);

        //if (!result.IsSuccess)
        //    return BadRequest(result.Error);

        // BackgroundJob.Enqueue(() => _notificationService.NotifyAuthor(authorId));

        return Ok();
    }

    [Authorize]
    [HttpDelete("{commentId}")]
    public async Task<ActionResult> DeleteComment(int commentId)
    {
        var result = await _commentService.DeleteCommentAsync(commentId);
        if (!result.IsSuccess)
        {
            if (result.Error.Equals(ResponseMessages.COMMENT_NOT_FOUND))
                return NotFound(result.Error);

            BadRequest(result.Error);
        }
        return NoContent();
    }

    [HttpPatch]
    public async Task<ActionResult> Edit(int commendId, string newContent)
    {
        if (commendId > 0)
            return BadRequest("");

        if (string.IsNullOrEmpty(newContent))
            return BadRequest("");

        var result = await _commentService.EditCommentAsync(newContent, commendId);

        if (!result.IsSuccess)
        {
            if (result.Error.Equals(ResponseMessages.COMMENT_NOT_FOUND))
            {
                return NotFound(result.Error);
            }
            return BadRequest(result.Error);
        }
        return NoContent();
    }

    [HttpGet("counts")]
    public async Task<ActionResult> GetCommentsCount(string postId)
    {
        var result = await _commentService.GetCommentsCount(postId);
        if (!result.IsSuccess)
        {
            if (result.Error.Equals(ResponseMessages.POST_NOT_FOUND))
            {
                return NotFound(result.Error);
            }
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }

    [Authorize]
    [HttpGet("user-comments/{userId}")]
    public IActionResult GetMyComments()
    {
        throw new NotImplementedException();
    }
}
