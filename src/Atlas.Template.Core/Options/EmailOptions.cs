
namespace Atlas.Template.Core.Options
{
    public class EmailOptions
    {
        public string Host { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; } = 587;
    }
}
