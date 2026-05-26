using Microsoft.AspNetCore.Mvc;
using TransferApp.Models;
public class ParametersController : Controller
{
    private readonly ParametersService _service;

    public ParametersController(ParametersService service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Update(int id)
    {
        ViewBag.Id = id;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Json(await _service.GetAll());
    }

    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        return Json(await _service.GetById(id));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ParametersModel m)
    {
        try
        {
            m.UserC = "admin";
            var id = await _service.Create(m);
            return Ok(new
            {
                success = true,
                idParameters = id
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

    [HttpPut]
    public async Task<IActionResult> UpdateData([FromBody] ParametersModel m)
    {
        try
        {
            m.UserU = "admin";
            await _service.Update(m);
            return Ok(new
            {
                success = true
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

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.Delete(id, "admin");
            return Ok(new
            {
                success = true
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

    [HttpGet]
    public async Task<IActionResult>
    ValidateClientTransactions(string documentNumber, int TransactionType)
    {
        try
        {
            var result = await _service.ValidateClientTransactions(documentNumber, TransactionType);
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