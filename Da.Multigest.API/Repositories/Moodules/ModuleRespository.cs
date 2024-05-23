using SharedClassLibrary.Contract.Modules;
using SharedClassLibrary.Models.Contacts;

namespace Da.Multigest.API.Repositories.Moodules;

public class ModuleRespository : IModuleFacade
{
    public Task<Contact> GetModuleByIdAsync(Guid moduleId)
    {
        throw new NotImplementedException();
    }
}
