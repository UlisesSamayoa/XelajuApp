using Microsoft.AspNetCore.Mvc;
using TransferApp.Models;

public class CompaniesController : Controller
{
    private readonly CompanyService _service;
    public CompaniesController(CompanyService service)
    {
        _service = service;
    }
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _service.GetAll();
        return Json(data);
    }
    [HttpPost] 
    public async Task<IActionResult> Create([FromBody] CompaniesModel model)
    {
        try
        {
            await _service.Create(model);
            return Ok(new { success = true, message = "Company created successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
    [HttpGet]
    public IActionResult Update(int id)
    {
        ViewBag.IdCompany = id;
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var data = await _service.GetById(id);
        return Json(data);
    }
    [HttpPost]
    public async Task<IActionResult> Update([FromBody] CompaniesModel model)
    {
        try
        {
            await _service.Update(model);
            return Ok(new { success = true, message = "Company updated successfully" });
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
            string user = "admin";
            await _service.Delete(id, user);

            return Ok(new { success = true, message = "Company deleted" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}