namespace PedidosApp.API.Records
{
    /// <summary>
    /// Modelo de dados para as requisições de pedido
    /// </summary>
    public record PedidoRequest
    (
    string Cliente, //Nome do cliente
    decimal Valor, //Valor do pedido
    string Detalhes //Detalhes do pedido
    );
}
