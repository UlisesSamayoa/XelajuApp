using Microsoft.AspNetCore.Mvc;
using TransferApp.Models;

public class BeneficiariesController : Controller
{
    private readonly BeneficiariesService _service;

    public BeneficiariesController(BeneficiariesService service)
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

    //[HttpPost]
    //public async Task<IActionResult> Create([FromBody] BeneficiariesModel m)
    //{
    //    try
    //    {
    //        m.UserC = "admin";
    //        await _service.Create(m);
    //        return Json(new { success = true, message = "Saved!" });
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(new { message = ex.Message });
    //    }
    //}
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BeneficiariesModel m)
    {
        try
        {
            m.UserC = "admin";

            var id = await _service.Create(m);

            return Json(new
            {
                success = true,
                message = "Saved!",
                idBeneficiarie = id
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Update([FromBody] BeneficiariesModel m)
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
    [HttpGet]
    public async Task<IActionResult> GetByClient(int id)
    {
        var data = await _service.GetByClient(id);
        return Json(data);
    }
}