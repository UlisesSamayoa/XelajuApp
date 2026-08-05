using Microsoft.AspNetCore.Mvc;
using TransferApp.Models;
using TransferApp.Security;

public class ClientsController : Controller
{
    private readonly ClientsService _service;

    public ClientsController(ClientsService service)
    {
        _service = service;
    }

    [Permission("Clients.View")]
    public IActionResult Index() => View();

    [Permission("Clients.View")]
    public IActionResult Update(int id) => View();

    [HttpGet]
    [Permission("Clients.View")]
    public async Task<IActionResult> GetAll()
        => Json(await _service.GetAll());

    [HttpGet]
    [Permission("Clients.View")]
    public async Task<IActionResult> GetById(int id)
        => Json(await _service.GetById(id));

    [HttpPost]
    [Permission("Clients.Create")]
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
    [Permission("Clients.Edit")]
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
    [Permission("Clients.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id, "admin");
        return Ok(new { message = "Deleted" });
    }

    [HttpGet]
    [Permission("Clients.View")]
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
    [Permission("Clients.View")]
    public async Task<IActionResult> ExistsClient(string documentNumber)
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

    [HttpGet]
    //[Permission("Clients.View")]
    public async Task<IActionResult> ProfileImage(int id)
    {
        var client = await _service.GetById(id);
        if (client == null || string.IsNullOrEmpty(client.Picture))
            return NotFound();
        if (!System.IO.File.Exists(client.Picture))
            return NotFound();
        var ext = Path.GetExtension(client.Picture).ToLower();
        string contentType = ext switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };
        //return PhysicalFile(client.Picture, contentType);
        return PhysicalFile(client.Picture, contentType, Path.GetFileName(client.Picture));
    }

}