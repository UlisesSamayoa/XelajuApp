using Microsoft.AspNetCore.Mvc;
using TransferApp.Models;

public class TrainingsLicensesController : Controller
{
    private readonly TrainingsLicensesService _service;

    public TrainingsLicensesController(TrainingsLicensesService service)
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
    public async Task<IActionResult> Create(
        [FromForm] TrainingsLicensesModel model,
        IFormFile File)
    {
        try
        {
            await _service.Create(model, File);

            return Ok(new { success = true, message = "Saved successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Update(
        [FromForm] TrainingsLicensesModel model,
        IFormFile File)
    {
        try
        {
            await _service.Update(model, File);

            return Ok(new { success = true, message = "Updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id, "admin");

        return Ok(new { success = true, message = "Deleted successfully" });
    }
}