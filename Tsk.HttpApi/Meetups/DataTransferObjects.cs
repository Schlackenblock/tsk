namespace Tsk.HttpApi.Meetups;

public record ReadMeetupDto(Guid Id, string? Topic, string? Place, int Duration);

// public class ReadMeetupDto
// {
//     public Guid Id { set{} }
//     public string? Topic { get; set; }
//     public string? Place { get; set; }
//     public int Duration { get; set; }
// }

public record CreateMeetupDto(string? Topic, string? Place, int Duration);
// public class CreateMeetupDto
// {
//     public string? Topic { get; set; }
//     public string? Place { get; set; }
//     public int Duration { get; set; }
// }

public record UpdateMeetupDto(string? Topic, string? Place, int Duration);
// public class UpdateMeetupDto
// {
//     public string? Topic { get; set; }
//     public string? Place { get; set; }
//     public int Duration { get; set; }
// }
