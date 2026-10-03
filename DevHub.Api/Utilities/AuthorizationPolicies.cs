using DevHub.AuthorizationPolicies;
using DevHub.AuthorizationRequirements.AddPostPolicy;
using DevHub.AuthorizationRequirements.CommentPolicy;
using Microsoft.AspNetCore.Authorization;

namespace DevHub.Utilities;
public static class AuthorizationPolicies
{
    extension(AuthorizationOptions opts)
    {
        public void AddAuthorizationPolicies()
        {
            opts.AddPolicy(AuthPolicies.AddCommentPolicy, config => config.AddRequirements(new CommentPolicyRequirement()));
            opts.AddPolicy(AuthPolicies.AddPostPolicy, config => config.AddRequirements(new AddPostRequirement()));
        }
    }

}