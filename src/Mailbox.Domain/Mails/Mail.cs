using Mailbox.Domain.Accounts;
using Mailbox.Domain.Labels;

namespace Mailbox.Domain.Mails;

public class Mail : Entity
{
    #region Fields
    private Account _from;
    private ISet<Account> _to;
    private string _subject;
    private string _body;
    private DateTime _sentOn;
    private bool _isRead;
    private bool _isSent;
    private bool _isStarred;
    private ISet<Label> _labels;
    #endregion

    #region Properties
    public Account From
    {
        get => _from;
        set => _from = Guard.Against.Null(value);
    }

    public ISet<Account> To
    {
        get => _to;
        private set => _to = Guard.Against.Null(value);
    }

    public string Subject
    {
        get => _subject;
        set => _subject = Guard.Against.Null(value);
    }

    public string Body
    {
        get => _body;
        set => _body = Guard.Against.Null(value);
    }

    public DateTime SentOn
    {
        get => _sentOn;
        set => _sentOn = Guard.Against.Null(value);
    }

    public bool IsRead
    {
        get => _isRead;
        set => _isRead = Guard.Against.Null(value) && _isSent;
    }

    public bool IsSent
    {
        get => _isSent;
        set => _isSent = Guard.Against.Null(value) && IsValid();
    }

    public bool IsStarred
    {
        get => _isStarred;
        set => _isStarred = Guard.Against.Null(value);
    }

    public ISet<Label> Labels
    {
        get => _labels;
        private set => _labels = Guard.Against.Null(value);
    }
    #endregion
    
    #region Constructors

    private Mail() : base()
    {}
    
    public Mail(Account from, ISet<Account>? to, string? subject, string? body) : this()
    {
        if((to == null|| to.Count == 0) && subject == null && body == null)
            return;
        
        From = from;
        _subject = subject??"";
        _body = body??"";
        To = to??new HashSet<Account>();
        Labels = new HashSet<Label>();
    }

    public Mail(Account from, ISet<Account> to, string subject, string body, DateTime sentOn, bool isRead, bool isSent, bool isStarred, ISet<Label> labels) : this(from, to, subject, body)
    {
        SentOn = sentOn;
        IsRead = isRead;
        IsSent = isSent;
        IsStarred = isStarred;
        Labels = labels;
    }

    #endregion

    #region Methods

    private bool IsValid()
    {
        bool isValid = true;
        isValid &= _to.Count != 0;
        return isValid;
    }

    #endregion
}