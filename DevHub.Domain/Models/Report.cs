using DevHub.Domain.Enums;

namespace DevHub.Domain.Models;

public class Report
{
    public int Id { get; set; }
    public bool IsAdminViewed { get; set; }
    public string ReportDetails { get; set; }
    public ReportAnswer ReportAnswer { get; set; }
    public ReportType Type { get; set; }
    public int ReporterId { get; set; }
    public User Reporter { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int? CommentId { get; set; }
    public Comment Comment { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; }
    public DateTime CreatedAt { get; set; }
}


public enum ReportStatus { 
  Pending  
};
