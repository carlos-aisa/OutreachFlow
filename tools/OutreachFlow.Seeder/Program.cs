using System.Text;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OutreachFlow.Application.Attachments;
using OutreachFlow.Application.Campaigns;
using OutreachFlow.Application.ContactGroups;
using OutreachFlow.Application.ContactImports;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.DependencyInjection;
using OutreachFlow.Application.EmailDrafts;
using OutreachFlow.Application.EmailTemplates;
using OutreachFlow.Application.FollowUps;
using OutreachFlow.Application.Organizations;
using OutreachFlow.Application.SenderProfiles;
using OutreachFlow.Application.Tags;
using OutreachFlow.Domain.Campaigns;
using OutreachFlow.Domain.ContactGroups;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.EmailDrafts;
using OutreachFlow.Domain.FollowUps;
using OutreachFlow.Domain.SenderProfiles;
using OutreachFlow.Infrastructure.DependencyInjection;
using OutreachFlow.Infrastructure.Persistence;

var solutionRoot = FindSolutionRoot(AppContext.BaseDirectory);
var apiProjectDir = Path.Combine(solutionRoot, "src", "OutreachFlow.Api");
var dbPath = Path.Combine(apiProjectDir, "outreachflow.development.db");
var attachmentStorageRoot = Path.Combine(apiProjectDir, "storage", "attachments");

Console.WriteLine($"Solution root: {solutionRoot}");
Console.WriteLine($"Database file: {dbPath}");

foreach (var suffix in new[] { "", "-shm", "-wal" })
{
    var file = dbPath + suffix;
    if (File.Exists(file))
    {
        File.Delete(file);
    }
}

if (Directory.Exists(attachmentStorageRoot))
{
    Directory.Delete(attachmentStorageRoot, recursive: true);
}

Directory.CreateDirectory(attachmentStorageRoot);

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["ConnectionStrings:OutreachFlow"] = $"Data Source={dbPath}",
    ["AttachmentStorage:RootPath"] = attachmentStorageRoot,
    ["EmailSending:Provider"] = "Fake",
    ["EmailSending:FakeFailureKeyword"] = "SEEDFAIL",
});
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

using var host = builder.Build();
using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;

var dbContext = services.GetRequiredService<OutreachFlowDbContext>();
await dbContext.Database.MigrateAsync();

var organizationService = services.GetRequiredService<IOrganizationService>();
var organizationTypeService = services.GetRequiredService<IOrganizationTypeService>();
var contactService = services.GetRequiredService<IContactService>();
var tagService = services.GetRequiredService<ITagService>();
var contactGroupService = services.GetRequiredService<IContactGroupService>();
var senderProfileService = services.GetRequiredService<ISenderProfileService>();
var emailTemplateService = services.GetRequiredService<IEmailTemplateService>();
var attachmentAssetService = services.GetRequiredService<IAttachmentAssetService>();
var emailDraftService = services.GetRequiredService<IEmailDraftService>();
var followUpTaskService = services.GetRequiredService<IFollowUpTaskService>();
var campaignService = services.GetRequiredService<ICampaignService>();
var campaignRecipientService = services.GetRequiredService<ICampaignRecipientService>();
var contactImportService = services.GetRequiredService<IContactImportService>();

var faker = new Faker("es");

var provinces = new[] { "Asturias", "León", "Madrid", "Barcelona", "Valencia", "Sevilla", "Vizcaya" };
var citiesByProvince = new Dictionary<string, string[]>
{
    ["Asturias"] = ["Oviedo", "Gijón", "Avilés"],
    ["León"] = ["León", "Ponferrada"],
    ["Madrid"] = ["Madrid", "Alcalá de Henares"],
    ["Barcelona"] = ["Barcelona", "Badalona"],
    ["Valencia"] = ["Valencia", "Gandía"],
    ["Sevilla"] = ["Sevilla", "Dos Hermanas"],
    ["Vizcaya"] = ["Bilbao", "Getxo"],
};
var orgTypes = new[] { "Colegio", "Universidad", "ONG", "Empresa", "Ayuntamiento", "Asociación" };
var roles = new[] { "Director/a", "Coordinador/a", "Responsable de Comunicación", "Secretario/a", "Gerente", null };
var sources = new[] { "Feria", "Web", "Referido", "LinkedIn", "Llamada en frío", null };

