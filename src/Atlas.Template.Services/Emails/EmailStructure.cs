using Atlas.Template.Core.Interfaces;
using System;

namespace Atlas.Template.Services.Emails
{
    public abstract class EmailStructure : IEmailStructure
    {
        public string To { get; }
        public string RecipientName { get; }
        public string Subject { get; }
        public string Body { get; }
        public bool IsHtml { get; }


        private readonly string defaultLayout = $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="utf-8" />
                <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                <title>Atlas Template</title>
            </head>
            <body style="margin:0; padding:0; background-color:#f1f5f9; font-family:-apple-system,Segoe UI,Roboto,Arial,sans-serif;">
                <!-- Preheader: shows in inbox preview, hidden in the email body itself -->
                <div style="display:none; max-height:0; overflow:hidden; opacity:0;">
                    __preheader__
                </div>
 
                <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background-color:#f1f5f9; padding:32px 16px;">
                    <tr>
                        <td align="center">
                            <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="max-width:520px; background-color:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 1px 3px rgba(0,0,0,0.08);">
 
                                <!-- Header / Brand -->
                                <tr>
                                    <td style="background-color:#1e293b; padding:24px 32px;">
                                        <span style="color:#ffffff; font-size:20px; font-weight:700; letter-spacing:0.3px;">
                                            Atlas Template
                                        </span>
                                    </td>
                                </tr>
 
                                <!-- Content -->
                                <tr>
                                    <td style="padding:40px 32px; color:#1e293b; font-size:15px; line-height:1.6;">
                                        __content__
                                    </td>
                                </tr>
 
                                <!-- Divider -->
                                <tr>
                                    <td style="padding:0 32px;">
                                        <div style="border-top:1px solid #e2e8f0;"></div>
                                    </td>
                                </tr>
 
                                <!-- Footer -->
                                <tr>
                                    <td style="padding:24px 32px; color:#94a3b8; font-size:12px; line-height:1.6;">
                                        <p style="margin:0 0 4px 0;">
                                            &copy; {DateTime.Now.Year} Atlas Template. All rights reserved.
                                        </p>
                                        <p style="margin:0;">
                                            This email was sent to you because you have an account with Atlas Template.
                                        </p>
                                    </td>
                                </tr>
 
                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>
            """;

        public EmailStructure(
            string to,
            string recipientName,
            string subject, 
            string body,
            bool isHtml = true,
            string? layout = null, 
            bool wrapInLayout = true,
            string? preheader = null)
        {
            To = to;
            RecipientName = recipientName;
            Subject = subject;
            IsHtml = isHtml;
            Body = wrapInLayout==true? WrapInLayout(body, layout, preheader ?? subject): body;
        }

        private string WrapInLayout(string content, string? layout, string preheader)
        {
            var emailLayout = layout ?? defaultLayout;
            return emailLayout.Replace("__content__", content)
                              .Replace("__preheader__", preheader);
        }

        protected static string Button(string text, string url) => $"""
            <table role="presentation" cellpadding="0" cellspacing="0" style="margin:24px 0;">
                <tr>
                    <td style="border-radius:6px; background-color:#2563eb;">
                        <a href="{url}" target="_blank"
                           style="display:inline-block; padding:12px 28px; color:#ffffff; font-size:14px; font-weight:600; text-decoration:none;">
                            {text}
                        </a>
                    </td>
                </tr>
            </table>
            <p style="font-size:12px; color:#94a3b8; word-break:break-all;">
                Or copy and paste this link: <a href="{url}" style="color:#2563eb;">{url}</a>
            </p>
            """;
    }
}

