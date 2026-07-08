Because I am very interested in LanguageExt I wanted to complete this assignment using the types from this library. It looks really powerful, but very abstract for me(some of its parts), so I touched only those parts which we covered in the FP courses. I know that the most interesting will come where I will understand how to take these `Monoid<A>` and `Semigroup<A>` and use their properties on some very abstract level to write code using them, but I think these are advanced topics and I should not rip the ass with them after my second FP course, only for now of course :)

# 1. Interval overlapping

*Before*

```cs
public static List<Guid> CheckForOverlappingTimes(IEnumerable<IGrouping<Guid?, IActivityUser>> groupedUsers)
{
    List<Guid> usersWithOverlappingTimes = [];
    foreach (var grouping in groupedUsers.Where(x => x.Count() > 1))
        if (GroupingHasOverlapping(grouping))
            usersWithOverlappingTimes.Add(grouping.FirstOrDefault()!.UserId!.Value);
    return usersWithOverlappingTimes;
}

private static bool GroupingHasOverlapping(IGrouping<Guid?, IActivityUser> grouping)
{
    List<IActivityUser> users = grouping.OrderBy(u => u.TimeFrom).ToList();
    for (int i = 0; i < users.Count; i++)
        for (int x = i + 1; x < users.Count; x++)
            if (users[i].TimeFrom <= users[x].TimeTo && users[x].TimeFrom <= users[i].TimeTo)
                return true;
    return false;
}
```

*After*

```cs
public readonly record struct Interval<T> where T : IComparable<T>
{
    public T From { get; }
    public T To { get; }
    public Interval(T from, T to) => (From, To) = from.CompareTo(to) <= 0 ? (from, to) : (to, from);
}

static bool AnyOverlap<T>(Seq<Interval<T>> intervals) where T : IComparable<T> =>
    toSeq(intervals.OrderBy(iv => iv.From))
        .Fold((Found: false, MaxEnd: Option<T>.None),
              (s, iv) => s.MaxEnd.Match(
                  Some: end => (s.Found || iv.From.CompareTo(end) <= 0, Some(Max(end, iv.To))),
                  None: ()  => (s.Found, Some(iv.To))))
        .Found;

static T Max<T>(T a, T b) where T : IComparable<T> => a.CompareTo(b) >= 0 ? a : b;


// Refactored
public static Seq<Guid> CheckForOverlappingTimes(IEnumerable<IGrouping<Guid?, IActivityUser>> groupedUsers) =>
    toSeq(groupedUsers)
        .Filter(g => AnyOverlap(toSeq(g).Map(u => new Interval<DateTime>(u.TimeFrom, u.TimeTo))))
        .Map(g => g.Key!.Value);
```


The actual overlap computation logic does not need to know anything about `IActivityUser` or any `Guid`s in the key. Everything we need is just `IComparable` and a sequence of intervals. To improve the type safety added a small `Interval` to encode the correctness that `From <= To`. We can also throw exception or create the `Optional<Interval<T>>` factory method instead of swapping values in constructor. The key is that `AnyOverlap` now may work with any intervals.


---

# 2. Merging of dictionaries

*Before*

```cs
private static Dictionary<Guid, List<TimeSlotResponseDtoWithCreatedUser>> MergeDictionaries(
    Dictionary<Guid, List<TimeSlotResponseDtoWithCreatedUser>> dict1,
    Dictionary<Guid, List<TimeSlotResponseDtoWithCreatedUser>> dict2)
{
    foreach (var kvp2 in dict2)
    {
        if (dict1.TryGetValue(kvp2.Key, out var value)) { value.AddRange(kvp2.Value); continue; }
        dict1.Add(kvp2.Key, kvp2.Value);
    }
    return dict1;
}
```

*After*

```cs
public static Map<K, V> Merge<K, V>(Map<K, V> left, Map<K, V> right, Func<V, V, V> combine)
    where K : notnull =>
    right.Pairs.Fold(left, (acc, pair) =>
        acc.AddOrUpdate(pair.Key, existing => combine(existing, pair.Value), pair.Value));

// Refactored under assumption that for matching keys we take the values from first dictionary as the initial funcitons does
Merge(dict1, dict2, (a, _) => a);
```

