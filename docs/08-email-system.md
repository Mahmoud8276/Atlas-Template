# 08 — Email System

Atlas sends email through a small custom layout system rather than a full templating engine (Razor views, Scriban, etc.). For a handful of transactional emails, pulling in a templating dependency and its build-time compilation step is more machinery than the problem needs — string interpolation plus one shared HTML layout covers it, and that's what this is.

## The pieces

- **`IEmailStructure`** (Core) — the contract: `To`, `RecipientName`, `Subject`, `Body`, `IsHtml`. This is what `IEmailService.SendAsync` actually depends on, not any concrete email class.
- **`EmailStructure`** (Services, abstract) — implements `IEmailStructure` and owns the layout-wrapping logic. Every concrete email inherits this.
- **A default HTML layout**, hard-coded as a string inside `EmailStructure` — header bar, a content area, a footer, styled inline (required for HTML email — most clients strip `<style>` blocks). It has two placeholders: `__content__` and `__preheader__`.
- **`WrapInLayout(content, layout, preheader)`** — takes whichever layout you pass (or the default if you pass `null`), and does two string replacements: `__content__` → your email's actual body, `__preheader__` → the short preview text inbox clients show next to the subject line.
- **`Button(text, url)`** — a small protected helper on `EmailStructure` that concrete emails can call to render a consistently styled call-to-action button, so you're not rewriting button HTML in every email.

## How a concrete email is built

`ConfirmAccountEmail` and `ForgetPasswordEmail` follow the same shape: build a content string specific to that email, then hand it to the base constructor, which wraps it in the layout.

```csharp
public class ForgetPasswordEmail : EmailStructure
{
    public ForgetPasswordEmail(string to, string recipientName, string resetLink,
        string subject = "Reset Your Atlas Password", bool isHtml = true,
        string? layout = null, bool wrapInLayout = true)
        : base(to, recipientName, subject, BuildEmailBody(recipientName, resetLink),
               isHtml, layout, wrapInLayout, "Reset your password to regain access to your account.")
    { }

    private static string BuildEmailBody(string firstName, string resetLink) => $"""
        <h2 style="margin:0 0 16px 0; font-size:20px; color:#1e293b;">Hi {firstName},</h2>
        <p style="margin:0 0 16px 0;">We received a request to reset your password...</p>
        {Button("Reset Password", resetLink)}
        """;
}
```

Both existing emails accept an optional `layout` parameter allowing a caller to override the layout per-send — but neither `AccountService` call site actually passes one, so in practice both currently always render with the default layout. The override exists and works; nobody's used it yet.

## `EmailService` — actually sending it

```csharp
public async Task SendAsync(IEmailStructure email)
{
    var message = new MimeMessage();
    message.From.Add(new MailboxAddress(_emailOptions.SenderName, _emailOptions.SenderEmail));
    message.To.Add(new MailboxAddress(email.RecipientName, email.To));
    message.Subject = email.Subject;
    message.Body = email.IsHtml ? new TextPart("html") { Text = email.Body } : new TextPart("plain") { Text = email.Body };

    using var smtpClient = new SmtpClient();
    await smtpClient.ConnectAsync(_emailOptions.Host, _emailOptions.Port, SecureSocketOptions.StartTls);
    await smtpClient.AuthenticateAsync(_emailOptions.SenderEmail, _emailOptions.Password);
    await smtpClient.SendAsync(message);
    await smtpClient.DisconnectAsync(true);
}
```

Because `SendAsync` takes the `IEmailStructure` interface, `EmailService` never needs to know which concrete email it's sending — it just needs something that can produce `To`/`Subject`/`Body`/`IsHtml`. That's what makes adding a new email type a Services-layer-only change; `EmailService` itself never needs editing.

Configuration comes from `EmailOptions`, bound from the `Email` section of `appsettings` (`Host`, `Port`, `SenderEmail`, `SenderName`, `Password`) — see [`docs/11-configuration-and-options.md`](11-configuration-and-options.md) for how the options pattern wires that up, and don't commit real SMTP credentials into `appsettings.Development.json`.

## Worked example: adding an order confirmation email

```csharp
public class OrderConfirmationEmail : EmailStructure
{
    public OrderConfirmationEmail(string to, string recipientName, string orderNumber, decimal total,
        string subject = "Your Atlas Order Confirmation", bool isHtml = true,
        string? layout = null, bool wrapInLayout = true)
        : base(to, recipientName, subject, BuildEmailBody(recipientName, orderNumber, total),
               isHtml, layout, wrapInLayout, $"Order #{orderNumber} is confirmed.")
    { }

    private static string BuildEmailBody(string firstName, string orderNumber, decimal total) => $"""
        <h2 style="margin:0 0 16px 0; font-size:20px; color:#1e293b;">Hi {firstName},</h2>
        <p style="margin:0 0 16px 0;">Your order <strong>#{orderNumber}</strong> is confirmed — total: {total:C}.</p>
        """;
}
```

Sending it from a service is just:

```csharp
await _emailService.SendAsync(new OrderConfirmationEmail(user.Email, user.FirstName, order.Number, order.Total));
```

No DI registration needed for the email class itself — only `IEmailService`/`EmailService` is registered; concrete emails are just objects you construct and pass in.

---

**Next:** [09 — Data Seeding](09-data-seeding.md)