// ---- Tags ----
Console.WriteLine("Seeding tags...");
var tagSeeds = new (string Name, string? Category)[]
{
    ("VIP", "Audiencia"),
    ("Prospecto", "Audiencia"),
    ("Cliente", "Audiencia"),
    ("Alumni", "Audiencia"),
    ("Newsletter", "Comunicación"),
    ("Evento 2026", "Comunicación"),
    ("Feria Educativa", "Origen"),
    ("Referido", "Origen"),
    ("Decisor", "Rol"),
    ("Técnico", "Rol"),
    ("Urgente", "Prioridad"),
    ("Baja prioridad", "Prioridad"),
    ("Internacional", "Segmento"),
    ("Nacional", "Segmento"),
};
var tags = new List<TagDto>();
foreach (var (name, category) in tagSeeds)
{
    tags.Add(await tagService.CreateAsync(new CreateTagRequest(name, category)));
}
Console.WriteLine($"  {tags.Count} tags created.");

// ---- Organizations ----
Console.WriteLine("Seeding organization types...");
foreach (var type in orgTypes)
{
    await organizationTypeService.CreateAsync(new CreateOrganizationTypeRequest(type));
}
Console.WriteLine($"  {orgTypes.Length} organization types created.");

Console.WriteLine("Seeding organizations...");
const int OrganizationCount = 90;
var organizations = new List<(OrganizationDto Dto, string Province, string City, string Type)>();
for (var i = 0; i < OrganizationCount; i++)
{
    var province = faker.PickRandom(provinces);
    var city = faker.PickRandom(citiesByProvince[province]);
    var type = faker.PickRandom(orgTypes);
    var name = GenerateOrganizationName(faker, type, city);
    var website = faker.Random.Bool(0.7f) ? $"https://{Slugify(name)}.example.org" : null;
    var notes = faker.Random.Bool(0.3f) ? faker.Lorem.Sentence() : null;

    var dto = await organizationService.CreateAsync(new CreateOrganizationRequest(
        name, type, website, city, province, "España", notes));

    organizations.Add((dto, province, city, type));
}
Console.WriteLine($"  {organizations.Count} organizations created.");

// ---- Sender profiles ----
Console.WriteLine("Seeding sender profiles...");
var senderSeeds = new[]
{
    ("Laura Fernández", "laura.fernandez@outreachflow-demo.org", true),
    ("Equipo de Admisiones", "admisiones@outreachflow-demo.org", false),
    ("Marketing OutreachFlow", "marketing@outreachflow-demo.org", false),
    ("Carlos Ruiz", "carlos.ruiz@outreachflow-demo.org", false),
};
var senderProfiles = new List<SenderProfileDto>();
foreach (var (name, email, isDefault) in senderSeeds)
{
    var dto = await senderProfileService.CreateAsync(new CreateSenderProfileRequest(
        name,
        email,
        faker.Phone.PhoneNumber(),
        "OutreachFlow Demo",
        "https://outreachflow-demo.org",
        $"<p>Un saludo,<br>{name}<br>OutreachFlow Demo</p>",
        isDefault,
        SenderSignatureFormat.Html));
    senderProfiles.Add(dto);
}
Console.WriteLine($"  {senderProfiles.Count} sender profiles created.");

