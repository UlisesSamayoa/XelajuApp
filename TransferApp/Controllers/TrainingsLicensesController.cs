using Microsoft.AspNetCore.Mvc;
using TransferApp.Models;
using TransferApp.Security;

public class TrainingsLicensesController : Controller
{
    private readonly TrainingsLicensesService _service;

    public TrainingsLicensesController(TrainingsLicensesService service)
    {
        _service = service;
    }

    [Permission("TrainingsLicenses.View")]
    public IActionResult Index() => View();

    [Permission("TrainingsLicenses.Edit")]
    public IActionResult Update(int id) => View();

    [HttpGet]
    [Permission("TrainingsLicenses.View")]
    public async Task<IActionResult> GetAll()
        => Json(await _service.GetAll());

    [HttpGet]
    [Permission("TrainingsLicenses.View")]
    public async Task<IActionResult> GetById(int id)
        => Json(await _service.GetById(id));

    [HttpPost]
    [Permission("TrainingsLicenses.Create")]
    public async Task<IActionResult> Create([FromForm] TrainingsLicensesModel model, IFormFile File)
    {
        try
        {
            model.UserC = User.Identity!.Name!;
            await _service.Create(model, File);

            return Ok(new { success = true, message = "Saved successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [Permission("TrainingsLicenses.Edit")]
    public async Task<IActionResult> Update([FromForm] TrainingsLicensesModel model, IFormFile File)
    {
        try
        {
            model.UserU = User.Identity!.Name!;
            await _service.Update(model, File);

            return Ok(new { success = true, message = "Updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [Permission("TrainingsLicenses.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id, User.Identity!.Name!);

        return Ok(new { success = true, message = "Deleted successfully" });
    }
}