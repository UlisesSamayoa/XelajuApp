using Microsoft.AspNetCore.Mvc;
using TransferApp.Models;
using TransferApp.Security;

public class CountriesController : Controller
{
    private readonly CountryService _service;

    public CountriesController(CountryService service)
    {
        _service = service;
    }

    [Permission("Countries.View")]
    public IActionResult Index() => View();

    [Permission("Countries.Edit")]
    public IActionResult Update(int id) => View();

    [HttpGet]
    [Permission("Countries.View")]
    public async Task<IActionResult> GetAll()
        => Json(await _service.GetAll());

    [HttpGet]
    [Permission("Countries.View")]
    public async Task<IActionResult> GetById(int id)
        => Json(await _service.GetById(id));

    [HttpPost]
    [Permission("Countries.Create")]
    public async Task<IActionResult> Create([FromBody] CountriesModel model)
    {
        try
        {
            model.UserC = User.Identity!.Name!;
            await _service.Create(model);
            return Ok(new { success = true, message = "Country created" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [Permission("Countries.Edit")]
    public async Task<IActionResult> Update([FromBody] CountriesModel model)
    {
        try
        {
            model.UserU = User.Identity!.Name!;
            await _service.Update(model);
            return Ok(new { success = true, message = "Country updated" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [Permission("Countries.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.Delete(id, User.Identity!.Name!);
            return Ok(new { success = true, message = "Country deleted" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}