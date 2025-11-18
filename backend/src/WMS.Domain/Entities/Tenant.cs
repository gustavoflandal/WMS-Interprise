namespace WMS.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Domain { get; private set; }
    public bool IsActive { get; private set; }
    public string? ContactEmail { get; private set; }
    public string? ContactPhone { get; private set; }
    public string? Address { get; private set; }
    public DateTime? SubscriptionStartDate { get; private set; }
    public DateTime? SubscriptionEndDate { get; private set; }
    public int MaxUsers { get; private set; }

    // Navigation properties
    public virtual ICollection<User> Users { get; private set; } = new List<User>();
    public virtual ICollection<Role> Roles { get; private set; } = new List<Role>();

    private Tenant() { } // For EF Core

    public Tenant(
        string name,
        string slug,
        string? domain = null,
        string? contactEmail = null,
        int maxUsers = 10)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Slug = slug ?? throw new ArgumentNullException(nameof(slug));
        Domain = domain;
        ContactEmail = contactEmail;
        MaxUsers = maxUsers;
        IsActive = true;
        SubscriptionStartDate = DateTime.UtcNow;
    }

    public void Update(string name, string? domain, string? contactEmail, string? contactPhone, string? address)
    {
        Name = name;
        Domain = domain;
        ContactEmail = contactEmail;
        ContactPhone = contactPhone;
        Address = address;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void UpdateSubscription(DateTime startDate, DateTime endDate, int maxUsers)
    {
        SubscriptionStartDate = startDate;
        SubscriptionEndDate = endDate;
        MaxUsers = maxUsers;
    }

    public bool IsSubscriptionActive()
    {
        if (!SubscriptionEndDate.HasValue) return true;
        return SubscriptionEndDate.Value > DateTime.UtcNow;
    }

    public bool CanAddUser()
    {
        return Users.Count(u => !u.IsDeleted) < MaxUsers;
    }
}
