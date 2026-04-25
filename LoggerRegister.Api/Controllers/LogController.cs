using LoggerRegister.Api.Database;
using LoggerRegister.Api.DTOs;
using LoggerRegister.Api.Models;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;

namespace LoggerRegister.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ConnectionFactory _factory = new ConnectionFactory()
    {
        HostName = "localhost",
        UserName = "admin",
        Password = "admin"
    };

    private readonly List<CreateLoggerDTO> LogList = new List<CreateLoggerDTO>
    {
        new CreateLoggerDTO
        {
            Description = "INFO",
            Name = "INFO",
            Severity = Enums.Severity.INFO
        },
        new CreateLoggerDTO
        {
            Description = "INFO",
            Name = "INFO",
            Severity = Enums.Severity.WARNING
        },
        new CreateLoggerDTO
        {
            Description = "INFO",
            Name = "INFO",
            Severity = Enums.Severity.WARNING
        },
        new CreateLoggerDTO
        {
            Description = "INFO",
            Name = "INFO",
            Severity = Enums.Severity.INFO
        },
        new CreateLoggerDTO
        {
            Description = "INFO",
            Name = "INFO",
            Severity = Enums.Severity.CRITICAL
        },
    };

    public LogController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("create-log")]
    public async Task<IActionResult> CreateLogger()
    {
        try
        {
            await using var _connection = await _factory.CreateConnectionAsync();
            await using var _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(exchange: "direct_logs", type: ExchangeType.Direct);

            foreach (var input in LogList)
            {
                var logModel = new LogModel(input.Name, input.Description, input.Severity);

                if (logModel.Severity == Enums.Severity.ERROR || logModel.Severity == Enums.Severity.CRITICAL)
                {
                    await _context.Set<LogModel>().AddAsync(logModel);
                    await _context.SaveChangesAsync();
                }

                var body = Encoding.UTF8.GetBytes(logModel.Description);

                await _channel.BasicPublishAsync(exchange: "direct_logs", routingKey: logModel.Severity.ToString(), body: body);
            }

            return Ok();

        } catch (Exception)
        {
            return BadRequest();
        }
    }
}
