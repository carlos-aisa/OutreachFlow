using FluentAssertions;
using OutreachFlow.Application.Common;
using OutreachFlow.Application.ContactActivities;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.EmailDrafts;
using OutreachFlow.Application.EmailSending;
using OutreachFlow.Application.Templates;
using OutreachFlow.Application.Tests.Support;
using OutreachFlow.Domain.Attachments;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.ContactActivities;
using OutreachFlow.Domain.EmailDrafts;
using OutreachFlow.Domain.EmailMessages;
using OutreachFlow.Domain.EmailTemplates;
using OutreachFlow.Domain.FollowUps;
using OutreachFlow.Domain.SenderProfiles;

namespace OutreachFlow.Application.Tests.EmailDrafts;

public sealed class EmailDraftServiceTests
{
    [Fact]
    public async Task ShouldAppendSenderSignatureWhenGeneratingDraft()
    {
        var service = CreateEmailDraftService(
            out var contactRepository,
            out var emailTemplateRepository,
            out var senderProfileRepository,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _);
        var contact = new Contact("Alex Morgan", "alex@example.com");
        await contactRepository.AddAsync(contact);
        var senderProfile = new SenderProfile(
            "Primary Sender",
            "sender@example.com",
            signature: "<p>Best regards</p>",
            signatureFormat: SenderSignatureFormat.Html);
        await senderProfileRepository.AddAsync(senderProfile);
        var template = new EmailTemplate("Intro", null, "Subject", "Hello {{contact.displayName}}.");
        await emailTemplateRepository.AddAsync(template);

        var result = await service.GenerateAsync(new GenerateEmailDraftsRequest(
            Search: null,
            TagId: null,
            Status: null,
            DoNotContact: null,
            OrganizationId: null,
            LastContactedFrom: null,
            LastContactedTo: null,
            TemplateId: template.Id,
            SenderProfileId: senderProfile.Id,
            AttachmentAssetIds: []));

        result.Drafts.Should().ContainSingle();
        result.Drafts[0].Body.Should().Be("Hello Alex Morgan.\n\n<p>Best regards</p>");
    }

    [Fact]
    public async Task ShouldKeepRenderedBodyUnchangedWhenSenderHasNoSignature()
    {
        var service = CreateEmailDraftService(
            out var contactRepository,
            out var emailTemplateRepository,
            out var senderProfileRepository,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _);
        var contact = new Contact("Alex Morgan", "alex@example.com");
        await contactRepository.AddAsync(contact);
        var senderProfile = new SenderProfile("Primary Sender", "sender@example.com");
        await senderProfileRepository.AddAsync(senderProfile);
        var template = new EmailTemplate("Intro", null, "Subject", "Hello {{contact.displayName}}.");
        await emailTemplateRepository.AddAsync(template);

        var result = await service.GenerateAsync(new GenerateEmailDraftsRequest(
            Search: null,
            TagId: null,
            Status: null,
            DoNotContact: null,
            OrganizationId: null,
            LastContactedFrom: null,
            LastContactedTo: null,
            TemplateId: template.Id,
            SenderProfileId: senderProfile.Id,
            AttachmentAssetIds: []));

        result.Drafts.Should().ContainSingle();
        result.Drafts[0].Body.Should().Be("Hello Alex Morgan.");
    }

