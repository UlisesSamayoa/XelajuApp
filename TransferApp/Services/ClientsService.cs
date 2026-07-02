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

    public async Task<int> Create(ClientsModel model, IFormFile file)
    {
        CreateClientProfileFolder($"{model.FirstName} {model.LastName}", model.DocumentNumber);
        if (file != null && file.Length > 0)
        {
            model.Picture = SaveClientProfile(
                file,
                $"{model.FirstName} {model.LastName}",
                model.DocumentNumber);
        }
        else
        {
            model.Picture = null;
        }
        return await _repo.Create(model);
    }


    //public async Task<int> Create(ClientsModel model, IFormFile file)
    //{
    //    if (file != null && file.Length > 0)
    //    {
    //        if (file != null && file.Length > 0)
    //        {
    //            model.Picture = SaveClientProfile(file, $"{model.FirstName} {model.LastName}", model.DocumentNumber);
    //        }
    //        else
    //        {
    //            model.Picture = null;
    //        }
    //    }
    //    else
    //    {
    //        model.Picture = "/uploads/clients/default.png";
    //    }

    //    return await _repo.Create(model);
    //}

    private string SaveClientProfile(IFormFile file, string clientName, string clientDocument)
    {
        if (file == null)
            return null;
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var ext = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(ext))
            throw new Exception("Invalid file type");
        if (file.Length > 5 * 1024 * 1024)
            throw new Exception("File too large (max 5MB)");
        string profileFolder = CreateClientProfileFolder(clientName, clientDocument);
        foreach (var existingFile in Directory.GetFiles(profileFolder))
        {
            File.Delete(existingFile);
        }
        clientName = CleanPathValue(clientName);
        string fileName = $"{clientName}{ext}";
        string fullPath = Path.Combine(profileFolder, fileName);
        using (var stream = new FileStream(fullPath, FileMode.Create)) { file.CopyTo(stream); }
        return fullPath;
    }


    //public async Task Update(ClientsModel model, IFormFile file)
    //{
    //    if (file != null)
    //        model.Picture = SaveImage(file, model.DocumentNumber);
    //    else
    //        model.Picture = (await _repo.GetById(model.IdClient)).Picture;

    //    await _repo.Update(model);
    //}
    public async Task Update(ClientsModel model, IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            model.Picture = SaveClientProfile(file, $"{model.FirstName} {model.LastName}", model.DocumentNumber);
        }
        else
        {
            model.Picture = (await _repo.GetById(model.IdClient)).Picture;
        }
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
    public async Task<bool> ExistsClient(string documentNumber)
    {
        return await _repo.ExistsClient(documentNumber);
    }

    private string CleanPathValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            value = value.Replace(c.ToString(), "");
        }
        return value.Trim();
    }
    private string CreateClientProfileFolder(string clientName, string clientDocument)
    {
        string basePath = @"C:\TransactionFiles";
        clientName = CleanPathValue(clientName);
        clientDocument = CleanPathValue(clientDocument);
        string clientFolder = $"{clientName}_{clientDocument}";
        string profileFolder = Path.Combine(basePath, clientFolder, "Profile");
        if (!Directory.Exists(profileFolder)) Directory.CreateDirectory(profileFolder);
        return profileFolder;
    }


}