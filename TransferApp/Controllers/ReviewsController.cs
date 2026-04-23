using Microsoft.AspNetCore.Mvc;
using TransferApp.Models;

public class ReviewsController : Controller
{
    private readonly ReviewsService _service;

    public ReviewsController(ReviewsService service)
    {
        _service = service;
    }

    public IActionResult Index() => View();
    public IActionResult Update(int id) => View();

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Json(await _service.GetAll());

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
        => Json(await _service.GetById(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ReviewsModel model, IFormFile File)
    {
        try
        {
            await _service.Create(model, File);
            return Ok(new { message = "Saved successfully!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Update([FromForm] ReviewsModel model, IFormFile File)
    {
        try
        {
            await _service.Update(model, File);
            return Ok(new { message = "Updated successfully!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id, "admin");
        return Ok(new { message = "Deleted successfully!" });
    }
}