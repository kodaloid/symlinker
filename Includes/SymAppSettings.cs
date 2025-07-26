using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace SymLinker.Includes
{
    public class SymAppSettings
    {
        public ObservableCollection<SymLink> Links { get; set; } = [];


        public SymAppSettings() { }


        public async Task Save(CancellationToken cancellationToken = default)
        {
            string file = App.AppDataPath("settings.json");
            if (!File.Exists(file)) File.Delete(file);
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            await File.WriteAllTextAsync(file, json, cancellationToken);
        }


        public static async Task<SymAppSettings?> Load(CancellationToken cancellationToken = default)
        {
            string file = App.AppDataPath("settings.json");
            if (!File.Exists(file)) return null;
            string json = await File.ReadAllTextAsync(file, cancellationToken);
            return JsonConvert.DeserializeObject<SymAppSettings>(json);
        }
    }
}