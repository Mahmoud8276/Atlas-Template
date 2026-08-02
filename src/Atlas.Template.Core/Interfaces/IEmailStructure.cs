
namespace Atlas.Template.Core.Interfaces
{
    public interface IEmailStructure
    {
        public string To { get; }
        public string RecipientName { get; }
        public string Subject { get; }
        public string Body { get; }
        public bool IsHtml { get; }

    }
}