// ---- Attachment assets ----
Console.WriteLine("Seeding attachment assets...");
var attachmentSeeds = new[]
{
    ("Dossier institucional", "dossier.pdf", "application/pdf"),
    ("Catálogo de servicios", "catalogo.pdf", "application/pdf"),
    ("Folleto evento", "folleto-evento.pdf", "application/pdf"),
    ("Ficha técnica", "ficha-tecnica.pdf", "application/pdf"),
    ("Programa formativo", "programa.pdf", "application/pdf"),
    ("Presentación", "presentacion.pdf", "application/pdf"),
};
var attachments = new List<AttachmentAssetDto>();
foreach (var (name, fileName, contentType) in attachmentSeeds)
{
    var bytes = Encoding.UTF8.GetBytes($"Contenido de ejemplo para «{name}». Generado por el seeder de datos falsos de OutreachFlow.");
    await using var stream = new MemoryStream(bytes);
    var dto = await attachmentAssetService.UploadAsync(new UploadAttachmentAssetRequest(
        name, null, fileName, contentType, stream, bytes.Length));
    attachments.Add(dto);
}
Console.WriteLine($"  {attachments.Count} attachments created.");

// ---- Email templates ----
Console.WriteLine("Seeding email templates...");
var templateSeeds = new[]
{
    ("Primer contacto", "Una propuesta para {{contact.displayName}}",
        "Hola {{contact.displayName}},\n\nMe llamo {{sender.name}} y trabajo en {{sender.organizationName}}. Quería presentarte lo que hacemos y ver si encaja con lo que buscáis.\n\n{{sender.signature}}"),
    ("Seguimiento", "¿Seguimos en contacto?",
        "Hola {{contact.displayName}},\n\nTe escribo desde {{sender.organizationName}} para saber si sigues interesado/a en lo que hablamos.\n\n{{sender.name}}\n{{sender.phone}}"),
    ("Invitación a evento", "Te invitamos, {{contact.role}}",
        "Hola {{contact.displayName}} ({{contact.role}}),\n\nQueremos invitarte a nuestro próximo evento en {{organization.city}}.\n\n{{sender.signature}}"),
    ("Bienvenida", "Bienvenido/a, {{contact.displayName}}",
        "Hola {{contact.displayName}},\n\nGracias por tu interés. Adjunto encontrarás más información.\n\nUn saludo,\n{{sender.name}}"),
    ("Newsletter", "Novedades de {{sender.organizationName}}",
        "Hola {{contact.displayName}},\n\nEstas son las últimas novedades de {{sender.organizationName}}. Visítanos en {{sender.website}}.\n\n{{sender.signature}}"),
    ("Propuesta de colaboración", "Colaboremos, {{organization.name}}",
        "Hola {{contact.displayName}},\n\nDesde {{sender.organizationName}} nos gustaría explorar una colaboración con {{organization.name}}.\n\n{{sender.name}}"),
    ("Recordatorio", "Recordatorio para {{contact.displayName}}",
        "Hola {{contact.displayName}},\n\nSolo un recordatorio rápido de nuestra última conversación.\n\n{{sender.signature}}"),
    ("Prueba de fallos (seed)", "[SEEDFAIL] Mensaje de prueba",
        "Este envío está pensado para fallar en el seeder de datos falsos (SEEDFAIL) y así probar el estado de envío fallido.\n\n{{sender.signature}}"),
};
var templates = new List<EmailTemplateDto>();
foreach (var (name, subject, body) in templateSeeds)
{
    templates.Add(await emailTemplateService.CreateAsync(new CreateEmailTemplateRequest(name, null, subject, body)));
}
var failureTemplate = templates[^1];
var cleanTemplates = templates.Take(templates.Count - 1).ToArray();
Console.WriteLine($"  {templates.Count} email templates created.");

// ---- Contacts ----
Console.WriteLine("Seeding contacts...");
const int ContactCount = 700;
var emailDomains = new[] { "gmail.com", "outlook.com", "empresa-ejemplo.es", "colegio-ejemplo.es", "org-ejemplo.org" };
var statusWeights = new (ContactStatus Status, int Weight)[]
{
    (ContactStatus.New, 30),
    (ContactStatus.Contacted, 28),
    (ContactStatus.Replied, 15),
    (ContactStatus.MeetingScheduled, 6),
    (ContactStatus.NotInterested, 10),
    (ContactStatus.DoNotContact, 5),
    (ContactStatus.Archived, 6),
};
var weightedStatuses = statusWeights.SelectMany(pair => Enumerable.Repeat(pair.Status, pair.Weight)).ToArray();

