using Microsoft.AspNetCore.Mvc;
using TransferApp.Models;

public class DocumentsTypesController : Controller
{
    private readonly DocumentsTypesService _service;

    public DocumentsTypesController(DocumentsTypesService service)
    {
        _service = service;
    }

    public IActionResult Index() => View();

    public IActionResult Update(int id)
    {
        ViewBag.Id = id;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Json(await _service.GetAll());

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
        => Json(await _service.GetById(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DocumentsTypes m)
    {
        try
        {
            m.UserC = "admin";
            await _service.Create(m);
            return Json(new { success = true, message = "Saved!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Update([FromBody] DocumentsTypes m)
    {
        try
        {
            m.UserU = "admin";
            await _service.Update(m);
            return Json(new { success = true, message = "Updated!" });
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
        return Json(new { success = true });
    }
}