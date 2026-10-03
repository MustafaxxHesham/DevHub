using Microsoft.AspNetCore.DataProtection;

namespace DevHub.Utilities;
public class ProtectionHandler(IDataProtectionProvider provider)
{
    private const string UserId = "userId";
    private const string PostId = "postId";
    private const string CategoryId = "categoryId";
    private const string FeedbackId = "feedbackId";
    private const string Password = "password";
    private const string CommentId = "commentId";
    private const string SubscriptionId = "subscriptionId";
    private const string ReportId = "reportId";
    private const string TagId = "tagId";
    private const string ReactionId = "reactionId";
    private const string CourseId = "courseId";
    private const string CourseChapterId = "courseChapterId";
    private const string ChapterVideoId = "chapterVideoId";

    private Dictionary<string, IDataProtector> Protectors = new()
    {
        [UserId]            = provider.CreateProtector(ProtectionPurposes.USER_ID_PURPOSE),
        [PostId]            = provider.CreateProtector(ProtectionPurposes.POST_ID_PURPOSE),
        [CommentId]         = provider.CreateProtector(ProtectionPurposes.COMMENT_ID_PURPOSE),
        [CategoryId]        = provider.CreateProtector(ProtectionPurposes.CATEGORY_ID_PURPOSE),
        [ReportId]          = provider.CreateProtector(ProtectionPurposes.REPORT_ID_PURPOSE),
        [TagId]             = provider.CreateProtector(ProtectionPurposes.TAG_ID_PURPOSE),
        [ReactionId]        = provider.CreateProtector(ProtectionPurposes.REACTION_ID_PURPOSE),
        [CourseId]          = provider.CreateProtector(ProtectionPurposes.COURSE_ID_PURPOSE),
        [ChapterVideoId]    = provider.CreateProtector(ProtectionPurposes.COURSE_VIDEO_ID_PURPOSE),
        [Password]          = provider.CreateProtector(ProtectionPurposes.PASSWORD_PURPOSE),
        [FeedbackId]        = provider.CreateProtector(ProtectionPurposes.FEEDBACK_ID_PURPOSE),
        [CourseChapterId]   = provider.CreateProtector(ProtectionPurposes.COURSE_CHAPTER_ID_PURPOSE)
    };

    public int GetRealUserId(string userId)
    {
        try
        {
            return int.Parse(Protectors[UserId].Unprotect(userId));
        }
        catch {
            return -1;
        }
    }
    public string GetProtectedUserId(int userId)
    {
        try
        {
            return Protectors[UserId].Protect(userId.ToString());
        }
        catch {
            return string.Empty;
        }
    }

    public int GetRealFeedbackId(string feedbackId)
    {
        try
        {
            return int.Parse(Protectors[FeedbackId].Unprotect(feedbackId));
        }
        catch {
            return -1;
        }
    }
    public string GetProtectedFeedbackId(int feedbackId)
    {
        try
        {
            return Protectors[FeedbackId].Protect(feedbackId.ToString());
        }
        catch {
            return string.Empty;
        }
    }

    public int GetRealReportId(string reportId)
    {
        try
        {
            return int.Parse(Protectors[ReportId].Unprotect(reportId));
        }
        catch {
            return -1;
        }
    }
    public string GetProtectedReportId(int reportId)
    {
        try
        {
            return Protectors[ReportId].Protect(reportId.ToString());
        }
        catch {
            return string.Empty;
        }
    }


    public int GetRealCommentId(string commentId)
    {
        try
        {
            return int.Parse(Protectors[CommentId].Unprotect(commentId));
        }
        catch {
            return -1;
        }
    }
    public string GetProtectedCommentId(int commentId)
    {
        try
        {
            return Protectors[CommentId].Protect(commentId.ToString());
        }
        catch {
            return string.Empty;
        }
    }


    public int GetRealCategoryId(string categoryId)
    {
        try
        {
            return int.Parse(Protectors[CategoryId].Unprotect(categoryId));
        }
        catch {
            return -1;
        }
    }
    public string GetProtectedCategoryId(int categoryId)
    {
        try {
            return Protectors[CategoryId].Protect(categoryId.ToString());
        }
        catch {
            return string.Empty;
        }
    }

    public int GetRealPostId(string postId)
    {
        try
        {
            return int.Parse(Protectors[PostId].Unprotect(postId));
        }
        catch {
            return -1;
        }
    }
    public string GetProtectedPostId(int postId)
    {
        try {
            return Protectors[PostId].Protect(postId.ToString());
        }
        catch {
            return string.Empty;
        }
    }


    public int GetRealReactionId(string reactionId)
    {
        try
        {
            return int.Parse(Protectors[ReactionId].Unprotect(reactionId));
        }
        catch {
            return -1;
        }
    }
    public string GetProtectedReactionId(int reactionId)
    {
        try
        {
            return Protectors[ReactionId].Protect(reactionId.ToString());
        }
        catch {
            return string.Empty;
        }
    }

    public int GetRealCourseId(string courseId)
    {
        try
        {
            return int.Parse(Protectors[CourseId].Unprotect(courseId));
        }
        catch {
            return -1;
        }
    }
    public string GetProtectedCourseId(int courseId)
    {
        try
        {
            return Protectors[CourseId].Protect(courseId.ToString());
        }
        catch {
            return string.Empty;
        }
    }

    public int GetRealChapterId(string chapterId)
    {
        try {
            return int.Parse(Protectors[ChapterVideoId].Unprotect(chapterId));
        }
        catch {
            return -1;
        }
    }
    public string GetProtectedChapterId(int chapterId)
    {
        try {
            return Protectors[ChapterVideoId].Protect(chapterId.ToString());
        }
        catch {
            return string.Empty;
        }
    }
    
    
    public int GetRealTagId(string tagId)
    {
        try
        {
            return int.Parse(Protectors[TagId].Unprotect(tagId));
        }
        catch {
            return -1;
        }
    }
    public string GetProtectedTagId(int tagId)
    {
        try {
            return Protectors[TagId].Protect(tagId.ToString());
        }
        catch {
            return string.Empty;
        }
    }
    
    
    public string GetRealPassword(string password)
    {
        try {
            return Protectors[Password].Unprotect(password);
        }
        catch {
            return string.Empty;
        }
    }
    public string GetProtectedPassword(string password)
    {
        try {
            return Protectors[Password].Protect(password);
        }
        catch {
            return string.Empty;
        }
    }
    
    public int GetRealChapterVideoId(string chapterVideoId)
    {
        try {
            return int.Parse(Protectors[ChapterVideoId].Unprotect(chapterVideoId));
        }
        catch {
            return -1;
        }
    }
    public string GetProtectedChapterVideoId(int chapterVideoId)
    {
        try {
            return Protectors[ChapterVideoId].Protect(chapterVideoId.ToString());
        }
        catch {
            return string.Empty;
        }
    }
    
    public int GetRealSubscriptionId(string subscriptionId)
    {
        try {
            return int.Parse(Protectors[SubscriptionId].Unprotect(subscriptionId));
        }
        catch {
            return -1;
        }
    }
    public string GetProtectedSubscriptionId(int subscriptionId)
    {
        try {
            return Protectors[SubscriptionId].Protect(subscriptionId.ToString());
        }
        catch {
            return string.Empty;
        }
    }
}
