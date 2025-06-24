using System.Text.Json.Serialization;

public class AppLog
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Id { get; set; }
    public string Event { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
}



