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
    public async Task<List<CompaniesModel>> GetByCountry(int countryId)
    {
        if (countryId <= 0)
            throw new Exception("Invalid country");

        return await _repo.GetByCountry(countryId);
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
    public async Task<List<CompaniesModel>> GetByTransactionType(int transactionType)
    {
        return await _repo.GetByTransactionType(transactionType);
    }

    public async Task ChangeStatus(int idCompany, string status, string StatusCompanyComment)
    {
        if (
            status != "Excellent" &&
            status != "Suspicious" &&
            status != "Rejected"
        )
        {
            throw new Exception(
                "Invalid status"
            );
        }

        await _repo.ChangeStatus(
            idCompany,
            status,
            StatusCompanyComment
        );
    }

}