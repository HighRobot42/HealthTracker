namespace Domain.Entities.SeedWork;

/// <summary>
/// Base class that tracks who created/last-modified an entity and when.
/// In production this is typically provided by a shared NuGet package.
/// </summary>
public abstract class AuditableEntity
{
    public Guid     Id             { get; protected set; }
    public string?  CreatedBy      { get; set; }
    public DateTime CreatedAt      { get; set; }
    public string?  LastModifiedBy { get; set; }
    public DateTime LastModifiedAt { get; set; }
}
