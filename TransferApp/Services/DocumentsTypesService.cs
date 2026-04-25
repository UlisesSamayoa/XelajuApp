using TransferApp.Models;

public class DocumentsTypesService
{
    private readonly DocumentsTypesRepository _repo;

    public DocumentsTypesService(DocumentsTypesRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<DocumentsTypes>> GetAll() => await _repo.GetAll();
    public async Task<DocumentsTypes> GetById(int id) => await _repo.GetById(id);
    public async Task Create(DocumentsTypes m) => await _repo.Create(m);
    public async Task Update(DocumentsTypes m) => await _repo.Update(m);
    public async Task Delete(int id, string user) => await _repo.Delete(id, user);
}