    [Fact]
    public async Task ShouldGenerateDraftsForEligibleContactsAndStoreDiagnostics()
    {
        var service = CreateEmailDraftService(
            out var contactRepository,
            out var emailTemplateRepository,
            out var senderProfileRepository,
            out var attachmentRepository,
            out var draftRepository,
            out _,
            out _,
            out var contactActivityRepository,
            out var unitOfWork);
        var eligibleContact = new Contact(
            "Alex Morgan",
            "alex@example.com");
        var doNotContact = new Contact(
            "Jamie Smith",
            "jamie@example.com",
            doNotContact: true);
        await contactRepository.AddAsync(eligibleContact);
        await contactRepository.AddAsync(doNotContact);
        var senderProfile = new SenderProfile(
            "Primary Sender",
            "sender@example.com",
            signature: "<p>Best regards</p>",
            signatureFormat: SenderSignatureFormat.Html);
        await senderProfileRepository.AddAsync(senderProfile);
        var emailTemplate = new EmailTemplate(
            "Intro",
            null,
            "Subject for {{contact.displayName}}",
            "Hello {{contact.displayName}}, organization {{organization.name}}. {{sender.signature}}");
        var defaultAttachment = new AttachmentAsset(
            "Brochure",
            "brochure.pdf",
            "application/pdf",
            "storage/brochure.pdf",
            1024);
        var optionalAttachment = new AttachmentAsset(
            "Proposal",
            "proposal.pdf",
            "application/pdf",
            "storage/proposal.pdf",
            2048);
        emailTemplate.AssignDefaultAttachment(defaultAttachment, DateTimeOffset.UtcNow);
        await emailTemplateRepository.AddAsync(emailTemplate);
        await attachmentRepository.AddAsync(defaultAttachment);
        await attachmentRepository.AddAsync(optionalAttachment);

        var result = await service.GenerateAsync(new GenerateEmailDraftsRequest(
            Search: null,
            TagId: null,
            Status: null,
            DoNotContact: null,
            OrganizationId: null,
            LastContactedFrom: null,
            LastContactedTo: null,
            TemplateId: emailTemplate.Id,
            SenderProfileId: senderProfile.Id,
            AttachmentAssetIds: [optionalAttachment.Id]));

        result.RequestedContacts.Should().Be(2);
        result.GeneratedDrafts.Should().Be(1);
        result.SkippedContacts.Should().Be(1);
        result.Skipped.Should().ContainSingle(item => item.ContactId == doNotContact.Id);
        result.Drafts.Should().ContainSingle();
        var generatedDraft = result.Drafts.Single();
        generatedDraft.ContactId.Should().Be(eligibleContact.Id);
        generatedDraft.Status.Should().Be(EmailDraftStatus.NeedsReview);
        generatedDraft.MissingVariables.Should().Contain("organization.name");
        generatedDraft.AttachmentAssetIds.Should().BeEquivalentTo([defaultAttachment.Id, optionalAttachment.Id]);
        draftRepository.Drafts.Should().ContainSingle();
        unitOfWork.SaveChangesCount.Should().Be(1);
        contactActivityRepository.Activities.Should().ContainSingle(activity =>
            activity.Type == ContactActivityType.EmailDraftCreated &&
            activity.ContactId == eligibleContact.Id);
    }

    [Fact]
    public async Task ShouldRejectInactiveSelectedAttachment()
    {
        var service = CreateEmailDraftService(
            out var contactRepository,
            out var emailTemplateRepository,
            out var senderProfileRepository,
            out var attachmentRepository,
            out _,
            out _,
            out _,
            out _,
            out _);
        var contact = new Contact("Alex Morgan", "alex@example.com");
        await contactRepository.AddAsync(contact);
        var senderProfile = new SenderProfile("Primary Sender", "sender@example.com");
        await senderProfileRepository.AddAsync(senderProfile);
        var emailTemplate = new EmailTemplate("Intro", null, "Subject", "Body");
        await emailTemplateRepository.AddAsync(emailTemplate);
        var inactiveAttachment = new AttachmentAsset(
            "Brochure",
            "brochure.pdf",
            "application/pdf",
            "storage/brochure.pdf",
            1024);
        inactiveAttachment.Deactivate();
        await attachmentRepository.AddAsync(inactiveAttachment);

        var act = () => service.GenerateAsync(new GenerateEmailDraftsRequest(
            Search: null,
            TagId: null,
            Status: null,
            DoNotContact: null,
            OrganizationId: null,
            LastContactedFrom: null,
            LastContactedTo: null,
            TemplateId: emailTemplate.Id,
            SenderProfileId: senderProfile.Id,
            AttachmentAssetIds: [inactiveAttachment.Id]));

        await act.Should().ThrowAsync<ApplicationValidationException>()
            .WithMessage("Inactive attachments cannot be used for draft generation.");
    }

