using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ampol.Domain.Enums;
using Ampol.Domain.Enums.GroupEventEnums;

namespace EntityBenchmark;

[Table("AspNetUsers")]
public class BenchmarkUser
{
    [Key] public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? FullName { get; set; }

    // Identity columns required by the table schema
    public string? UserName { get; set; }
    public string? NormalizedUserName { get; set; }
    public string? Email { get; set; }
    public string? NormalizedEmail { get; set; }
    public bool EmailConfirmed { get; set; }
    public string? PasswordHash { get; set; }
    public string? SecurityStamp { get; set; }
    public string? ConcurrencyStamp { get; set; }
    public string? PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public bool LockoutEnabled { get; set; }
    public int AccessFailedCount { get; set; }
}

[Table("UserEvents")]
public class BenchmarkUserEvent
{
    [Key] public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Color { get; set; } = "#FFFFFF";
    public DateTime DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
    public bool AllDay { get; set; }
    public int? ZipCode { get; set; }
    public Guid? CityId { get; set; }
    public DaysOfWeekGermanFlagEnum DaysOfWeekGermanFlagEnum { get; set; }
    public Guid UserId { get; set; }
    public BenchmarkUser? User { get; set; }
    public UserEventType Type { get; set; }
}

[Table("Participants")]
public class BenchmarkParticipant
{
    [Key] public Guid Id { get; set; }
    public string Firstname { get; set; } = null!;
    public string Lastname { get; set; } = null!;
    public Guid ProjectId { get; set; }
}
 
[Table("GroupEvents")]
public class BenchmarkGroupEvent
{
    [Key] public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string EventName { get; set; } = null!;
    public GroupEventType? GroupEventType { get; set; }
    public List<BenchmarkAppointment> Appointments { get; set; } = [];
}
 
[Table("AppointmentLocations")]
public class BenchmarkAppointmentLocation
{
    [Key] public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
 
[Table("Appointments")]
public class BenchmarkAppointment
{
    [Key] public Guid Id { get; set; }
    public Guid GroupEventId { get; set; }
    public Guid? LocationId { get; set; }
    public string Designation { get; set; } = null!;
    public DateTime Date { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
    public int DurationInMinutes { get; set; }
    public AppointmentType Type { get; set; }
    public AppointmentState AppointmentState { get; set; }
 
    public BenchmarkGroupEvent? GroupEvent { get; set; }
    public BenchmarkAppointmentLocation? Location { get; set; }
    public List<BenchmarkAppointmentUser> AppointmentUsers { get; set; } = [];
    public List<BenchmarkAppointmentParticipant> AppointmentParticipants { get; set; } = [];
}
 
[Table("AppointmentUsers")]
public class BenchmarkAppointmentUser
{
    [Key] public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid? UserId { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
    public int DurationInMinutes { get; set; }
    public Guid CostBearerProjectId { get; set; }
    public OverrideAppointmentUserState OverrideAppointmentUserState { get; set; }
 
    public BenchmarkUser? User { get; set; }
    public BenchmarkAppointment? Appointment { get; set; }
}
 
[Table("AppointmentParticipants")]
public class BenchmarkAppointmentParticipant
{
    [Key] public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid ParticipantId { get; set; }
    public AppointmentParticipantParticipationState AppointmentParticipantParticipationState { get; set; }
 
    public BenchmarkAppointment? Appointment { get; set; }
    public BenchmarkParticipant? Participant { get; set; }
}

[Table("Projects")]
public class BenchmarkProject
{
    [Key] public Guid Id { get; set; }
    public long Key { get; set; }
    public string Designation { get; set; } = null!;
    public string MeasureNumber { get; set; } = null!;
    public Guid? ContactPersonId { get; set; }
    public Guid? SecondaryContactPersonId { get; set; }
    public DateTime ProjectStart { get; set; }
    public DateTime? ProjectEnd { get; set; }
    public DateTime? EndOfContinuingCare { get; set; }
    public DateTime? RetentionObligation { get; set; }
    public bool AutomaticAnonymization { get; set; }
    public int? SustainabilityTargetNumber { get; set; }
    [MaxLength(20)] public string? ContractorCostObjectId { get; set; }
    public bool Anonymized { get; set; }
    public DateTime? AnonymizedOn { get; set; }
    public string? Street { get; set; }
    public string? ZipCode { get; set; }
    public string? Location { get; set; }
    public Guid? CountryId { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? VatNumber { get; set; }
    public Guid? ProjectMeasureId { get; set; }
    public int DaysBtwConsultationsLimit { get; set; }
    public int DaysBtwConsultationsAfterCareLimit { get; set; }
    public int DaysSinceF2FOrangeLimit { get; set; }
    public int DaysSinceF2FRedLimit { get; set; }
    public int DaysSinceF2FOrangeAfterCareLimit { get; set; }
    public int DaysSinceF2FRedAfterCareLimit { get; set; }
    public bool AllowTransfersFromExternalProjects { get; set; }
    public decimal AverageJobPlacementsPerMonth { get; set; }
    public Guid? FollowUpProjectId { get; set; }
    public Guid? AmsContingentId { get; set; }
    public Guid? ProjectCategoryContingentId { get; set; }
    public Guid? MunicipalId { get; set; }
    public Guid? DistrictId { get; set; }
    public Guid? FederalStateId { get; set; }
    public string? City { get; set; }
    public int ProjectCategory { get; set; }
}