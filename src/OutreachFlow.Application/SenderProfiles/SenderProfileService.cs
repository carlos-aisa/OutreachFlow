using OutreachFlow.Application.Common;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.SenderProfiles;

namespace OutreachFlow.Application.SenderProfiles;

public sealed class SenderProfileService(
    ISenderProfileRepository senderProfileRepository,
    IUnitOfWork unitOfWork)
    : ISenderProfileService
{
    public async Task<SenderProfileDto> CreateAsync(
        CreateSenderProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var senderProfile = CreateSenderProfile(request);
        var now = DateTimeOffset.UtcNow;

        if (request.IsDefault)
        {
            await ClearDefaultProfilesAsync(null, now, cancellationToken);
        }

        await senderProfileRepository.AddAsync(senderProfile, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(senderProfile);
    }

    public async Task<SenderProfileDto> UpdateAsync(
        Guid id,
        UpdateSenderProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var senderProfile = await senderProfileRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApplicationNotFoundException("Sender profile was not found.");

        var now = DateTimeOffset.UtcNow;

        try
        {
            senderProfile.Update(
                request.Name,
                request.Email,
                request.Phone,
                request.OrganizationName,
                request.Website,
                request.Signature,
                request.SignatureFormat,
                now);

            if (request.IsActive)
            {
                senderProfile.Activate(now);
            }
            else
            {
                senderProfile.Deactivate(now);
            }

            if (request.IsDefault)
            {
                await ClearDefaultProfilesAsync(senderProfile.Id, now, cancellationToken);
                senderProfile.MarkDefault(now);
            }
            else
            {
                senderProfile.ClearDefault(now);
            }
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(senderProfile);
    }

    public async Task<SenderProfileDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var senderProfile = await senderProfileRepository.GetByIdAsync(id, cancellationToken);
        return senderProfile is null ? null : Map(senderProfile);
    }

    public async Task<SenderProfileDto?> GetDefaultAsync(CancellationToken cancellationToken = default)
    {
        var senderProfile = await senderProfileRepository.GetDefaultAsync(cancellationToken);
        return senderProfile is null ? null : Map(senderProfile);
    }

    public async Task<IReadOnlyList<SenderProfileDto>> ListAsync(
        bool? activeOnly,
        CancellationToken cancellationToken = default)
    {
        var senderProfiles = await senderProfileRepository.ListAsync(activeOnly, cancellationToken);
        return senderProfiles.Select(Map).ToArray();
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var senderProfile = await senderProfileRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApplicationNotFoundException("Sender profile was not found.");

        senderProfile.Deactivate(DateTimeOffset.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ClearDefaultProfilesAsync(
        Guid? exceptSenderProfileId,
        DateTimeOffset updatedAt,
        CancellationToken cancellationToken)
    {
        var senderProfiles = await senderProfileRepository.ListAsync(activeOnly: null, cancellationToken);

        foreach (var senderProfile in senderProfiles.Where(senderProfile =>
            senderProfile.IsDefault && senderProfile.Id != exceptSenderProfileId))
        {
            senderProfile.ClearDefault(updatedAt);
        }
    }

    private static SenderProfile CreateSenderProfile(CreateSenderProfileRequest request)
    {
        try
        {
            return new SenderProfile(
                request.Name,
                request.Email,
                request.Phone,
                request.OrganizationName,
                request.Website,
                request.Signature,
                request.SignatureFormat,
                request.IsDefault);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }
    }

    private static SenderProfileDto Map(SenderProfile senderProfile)
    {
        return new SenderProfileDto(
            senderProfile.Id,
            senderProfile.Name,
            senderProfile.Email,
            senderProfile.Phone,
            senderProfile.OrganizationName,
            senderProfile.Website,
            senderProfile.Signature,
            senderProfile.SignatureFormat,
            senderProfile.IsDefault,
            senderProfile.IsActive,
            senderProfile.CreatedAt,
            senderProfile.UpdatedAt);
    }
}
