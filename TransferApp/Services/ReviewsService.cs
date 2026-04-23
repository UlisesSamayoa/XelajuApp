using TransferApp.Models;

public class ReviewsService
{
    private readonly ReviewsRepository _repo;
    private readonly IWebHostEnvironment _env;

    public ReviewsService(ReviewsRepository repo, IWebHostEnvironment env)
    {
        _repo = repo;
        _env = env;
    }

    private string SaveFile(IFormFile file, string name)
    {
        if (file == null) return null;

        var ext = Path.GetExtension(file.FileName).ToLower();
        var allowed = new[] { ".jpg", ".png", ".jpeg", ".pdf" };

        if (!allowed.Contains(ext))
            throw new Exception("Invalid file");

        if (file.Length > 5 * 1024 * 1024)
            throw new Exception("Max 5MB");

        var safeName = name.Replace(" ", "_");
        var fileName = $"{safeName}_{DateTime.Now:yyyyMMddHHmmss}{ext}";

        var path = Path.Combine(_env.WebRootPath, "uploads", "reviews");

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var fullPath = Path.Combine(path, fileName);

        using var stream = new FileStream(fullPath, FileMode.Create);
        file.CopyTo(stream);

        return $"/uploads/reviews/{fileName}";
    }

    public async Task<List<ReviewsModel>> GetAll()
        => await _repo.GetAll();

    public async Task<ReviewsModel> GetById(int id)
        => await _repo.GetById(id);

    public async Task Create(ReviewsModel model, IFormFile file)
    {
        model.ReviewFile = SaveFile(file, model.Name);
        await _repo.Create(model);
    }

    public async Task Update(ReviewsModel model, IFormFile file)
    {
        if (file != null)
            model.ReviewFile = SaveFile(file, model.Name);
        else
        {
            var existing = await _repo.GetById(model.IdReview);
            model.ReviewFile = existing.ReviewFile;
        }

        await _repo.Update(model);
    }

    public async Task Delete(int id, string user)
        => await _repo.Delete(id, user);
}