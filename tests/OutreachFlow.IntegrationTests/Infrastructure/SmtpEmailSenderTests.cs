using System.Net.Mail;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using OutreachFlow.Application.EmailSending;
using OutreachFlow.Infrastructure.DependencyInjection;
using OutreachFlow.Infrastructure.EmailSending;
using OutreachFlow.IntegrationTests.Api;

namespace OutreachFlow.IntegrationTests.Infrastructure;

public sealed class SmtpEmailSenderTests
{
    [Fact]
    public void ShouldResolveFakeEmailSenderWhenProviderIsFake()
    {
        using var serviceProvider = BuildServiceProvider(new Dictionary<string, string?>
        {
            ["ConnectionStrings:OutreachFlow"] = "Data Source=:memory:",
            ["EmailSending:Provider"] = "Fake"
        });

        var sender = serviceProvider.GetRequiredService<IEmailSender>();

        sender.Should().BeOfType<FakeEmailSender>();
    }

    [Fact]
    public void ShouldResolveSmtpEmailSenderWhenProviderIsSmtpAndConfigurationIsValid()
    {
        using var serviceProvider = BuildServiceProvider(new Dictionary<string, string?>
        {
            ["ConnectionStrings:OutreachFlow"] = "Data Source=:memory:",
            ["EmailSending:Provider"] = "SMTP",
            ["EmailSending:Smtp:Host"] = "smtp.example.com",
            ["EmailSending:Smtp:Port"] = "587",
            ["EmailSending:Smtp:UseSsl"] = "true",
            ["EmailSending:Smtp:Username"] = "smtp-user",
            ["EmailSending:Smtp:Password"] = "smtp-pass",
            ["EmailSending:Smtp:TimeoutSeconds"] = "30"
        });

        var sender = serviceProvider.GetRequiredService<IEmailSender>();

        sender.Should().BeOfType<SmtpEmailSender>();
    }

    [Fact]
    public void ShouldRejectSmtpProviderWhenRequiredConfigurationIsMissing()
    {
        using var serviceProvider = BuildServiceProvider(new Dictionary<string, string?>
        {
            ["ConnectionStrings:OutreachFlow"] = "Data Source=:memory:",
            ["EmailSending:Provider"] = "SMTP",
            ["EmailSending:Smtp:Host"] = "smtp.example.com",
            ["EmailSending:Smtp:Port"] = "587",
            ["EmailSending:Smtp:UseSsl"] = "true",
            ["EmailSending:Smtp:Username"] = "smtp-user",
            ["EmailSending:Smtp:TimeoutSeconds"] = "30"
        });

        var act = () => serviceProvider.GetRequiredService<IEmailSender>();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*EmailSending:Smtp:Password*");
    }

    [Fact]
    public async Task ShouldMapEmailCommandAndAttachmentsIntoSmtpMessage()
    {
        var snapshot = default(SentMailSnapshot);
        var options = Options.Create(new SmtpEmailSenderOptions
        {
            Host = "smtp.example.com",
            Port = 587,
            UseSsl = true,
            Username = "smtp-user",
            Password = "smtp-pass",
            TimeoutSeconds = 30
        });
        var factory = new FakeSmtpTransportFactory(message =>
        {
            snapshot = new SentMailSnapshot(
                message.From?.Address,
                message.From?.DisplayName,
                message.To.Single().Address,
                message.Subject,
                message.Body,
                message.Attachments.Single().Name ?? string.Empty,
                message.Attachments.Single().ContentType.MediaType ?? string.Empty);
        });
        var sender = new SmtpEmailSender(options, factory, NullLogger<SmtpEmailSender>.Instance);
        var tempAttachmentPath = Path.GetTempFileName();

        await File.WriteAllTextAsync(tempAttachmentPath, "attachment-content");
        var command = new SendEmailCommand(
            To: "recipient@example.com",
            Subject: "Subject",
            Body: "Body",
            Sender: new EmailSenderPayload(
                Guid.NewGuid(),
                "Primary Sender",
                "sender@example.com",
                null,
                null,
                null,
                null),
            Attachments:
            [
                new EmailAttachmentPayload(
                    Guid.NewGuid(),
                    "Brochure",
                    "brochure.txt",
                    "text/plain",
                    tempAttachmentPath,
                    17)
            ],
            Metadata: null);

        try
        {
            var result = await sender.SendAsync(command);

            result.Success.Should().BeTrue();
            result.Provider.Should().Be("SMTP");
            snapshot.Should().NotBeNull();
            snapshot!.FromAddress.Should().Be("sender@example.com");
            snapshot.FromDisplayName.Should().Be("Primary Sender");
            snapshot.ToAddress.Should().Be("recipient@example.com");
            snapshot.Subject.Should().Be("Subject");
            snapshot.Body.Should().Be("Body");
            snapshot.AttachmentName.Should().Be("brochure.txt");
            snapshot.AttachmentContentType.Should().Be("text/plain");
        }
        finally
        {
            File.Delete(tempAttachmentPath);
        }
    }

    [Fact]
    public async Task ShouldReturnFailureResultWhenSmtpTransportThrows()
    {
        var options = Options.Create(new SmtpEmailSenderOptions
        {
            Host = "smtp.example.com",
            Port = 587,
            UseSsl = true,
            Username = "smtp-user",
            Password = "smtp-pass",
            TimeoutSeconds = 30
        });
        var factory = new FakeSmtpTransportFactory(
            _ => throw new InvalidOperationException("simulated smtp failure"));
        var sender = new SmtpEmailSender(options, factory, NullLogger<SmtpEmailSender>.Instance);
        var command = new SendEmailCommand(
            To: "recipient@example.com",
            Subject: "Subject",
            Body: "Body",
            Sender: new EmailSenderPayload(
                Guid.NewGuid(),
                "Primary Sender",
                "sender@example.com",
                null,
                null,
                null,
                null),
            Attachments: [],
            Metadata: null);

        var result = await sender.SendAsync(command);

        result.Success.Should().BeFalse();
        result.Provider.Should().Be("SMTP");
        result.ErrorMessage.Should().Contain("simulated smtp failure");
    }

    [Fact]
    public void ShouldKeepFakeSenderInApiIntegrationFactory()
    {
        using var factory = new OutreachFlowApiFactory();
        using var scope = factory.Services.CreateScope();

        var sender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

        sender.Should().BeOfType<FakeEmailSender>();
    }

    private static ServiceProvider BuildServiceProvider(Dictionary<string, string?> values)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(configuration);
        return services.BuildServiceProvider();
    }

    private sealed record SentMailSnapshot(
        string? FromAddress,
        string? FromDisplayName,
        string ToAddress,
        string Subject,
        string Body,
        string AttachmentName,
        string AttachmentContentType);

    private sealed class FakeSmtpTransportFactory(Action<MailMessage> onSend) : ISmtpTransportFactory
    {
        public ISmtpTransport Create(SmtpEmailSenderOptions options)
        {
            return new FakeSmtpTransport(onSend);
        }
    }

    private sealed class FakeSmtpTransport(Action<MailMessage> onSend) : ISmtpTransport
    {
        public Task SendMailAsync(MailMessage message, CancellationToken cancellationToken)
        {
            onSend(message);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
