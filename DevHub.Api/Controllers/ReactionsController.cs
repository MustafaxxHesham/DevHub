using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Enums;
using DevHub.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevHub.Controllers;

[ApiController]
[Route("api/v1/Reactions")]
public class ReactionsController(IDataStore _dataStore) : ControllerBase
{
    [HttpGet("post-reactions/{postId}")]
    public async Task<ActionResult<int>> GetPostReactions(int postId)
    {
        //Result..
        var reactions = await _dataStore.Reactions.GetCountWithCriteriaAsync(r => r.PostId == postId);
        return Ok(reactions);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> AddReaction(int userId, int postId, int reactionType)
    {
        var userExist = await _dataStore.Users.IsExistAsync(userId);
        
        if (!userExist)
            return NotFound();

        var postExist = await _dataStore.Posts.IsExistAsync(postId);
        
        if (!postExist)
            return NotFound();

        if (reactionType < 0 || reactionType > 2)
            return BadRequest("Invalid reaction type"); 

        var reaction = new Reaction
        {
            PostId = postId,
            ReactionType = ReactionType.Love,
            UserId = userId
        };
        await _dataStore.Reactions.AddAsync(reaction);

        if (await _dataStore.CompleteAsync() > 0)
            return Ok();

        throw new Exception("Error Due to db server");
    }
}
