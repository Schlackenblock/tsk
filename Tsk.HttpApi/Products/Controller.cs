using Microsoft.AspNetCore.Mvc;

namespace Tsk.HttpApi.Products;

[ApiController]
[Route("/products")]
public class MeetupController : ControllerBase
{
    private static readonly List<Product> products = [];

    [HttpGet]
    public IActionResult GetMeetups() =>
        Ok(products);

    [HttpPost]
    public IActionResult CreateMeetup([FromBody] CreateProductDto createDto)
    {
        var newMeetup = new Product
        {
            Id = Guid.NewGuid(),
            Description = createDto.Title,
            Name = createDto.Description,
            Price = createDto.Price
        };

        products.Add(newMeetup);

        var readDto = new ReadProductDto(
            newMeetup.Id,
            newMeetup.Description,
            newMeetup.Name,
            newMeetup.Price
        );
        return Ok(readDto);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteMeetup([FromRoute] Guid id)
    {
        var meetupToDelete = products.SingleOrDefault(meetup => meetup.Id == id);
        if (meetupToDelete is null)
        {
            return NotFound();
        }

        products.Remove(meetupToDelete);

        var readDto = new ReadProductDto(
            meetupToDelete.Id,
            meetupToDelete.Description,
            meetupToDelete.Name,
            meetupToDelete.Price
        );
        return Ok(readDto);
    }

    [HttpPut("{id:guid}")]
    public IActionResult UpdateMeetup([FromRoute] Guid id, [FromBody] UpdateProductDto updateMeetupDto)
    {
        var oldMeetup = products.SingleOrDefault(meetup => meetup.Id == id);
        if (oldMeetup is null)
        {
            return NotFound();
        }

        oldMeetup.Description = updateMeetupDto.Title;
        oldMeetup.Name = updateMeetupDto.Description;
        oldMeetup.Price = updateMeetupDto.Price;

        var readDto = new ReadProductDto(
            oldMeetup.Id,
            updateMeetupDto.Title,
            updateMeetupDto.Description,
            updateMeetupDto.Price
        );
        return Ok(readDto);
    }
}
