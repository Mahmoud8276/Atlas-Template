
namespace Atlas.Template.Services.Emails
{
    public class ForgetPasswordEmail : EmailStructure
    {
        public ForgetPasswordEmail(
            string to,
            string recipientName, 
            string resetLink,
            string subject = "Reset Your Atlas Password", 
            bool isHtml = true,
            string? layout = null,
            bool wrapInLayout = true
            ) : base(to, recipientName, subject, BuildEmailBody(recipientName, resetLink), isHtml, layout, wrapInLayout, "Reset your password to regain access to your account.")
        {
        }

        private static string BuildEmailBody(string firstName, string resetLink) => $"""
            <h2 style="margin:0 0 16px 0; font-size:20px; color:#1e293b;">Hi {firstName},</h2>
            <p style="margin:0 0 16px 0;">
                We received a request to reset your password. Click the button below to choose a new one.
            </p>
            {Button("Reset Password", resetLink)}
            <p style="margin:16px 0 0 0; color:#64748b;">
                This link will expire soon for your security. If you didn't request a password reset,
                you can safely ignore this email — your password won't be changed.
            </p>
            """;
    }
}
