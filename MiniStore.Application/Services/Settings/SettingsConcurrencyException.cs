namespace MiniStore.Application.Services;

public sealed class SettingsConcurrencyException(string message) : InvalidOperationException(message);
