using System.Text.Json.Serialization;

public class Client
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CardNumber { get; set; }
    public string[] DiagnosisIds { get; set; }
    public int Sex { get; set; }
    public string BirthDate { get; set; }
}
