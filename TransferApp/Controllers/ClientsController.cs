using Microsoft.AspNetCore.Mvc;
using TransferApp.Models;

public class ClientsController : Controller
{
    private readonly ClientsService _service;

    public ClientsController(ClientsService service)
    {
        _service = service;
    }

    public IActionResult Index() => View();
    public IActionResult Update(int id) => View();

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Json(await _service.GetAll());

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
        => Json(await _service.GetById(id));

    //[HttpPost]
    //public async Task<IActionResult> Create([FromForm] ClientsModel model, IFormFile File)
    //{
    //    try
    //    {
    //        await _service.Create(model, File);
    //        return Ok(new { message = "Client created" });
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(new { message = ex.Message });
    //    }
    //}

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ClientsModel model, IFormFile File)
    {
        try
        {
            model.UserC = "admin";

            var id = await _service.Create(model, File);

            return Ok(new
            {
                success = true,
                message = "Client created",
                idClient = id
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Update([FromForm] ClientsModel model, IFormFile File)
    {
        try
        {
            await _service.Update(model, File);
            return Ok(new { message = "Client updated" });
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
        return Ok(new { message = "Deleted" });
    }
    [HttpGet]
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
    
    [HttpGet]
    public async Task<IActionResult>
    ExistsClient(string documentNumber)
    {
        try
        {
            var result = await _service.ExistsClient(documentNumber);
            return Json(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}