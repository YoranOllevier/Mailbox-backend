using Mailbox.Domain.Accounts;

namespace Mailbox.Domain.Labels;

public class Label : Entity
{
    #region Fields

        private string _name;
        private Account? _owner;

    #endregion
    
    #region Properties

    public string Name
    {
        get => _name;
        set => _name = Guard.Against.NullOrEmpty(value);
    }

    public Account? Owner
    {
        get => _owner;
        set => _owner = value;
    }

    #endregion
    
    #region Constructors

    private Label()
    {
    }

    public Label(string name, Account? owner = null) :  this()
    {
        Name = name;
        Owner = owner;
    }

    #endregion
}