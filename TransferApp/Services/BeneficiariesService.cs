using TransferApp.Models;

public class BeneficiariesService
{
    private readonly BeneficiariesRepository _repo;

    public BeneficiariesService(BeneficiariesRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<BeneficiariesModel>> GetAll() => await _repo.GetAll();
    public async Task<BeneficiariesModel> GetById(int id) => await _repo.GetById(id);
    public async Task Create(BeneficiariesModel m) => await _repo.Create(m);
    public async Task Update(BeneficiariesModel m) => await _repo.Update(m);
    public async Task Delete(int id, string user) => await _repo.Delete(id, user);
}