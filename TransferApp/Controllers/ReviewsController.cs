using Microsoft.AspNetCore.Mvc;
using TransferApp.Models;
using TransferApp.Security;

public class ReviewsController : Controller
{
    private readonly ReviewsService _service;

    public ReviewsController(ReviewsService service)
    {
        _service = service;
    }

    [Permission("Reviews.View")]
    public IActionResult Index() => View();

    [Permission("Reviews.Edit")]
    public IActionResult Update(int id) => View();

    [HttpGet]
    [Permission("Reviews.View")]
    public async Task<IActionResult> GetAll()
        => Json(await _service.GetAll());

    [HttpGet]
    [Permission("Reviews.View")]
    public async Task<IActionResult> GetById(int id)
        => Json(await _service.GetById(id));

    [HttpPost]
    [Permission("Reviews.Create")]
    public async Task<IActionResult> Create([FromForm] ReviewsModel model, IFormFile File)
    {
        try
        {
            model.UserC = User.Identity!.Name!;
            await _service.Create(model, File);
            return Ok(new { message = "Saved successfully!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Permission("Reviews.Edit")]
    public async Task<IActionResult> Update([FromForm] ReviewsModel model, IFormFile File)
    {
        try
        {
            model.UserU = User.Identity!.Name!;
            await _service.Update(model, File);
            return Ok(new { message = "Updated successfully!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Permission("Reviews.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id, User.Identity!.Name!);
        return Ok(new { message = "Deleted successfully!" });
    }
}