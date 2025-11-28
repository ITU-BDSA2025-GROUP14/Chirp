using Microsoft.AspNetCore.Identity.UI.Services;

namespace Chirp.Web.Services;

public class NullEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // some "error" was caused, so had to make this noop sender in order for identity ui without actually using external mail
        return Task.CompletedTask;
    }
}
