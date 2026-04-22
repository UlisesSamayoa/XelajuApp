using TransferApp.Models;

public class CountryService
{
    private readonly CountryRepository _repo;

    public CountryService(CountryRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<CountriesModel>> GetAll()
    {
        return await _repo.GetAll();
    }

    public async Task<CountriesModel> GetById(int id) { 
        return await _repo.GetById(id); 
    }

    public async Task Create(CountriesModel model)
    {
        await _repo.Create(model);
    }

    public async Task Update(CountriesModel model)
    {
        await _repo.Update(model);
    }

    public async Task Delete(int id, string userU)
    {
        await _repo.Delete(id, userU);
    }
}