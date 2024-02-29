using JetBrains.Annotations;

namespace Tsk.HttpApi.Meetups;

[PublicAPI]
public record ReadMeetupDto(Guid Id, string Topic, string Place, int Duration);

[PublicAPI]
public record CreateMeetupDto(string Topic, string Place, int Duration);

[PublicAPI]
public record UpdateMeetupDto(string Topic, string Place, int Duration);
