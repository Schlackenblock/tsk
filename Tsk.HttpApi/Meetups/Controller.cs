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
    public IActionResult CreateMeetup([FromBody] CreateMeetupDto createDto)
    {
        var newMeetup = new Meetup
        {
            Id = Guid.NewGuid(),
            Topic = createDto.Topic,
            Place = createDto.Place,
            Duration = createDto.Duration
        };
        meetups.Add(newMeetup);

        var readDto = new ReadMeetupDto
        {
            Id = newMeetup.Id,
            Topic = newMeetup.Topic,
            Place = newMeetup.Place,
            Duration = newMeetup.Duration
        };
        return Ok(readDto);
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

        var readDto = new ReadMeetupDto
        {
            Id = meetupToDelete.Id,
            Topic = meetupToDelete.Topic,
            Place = meetupToDelete.Place,
            Duration = meetupToDelete.Duration
        };

        return Ok(readDto);
    }

    [HttpPut("{id:guid}")]
    public IActionResult UpdateMeetup([FromRoute] Guid id, [FromBody] UpdateMeetupDto updateMeetupDto)
    {
        var oldMeetup = meetups.SingleOrDefault(meetup => meetup.Id == id);
        if (oldMeetup is null)
        {
            return NotFound();
        }
        oldMeetup.Topic = updateMeetupDto.Topic;
        oldMeetup.Place = updateMeetupDto.Place;
        oldMeetup.Duration = updateMeetupDto.Duration;

        var readDto = new ReadMeetupDto
        {
            Id = oldMeetup.Id,
            Topic = updateMeetupDto.Topic,
            Place = updateMeetupDto.Place,
            Duration = updateMeetupDto.Duration
        };

        return Ok(readDto);
    }
}
