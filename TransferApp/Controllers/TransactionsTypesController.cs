using Microsoft.AspNetCore.Mvc;
using TransferApp.Models;

public class TransactionsTypesController : Controller
{
    private readonly TransactionsTypesService _service;

    public TransactionsTypesController(TransactionsTypesService service)
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
    public async Task<IActionResult> Create([FromBody] TransactionsTypesModel m)
    {
        try
        {
            m.UserC = "admin";
            await _service.Create(m);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Update([FromBody] TransactionsTypesModel m)
    {
        try
        {
            m.UserU = "admin";
            await _service.Update(m);
            return Json(new { success = true });
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
    [HttpGet]
    public async Task<IActionResult> GetAllTypes()
    {
        try
        {
            var list = await _service.GetAllTypes();
            return Json(list);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}