using DevHub.Domain.Enums;

namespace DevHub.Domain.Models;
public class Idempotency
{
    public Guid Id { get; set; }
    public string Key { get; set; }
    public IdempotencyState State { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}