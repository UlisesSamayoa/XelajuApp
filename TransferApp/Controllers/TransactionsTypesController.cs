using Microsoft.AspNetCore.Mvc;
using TransferApp.Models;
using TransferApp.Security;

public class TransactionsTypesController : Controller
{
    private readonly TransactionsTypesService _service;
    public TransactionsTypesController(TransactionsTypesService service)
    {
        _service = service;
    }

    [Permission("TransactionsTypes.View")]
    public IActionResult Index() => View();

    [Permission("TransactionsTypes.Edit")]
    public IActionResult Update(int id)
    {
        ViewBag.Id = id;
        return View();
    }

    [HttpGet]
    [Permission("TransactionsTypes.View")]
    public async Task<IActionResult> GetAll()
        => Json(await _service.GetAll());

    [HttpGet]
    [Permission("TransactionsTypes.View")]
    public async Task<IActionResult> GetById(int id)
        => Json(await _service.GetById(id));

    [Permission("TransactionsTypes.View")]
    public async Task<IActionResult> GetByNumber(int id)
       => Json(await _service.GetByNumber(id));

    [HttpPost]
    [Permission("TransactionsTypes.Create")]
    public async Task<IActionResult> Create([FromBody] TransactionsTypesModel m)
    {
        try
        {
            m.UserC = User.Identity!.Name!;
            await _service.Create(m);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Permission("TransactionsTypes.Edit")]
    public async Task<IActionResult> Update([FromBody] TransactionsTypesModel m)
    {
        try
        {
            m.UserU = User.Identity!.Name!;
            await _service.Update(m);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Permission("TransactionsTypes.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id, User.Identity!.Name!);
        return Json(new { success = true });
    }

    [HttpGet]
    [Permission("TransactionsTypes.View")]
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