var contacts = new List<(ContactDto Dto, string? Province, string? City, string? Type)>();
for (var i = 0; i < ContactCount; i++)
{
    var hasOrg = faker.Random.Bool(0.9f);
    var org = hasOrg ? faker.PickRandom(organizations) : default;
    var firstName = faker.Name.FirstName();
    var lastName = faker.Name.LastName();
    var displayName = $"{firstName} {lastName}";
    var email = $"{Slugify(firstName)}.{Slugify(lastName)}{i}@{faker.PickRandom(emailDomains)}";
    var status = faker.PickRandom(weightedStatuses);
    var doNotContact = status == ContactStatus.DoNotContact || faker.Random.Bool(0.03f);

    var dto = await contactService.CreateAsync(new CreateContactRequest(
        hasOrg ? org.Dto.Id : null,
        displayName,
        email,
        faker.Phone.PhoneNumber(),
        faker.PickRandom(roles),
        faker.PickRandom(sources),
        status,
        doNotContact));

    var tagCount = faker.Random.Int(0, 3);
    foreach (var tag in faker.PickRandom(tags, tagCount).ToArray())
    {
        await contactService.AssignTagAsync(dto.Id, tag.Id);
    }

    contacts.Add((dto, hasOrg ? org.Province : null, hasOrg ? org.City : null, hasOrg ? org.Type : null));

    if ((i + 1) % 100 == 0)
    {
        Console.WriteLine($"  {i + 1}/{ContactCount} contacts created...");
    }
}
Console.WriteLine($"  {contacts.Count} contacts created.");

// ---- Contact groups ----
Console.WriteLine("Seeding contact groups...");
var vipTag = tags.First(t => t.Name == "VIP");
var groupSeeds = new (string Name, ContactGroupCriterionRequest[] Criteria)[]
{
    ("Colegios de Asturias", [new(ContactGroupCriterionType.Province, "Asturias"), new(ContactGroupCriterionType.OrganizationType, "Colegio")]),
    ("Universidades", [new(ContactGroupCriterionType.OrganizationType, "Universidad")]),
    ("Contactos de Madrid", [new(ContactGroupCriterionType.Province, "Madrid")]),
    ("ONGs", [new(ContactGroupCriterionType.OrganizationType, "ONG")]),
    ("Empresas de Barcelona", [new(ContactGroupCriterionType.Province, "Barcelona"), new(ContactGroupCriterionType.OrganizationType, "Empresa")]),
    ("Contactos VIP", [new(ContactGroupCriterionType.Tag, vipTag.Id.ToString())]),
    ("Ayuntamientos", [new(ContactGroupCriterionType.OrganizationType, "Ayuntamiento")]),
    ("Todos los contactos", []),
};
var contactGroups = new List<ContactGroupDto>();
foreach (var (name, criteria) in groupSeeds)
{
    contactGroups.Add(await contactGroupService.CreateAsync(new CreateContactGroupRequest(name, criteria)));
}
Console.WriteLine($"  {contactGroups.Count} contact groups created.");

Console.WriteLine("Seeding a few membership overrides...");
var overrideCandidates = faker.PickRandom(contacts, 16).ToArray();
var overrideGroups = new[] { contactGroups[0], contactGroups[5] };
for (var i = 0; i < overrideCandidates.Length; i++)
{
    var group = overrideGroups[i % overrideGroups.Length];
    var overrideType = faker.Random.Bool(0.7f) ? ContactGroupOverrideType.Include : ContactGroupOverrideType.Exclude;
    try
    {
        await contactGroupService.SetOverrideAsync(group.Id, overrideCandidates[i].Dto.Id, overrideType);
    }
    catch (Exception exception)
    {
        Console.WriteLine($"  Skipped override: {exception.Message}");
    }
}

