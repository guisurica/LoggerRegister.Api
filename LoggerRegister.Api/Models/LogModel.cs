using LoggerRegister.Api.Enums;

namespace LoggerRegister.Api.Models;

public sealed class LogModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Severity Severity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
    public DateTime UpdatedAt { get; set; }

    public LogModel(string name, string description, Severity severity)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Severity = severity;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
