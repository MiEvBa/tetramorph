using System.Text.Json.Serialization;

namespace Tetramorph.Doctor.Database.Models;


public class Drug
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UpName { get; set; } = string.Empty;
    public string[] Dosage { get; set; } = new string[]{};
}