// ---- Campaigns + recipient lifecycle ----
Console.WriteLine("Seeding campaigns...");
var campaignSeeds = new[]
{
    ("Colegios Asturias — Otoño 2026", "Captación de colegios en Asturias para el nuevo curso.", contactGroups[0], cleanTemplates[0], true, 7, FollowUpTaskType.Email, false),
    ("Universidades — Alianzas", "Propuesta de colaboración con universidades.", contactGroups[1], cleanTemplates[5], false, 0, FollowUpTaskType.Email, false),
    ("Seguimiento clientes potenciales", "Seguimiento de contactos de Madrid, incluye una tanda de prueba de fallos.", contactGroups[2], failureTemplate, true, 5, FollowUpTaskType.Call, true),
    ("ONGs — Colaboración", "Propuesta de colaboración con ONGs.", contactGroups[3], cleanTemplates[3], false, 0, FollowUpTaskType.Email, false),
    ("Empresas Barcelona — Patrocinio", "Búsqueda de patrocinadores en Barcelona.", contactGroups[4], cleanTemplates[2], true, 10, FollowUpTaskType.Meeting, false),
    ("Newsletter VIP", "Envío de novedades a contactos VIP.", contactGroups[5], cleanTemplates[4], false, 0, FollowUpTaskType.Email, false),
};

var closedCampaignNames = new HashSet<string> { "Universidades — Alianzas", "ONGs — Colaboración" };

foreach (var (name, description, group, template, followUpEnabled, followUpDueDays, followUpType, useFailureTemplate) in campaignSeeds)
{
    Console.WriteLine($"  Campaign: {name}");
    var campaign = await campaignService.CreateAsync(new CreateCampaignRequest(
        name, description, template.Id, [group.Id], followUpEnabled, Math.Max(followUpDueDays, 1), followUpType));

    var candidates = await campaignRecipientService.DiscoverCandidatesAsync(campaign.Id);
    var toIncorporate = faker.Random.Bool(0.01f) ? candidates : faker.PickRandom(candidates, (int)(candidates.Count * 0.75)).ToArray();

    foreach (var candidate in toIncorporate)
    {
        try
        {
            await campaignRecipientService.IncorporateAsync(campaign.Id, candidate.ContactId);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"    Skipped incorporate: {exception.Message}");
        }
    }

    var senderProfile = faker.PickRandom(senderProfiles);
    var attachmentIds = faker.Random.Bool(0.4f)
        ? faker.PickRandom(attachments, faker.Random.Int(1, 2)).Select(a => a.Id).ToArray()
        : null;

    await campaignRecipientService.GenerateDraftsAsync(campaign.Id, new GenerateCampaignDraftsRequest(senderProfile.Id, attachmentIds));

    var recipients = await campaignRecipientService.ListAsync(campaign.Id);
    var draftedRecipients = recipients.Where(r => r.Status == CampaignRecipientStatus.Drafted && r.EmailDraftId is not null).ToArray();

    foreach (var recipient in draftedRecipients)
    {
        if (!faker.Random.Bool(0.75f))
        {
            continue;
        }

        try
        {
            var draft = await emailDraftService.GetByIdAsync(recipient.EmailDraftId!.Value);
            if (draft is null || draft.Status == EmailDraftStatus.NeedsReview)
            {
                continue;
            }

            if (draft.Status == EmailDraftStatus.Draft)
            {
                draft = await emailDraftService.ApproveAsync(draft.Id);
            }

            if (draft.Status == EmailDraftStatus.Approved)
            {
                await emailDraftService.SendApprovedDraftAsync(draft.Id);
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine($"    Skipped send: {exception.Message}");
        }
    }

    if (closedCampaignNames.Contains(name))
    {
        await campaignService.CloseAsync(campaign.Id);
    }
}
Console.WriteLine("  Campaigns seeded.");

