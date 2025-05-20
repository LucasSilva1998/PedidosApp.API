namespace PedidosApp.API.Records
{
    /// <summary>
    /// Modelo de dados para as resposta de pedido
    /// </summary>
    public record PedidoResponse
    (
    Guid Id, //Ídentificador único do pedido
    string Cliente, //Nome do cliente
    decimal Valor, //Valor do pedido
    DateTime DataHora, //Data e hora de cadastro do pedido
    string Detalhes //Detalhes do pedido
    );
}
