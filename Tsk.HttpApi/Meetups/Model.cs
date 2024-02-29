namespace Tsk.HttpApi.Meetups;

internal class Meetup
{
    public Guid? Id { get; set; }
    public required string Topic { get; set; }
    public required string Place { get; set; }
    public required int Duration { get; set; }
}
