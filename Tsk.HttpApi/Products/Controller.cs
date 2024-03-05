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
            Title = createDto.Title,
            Name = createDto.Name,
            Type = createDto.Type,
            Length = createDto.Length,
            Width = createDto.Width,
            Height = createDto.Height,
            Price = createDto.Price,
            Discount = createDto.Discount
        };

        products.Add(newMeetup);

        var readDto = new ReadProductDto(
            newMeetup.Id,
            newMeetup.Title,
            newMeetup.Name,
            newMeetup.Type,
            newMeetup.Length,
            newMeetup.Width,
            newMeetup.Height,
            newMeetup.Price,
            newMeetup.Discount
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
            meetupToDelete.Title,
            meetupToDelete.Name,
            meetupToDelete.Type,
            meetupToDelete.Length,
            meetupToDelete.Width,
            meetupToDelete.Height,
            meetupToDelete.Price,
            meetupToDelete.Discount
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

        oldMeetup.Title = updateMeetupDto.Title;
        oldMeetup.Name = updateMeetupDto.Name;
        oldMeetup.Type = updateMeetupDto.Type;
        oldMeetup.Length = updateMeetupDto.Length;
        oldMeetup.Width = updateMeetupDto.Width;
        oldMeetup.Height = updateMeetupDto.Height;
        oldMeetup.Price = updateMeetupDto.Price;
        oldMeetup.Discount = updateMeetupDto.Discount;

        var readDto = new ReadProductDto(
            oldMeetup.Id,
            updateMeetupDto.Title,
            updateMeetupDto.Name,
            updateMeetupDto.Type,
            updateMeetupDto.Length,
            updateMeetupDto.Width,
            updateMeetupDto.Height,
            updateMeetupDto.Price,
            updateMeetupDto.Discount
        );
        return Ok(readDto);
    }
}
