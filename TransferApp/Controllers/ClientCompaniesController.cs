using Microsoft.AspNetCore.Mvc;
using TransferApp.Models;

public class ClientCompaniesController : Controller
{
    private readonly ClientCompaniesService _service;

    public ClientCompaniesController(ClientCompaniesService service)
    {
        _service = service;
    }

    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Json(await _service.GetAll());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ClientCompaniesModel m)
    {
        try
        {
            m.UserC = "admin";

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
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id, "admin");
        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> GetPaidServiceCompaniesByClient(int clientId)
    {
        var data = await _service.GetPaidServiceCompaniesByClient(clientId);
        return Json(data);
    }

}