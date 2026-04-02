using Ampol.Domain.Enums;
using Ampol.Domain.Enums.GroupEventEnums;

namespace EntityBenchmark;

public static class AppointmentDataSeeder
{
    private static readonly GroupEventType[] GroupEventTypes = [GroupEventType.GroupConsultation, GroupEventType.Workshop, GroupEventType.OpenWorkSpace];

    private static readonly AppointmentType[] AppointmentTypes = [AppointmentType.Local, AppointmentType.Online, AppointmentType.Hybrid];

    public static Guid ProjectId { get; } = Guid.NewGuid();

    public static List<BenchmarkAppointmentLocation> CreateLocations() =>
    [
        new() { Id = Guid.NewGuid(), Name = "Buero Wien" },
        new() { Id = Guid.NewGuid(), Name = "Standort Graz" },
        new() { Id = Guid.NewGuid(), Name = "Schulungszentrum Linz" },
        new() { Id = Guid.NewGuid(), Name = "Besprechungsraum Salzburg" },
        new() { Id = Guid.NewGuid(), Name = "Online" },
    ];

    public static List<BenchmarkParticipant> CreateParticipants(int count) =>
        Enumerable.Range(0, count).Select(i => new BenchmarkParticipant
        {
            Id = Guid.NewGuid(),
            Firstname = $"Teilnehmer{i}",
            Lastname = $"Nachname{i}",
            ProjectId = ProjectId,
        }).ToList();

    public static List<BenchmarkGroupEvent> CreateGroupEvents(int count)
    {
        var rng = new Random(100);
        return Enumerable.Range(0, count).Select(i => new BenchmarkGroupEvent
        {
            Id = Guid.NewGuid(),
            ProjectId = ProjectId,
            EventName = $"Veranstaltung {i}",
            GroupEventType = GroupEventTypes[rng.Next(GroupEventTypes.Length)],
        }).ToList();
    }

    /// <summary>
    /// Creates appointments spread across 90 days, each linked to a random group event and optionally a location.
    /// </summary>
    public static List<BenchmarkAppointment> CreateAppointments(
        List<BenchmarkGroupEvent> groupEvents,
        List<BenchmarkAppointmentLocation> locations,
        int perGroupEvent,
        DateTime rangeStart, int rangeDays = 90)
    {
        var rng = new Random(200);

        return groupEvents.SelectMany(ge =>
            Enumerable.Range(0, perGroupEvent).Select(_ =>
            {
                var date = rangeStart.AddDays(rng.Next(0, rangeDays)).Date;
                var startHour = rng.Next(8, 15);
                var duration = rng.Next(1, 4);
                var hasLocation = rng.NextDouble() < 0.7;

                return new BenchmarkAppointment
                {
                    Id = Guid.NewGuid(),
                    GroupEventId = ge.Id,
                    LocationId = hasLocation ? locations[rng.Next(locations.Count)].Id : null,
                    Designation = $"Termin {ge.EventName}",
                    Date = date,
                    TimeFrom = date.AddHours(startHour),
                    TimeTo = date.AddHours(startHour + duration),
                    DurationInMinutes = duration * 60,
                    Type = AppointmentTypes[rng.Next(AppointmentTypes.Length)],
                    AppointmentState = rng.NextDouble() < 0.6
                        ? AppointmentState.Done
                        : AppointmentState.Scheduled,
                };
            })).ToList();
    }

    /// <summary>
    /// Assigns 1-3 trainers per appointment from the user pool.
    /// This is the collection that Include(AppointmentUsers).ThenInclude(User) loads entirely.
    /// </summary>
    public static List<BenchmarkAppointmentUser> CreateAppointmentUsers(
        List<BenchmarkAppointment> appointments, List<BenchmarkUser> users)
    {
        var rng = new Random(300);

        return appointments.SelectMany(apt =>
        {
            var trainerCount = rng.Next(1, 4); // 1-3 trainers
            return Enumerable.Range(0, trainerCount).Select(_ =>
            {
                var user = users[rng.Next(users.Count)];
                return new BenchmarkAppointmentUser
                {
                    Id = Guid.NewGuid(),
                    AppointmentId = apt.Id,
                    UserId = user.Id,
                    TimeFrom = apt.TimeFrom,
                    TimeTo = apt.TimeTo,
                    DurationInMinutes = apt.DurationInMinutes,
                    CostBearerProjectId = ProjectId,
                    OverrideAppointmentUserState = OverrideAppointmentUserState.NotSet,
                };
            });
        }).ToList();
    }

    /// <summary>
    /// Assigns each participant to ~20 random appointments.
    /// This is the root table of the query.
    /// </summary>
    public static List<BenchmarkAppointmentParticipant> CreateAppointmentParticipants(
        List<BenchmarkParticipant> participants,
        List<BenchmarkAppointment> appointments,
        int assignmentsPerParticipant = 20)
    {
        var rng = new Random(400);

        return participants.SelectMany(p =>
            appointments.OrderBy(_ => rng.Next())
                .Take(assignmentsPerParticipant)
                .Select(apt => new BenchmarkAppointmentParticipant
                {
                    Id = Guid.NewGuid(),
                    AppointmentId = apt.Id,
                    ParticipantId = p.Id,
                    AppointmentParticipantParticipationState = rng.NextDouble() < 0.7
                        ? AppointmentParticipantParticipationState.Participated
                        : AppointmentParticipantParticipationState.Scheduled,
                })).ToList();
    }
}