// ---- Standalone (non-campaign) email drafts ----
Console.WriteLine("Seeding standalone email drafts...");
foreach (var status in new[] { ContactStatus.New, ContactStatus.Contacted })
{
    var template = faker.PickRandom(cleanTemplates);
    var senderProfile = faker.PickRandom(senderProfiles);

    try
    {
        var result = await emailDraftService.GenerateAsync(new GenerateEmailDraftsRequest(
            Search: null,
            TagId: null,
            Status: status,
            DoNotContact: false,
            OrganizationId: null,
            LastContactedFrom: null,
            LastContactedTo: null,
            TemplateId: template.Id,
            SenderProfileId: senderProfile.Id,
            AttachmentAssetIds: null));

        Console.WriteLine($"  Generated {result.GeneratedDrafts} standalone drafts for status {status} (skipped {result.SkippedContacts}).");

        foreach (var draft in result.Drafts.Where(d => d.Status == EmailDraftStatus.Draft).Take(result.Drafts.Count / 3))
        {
            var approved = await emailDraftService.ApproveAsync(draft.Id);
            if (faker.Random.Bool(0.6f))
            {
                await emailDraftService.SendApprovedDraftAsync(approved.Id);
            }
        }

        foreach (var draft in result.Drafts.Where(d => d.Status == EmailDraftStatus.Draft).Skip(result.Drafts.Count * 2 / 3))
        {
            await emailDraftService.CancelAsync(draft.Id);
        }
    }
    catch (Exception exception)
    {
        Console.WriteLine($"  Skipped standalone draft batch: {exception.Message}");
    }
}

// ---- Follow-up tasks ----
Console.WriteLine("Seeding follow-up tasks...");
const int FollowUpCount = 400;
var followUpTypes = Enum.GetValues<FollowUpTaskType>();
var createdFollowUps = 0;
for (var i = 0; i < FollowUpCount; i++)
{
    var contact = faker.PickRandom(contacts);
    var dueOffsetDays = faker.Random.Int(-30, 45);
    var dueAt = DateTimeOffset.UtcNow.Date.AddDays(dueOffsetDays).AddHours(faker.Random.Int(8, 18));
    var type = faker.PickRandom(followUpTypes);
    var notes = faker.Random.Bool(0.5f) ? faker.Lorem.Sentence() : null;

    try
    {
        var task = await followUpTaskService.CreateAsync(new CreateFollowUpTaskRequest(
            contact.Dto.Id, null, dueAt, type, notes));

        var shouldComplete = dueOffsetDays < 0 ? faker.Random.Bool(0.55f) : faker.Random.Bool(0.1f);
        if (shouldComplete)
        {
            await followUpTaskService.CompleteAsync(task.Id);
        }

        createdFollowUps++;
    }
    catch (Exception exception)
    {
        Console.WriteLine($"  Skipped follow-up: {exception.Message}");
    }

    if ((i + 1) % 100 == 0)
    {
        Console.WriteLine($"  {i + 1}/{FollowUpCount} follow-up tasks processed...");
    }
}
Console.WriteLine($"  {createdFollowUps} follow-up tasks created.");

// ---- Contact imports (also creates additional contacts) ----
Console.WriteLine("Seeding contact import history...");
for (var batch = 0; batch < 3; batch++)
{
    var rows = new List<string> { "displayname,email,phone,role,source" };

    for (var i = 0; i < 25; i++)
    {
        var firstName = faker.Name.FirstName();
        var lastName = faker.Name.LastName();
        var email = $"{Slugify(firstName)}.{Slugify(lastName)}{batch}{i}@import-ejemplo.es";
        rows.Add($"{firstName} {lastName},{email},{faker.Phone.PhoneNumber()},{faker.PickRandom(roles)},Importación CSV");
    }

    // Intra-file duplicate.
    rows.Add(rows[1]);

    // Duplicate of an already-existing contact.
    rows.Add($"Contacto Duplicado,{contacts[batch].Dto.Email},,,Importación CSV");

    // Invalid row: missing display name.
    rows.Add(",sin-nombre@import-ejemplo.es,,,");

    var csvContent = string.Join('\n', rows);

    try
    {
        var result = await contactImportService.CommitAsync(new ContactImportCommitRequest(
            $"contactos-lote-{batch + 1}.csv", csvContent, [tags[1].Id]));
        Console.WriteLine($"  Import batch {batch + 1}: {result.CreatedCount} created, {result.DuplicateCount} duplicates, {result.InvalidCount} invalid.");
    }
    catch (Exception exception)
    {
        Console.WriteLine($"  Skipped import batch {batch + 1}: {exception.Message}");
    }
}

