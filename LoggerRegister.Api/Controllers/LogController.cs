using LoggerRegister.Api.Database;
using LoggerRegister.Api.DTOs;
using LoggerRegister.Api.Models;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Reflection.PortableExecutable;
using System.Text;

namespace LoggerRegister.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private IConnection _connection;
    private IChannel _channel;



    public LogController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("create-log")]
    public async Task<IActionResult> CreateLogger([FromBody] CreateLoggerDTO input)
    {
        try
        {
            var logModel = new LogModel(input.Name, input.Description, input.Severity);

            if (logModel.Severity == Enums.Severity.ERROR || logModel.Severity == Enums.Severity.CRITICAL)
            {
                await _context.Set<LogModel>().AddAsync(logModel);
                await _context.SaveChangesAsync();
            }

            await InstantiateRabbitMq();

            await _channel.ExchangeDeclareAsync(exchange: "direct_logs", type: ExchangeType.Direct);

            var body = Encoding.UTF8.GetBytes(logModel.Description);

            await _channel.BasicPublishAsync(exchange: "direct_logs", routingKey: logModel.Severity.ToString(), body: body);

            return Ok(logModel);
           
        } catch (Exception)
        {
            return BadRequest();
        } 
    }

    private async Task InstantiateRabbitMq()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "admin",
            Password = "admin"
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
    }
}
