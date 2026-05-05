using FluentAssertions;
using OutreachFlow.Application.Common;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.EmailDrafts;
using OutreachFlow.Application.Templates;
using OutreachFlow.Application.Tests.Support;
using OutreachFlow.Domain.Attachments;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.EmailDrafts;
using OutreachFlow.Domain.EmailTemplates;
using OutreachFlow.Domain.Organizations;
using OutreachFlow.Domain.SenderProfiles;

namespace OutreachFlow.Application.Tests.EmailDrafts;

public sealed class EmailDraftServiceTests
{
    [Fact]
    public async Task ShouldGenerateDraftsForEligibleContactsAndStoreDiagnostics()
    {
        var contactRepository = new InMemoryContactRepository();
        var organizationRepository = new InMemoryOrganizationRepository();
        var emailTemplateRepository = new InMemoryEmailTemplateRepository();
        var senderProfileRepository = new InMemorySenderProfileRepository();
        var attachmentRepository = new InMemoryAttachmentAssetRepository();
        var draftRepository = new InMemoryEmailDraftRepository();
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new EmailDraftService(
            contactRepository,
            organizationRepository,
            emailTemplateRepository,
            senderProfileRepository,
            attachmentRepository,
            draftRepository,
            new TemplateRenderer(),
            unitOfWork);
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
            signature: "Best regards");
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
    }

    [Fact]
    public async Task ShouldRejectInactiveSelectedAttachment()
    {
        var service = CreateService(
            out var contactRepository,
            out var emailTemplateRepository,
            out var senderProfileRepository,
            out var attachmentRepository);
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
        var service = CreateService(
            out var contactRepository,
            out var emailTemplateRepository,
            out var senderProfileRepository,
            out _);
        var contact = new Contact("Alex Morgan", "alex@example.com");
        await contactRepository.AddAsync(contact);
        var senderProfile = new SenderProfile(
            "Primary Sender",
            "sender@example.com",
            signature: "Best regards");
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
        var service = CreateService(
            out var contactRepository,
            out var emailTemplateRepository,
            out var senderProfileRepository,
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
        var service = CreateService(
            out var contactRepository,
            out var emailTemplateRepository,
            out var senderProfileRepository,
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

    private static EmailDraftService CreateService(
        out InMemoryContactRepository contactRepository,
        out InMemoryEmailTemplateRepository emailTemplateRepository,
        out InMemorySenderProfileRepository senderProfileRepository,
        out InMemoryAttachmentAssetRepository attachmentRepository)
    {
        contactRepository = new InMemoryContactRepository();
        var organizationRepository = new InMemoryOrganizationRepository();
        emailTemplateRepository = new InMemoryEmailTemplateRepository();
        senderProfileRepository = new InMemorySenderProfileRepository();
        attachmentRepository = new InMemoryAttachmentAssetRepository();
        var draftRepository = new InMemoryEmailDraftRepository();

        return new EmailDraftService(
            contactRepository,
            organizationRepository,
            emailTemplateRepository,
            senderProfileRepository,
            attachmentRepository,
            draftRepository,
            new TemplateRenderer(),
            new InMemoryUnitOfWork());
    }
}
