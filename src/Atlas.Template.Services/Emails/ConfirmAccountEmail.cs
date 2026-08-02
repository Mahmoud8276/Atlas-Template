
namespace Atlas.Template.Services.Emails
{
    public class ConfirmAccountEmail : EmailStructure
    {
        private static string? EmailLayout = null;

        public ConfirmAccountEmail(
            string to,
            string recipientName,
            string firstName,
            string emailConfirmationLink,
            string subject = "Confirm Your Account", 
            bool isHtml = true,
            bool wrapInLayout = true,
            string preheader = "ust one more step to activate your account."
            ) : base(to, recipientName, subject, BuildEmailBody(firstName, emailConfirmationLink), isHtml, EmailLayout, wrapInLayout, preheader)
        {
        }

        public static string BuildEmailBody(string firstName, string confirmationLink) => $"""
            <h2 style="margin:0 0 16px 0; font-size:20px; color:#1e293b;">Hi {firstName},</h2>
            <p style="margin:0 0 16px 0;">
                Thanks for signing up for Atlas. Please confirm your email address to activate your account.
            </p>
            {Button("Confirm Email Address", confirmationLink)}
            <p style="margin:16px 0 0 0; color:#64748b;">
                If you didn't create this account, you can safely ignore this email.
            </p>
            """;
    }
}
