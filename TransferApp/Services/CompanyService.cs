using TransferApp.Models;

public class CompanyService
{
    private readonly CompanyRepository _repo;

    public CompanyService(CompanyRepository repo)
    {
        _repo = repo;
    }
    public async Task Create(CompaniesModel model)
    {
        await _repo.Create(model);
    }
    public async Task<List<CompaniesModel>> GetAll()
    {
        return await _repo.GetAll();
    }
    public async Task<CompaniesModel> GetById(int id)
    {
        return await _repo.GetById(id);
    }
    public async Task Update(CompaniesModel model)
    {
        await _repo.Update(model);
    }
    public async Task Delete(int id, string userU)
    {
        if (id <= 0)
            throw new Exception("Invalid ID");

        await _repo.Delete(id, userU);
    }
}