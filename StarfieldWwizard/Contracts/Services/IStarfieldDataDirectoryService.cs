namespace StarfieldWwizard.Contracts.Services;

public interface IStarfieldDataDirectoryService
{
    string StarfieldDataDirectory { get; }
    
    Task InitializeAsync();

    Task SetDataDirectoryAsync(string StarfieldDataDirectory);
}