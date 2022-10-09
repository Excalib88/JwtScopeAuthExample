namespace ExampleApp.DataAccess.Entities;

public class ScopeEntity : BaseEntity
{
    public string Name { get; set; } = null!;
    public ICollection<AppUser>? Users { get; set; }

    public ScopeEntity()
    {
    }

    public ScopeEntity(string name)
    {
        Name = name;
    }
}