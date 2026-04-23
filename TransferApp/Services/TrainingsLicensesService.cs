using TransferApp.Models;

public class TrainingsLicensesService
{
    private readonly TrainingsLicensesRepository _repo;
    private readonly IWebHostEnvironment _env;

    public TrainingsLicensesService(TrainingsLicensesRepository repo, IWebHostEnvironment env)
    {
        _repo = repo;
        _env = env;
    }

    private string SaveFile(IFormFile file, string name)
    {
        if (file == null) return null;
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
        var ext = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(ext))
            throw new Exception("Invalid file type");
        if (file.Length > 5 * 1024 * 1024)
            throw new Exception("File too large (max 5MB)");
        var safeName = name.Replace(" ", "_");
        var fileName = $"{safeName}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
        var path = Path.Combine(_env.WebRootPath, "uploads", "trainings");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        var fullPath = Path.Combine(path, fileName);
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        return $"/uploads/trainings/{fileName}";
    }

    public async Task<List<TrainingsLicensesModel>> GetAll()
        => await _repo.GetAll();

    public async Task<TrainingsLicensesModel> GetById(int id)
        => await _repo.GetById(id);

    public async Task Create(TrainingsLicensesModel model, IFormFile file)
    {
        model.TrainingsLicensesFile = SaveFile(file, model.Name);
        await _repo.Create(model);
    }

    //public async Task Update(TrainingsLicensesModel model, IFormFile file)
    //{
    //    if (file != null)
    //    {
    //        model.TrainingsLicensesFile = SaveFile(file, model.Name);
    //    }

    //    await _repo.Update(model);
    //}
    public async Task Update(TrainingsLicensesModel model, IFormFile file)
    {
        if (file != null)
        {
            model.TrainingsLicensesFile = SaveFile(file, model.Name);
        }
        else
        {
            // 🔥 recuperar archivo actual
            var existing = await _repo.GetById(model.IdTrainingsLicenses);
            model.TrainingsLicensesFile = existing.TrainingsLicensesFile;
        }

        await _repo.Update(model);
    }

    public async Task Delete(int id, string user)
        => await _repo.Delete(id, user);
}