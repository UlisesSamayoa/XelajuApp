using TransferApp.Models;

public class ClientsService
{
    private readonly ClientsRepository _repo;
    private readonly IWebHostEnvironment _env;

    public ClientsService(ClientsRepository repo, IWebHostEnvironment env)
    {
        _repo = repo;
        _env = env;
    }

    private string SaveImage(IFormFile file, string doc)
    {
        if (file == null) return null;

        var ext = Path.GetExtension(file.FileName).ToLower();
        var allowed = new[] { ".jpg", ".jpeg", ".png" };

        if (!allowed.Contains(ext))
            throw new Exception("Only images allowed");

        if (file.Length > 3 * 1024 * 1024)
            throw new Exception("Max 3MB");

        var fileName = $"{doc}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
        var path = Path.Combine(_env.WebRootPath, "uploads", "clients");

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var fullPath = Path.Combine(path, fileName);

        using var stream = new FileStream(fullPath, FileMode.Create);
        file.CopyTo(stream);

        return $"/uploads/clients/{fileName}";
    }

    public async Task<List<ClientsModel>> GetAll() => await _repo.GetAll();

    public async Task<ClientsModel> GetById(int id) => await _repo.GetById(id);

    //public async Task Create(ClientsModel model, IFormFile file)
    //{
    //    model.Picture = SaveImage(file, model.DocumentNumber);
    //    await _repo.Create(model);
    //}
    public async Task<int> Create(ClientsModel model, IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            var fileName = $"{model.FirstName}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(file.FileName)}";

            var path = Path.Combine("wwwroot/uploads/clients", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            model.Picture = "/uploads/clients/" + fileName;
        }
        else
        {
            model.Picture = "/uploads/clients/default.png";
        }

        return await _repo.Create(model);
    }

    public async Task Update(ClientsModel model, IFormFile file)
    {
        if (file != null)
            model.Picture = SaveImage(file, model.DocumentNumber);
        else
            model.Picture = (await _repo.GetById(model.IdClient)).Picture;

        await _repo.Update(model);
    }

    public async Task Delete(int id, string user)
        => await _repo.Delete(id, user);

    public async Task<List<ClientsModel>> Search(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return new List<ClientsModel>();

        return await _repo.Search(term);
    }
}