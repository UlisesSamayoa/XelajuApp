using Microsoft.AspNetCore.Mvc;
using TransferApp.Models;
using TransferApp.Security;

public class CompaniesController : Controller
{
    private readonly CompanyService _service;
    public CompaniesController(CompanyService service)
    {
        _service = service;
    }

    [Permission("Companies.View")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Permission("Companies.View")]
    public async Task<IActionResult> GetAll()
    {
        var data = await _service.GetAll();
        return Json(data);
    }

    [HttpPost]
    [Permission("Companies.Create")]
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
    [Permission("Companies.Edit")]
    public IActionResult Update(int id)
    {
        ViewBag.IdCompany = id;
        return View();
    }

    [HttpGet]
    [Permission("Companies.View")]
    public async Task<IActionResult> GetById(int id)
    {
        var data = await _service.GetById(id);
        return Json(data);
    }

    [HttpPost]
    [Permission("Companies.Edit")]
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
    [Permission("Companies.Delete")]
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

    [HttpGet]
    [Permission("Companies.View")]
    public async Task<IActionResult> GetByCountry(int countryId)
    => Json(await _service.GetByCountry(countryId));

    [HttpGet]
    [Permission("Companies.View")]
    public async Task<IActionResult> GetByTransactionType(int transactionType)
    {
        var result = await _service.GetByTransactionType(transactionType);
        return Ok(result);
    }

    [HttpGet]
    [Permission("Companies.View")]
    public async Task<IActionResult> GetByTransactionType_Service(int transactionType)
    {
        var result = await _service.GetByTransactionType_Service(transactionType);
        return Ok(result);
    }

    [HttpPost]
    [Permission("Companies.Edit")]
    public async Task<IActionResult> ChangeStatus(int idCompany, string status, string StatusCompanyComment)
    {
        try
        {
            await _service.ChangeStatus(
                idCompany,
                status,
                StatusCompanyComment
            );
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Permission("Companies.View")]
    public async Task<IActionResult> Search(string term)
    {
        try
        {
            var data = await _service.Search(term);
            return Json(data);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


}