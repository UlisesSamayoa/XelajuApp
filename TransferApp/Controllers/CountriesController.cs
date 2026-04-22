using Microsoft.AspNetCore.Mvc;
using TransferApp.Models;

public class CountriesController : Controller
{
    private readonly CountryService _service;

    public CountriesController(CountryService service)
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
    public async Task<IActionResult> Create([FromBody] CountriesModel model)
    {
        try
        {
            await _service.Create(model);
            return Ok(new { success = true, message = "Country created" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Update([FromBody] CountriesModel model)
    {
        try
        {
            await _service.Update(model);
            return Ok(new { success = true, message = "Country updated" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.Delete(id, "admin");
            return Ok(new { success = true, message = "Country deleted" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}