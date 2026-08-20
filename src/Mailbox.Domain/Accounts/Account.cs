using System.Net.Mail;
using Mailbox.Domain.Common;

namespace Mailbox.Domain.Accounts;

public class Account : Entity
{
    #region Fields
    
    private string _name;
    private MailAddress _email;
    private string _userId;
    #endregion

    #region Properties
    public string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrEmpty(value);
    }

    public MailAddress Email
    {
        get => _email;
        set => _email = Guard.Against.Null(value);
    }
    
    public string UserId
    {
        get => _userId;
        private set => _userId = Guard.Against.NullOrEmpty(value);
    }
    #endregion
    
    #region Constructors
    public Account(string name, MailAddress email, string userId)
    {
        Name = name;
        Email = email;
        UserId = userId;
    }

    public Account(string name, string email, string userId) : this(name, new MailAddress(email), userId)
    {
        
    }
    #endregion
    
    #region Methods

    protected bool Equals(Account other)
    {
        return base.Equals(other) && _userId == other._userId;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Account)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), _userId);
    }

    #endregion
}