using Microsoft.AspNetCore.Mvc;
using TransferApp.Models;

public class TransactionsController : Controller
{
    private readonly TransactionsService _service;

    public TransactionsController(TransactionsService service)
    {
        _service = service;
    }

    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Json(await _service.GetAll());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TransactionsModel m)
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
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id, "admin");
        return Json(new { success = true });
    }
}