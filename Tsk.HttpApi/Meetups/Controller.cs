using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Tsk.HttpApi.Meetups;

[ApiController]
[Route("/meetups")]
public class MeetupController : ControllerBase
{
    private static readonly List<Meetup> meetups = [];

    [HttpGet]
    public IActionResult GetMeetups() =>
        Ok(meetups);

    [HttpPost]
    public IActionResult PostMeetup([FromBody] Meetup newMeetup)
    {
        newMeetup.Id = Guid.NewGuid();
        meetups.Add(newMeetup);
        return Ok(newMeetup);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteMeetup([FromRoute] Guid id)
    {
        var meetupToDelete = meetups.SingleOrDefault(meetup => meetup.Id == id);
        if (meetupToDelete is null)
        {
            return NotFound();
        }

        meetups.Remove(meetupToDelete);
        return Ok(meetupToDelete);
    }

    [HttpPut("{id:guid}")]
    public IActionResult UpdateMeetup([FromRoute] Guid id, [FromBody] Meetup updatedMeetup)
    {
        var oldMeetup = meetups.SingleOrDefault(meetup => meetup.Id == id);
        if (oldMeetup is null)
        {
            return NotFound();
        }

        oldMeetup.Topic = updatedMeetup.Topic;
        oldMeetup.Place = updatedMeetup.Place;
        oldMeetup.Duration = updatedMeetup.Duration;

        return NoContent();
    }

    [PublicAPI]
    public class Meetup
    {
        public Guid? Id { get; set; }
        public required string Topic { get; set; }
        public required string Place { get; set; }
        public required int Duration { get; set; }
    }
}