Console.WriteLine();
Console.WriteLine("Row counts:");
Console.WriteLine($"  Organizations: {await dbContext.Organizations.CountAsync()}");
Console.WriteLine($"  OrganizationTypes: {await dbContext.OrganizationTypes.CountAsync()}");
Console.WriteLine($"  Contacts: {await dbContext.Contacts.CountAsync()}");
Console.WriteLine($"  Tags: {await dbContext.Tags.CountAsync()}");
Console.WriteLine($"  ContactTags: {await dbContext.ContactTags.CountAsync()}");
Console.WriteLine($"  ContactActivities: {await dbContext.ContactActivities.CountAsync()}");
Console.WriteLine($"  SenderProfiles: {await dbContext.SenderProfiles.CountAsync()}");
Console.WriteLine($"  EmailTemplates: {await dbContext.EmailTemplates.CountAsync()}");
Console.WriteLine($"  AttachmentAssets: {await dbContext.AttachmentAssets.CountAsync()}");
Console.WriteLine($"  EmailDrafts: {await dbContext.EmailDrafts.CountAsync()}");
Console.WriteLine($"  EmailMessages: {await dbContext.EmailMessages.CountAsync()}");
Console.WriteLine($"  FollowUpTasks: {await dbContext.FollowUpTasks.CountAsync()}");
Console.WriteLine($"  ImportJobs: {await dbContext.ImportJobs.CountAsync()}");
Console.WriteLine($"  ContactGroups: {await dbContext.ContactGroups.CountAsync()}");
Console.WriteLine($"  ContactGroupCriteria: {await dbContext.ContactGroupCriteria.CountAsync()}");
Console.WriteLine($"  ContactGroupMembershipOverrides: {await dbContext.ContactGroupMembershipOverrides.CountAsync()}");
Console.WriteLine($"  Campaigns: {await dbContext.Campaigns.CountAsync()}");
Console.WriteLine($"  CampaignAudienceGroups: {await dbContext.CampaignAudienceGroups.CountAsync()}");
Console.WriteLine($"  CampaignRecipients: {await dbContext.CampaignRecipients.CountAsync()}");
Console.WriteLine();
Console.WriteLine("Seeding complete.");

static string Slugify(string value)
{
    var normalized = value.Trim().ToLowerInvariant()
        .Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u')
        .Replace('ñ', 'n').Replace(' ', '-');

    var builder = new StringBuilder();
    foreach (var character in normalized)
    {
        if (char.IsLetterOrDigit(character) || character == '-')
        {
            builder.Append(character);
        }
    }

    return builder.ToString();
}

static string GenerateOrganizationName(Faker faker, string type, string city)
{
    return type switch
    {
        "Colegio" => $"Colegio {faker.Name.LastName()}",
        "Universidad" => $"Universidad de {city}",
        "ONG" => $"Fundación {faker.Name.LastName()}",
        "Empresa" => faker.Company.CompanyName(),
        "Ayuntamiento" => $"Ayuntamiento de {city}",
        "Asociación" => $"Asociación {faker.Name.LastName()}",
        _ => faker.Company.CompanyName(),
    };
}

static string FindSolutionRoot(string startDirectory)
{
    var directory = new DirectoryInfo(startDirectory);

    while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "OutreachFlow.sln")))
    {
        directory = directory.Parent;
    }

    return directory?.FullName
        ?? throw new InvalidOperationException("Could not locate OutreachFlow.sln from the current base directory.");
}
