namespace DevHub.Domain.Models;
public class ReportAnswer
{
    public int ReportId { get; set; }
    public string ReportDetails { get; set; }
    public Report Report { get; set; }
    public int ReporterId { get; set; }
    public bool IsViewed { get; set; }
    public User Reporter { get; set; }
    public DateTime CreatedAt { get; set; }
}