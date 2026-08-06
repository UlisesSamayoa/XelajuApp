using Microsoft.AspNetCore.Mvc;
using TransferApp.Models;
using TransferApp.Security;

public class ClientCompaniesController : Controller
{
    private readonly ClientCompaniesService _service;
    public ClientCompaniesController(ClientCompaniesService service)
    {
        _service = service;
    }

    [Permission("ClientCompanies.View")]
    public IActionResult Index() => View();

    [HttpGet]
    [Permission("ClientCompanies.View")]
    public async Task<IActionResult> GetAll()
        => Json(await _service.GetAll());

    [HttpPost]
    [Permission("ClientCompanies.Create")]
    public async Task<IActionResult> Create([FromBody] ClientCompaniesModel m)
    {
        try
        {
            m.UserC = User.Identity!.Name!;

            var id = await _service.Create(m);

            return Json(new
            {
                success = true,
                message = "Saved!",
                idClientCompany = id
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost]
    [Permission("ClientCompanies.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id, User.Identity!.Name!);
        return Json(new { success = true });
    }

    [HttpGet]
    [Permission("ClientCompanies.View")]
    public async Task<IActionResult> GetPaidServiceCompaniesByClient(int clientId)
    {
        var data = await _service.GetPaidServiceCompaniesByClient(clientId);
        return Json(data);
    }

}