    [Fact]
    public async Task ShouldUpdateNeedsReviewDraftAndApproveAfterManualFix()
    {
        var service = CreateEmailDraftService(
            out var contactRepository,
            out var emailTemplateRepository,
            out var senderProfileRepository,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _);
        var contact = new Contact("Alex Morgan", "alex@example.com");
        await contactRepository.AddAsync(contact);
        var senderProfile = new SenderProfile(
            "Primary Sender",
            "sender@example.com",
            signature: "<p>Best regards</p>",
            signatureFormat: SenderSignatureFormat.Html);
        await senderProfileRepository.AddAsync(senderProfile);
        var template = new EmailTemplate(
            "Intro",
            null,
            "Subject for {{contact.displayName}}",
            "Hello {{contact.displayName}}, organization {{organization.name}}.");
        await emailTemplateRepository.AddAsync(template);

        var generationResult = await service.GenerateAsync(new GenerateEmailDraftsRequest(
            Search: null,
            TagId: null,
            Status: null,
            DoNotContact: null,
            OrganizationId: null,
            LastContactedFrom: null,
            LastContactedTo: null,
            TemplateId: template.Id,
            SenderProfileId: senderProfile.Id,
            AttachmentAssetIds: []));
        var generatedDraft = generationResult.Drafts.Single();

        var updatedDraft = await service.UpdateAsync(
            generatedDraft.Id,
            new UpdateEmailDraftRequest(
                "Manual subject",
                "Manual body without unresolved variables."));

        updatedDraft.Status.Should().Be(EmailDraftStatus.Draft);
        updatedDraft.HasRenderErrors.Should().BeFalse();
        updatedDraft.MissingVariables.Should().BeEmpty();
        updatedDraft.UnknownVariables.Should().BeEmpty();

        var approvedDraft = await service.ApproveAsync(generatedDraft.Id);

        approvedDraft.Status.Should().Be(EmailDraftStatus.Approved);
        approvedDraft.ApprovedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldRejectApprovalWhenDraftStillHasRenderErrors()
    {
        var service = CreateEmailDraftService(
            out var contactRepository,
            out var emailTemplateRepository,
            out var senderProfileRepository,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _);
        var contact = new Contact("Alex Morgan", "alex@example.com");
        await contactRepository.AddAsync(contact);
        var senderProfile = new SenderProfile("Primary Sender", "sender@example.com");
        await senderProfileRepository.AddAsync(senderProfile);
        var template = new EmailTemplate(
            "Intro",
            null,
            "Subject {{contact.displayName}}",
            "Body {{organization.name}}");
        await emailTemplateRepository.AddAsync(template);

        var generationResult = await service.GenerateAsync(new GenerateEmailDraftsRequest(
            Search: null,
            TagId: null,
            Status: null,
            DoNotContact: null,
            OrganizationId: null,
            LastContactedFrom: null,
            LastContactedTo: null,
            TemplateId: template.Id,
            SenderProfileId: senderProfile.Id,
            AttachmentAssetIds: []));
        var draft = generationResult.Drafts.Single();

        var act = () => service.ApproveAsync(draft.Id);

        await act.Should().ThrowAsync<ApplicationValidationException>()
            .WithMessage("Draft cannot be approved while render errors remain.");
    }

    [Fact]
    public async Task ShouldCancelDraftAndRejectFurtherApprovals()
    {
        var service = CreateEmailDraftService(
            out var contactRepository,
            out var emailTemplateRepository,
            out var senderProfileRepository,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _);
        var contact = new Contact("Alex Morgan", "alex@example.com");
        await contactRepository.AddAsync(contact);
        var senderProfile = new SenderProfile("Primary Sender", "sender@example.com");
        await senderProfileRepository.AddAsync(senderProfile);
        var template = new EmailTemplate("Intro", null, "Subject", "Body");
        await emailTemplateRepository.AddAsync(template);

        var generationResult = await service.GenerateAsync(new GenerateEmailDraftsRequest(
            Search: null,
            TagId: null,
            Status: null,
            DoNotContact: null,
            OrganizationId: null,
            LastContactedFrom: null,
            LastContactedTo: null,
            TemplateId: template.Id,
            SenderProfileId: senderProfile.Id,
            AttachmentAssetIds: []));
        var draft = generationResult.Drafts.Single();

        var cancelledDraft = await service.CancelAsync(draft.Id);

        cancelledDraft.Status.Should().Be(EmailDraftStatus.Cancelled);
        cancelledDraft.CancelledAt.Should().NotBeNull();

        var approveAct = () => service.ApproveAsync(draft.Id);
        await approveAct.Should().ThrowAsync<ApplicationValidationException>()
            .WithMessage("Cancelled drafts cannot be approved.");
    }

    [Fact]
    public async Task ShouldSendApprovedDraftAndPersistSuccessfulEmailMessage()
    {
        var service = CreateEmailDraftService(
            out var contactRepository,
            out var emailTemplateRepository,
            out var senderProfileRepository,
            out _,
            out _,
            out var emailMessageRepository,
            out _,
            out var contactActivityRepository,
            out _);
        var contact = new Contact("Alex Morgan", "alex@example.com");
        await contactRepository.AddAsync(contact);
        var senderProfile = new SenderProfile("Primary Sender", "sender@example.com");
        await senderProfileRepository.AddAsync(senderProfile);
        var template = new EmailTemplate("Intro", null, "Subject", "Body");
        await emailTemplateRepository.AddAsync(template);

        var generationResult = await service.GenerateAsync(new GenerateEmailDraftsRequest(
            Search: null,
            TagId: null,
            Status: null,
            DoNotContact: null,
            OrganizationId: null,
            LastContactedFrom: null,
            LastContactedTo: null,
            TemplateId: template.Id,
            SenderProfileId: senderProfile.Id,
            AttachmentAssetIds: []));
        var draftId = generationResult.Drafts.Single().Id;
        await service.ApproveAsync(draftId);

        var sentDraft = await service.SendApprovedDraftAsync(draftId);

        sentDraft.Status.Should().Be(EmailDraftStatus.Sent);
        sentDraft.SentAt.Should().NotBeNull();
        sentDraft.FailureReason.Should().BeNull();
        emailMessageRepository.EmailMessages.Should().ContainSingle();
        emailMessageRepository.EmailMessages[0].Status.Should().Be(EmailMessageStatus.Sent);
        contactActivityRepository.Activities.Should().Contain(activity =>
            activity.Type == ContactActivityType.EmailSent &&
            activity.ContactId == contact.Id &&
            activity.Subject == "Subject");

        var updatedContact = await contactRepository.GetByIdAsync(contact.Id);
        updatedContact.Should().NotBeNull();
        updatedContact!.LastContactedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldCreateFollowUpTaskAutomaticallyAfterSuccessfulSendWhenEnabled()
    {
        var service = CreateEmailDraftService(
            out var contactRepository,
            out var emailTemplateRepository,
            out var senderProfileRepository,
            out _,
            out _,
            out _,
            out var followUpTaskRepository,
            out _,
            out _,
            autoCreateFollowUp: true);
        var contact = new Contact("Alex Morgan", "alex@example.com");
        await contactRepository.AddAsync(contact);
        var senderProfile = new SenderProfile("Primary Sender", "sender@example.com");
        await senderProfileRepository.AddAsync(senderProfile);
        var template = new EmailTemplate("Intro", null, "Subject", "Body");
        await emailTemplateRepository.AddAsync(template);

        var generationResult = await service.GenerateAsync(new GenerateEmailDraftsRequest(
            Search: null,
            TagId: null,
            Status: null,
            DoNotContact: null,
            OrganizationId: null,
            LastContactedFrom: null,
            LastContactedTo: null,
            TemplateId: template.Id,
            SenderProfileId: senderProfile.Id,
            AttachmentAssetIds: []));
        var draftId = generationResult.Drafts.Single().Id;
        await service.ApproveAsync(draftId);

        _ = await service.SendApprovedDraftAsync(draftId);

        followUpTaskRepository.Tasks.Should().ContainSingle(task =>
            task.ContactId == contact.Id &&
            task.Type == FollowUpTaskType.Email &&
            !task.IsCompleted);
    }

    [Fact]
    public async Task ShouldPersistFailedEmailMessageWhenSenderFails()
    {
        var service = CreateEmailDraftService(
            out var contactRepository,
            out var emailTemplateRepository,
            out var senderProfileRepository,
            out _,
            out _,
            out var emailMessageRepository,
            out _,
            out var contactActivityRepository,
            out _,
            sendHandler: _ => new EmailSendResult(
                Success: false,
                Provider: "Fake",
                ProviderMessageId: null,
                ErrorMessage: "Simulated failure from tests."));
        var contact = new Contact("Alex Morgan", "alex@example.com");
        await contactRepository.AddAsync(contact);
        var senderProfile = new SenderProfile("Primary Sender", "sender@example.com");
        await senderProfileRepository.AddAsync(senderProfile);
        var template = new EmailTemplate("Intro", null, "Subject", "Body");
        await emailTemplateRepository.AddAsync(template);

        var generationResult = await service.GenerateAsync(new GenerateEmailDraftsRequest(
            Search: null,
            TagId: null,
            Status: null,
            DoNotContact: null,
            OrganizationId: null,
            LastContactedFrom: null,
            LastContactedTo: null,
            TemplateId: template.Id,
            SenderProfileId: senderProfile.Id,
            AttachmentAssetIds: []));
        var draftId = generationResult.Drafts.Single().Id;
        await service.ApproveAsync(draftId);

        var failedDraft = await service.SendApprovedDraftAsync(draftId);

        failedDraft.Status.Should().Be(EmailDraftStatus.Failed);
        failedDraft.FailureReason.Should().Be("Simulated failure from tests.");
        emailMessageRepository.EmailMessages.Should().ContainSingle();
        emailMessageRepository.EmailMessages[0].Status.Should().Be(EmailMessageStatus.Failed);
        contactActivityRepository.Activities.Should().Contain(activity =>
            activity.Type == ContactActivityType.EmailFailed &&
            activity.ContactId == contact.Id &&
            activity.Subject == "Subject");
    }

    [Fact]
    public async Task ShouldBlockDuplicateSendForAlreadySentDraft()
    {
        var service = CreateEmailDraftService(
            out var contactRepository,
            out var emailTemplateRepository,
            out var senderProfileRepository,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _);
        var contact = new Contact("Alex Morgan", "alex@example.com");
        await contactRepository.AddAsync(contact);
        var senderProfile = new SenderProfile("Primary Sender", "sender@example.com");
        await senderProfileRepository.AddAsync(senderProfile);
        var template = new EmailTemplate("Intro", null, "Subject", "Body");
        await emailTemplateRepository.AddAsync(template);

        var generationResult = await service.GenerateAsync(new GenerateEmailDraftsRequest(
            Search: null,
            TagId: null,
            Status: null,
            DoNotContact: null,
            OrganizationId: null,
            LastContactedFrom: null,
            LastContactedTo: null,
            TemplateId: template.Id,
            SenderProfileId: senderProfile.Id,
            AttachmentAssetIds: []));
        var draftId = generationResult.Drafts.Single().Id;
        await service.ApproveAsync(draftId);
        _ = await service.SendApprovedDraftAsync(draftId);

        var act = () => service.SendApprovedDraftAsync(draftId);

        await act.Should().ThrowAsync<ApplicationValidationException>()
            .WithMessage("This draft was already sent.");
    }

    [Fact]
    public async Task ShouldBlockEquivalentRecentEmailSend()
    {
        var service = CreateEmailDraftService(
            out var contactRepository,
            out var emailTemplateRepository,
            out var senderProfileRepository,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _,
            sendHandler: _ => new EmailSendResult(
                Success: true,
                Provider: "Fake",
                ProviderMessageId: $"fake-{Guid.NewGuid():N}",
                ErrorMessage: null),
            equivalentEmailWindow: TimeSpan.FromDays(7));
        var contact = new Contact("Alex Morgan", "alex@example.com");
        await contactRepository.AddAsync(contact);
        var senderProfile = new SenderProfile("Primary Sender", "sender@example.com");
        await senderProfileRepository.AddAsync(senderProfile);
        var template = new EmailTemplate("Intro", null, "Subject", "Body");
        await emailTemplateRepository.AddAsync(template);

        var firstGeneration = await service.GenerateAsync(new GenerateEmailDraftsRequest(
            Search: null,
            TagId: null,
            Status: null,
            DoNotContact: null,
            OrganizationId: null,
            LastContactedFrom: null,
            LastContactedTo: null,
            TemplateId: template.Id,
            SenderProfileId: senderProfile.Id,
            AttachmentAssetIds: []));
        var firstDraftId = firstGeneration.Drafts.Single().Id;
        await service.ApproveAsync(firstDraftId);
        _ = await service.SendApprovedDraftAsync(firstDraftId);

        var secondGeneration = await service.GenerateAsync(new GenerateEmailDraftsRequest(
            Search: null,
            TagId: null,
            Status: null,
            DoNotContact: null,
            OrganizationId: null,
            LastContactedFrom: null,
            LastContactedTo: null,
            TemplateId: template.Id,
            SenderProfileId: senderProfile.Id,
            AttachmentAssetIds: []));
        var secondDraftId = secondGeneration.Drafts.Single().Id;
        await service.ApproveAsync(secondDraftId);

        var act = () => service.SendApprovedDraftAsync(secondDraftId);

        await act.Should().ThrowAsync<ApplicationValidationException>()
            .WithMessage("An equivalent email was already sent to this contact recently.");
    }

    [Fact]
    public async Task ShouldBlockSendingToDoNotContactRecipient()
    {
        var service = CreateEmailDraftService(
            out var contactRepository,
            out var emailTemplateRepository,
            out var senderProfileRepository,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _);
        var contact = new Contact("Alex Morgan", "alex@example.com");
        await contactRepository.AddAsync(contact);
        var senderProfile = new SenderProfile("Primary Sender", "sender@example.com");
        await senderProfileRepository.AddAsync(senderProfile);
        var template = new EmailTemplate("Intro", null, "Subject", "Body");
        await emailTemplateRepository.AddAsync(template);

        var generationResult = await service.GenerateAsync(new GenerateEmailDraftsRequest(
            Search: null,
            TagId: null,
            Status: null,
            DoNotContact: null,
            OrganizationId: null,
            LastContactedFrom: null,
            LastContactedTo: null,
            TemplateId: template.Id,
            SenderProfileId: senderProfile.Id,
            AttachmentAssetIds: []));
        var draftId = generationResult.Drafts.Single().Id;
        await service.ApproveAsync(draftId);
        contact.MarkDoNotContact(DateTimeOffset.UtcNow);

        var act = () => service.SendApprovedDraftAsync(draftId);

        await act.Should().ThrowAsync<ApplicationValidationException>()
            .WithMessage("Do Not Contact recipients cannot receive emails.");
    }

    private static EmailDraftService CreateEmailDraftService(
        out InMemoryContactRepository contactRepository,
        out InMemoryEmailTemplateRepository emailTemplateRepository,
        out InMemorySenderProfileRepository senderProfileRepository,
        out InMemoryAttachmentAssetRepository attachmentRepository,
        out InMemoryEmailDraftRepository draftRepository,
        out InMemoryEmailMessageRepository emailMessageRepository,
        out InMemoryFollowUpTaskRepository followUpTaskRepository,
        out InMemoryContactActivityRepository contactActivityRepository,
        out InMemoryUnitOfWork unitOfWork,
        Func<SendEmailCommand, EmailSendResult>? sendHandler = null,
        TimeSpan? equivalentEmailWindow = null,
        bool autoCreateFollowUp = false)
    {
        contactRepository = new InMemoryContactRepository();
        var organizationRepository = new InMemoryOrganizationRepository();
        emailTemplateRepository = new InMemoryEmailTemplateRepository();
        senderProfileRepository = new InMemorySenderProfileRepository();
        attachmentRepository = new InMemoryAttachmentAssetRepository();
        draftRepository = new InMemoryEmailDraftRepository();
        emailMessageRepository = new InMemoryEmailMessageRepository();
        followUpTaskRepository = new InMemoryFollowUpTaskRepository();
        contactActivityRepository = new InMemoryContactActivityRepository();
        unitOfWork = new InMemoryUnitOfWork();
        var emailSender = new InMemoryEmailSender(sendHandler);
        var policy = new FixedEmailSendingPolicy(equivalentEmailWindow ?? TimeSpan.FromDays(7));
        var followUpPolicy = new FixedFollowUpAutomationPolicy(
            autoCreateFollowUp,
            autoCreateDueDays: 7,
            FollowUpTaskType.Email);
        var contactActivityService = new ContactActivityService(contactRepository, contactActivityRepository);

        return new EmailDraftService(
            contactRepository,
            organizationRepository,
            emailTemplateRepository,
            senderProfileRepository,
            attachmentRepository,
            draftRepository,
            emailMessageRepository,
            followUpTaskRepository,
            emailSender,
            policy,
            followUpPolicy,
            contactActivityService,
            new TemplateRenderer(),
            unitOfWork);
    }
}
