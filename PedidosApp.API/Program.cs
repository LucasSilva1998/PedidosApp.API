using Microsoft.AspNetCore.Mvc;
using PedidosApp.API.Entities;
using PedidosApp.API.Producers;
using PedidosApp.API.Records;
using PedidosApp.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();
builder.Services.AddScoped( //Configurando serviço para injeção de dependência
 map => new PedidoRepository(
 builder.Configuration.GetConnectionString("PedidosBD"))
);

builder.Services.AddScoped( //Configurando serviço para injeção de dependência
 map => new MessageProducer(
 builder.Configuration["RabbitMQ:Host"],
 builder.Configuration["RabbitMQ:Queue"])
);

var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

//ENDPOINT para cadastro de pedido
app.MapPost("/api/pedidos", async (
 [FromBody] PedidoRequest request,
 PedidoRepository pedidoRepository,
 MessageProducer messageProducer) =>
{
//capturar os dados do pedido
var pedido = new Pedido
{
    Id = Guid.NewGuid(),
    DataHora = DateTime.Now,
    Cliente = request.Cliente,
    Valor = request.Valor,
    Detalhes = request.Detalhes
};

//gravar os dados no banco
await pedidoRepository.Inserir(pedido);

//enviar para a mensageria
messageProducer.SendMessage(pedido);

//retornando os dados do pedido cadastrado
return Results.Created("/api/pedidos", new PedidoResponse(
pedido.Id.Value,
pedido.Cliente,
pedido.Valor,
pedido.DataHora.Value,
pedido.Detalhes
));
})
.Produces<PedidoResponse>(StatusCodes.Status201Created);

app.Run();
