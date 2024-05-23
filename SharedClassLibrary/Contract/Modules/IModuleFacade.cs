using SharedClassLibrary.Models.Contacts;

namespace SharedClassLibrary.Contract.Modules;

public interface IModuleFacade
{
    Task<Contact> GetModuleByIdAsync(Guid moduleId);
}
