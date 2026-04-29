using System.Text.Json;
using ADTO.DCloud.RealtimeClientDemo.Models;

namespace ADTO.DCloud.RealtimeClientDemo;

public static class AppSettingsStore
{
    private static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "client-settings.json");

    public static ClientSettings Load()
    {
        if (!File.Exists(FilePath)) return new ClientSettings();
        var text = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<ClientSettings>(text) ?? new ClientSettings();
    }

    public static void Save(ClientSettings settings)
    {
        File.WriteAllText(FilePath, JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
    }
}
