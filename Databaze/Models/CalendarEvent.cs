using System.Text.Json.Serialization;


public class CalendarEvent
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Id { get; set; }
    public int EventType { get; set; }
    public string Date { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public int State { get; set; }
    public string Note { get; set; } = string.Empty;
    public string[] Drugs { get; set; } = new string[]{};
    public string[] Urls { get; set; } = new string[]{};
}