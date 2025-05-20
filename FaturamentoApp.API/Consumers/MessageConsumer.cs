using Dapper;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Data.SqlClient;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
namespace FaturamentoApp.API.Consumers
{
    /// <summary>
    /// Serviço para ler e processar as mensagens da fila do RabbitMQ
    /// </summary>
    public class MessageConsumer : BackgroundService
    {
        private readonly string _connectionString;
        private readonly string _host;
        private readonly string _queue;
        public MessageConsumer(string connectionString, string host,
       string queue)
        {
            _connectionString = connectionString;
            _host = host;
            _queue = queue;
        }
        protected override async Task ExecuteAsync
       (CancellationToken stoppingToken)
        {
            //conectando no servidor da mensageria
            var factory = new ConnectionFactory() { HostName = _host };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            //acessando a fila
            channel.QueueDeclare(queue: _queue, durable: true, autoDelete:
           false, exclusive: false, arguments: null);
            var consumer = new EventingBasicConsumer(channel); //consumidor (leitura da fila)
 //rotina de leitura da fila de forma incremental
 consumer.Received += async (sender, args) =>
 {
 //Lendo a mensagem contida na fila
 var payload = args.Body.ToArray();
 //ler a mensagem da fila em bytes
 var message = Encoding.UTF8.GetString(payload);
     //convertendo de bytes para string
     var pedido = JsonSerializer.Deserialize<Pedido>(message);
     //deserializando de JSON para objeto
     //gravar os dados do faturamento no banco de dados
     await RegistrarFaturamento(pedido);
     //Confirmando para o RabbitMQ que a mensagem foi lida e processada com sucesso
 channel.BasicAck(args.DeliveryTag, false);
     //args.DeliveryTag -> identificador único da mensagem que foi lida
     //false -> ACK faz a leitura de uma unica mensagem individualmente
 };
            //fazendo a execução do consumer e leitura das mensagens da fila
            channel.BasicConsume(
            queue: _queue, //nome da fila
            autoAck: false, //não retirar itens da fila automaticamente, mas só se houver confirmação(channel.BasicAck)
           
            consumer: consumer //Objeto que contem a rotina do consumer.
            );
        }
        private async Task RegistrarFaturamento(Pedido pedido)
        {
            var query = @"
                         INSERT INTO FATURAMENTO(ID, VALOR, DATAHORA, 
                        DADOS, PEDIDO_ID)
                         VALUES(@Id, @Valor, @DataHora, @Dados, @PedidoId)
                         ";
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(query, new
                {
                    @Id = Guid.NewGuid(),
                    @Valor = pedido.Valor,
                    @DataHora = pedido.DataHora,
                    @Dados = $"Cliente: {pedido.Cliente}, Detalhes: { pedido.Detalhes}",
                     @PedidoId = pedido.Id
                 });
            }
         }
    }

public class Pedido
{
    public Guid? Id { get; set; }
    public DateTime? DataHora { get; set; }
    public string? Cliente { get; set; }
    public decimal? Valor { get; set; }
    public string? Detalhes { get; set; }
}
}
