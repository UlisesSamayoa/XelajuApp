using Microsoft.AspNetCore.Mvc;
using TransferApp.Models;
using TransferApp.Security;

public class BeneficiariesController : Controller
{
    private readonly BeneficiariesService _service;

    public BeneficiariesController(BeneficiariesService service)
    {
        _service = service;
    }
    [Permission("Beneficiaries.View")]
    public IActionResult Index() => View();

    [Permission("Beneficiaries.Edit")]
    public IActionResult Update(int id)
    {
        ViewBag.Id = id;
        return View();
    }

    [HttpGet]
    [Permission("Beneficiaries.View")]
    public async Task<IActionResult> GetAll()
        => Json(await _service.GetAll());

    [HttpGet]
    [Permission("Beneficiaries.View")]
    public async Task<IActionResult> GetById(int id)
        => Json(await _service.GetById(id));

    [HttpPost]
    [Permission("Beneficiaries.Create")]
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
    [Permission("Beneficiaries.Edit")]
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
    [Permission("Beneficiaries.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id, "admin");
        return Json(new { success = true });
    }

    [HttpGet]
    [Permission("Beneficiaries.View")]
    public async Task<IActionResult> GetByClient(int id)
    {
        var data = await _service.GetByClient(id);
        return Json(data);
    }
}