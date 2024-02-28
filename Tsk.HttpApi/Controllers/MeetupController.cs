using Microsoft.AspNetCore.Mvc;

namespace Tsk.HttpApi.Controllers;

[ApiController]
[Route("/meetups")]
public class MeetupController : ControllerBase
{
     private static readonly ICollection<Meetup> Meetups  = new List<Meetup>();
     
     public class Meetup
     {
          public Guid? Id { get; set; }
          public required string Topic { get; set; }
          public required string Place { get; set; }
          public required int Duration { get; set; }
     }
}