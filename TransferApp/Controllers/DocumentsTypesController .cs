using Microsoft.AspNetCore.Mvc;
using TransferApp.Models;
using TransferApp.Security;

public class DocumentsTypesController : Controller
{
    private readonly DocumentsTypesService _service;
    public DocumentsTypesController(DocumentsTypesService service)
    {
        _service = service;
    }

    [Permission("DocumentsTypes.View")]
    public IActionResult Index() => View();

    [Permission("DocumentsTypes.Edit")]
    public IActionResult Update(int id)
    {
        ViewBag.Id = id;
        return View();
    }

    [HttpGet]
    [Permission("DocumentsTypes.View")]
    public async Task<IActionResult> GetAll()
        => Json(await _service.GetAll());

    [HttpGet]
    [Permission("DocumentsTypes.View")]
    public async Task<IActionResult> GetById(int id)
        => Json(await _service.GetById(id));

    [HttpPost]
    [Permission("DocumentsTypes.Create")]
    public async Task<IActionResult> Create([FromBody] DocumentsTypes m)
    {
        try
        {
            m.UserC = User.Identity!.Name!;
            await _service.Create(m);
            return Json(new { success = true, message = "Saved!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Permission("DocumentsTypes.Edit")]
    public async Task<IActionResult> Update([FromBody] DocumentsTypes m)
    {
        try
        {
            m.UserU = User.Identity!.Name!;
            await _service.Update(m);
            return Json(new { success = true, message = "Updated!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Permission("DocumentsTypes.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id, User.Identity!.Name!);
        return Json(new { success = true });
    }
}