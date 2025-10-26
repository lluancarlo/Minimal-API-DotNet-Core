namespace Minimal.Domain.Entities;

public abstract class BaseEntity { }

public class MutableEntity : BaseEntity
{
    public Guid Id { get; set; }
}

public class ImmutableEntity : BaseEntity
{
    public int Id { get; set; }
}
