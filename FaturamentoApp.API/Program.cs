using FaturamentoApp.API.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

builder.Services.AddHostedService
 (map => new MessageConsumer(
 builder.Configuration.GetConnectionString("FaturamentoBD"),
 builder.Configuration["RabbitMQ:Host"],
 builder.Configuration["RabbitMQ:Queue"]
 ));

var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
