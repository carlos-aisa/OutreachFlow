using FluentAssertions;
using OutreachFlow.Application.Common;
using OutreachFlow.Application.ContactGroups;
using OutreachFlow.Application.Tests.Support;
using OutreachFlow.Domain.ContactGroups;

namespace OutreachFlow.Application.Tests.ContactGroups;

public sealed class ContactGroupServiceTests
{
    [Fact]
    public async Task ShouldApplyOrWithinCriteriaAndAndBetweenCriteria()
    {
        var repository = new FakeContactGroupRepository
        {
            Contacts =
            [
                new(Guid.NewGuid(), "Asturias", "Gijón", "School", new HashSet<Guid> { Guid.Parse("11111111-1111-1111-1111-111111111111") }),
                new(Guid.NewGuid(), "Asturias", "Gijón", "School", new HashSet<Guid> { Guid.Parse("22222222-2222-2222-2222-222222222222") }),
                new(Guid.NewGuid(), "Asturias", "Avilés", "School", new HashSet<Guid> { Guid.Parse("11111111-1111-1111-1111-111111111111") })
            ]
        };
        var service = new ContactGroupService(repository, new InMemoryUnitOfWork());
        var group = await service.CreateAsync(new CreateContactGroupRequest("Asturias schools", [
            new(ContactGroupCriterionType.City, "Oviedo"), new(ContactGroupCriterionType.City, "Gijón"),
            new(ContactGroupCriterionType.Tag, "11111111-1111-1111-1111-111111111111") ]));

        var members = await service.ListMembersAsync(group.Id);

        members.Should().ContainSingle().Which.ContactId.Should().Be(repository.Contacts[0].ContactId);
    }

    [Fact]
    public async Task ShouldApplyManualIncludeAndExcludeOverrides()
    {
        var included = Guid.NewGuid(); var excluded = Guid.NewGuid();
        var repository = new FakeContactGroupRepository { Contacts = [new(included, null, null, null, new HashSet<Guid>()), new(excluded, null, null, null, new HashSet<Guid>())] };
        var service = new ContactGroupService(repository, new InMemoryUnitOfWork());
        var group = await service.CreateAsync(new CreateContactGroupRequest("Manual", []));
        await service.SetOverrideAsync(group.Id, excluded, ContactGroupOverrideType.Exclude);
        await service.SetOverrideAsync(group.Id, included, ContactGroupOverrideType.Include);

        var members = await service.ListMembersAsync(group.Id);

        members.Should().ContainSingle().Which.ContactId.Should().Be(included);
    }

    [Fact]
    public async Task ShouldClassifyMembershipStatusForEveryContact()
    {
        var criteriaMatch = Guid.NewGuid();
        var manuallyIncluded = Guid.NewGuid();
        var manuallyExcluded = Guid.NewGuid();
        var notAMember = Guid.NewGuid();
        var repository = new FakeContactGroupRepository
        {
            Contacts =
            [
                new(criteriaMatch, "Asturias", null, null, new HashSet<Guid>()),
                new(manuallyIncluded, null, null, null, new HashSet<Guid>()),
                new(manuallyExcluded, "Asturias", null, null, new HashSet<Guid>()),
                new(notAMember, null, null, null, new HashSet<Guid>())
            ]
        };
        var service = new ContactGroupService(repository, new InMemoryUnitOfWork());
        var group = await service.CreateAsync(new CreateContactGroupRequest(
            "Asturias", [new(ContactGroupCriterionType.Province, "Asturias")]));
        await service.SetOverrideAsync(group.Id, manuallyIncluded, ContactGroupOverrideType.Include);
        await service.SetOverrideAsync(group.Id, manuallyExcluded, ContactGroupOverrideType.Exclude);

        var statuses = (await service.ListMembershipStatusAsync(group.Id)).ToDictionary(status => status.ContactId, status => status.Status);

        statuses[criteriaMatch].Should().Be(ContactGroupMembershipStatus.MemberByCriteria);
        statuses[manuallyIncluded].Should().Be(ContactGroupMembershipStatus.MemberByManualInclusion);
        statuses[manuallyExcluded].Should().Be(ContactGroupMembershipStatus.ExcludedManually);
        statuses[notAMember].Should().Be(ContactGroupMembershipStatus.NotAMember);
    }

    [Fact]
    public async Task ShouldRevertToCriteriaBasedStatusWhenOverrideIsCleared()
    {
        var contactId = Guid.NewGuid();
        var repository = new FakeContactGroupRepository { Contacts = [new(contactId, "Madrid", null, null, new HashSet<Guid>())] };
        var service = new ContactGroupService(repository, new InMemoryUnitOfWork());
        var group = await service.CreateAsync(new CreateContactGroupRequest(
            "Asturias", [new(ContactGroupCriterionType.Province, "Asturias")]));
        await service.SetOverrideAsync(group.Id, contactId, ContactGroupOverrideType.Include);

        await service.ClearOverrideAsync(group.Id, contactId);

        var statuses = await service.ListMembershipStatusAsync(group.Id);
        statuses.Should().ContainSingle().Which.Status.Should().Be(ContactGroupMembershipStatus.NotAMember);
    }

    private sealed class FakeContactGroupRepository : IContactGroupRepository
    {
        public List<ContactGroup> Groups { get; } = []; public List<ContactGroupCriterion> Criteria { get; } = []; public List<ContactGroupMembershipOverride> Overrides { get; } = []; public IReadOnlyList<ContactGroupEvaluationContact> Contacts { get; set; } = [];
        public Task AddAsync(ContactGroup group, CancellationToken cancellationToken = default) { Groups.Add(group); return Task.CompletedTask; }
        public Task<ContactGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(Groups.SingleOrDefault(x => x.Id == id));
        public Task<IReadOnlyList<ContactGroup>> ListAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<ContactGroup>>(Groups);
        public Task<IReadOnlyList<ContactGroupCriterion>> ListCriteriaAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<ContactGroupCriterion>>(Criteria.Where(x => x.ContactGroupId == id).ToArray());
        public Task<IReadOnlyList<ContactGroupMembershipOverride>> ListOverridesAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<ContactGroupMembershipOverride>>(Overrides.Where(x => x.ContactGroupId == id).ToArray());
        public Task<IReadOnlyList<ContactGroupEvaluationContact>> ListEvaluationContactsAsync(CancellationToken cancellationToken = default) => Task.FromResult(Contacts);
        public Task AddCriterionAsync(ContactGroupCriterion item, CancellationToken cancellationToken = default) { Criteria.Add(item); return Task.CompletedTask; }
        public Task ReplaceCriteriaAsync(Guid id, IReadOnlyList<ContactGroupCriterion> items, CancellationToken cancellationToken = default) { Criteria.RemoveAll(item => item.ContactGroupId == id); Criteria.AddRange(items); return Task.CompletedTask; }
        public Task UpsertOverrideAsync(ContactGroupMembershipOverride item, CancellationToken cancellationToken = default) { Overrides.RemoveAll(x => x.ContactGroupId == item.ContactGroupId && x.ContactId == item.ContactId); Overrides.Add(item); return Task.CompletedTask; }
        public Task RemoveOverrideAsync(Guid contactGroupId, Guid contactId, CancellationToken cancellationToken = default) { Overrides.RemoveAll(x => x.ContactGroupId == contactGroupId && x.ContactId == contactId); return Task.CompletedTask; }
        public void Remove(ContactGroup contactGroup) => Groups.Remove(contactGroup);
    }
}
