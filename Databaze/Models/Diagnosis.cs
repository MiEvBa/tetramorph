using System.Text.Json.Serialization;

public class Diagnosis
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public override string ToString()
    {
        return Code + " - " + Name; 
    }
}