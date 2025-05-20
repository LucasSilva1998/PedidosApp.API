using Microsoft.AspNetCore.Components.Routing;
using PedidosApp.API.Entities;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PedidosApp.API.Producers
{
    /// <summary>
    /// Classe para que possamos gravar mensagens na fila do RabbitMQ
    /// </summary>
    public class MessageProducer
    {
        private readonly string _host;
        private readonly string _queue;

        public MessageProducer(string host, string queue)
        {
            _host = host;
            _queue = queue;
        }

        public void SendMessage(Pedido pedido)
        {
            //Conectando no servidor do RabbitMQ
            var factory = new ConnectionFactory() { HostName = _host };
            using var conn = factory.CreateConnection();
            using var channel = conn.CreateModel();

            //Criar a fila caso não exista
            channel.QueueDeclare(
                    queue: _queue, //Nome da fila
                  exclusive: false, //Fila pode ser acessada por várias conexões simultaneamente(aplicações)
                    durable: true, //Fila será gravada no disco (sobrevive a reinicializações do servidor)
                    autoDelete: false, //Fila não será apagada quando não houver mais mensagens
                    arguments: null //demais argumentos opcionais 
                );

            //Serializando os dados que serão gravados na fila
            var mensagem = Encoding.UTF8.GetBytes (JsonSerializer.Serialize(pedido));

            //Publicando / gravando os dados
            channel.BasicPublish(
                exchange: "", //componente opcional para redirecionamento
                routingKey: _queue, //nome da fila para onde a mensagem será enviada
                basicProperties: null, //propriedades adicionais
                body: mensagem //Conteudo da mensagem que será gravado
                );
        }
    }
}