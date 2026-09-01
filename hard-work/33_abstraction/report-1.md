# Same implementation and different specification

1. I recently had some sort of this case during my import. According to DSGVO and all this stuff, user during import may not see participants from the projects where they do not have
`Import` claim or `ParticipantEdit`. I mean I modelled it differently, but this could be a good example, because the code could look like something:

```cs
bool CanClassify(User user, Project project) => user.HasClaim(Import, project) || user.HasClaim(EditParticipant, project);
```

whereas we could also have the code to check whether the user may write into the project? They may not read and can only write to the project where they have a specific claim

```cs
bool CanWrite(User user, Project project) => user.HasClaim(Import, project) || user.HasClaim(EditParticipant, project);
```

Unifying them long term mixes the "abstractions" or just domains and can actually break the behavior. Because if we now(what I got) want that for classification it may also be just a `ViewParticipant`
claim, then this would break the `CanWrite`, because we can write into the project where we just have the `View` claim. And this is a leak...

2. Another good example which I found (I will not paste the code, it is too ugly) is the cancellation vs revert cancellation of a group event. The code in both methods looks pretty similar, we extract the group event, check if there are any scheduled appointments in case of cancellation, so that we can cancel or that there are any cancelled appointments, so that we can revert them to scheduled. If nothing found then of course exception, what else. Then we go through all the appointments and reconcile them to the desired state and add time recordings. This could be potentially parametrized and instead of enums we would pass polymoprhic target types, but the domain are quite different here and from the maitenance point we could get a requirement that we must not revert appointments older then two weeks or
that for cancellation the speaker must have confirmed this. In a single function this would case this parametrization hell with conditions.

3. I mean from my current understanding even small parts "can be maintained as different code parts" if they contain the same code for now. The question is how possible the change is and whether we really
need to do it right from the beginning. Because potentially what I saw recently in the appointments service:

```cs
List<AppointmentUser> deletedAppointmentUsers = appointmentFromDb.AppointmentUsers
    .ExceptBy(entity.AppointmentUsers.Select(p => p.Id), p => p.Id).ToList();

List<AppointmentParticipant> deletedAppointmentParticipants = appointmentFromDb.AppointmentParticipants
    .ExceptBy(entity.AppointmentParticipants.Select(p => p.Id), p => p.Id).ToList();

List<AppointmentCompanyConsultationAct> deletedAppointmentCompanyConsultationActs = appointmentFromDb.AppointmentCompanyConsultationActs
    .ExceptBy(entity.AppointmentCompanyConsultationActs.Select(p => p.Id), p => p.Id).ToList();
```

could be maintained separately as well in terms of "what could happen next according to the spec". Because here we delete from a single appointment the facts about the staff members working time, we delete the person'g attendence facts and the last one just the fact that this appointment was bound to an company consultation act. Theoretically we could extract all of this into a generic function, what I would actually do :) But for appointments users, this could become a soft delete because payroll and bla bla. For participants we may also want to do the same, because the funder wants so. Or for appointment participants we add some entry in a related entity after deletion. Potentially this could be 3 different actions which sould be maintained separately, because they could evolve differently, although for now they do the same thing...


## Boxing

Even when using Rider I never used this garbage extract into a function refactoring, because intuition always said to me that there is something wrong with it. I mean now I see that even super duper polymophic functions may not be enough and even if we just count occurenced and pass `Seq<T>` and `Func<T, bool>` with `Expression<S, int>` for the field where we write this occurence in the target type, there could be cases where this is not enough and we need something else and we need to either duplicate the counting code and insert something in between, what is actually fine and should not be treated as something bad and the lecture prooved it. But refactoring by putting garbage and parametrizing it is never an option... For my polymorphic functions I was thinking on just a concrete and specific operation on a type with some properties, nothing else.


## Look Ups in Files

This is actually an interesting problem and for now I need not to even look up the types/functions/signatures but read what the code does, otherwise it will not work when integrating existing code. This is bad and is the nature of exception based handling and side effects. And the answer may be here, the more we put in the types, the less we need to read(at least read) in the modules we depend on. Because one thing is having a method which retunrs `Customer` and another thing `IO<Either<Fin<Customer>, Order>>`, I did not know what to put instead of `Order` :) Then even jsut looking at the signature we see what we work with and what we can do with it. Because the customer itself will be just the record and the rest are effects we work with. But we still need to work with dependencies, at my current level of understanding. I jsut can not imagine a way that a layer does not have any dependencies in the DAG graph, even if it is a leaf we will work with some already existing types. And this forces us potentially to look smth up. But of course minizing outgoing links to other layers aka dependencies is the way to go. If we have only one outgoing link = 1 potential lookup. But maybe I misundertood the question, but the thing with proper signatures to save at least function reading type and be damm honest about what you do and not write to a file and then send something over network when I just want to create a participant in a transaction. This would help a lot!
