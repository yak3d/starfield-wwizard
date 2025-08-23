using System.Linq.Expressions;
using StarfieldWwizard.Core.Models;

namespace StarfieldWwizard.Contracts.Services;

public interface ILocalSettingsService
{
    Task<AppSettings> GetSettingsAsync();

    Task SaveSettingsAsync(AppSettings settings);

    Task<T?> GetSettingAsync<T>(Expression<Func<AppSettings, T>> selector);

    Task UpdateSettingAsync<T>(Expression<Func<AppSettings, T>> selector, T value);
}
