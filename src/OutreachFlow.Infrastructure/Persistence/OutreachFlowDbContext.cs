using Microsoft.EntityFrameworkCore;
using OutreachFlow.Domain.Attachments;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.EmailDrafts;
using OutreachFlow.Domain.EmailMessages;
using OutreachFlow.Domain.EmailTemplates;
using OutreachFlow.Domain.Organizations;
using OutreachFlow.Domain.SenderProfiles;
using OutreachFlow.Domain.Tags;

namespace OutreachFlow.Infrastructure.Persistence;

public sealed class OutreachFlowDbContext(DbContextOptions<OutreachFlowDbContext> options)
    : DbContext(options)
{
    public DbSet<Contact> Contacts => Set<Contact>();

    public DbSet<Organization> Organizations => Set<Organization>();

    public DbSet<Tag> Tags => Set<Tag>();

    public DbSet<ContactTag> ContactTags => Set<ContactTag>();

    public DbSet<SenderProfile> SenderProfiles => Set<SenderProfile>();

    public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();

    public DbSet<AttachmentAsset> AttachmentAssets => Set<AttachmentAsset>();

    public DbSet<EmailTemplateAttachment> EmailTemplateAttachments => Set<EmailTemplateAttachment>();

    public DbSet<EmailDraft> EmailDrafts => Set<EmailDraft>();

    public DbSet<EmailDraftAttachment> EmailDraftAttachments => Set<EmailDraftAttachment>();

    public DbSet<EmailMessage> EmailMessages => Set<EmailMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureOrganizations(modelBuilder);
        ConfigureContacts(modelBuilder);
        ConfigureTags(modelBuilder);
        ConfigureContactTags(modelBuilder);
        ConfigureSenderProfiles(modelBuilder);
        ConfigureAttachmentAssets(modelBuilder);
        ConfigureEmailTemplates(modelBuilder);
        ConfigureEmailTemplateAttachments(modelBuilder);
        ConfigureEmailDrafts(modelBuilder);
        ConfigureEmailDraftAttachments(modelBuilder);
        ConfigureEmailMessages(modelBuilder);
    }

    private static void ConfigureOrganizations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>(builder =>
        {
            builder.ToTable("Organizations");
            builder.HasKey(organization => organization.Id);

            builder.Property(organization => organization.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(organization => organization.Type).HasMaxLength(100);
            builder.Property(organization => organization.Website).HasMaxLength(500);
            builder.Property(organization => organization.City).HasMaxLength(100);
            builder.Property(organization => organization.Province).HasMaxLength(100);
            builder.Property(organization => organization.Country).HasMaxLength(100);
            builder.Property(organization => organization.Notes).HasMaxLength(4000);
        });
    }

    private static void ConfigureContacts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(builder =>
        {
            builder.ToTable("Contacts");
            builder.HasKey(contact => contact.Id);

            builder.Property(contact => contact.DisplayName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(contact => contact.Email)
                .HasMaxLength(320)
                .IsRequired();

            builder.Property(contact => contact.NormalizedEmail)
                .HasMaxLength(320)
                .IsRequired();

            builder.HasIndex(contact => contact.NormalizedEmail)
                .IsUnique();

            builder.HasIndex(contact => contact.OrganizationId);
            builder.HasIndex(contact => contact.Status);
            builder.HasIndex(contact => contact.DoNotContact);

            builder.Property(contact => contact.Phone).HasMaxLength(100);
            builder.Property(contact => contact.Role).HasMaxLength(200);
            builder.Property(contact => contact.Source).HasMaxLength(200);

            builder.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(contact => contact.OrganizationId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(contact => contact.Tags)
                .WithOne()
                .HasForeignKey(contactTag => contactTag.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(contact => contact.Tags)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });
    }

    private static void ConfigureTags(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tag>(builder =>
        {
            builder.ToTable("Tags");
            builder.HasKey(tag => tag.Id);

            builder.Property(tag => tag.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(tag => tag.NormalizedName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(tag => tag.Category).HasMaxLength(100);

            builder.Property(tag => tag.NormalizedCategory)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(tag => new { tag.NormalizedCategory, tag.NormalizedName })
                .IsUnique();
        });
    }

    private static void ConfigureContactTags(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContactTag>(builder =>
        {
            builder.ToTable("ContactTags");
            builder.HasKey(contactTag => new { contactTag.ContactId, contactTag.TagId });

            builder.HasIndex(contactTag => contactTag.TagId);

            builder.HasOne<Tag>()
                .WithMany()
                .HasForeignKey(contactTag => contactTag.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureSenderProfiles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SenderProfile>(builder =>
        {
            builder.ToTable("SenderProfiles");
            builder.HasKey(senderProfile => senderProfile.Id);

            builder.Property(senderProfile => senderProfile.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(senderProfile => senderProfile.Email)
                .HasMaxLength(320)
                .IsRequired();

            builder.Property(senderProfile => senderProfile.NormalizedEmail)
                .HasMaxLength(320)
                .IsRequired();

            builder.Property(senderProfile => senderProfile.Phone).HasMaxLength(100);
            builder.Property(senderProfile => senderProfile.OrganizationName).HasMaxLength(200);
            builder.Property(senderProfile => senderProfile.Website).HasMaxLength(500);
            builder.Property(senderProfile => senderProfile.Signature).HasMaxLength(4000);

            builder.HasIndex(senderProfile => senderProfile.IsDefault);
            builder.HasIndex(senderProfile => senderProfile.IsActive);
            builder.HasIndex(senderProfile => senderProfile.NormalizedEmail);
        });
    }

    private static void ConfigureEmailTemplates(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmailTemplate>(builder =>
        {
            builder.ToTable("EmailTemplates");
            builder.HasKey(emailTemplate => emailTemplate.Id);

            builder.Property(emailTemplate => emailTemplate.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(emailTemplate => emailTemplate.Description).HasMaxLength(4000);

            builder.Property(emailTemplate => emailTemplate.SubjectTemplate)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(emailTemplate => emailTemplate.BodyTemplate)
                .HasMaxLength(20000)
                .IsRequired();

            builder.HasIndex(emailTemplate => emailTemplate.IsActive);
            builder.HasIndex(emailTemplate => emailTemplate.Name);

            builder.HasMany(emailTemplate => emailTemplate.DefaultAttachments)
                .WithOne()
                .HasForeignKey(emailTemplateAttachment => emailTemplateAttachment.EmailTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(emailTemplate => emailTemplate.DefaultAttachments)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });
    }

    private static void ConfigureAttachmentAssets(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AttachmentAsset>(builder =>
        {
            builder.ToTable("AttachmentAssets");
            builder.HasKey(attachmentAsset => attachmentAsset.Id);

            builder.Property(attachmentAsset => attachmentAsset.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(attachmentAsset => attachmentAsset.FileName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(attachmentAsset => attachmentAsset.ContentType)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(attachmentAsset => attachmentAsset.StoragePath)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(attachmentAsset => attachmentAsset.Description)
                .HasMaxLength(4000);

            builder.HasIndex(attachmentAsset => attachmentAsset.IsActive);
            builder.HasIndex(attachmentAsset => attachmentAsset.Name);
            builder.HasIndex(attachmentAsset => attachmentAsset.FileName);
        });
    }

    private static void ConfigureEmailTemplateAttachments(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmailTemplateAttachment>(builder =>
        {
            builder.ToTable("EmailTemplateAttachments");
            builder.HasKey(emailTemplateAttachment => new
            {
                emailTemplateAttachment.EmailTemplateId,
                emailTemplateAttachment.AttachmentAssetId
            });

            builder.HasIndex(emailTemplateAttachment => emailTemplateAttachment.AttachmentAssetId);

            builder.HasOne<AttachmentAsset>()
                .WithMany()
                .HasForeignKey(emailTemplateAttachment => emailTemplateAttachment.AttachmentAssetId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureEmailDrafts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmailDraft>(builder =>
        {
            builder.ToTable("EmailDrafts");
            builder.HasKey(emailDraft => emailDraft.Id);

            builder.Property(emailDraft => emailDraft.Subject)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(emailDraft => emailDraft.Body)
                .HasMaxLength(20000)
                .IsRequired();

            builder.Property(emailDraft => emailDraft.MissingVariablesJson)
                .HasMaxLength(4000);

            builder.Property(emailDraft => emailDraft.UnknownVariablesJson)
                .HasMaxLength(4000);

            builder.Property(emailDraft => emailDraft.ApprovedAt);

            builder.Property(emailDraft => emailDraft.CancelledAt);

            builder.Property(emailDraft => emailDraft.SentAt);

            builder.Property(emailDraft => emailDraft.FailureReason)
                .HasMaxLength(4000);

            builder.HasIndex(emailDraft => emailDraft.ContactId);
            builder.HasIndex(emailDraft => emailDraft.OrganizationId);
            builder.HasIndex(emailDraft => emailDraft.TemplateId);
            builder.HasIndex(emailDraft => emailDraft.SenderProfileId);
            builder.HasIndex(emailDraft => emailDraft.Status);
            builder.HasIndex(emailDraft => emailDraft.CreatedAt);

            builder.HasOne<Contact>()
                .WithMany()
                .HasForeignKey(emailDraft => emailDraft.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(emailDraft => emailDraft.OrganizationId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne<EmailTemplate>()
                .WithMany()
                .HasForeignKey(emailDraft => emailDraft.TemplateId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne<SenderProfile>()
                .WithMany()
                .HasForeignKey(emailDraft => emailDraft.SenderProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(emailDraft => emailDraft.Attachments)
                .WithOne()
                .HasForeignKey(emailDraftAttachment => emailDraftAttachment.EmailDraftId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(emailDraft => emailDraft.Attachments)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });
    }

    private static void ConfigureEmailDraftAttachments(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmailDraftAttachment>(builder =>
        {
            builder.ToTable("EmailDraftAttachments");
            builder.HasKey(emailDraftAttachment => new
            {
                emailDraftAttachment.EmailDraftId,
                emailDraftAttachment.AttachmentAssetId
            });

            builder.HasIndex(emailDraftAttachment => emailDraftAttachment.AttachmentAssetId);

            builder.HasOne<AttachmentAsset>()
                .WithMany()
                .HasForeignKey(emailDraftAttachment => emailDraftAttachment.AttachmentAssetId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureEmailMessages(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmailMessage>(builder =>
        {
            builder.ToTable("EmailMessages");
            builder.HasKey(emailMessage => emailMessage.Id);

            builder.Property(emailMessage => emailMessage.To)
                .HasMaxLength(320)
                .IsRequired();

            builder.Property(emailMessage => emailMessage.Subject)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(emailMessage => emailMessage.Body)
                .HasMaxLength(20000)
                .IsRequired();

            builder.Property(emailMessage => emailMessage.Provider)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(emailMessage => emailMessage.ProviderMessageId)
                .HasMaxLength(500);

            builder.Property(emailMessage => emailMessage.FailureReason)
                .HasMaxLength(4000);

            builder.HasIndex(emailMessage => emailMessage.ContactId);
            builder.HasIndex(emailMessage => emailMessage.EmailDraftId);
            builder.HasIndex(emailMessage => emailMessage.Status);
            builder.HasIndex(emailMessage => emailMessage.CreatedAt);
            builder.HasIndex(emailMessage => new { emailMessage.ContactId, emailMessage.Subject, emailMessage.Status, emailMessage.CreatedAt });

            builder.HasOne<Contact>()
                .WithMany()
                .HasForeignKey(emailMessage => emailMessage.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(emailMessage => emailMessage.OrganizationId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne<EmailDraft>()
                .WithMany()
                .HasForeignKey(emailMessage => emailMessage.EmailDraftId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
