using LoggerRegister.Api.Enums;

namespace LoggerRegister.Api.DTOs;

public record CreateLoggerDTO
{
    public string Name { get; init; }
    
    public string Description { get; init; }

    public Severity Severity { get; init; }
}
