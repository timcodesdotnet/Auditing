namespace TimCodes.Auditing.Redactors;

/// <summary>
/// Used to log in each event which redactors ran
/// </summary>
public class RedactLog
{
    public string? Mode { get; set; }
    public List<string> Redactors { get; set; } = new List<string>();
}
