using Ampol.Domain.Enums;

namespace EntityBenchmark;

public static class DataSeeder
{
    public static List<BenchmarkUser> CreateUsers(int count) => Enumerable.Range(0, count).Select(i => new BenchmarkUser
        {
            Id = Guid.NewGuid(),
            FirstName = $"Berater{i}",
            LastName = $"Nachname{i}",
            FullName = $"Berater{i} Nachname{i}",
            UserName = $"berater{i}@test.at",
            NormalizedUserName = $"BERATER{i}@TEST.AT",
            Email = $"berater{i}@test.at",
            NormalizedEmail = $"BERATER{i}@TEST.AT",
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        }).ToList();

    public static List<BenchmarkUserEvent> CreateUserEvents(List<BenchmarkUser> users, int eventsPerUser, DateTime rangeStart, int rangeDays = 90)
    {
        var rng = new Random(42);

        return users.SelectMany(user => Enumerable.Range(0, eventsPerUser).Select(_ =>
            {
                var date = rangeStart.AddDays(rng.Next(0, rangeDays)).Date;
                var startHour = rng.Next(7, 16);
                var duration = rng.Next(1, 4);
                var isPresence = rng.NextDouble() < 0.7;

                return new BenchmarkUserEvent
                {
                    Id = Guid.NewGuid(),
                    Title = isPresence ? $"Buero {user.FirstName}" : $"Abwesend {user.FirstName}",
                    Color = isPresence ? "#4CAF50" : "#F44336",
                    DateFrom = date,
                    DateTo = date,
                    TimeFrom = date.AddHours(startHour),
                    TimeTo = date.AddHours(startHour + duration),
                    AllDay = false,
                    DaysOfWeekGermanFlagEnum = DaysOfWeekGermanFlagEnum.Montag,
                    UserId = user.Id,
                    Type = isPresence ? UserEventType.Presence : UserEventType.Absence,
                };
            })).ToList();
    }
}