Merging of two dictionaries again does not need to be monomorphic for concrete value or key types. The only thing we need is operation for merging values for the same key. In the `before` case it was taking the values from the first dictionary, but we could also take both. On the client side we know concrete types and we can do whatever we want :)


---

# 3. `foreach + TryGetValue + Add`

*Before*

```cs
var result = new List<YouthCoachingInformationPerPhase>();
foreach (var youthCoachingPerPhase in perPhaseConfiguration)
{
    if (participantProjectPhaseMapping.TryGetValue(youthCoachingPerPhase.Id, out var phase))
        result.Add((YouthCoachingInformationPerPhase)youthCoachingPerPhase.CloneWithParticipantProjectPhase(phase));
}
return result;
```

*After*

```cs
public static Seq<B> ChooseByKey<A, K, V, B>(
    Seq<A> items, Map<K, V> lookup, Func<A, K> keyOf, Func<A, V, B> project) where K : notnull =>
    items.Choose(a => lookup.Find(keyOf(a)).Map(v => project(a, v)));
```

Was also a repeating pattern with a generic operation of filtering out elements from the initial collection whose keys are in the map. In the case above it was also mapping the result, that's why we also pass the `project` which takes the value from the `dict` and the `item` itself. Quite polymorphic!


---

# 4. Counting predicates matches in sequence

*Before*

```cs
int count = 0, countf2f = 0, countAbsence = 0;
foreach (var participant in participants.Where(p => p.Project!.Id == userProject.ProjectId))
{
    if (f2fDto.ParticipantStates!.Any(p => p == ParticipantStateExtension.GetParticipantStateAtDate(participant, f2fDto.CutOffDate)))
    {
        count++;
        if (WasF2F(f2fDto, participant))          countf2f++;
        if (WasParticipantAbsent(f2fDto, participant)) countAbsence++;
    }
}
```

*After*

```cs
public static Seq<int> CountMatches<T>(Seq<T> items, Seq<Func<T, bool>> predicates) => predicates.Map(predicate => items.Filter(predicate).Count);

// Refactoried
var counts = CountMatches(matching, Seq<Func<Participant, bool>>(p => WasF2F(f2fDto, p), p => WasParticipantAbsent(f2fDto, p)));
```

Was very suspicious first time I saw it. A bit of functional thinking and we can create as many monomorphic domain implementations of our predicates as we want and test how many participants say were absent, were face to face and so on.


---

# 5. Correlate by ID

*Before*

```cs
private static Dictionary<Guid, Guid?> GetProjectPhaseMapping(Project previousProject, Project followUpProject) =>
    previousProject.ProjectPhases.ToDictionary(pp => pp.Id,
        pp => followUpProject.ProjectPhases.FirstOrDefault(x => x.Name == pp.Name)?.Id);

private static Dictionary<Guid, Guid?> GetAgeGroupMapping(Project previousProject, Project followUpProject) =>
    previousProject.AgeGroups.ToDictionary(ag => ag.Id,
        ag => followUpProject.AgeGroups.FirstOrDefault(x => x.Name == ag.Name)?.Id);

private static Dictionary<Guid, Guid?> GetSustainabilityDeterminationMapping(Project previousProject, Project followUpProject) =>
    previousProject.SustainabilityDeterminations.ToDictionary(sd => sd.Id,
        sd => followUpProject.SustainabilityDeterminations.FirstOrDefault(x => x.Days == sd.Days)?.Id);
```

*After*

```cs
public static Map<V, Option<V>> Correlate<A, K, V>(Seq<A> source, Seq<A> target, Func<A, K> matchOn, Func<A, V> project) where TId : notnull where K : notnull => toMap(source.Map(s => (project(s), target.Find(t => matchOn(t).Equals(matchOn(s))).Map(project))));

// Refactored
private static Map<Guid, Option<Guid>> GetProjectPhaseMapping(Project prev, Project next) => Correlate(toSeq(prev.ProjectPhases), toSeq(next.ProjectPhases), p => p.Id, p => p.Name!);
```

Firstly I though to make the correlation by the `Guid`, because the three helper methods in the file were all using `Guid`, but then I though that we actually can generalise it to any `V` to which we map the `A` and then also to the type `K` by which we match the entries from sequences. LGTM :)