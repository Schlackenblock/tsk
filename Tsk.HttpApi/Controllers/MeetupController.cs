using Microsoft.AspNetCore.Mvc;

namespace Tsk.HttpApi.Controllers;

[ApiController]
[Route("/meetups")]
public class MeetupController : ControllerBase
{ 
    private static readonly ICollection<Meetup> Meetups = new List<Meetup>();

    [HttpGet]
    public IActionResult GetMeetup() 
    { 
          return Ok(Meetups);
    }
     
    [HttpPost]
    public IActionResult PostMeetup([FromBody] Meetup newMeetup)
    {
         newMeetup.Id = Guid.NewGuid();
         Meetups.Add(newMeetup);
         
         return Ok(newMeetup);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteMeetup([FromRoute] Guid id)
    {
         var meetupToDelete = Meetups.SingleOrDefault(meetup => meetup.Id == id);

         if (meetupToDelete is null)
         {
              return NotFound();
         }

         Meetups.Remove(meetupToDelete);
         return Ok(meetupToDelete);
    }
     
    [HttpPut("{id:guid}")]
    public IActionResult UpdateMeetup([FromRoute] Guid id, [FromBody] Meetup updatedMeetup)
    {
         var oldMeetup = Meetups.SingleOrDefault(meetup => meetup.Id == id);
          // meetup with provided id does not exist
         if (oldMeetup is null)
         {
              return NotFound();
         }

         oldMeetup.Topic = updatedMeetup.Topic;
         oldMeetup.Place = updatedMeetup.Place;
         oldMeetup.Duration = updatedMeetup.Duration;

         return NoContent();
     }
     
     public class Meetup
     {
          public Guid? Id { get; set; }
          public required string Topic { get; set; }
          public required string Place { get; set; }
          public required int Duration { get; set; }
     }
}