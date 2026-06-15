using Microsoft.Extensions.Localization;
using OutreachFlow.Domain.ContactActivities;
using OutreachFlow.Domain.ContactImports;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.EmailDrafts;
using OutreachFlow.Domain.FollowUps;

namespace OutreachFlow.Web;

public static class SharedResourceDisplayNames
{
    public static string ContactStatus(IStringLocalizer<SharedResource> localizer, ContactStatus status)
        => localizer[$"ContactStatus.{status}"];

    public static string EmailDraftStatus(IStringLocalizer<SharedResource> localizer, EmailDraftStatus status)
        => localizer[$"EmailDraftStatus.{status}"];

    public static string FollowUpTaskType(IStringLocalizer<SharedResource> localizer, FollowUpTaskType type)
        => localizer[$"FollowUpTaskType.{type}"];

    public static string ImportJobStatus(IStringLocalizer<SharedResource> localizer, ImportJobStatus status)
        => localizer[$"ImportJobStatus.{status}"];

    public static string ContactActivityType(IStringLocalizer<SharedResource> localizer, ContactActivityType type)
        => localizer[$"ContactActivityType.{type}"];
}
