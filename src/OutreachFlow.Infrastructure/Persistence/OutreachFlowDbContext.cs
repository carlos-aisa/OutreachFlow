using Microsoft.EntityFrameworkCore;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.Organizations;
using OutreachFlow.Domain.Tags;

namespace OutreachFlow.Infrastructure.Persistence;

public sealed class OutreachFlowDbContext(DbContextOptions<OutreachFlowDbContext> options)
    : DbContext(options)
{
    public DbSet<Contact> Contacts => Set<Contact>();

    public DbSet<Organization> Organizations => Set<Organization>();

    public DbSet<Tag> Tags => Set<Tag>();

    public DbSet<ContactTag> ContactTags => Set<ContactTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureOrganizations(modelBuilder);
        ConfigureContacts(modelBuilder);
        ConfigureTags(modelBuilder);
        ConfigureContactTags(modelBuilder);
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
}
