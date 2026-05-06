using OutreachFlow.Application.Common;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.ContactActivities;
using OutreachFlow.Application.Organizations;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.ContactActivities;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.FollowUps;

namespace OutreachFlow.Application.FollowUps;

public sealed class FollowUpTaskService(
    IFollowUpTaskRepository followUpTaskRepository,
    IContactRepository contactRepository,
    IOrganizationRepository organizationRepository,
    IContactActivityService contactActivityService,
    IUnitOfWork unitOfWork) : IFollowUpTaskService
{
    public async Task<FollowUpTaskDto> CreateAsync(
        CreateFollowUpTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var contact = await contactRepository.GetByIdAsync(request.ContactId, cancellationToken)
            ?? throw new ApplicationNotFoundException("Contact was not found.");

        await EnsureOrganizationExistsAsync(request.OrganizationId, cancellationToken);

        FollowUpTask task;

        try
        {
            task = new FollowUpTask(
                request.ContactId,
                request.OrganizationId,
                request.DueAt,
                request.Type,
                request.Notes);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        await followUpTaskRepository.AddAsync(task, cancellationToken);
        await contactActivityService.RecordAsync(new CreateContactActivityRequest(
            task.ContactId,
            task.OrganizationId,
            ContactActivityType.FollowUpCreated,
            Subject: "Follow-up created",
            BodyPreview: task.Notes,
            MetadataJson: null,
            task.CreatedAt),
            cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(task, contact);
    }

    public async Task<FollowUpTaskDto> UpdateAsync(
        Guid id,
        UpdateFollowUpTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var task = await followUpTaskRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApplicationNotFoundException("Follow-up task was not found.");
        var contact = await contactRepository.GetByIdAsync(task.ContactId, cancellationToken)
            ?? throw new ApplicationNotFoundException("Contact was not found.");

        try
        {
            task.Update(
                request.DueAt,
                request.Type,
                request.Notes,
                DateTimeOffset.UtcNow);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(task, contact);
    }

    public async Task<FollowUpTaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await followUpTaskRepository.GetByIdAsync(id, cancellationToken);

        if (task is null)
        {
            return null;
        }

        var contact = await contactRepository.GetByIdAsync(task.ContactId, cancellationToken)
            ?? throw new ApplicationNotFoundException("Contact was not found.");

        return Map(task, contact);
    }

    public async Task<IReadOnlyList<FollowUpTaskDto>> ListAsync(
        FollowUpTaskFilterRequest filter,
        CancellationToken cancellationToken = default)
    {
        var tasks = await followUpTaskRepository.ListAsync(filter, cancellationToken);
        var result = new List<FollowUpTaskDto>(tasks.Count);

        foreach (var task in tasks)
        {
            var contact = await contactRepository.GetByIdAsync(task.ContactId, cancellationToken)
                ?? throw new ApplicationNotFoundException("Contact was not found.");
            result.Add(Map(task, contact));
        }

        return result;
    }

    public async Task<FollowUpTaskDto> CompleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var task = await followUpTaskRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApplicationNotFoundException("Follow-up task was not found.");
        var contact = await contactRepository.GetByIdAsync(task.ContactId, cancellationToken)
            ?? throw new ApplicationNotFoundException("Contact was not found.");

        try
        {
            task.Complete(DateTimeOffset.UtcNow);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        await contactActivityService.RecordAsync(new CreateContactActivityRequest(
            task.ContactId,
            task.OrganizationId,
            ContactActivityType.FollowUpCompleted,
            Subject: "Follow-up completed",
            BodyPreview: task.Notes,
            MetadataJson: null,
            task.CompletedAt),
            cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(task, contact);
    }

    private async Task EnsureOrganizationExistsAsync(
        Guid? organizationId,
        CancellationToken cancellationToken)
    {
        if (organizationId is null)
        {
            return;
        }

        _ = await organizationRepository.GetByIdAsync(organizationId.Value, cancellationToken)
            ?? throw new ApplicationNotFoundException("Organization was not found.");
    }

    private static FollowUpTaskDto Map(FollowUpTask task, Contact contact)
    {
        return new FollowUpTaskDto(
            task.Id,
            task.ContactId,
            contact.DisplayName,
            contact.Email,
            task.OrganizationId,
            task.DueAt,
            task.Type,
            task.Notes,
            task.IsCompleted,
            task.CompletedAt,
            task.CreatedAt,
            task.UpdatedAt);
    }
}
