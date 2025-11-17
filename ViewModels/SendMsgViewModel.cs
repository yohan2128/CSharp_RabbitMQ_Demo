using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RabbitMQ.Client	;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_RabbitMQ_Demo.ViewModels
{
	public partial class SendMsgViewModel : ObservableObject
	{
		[ObservableProperty]
		private string message;

		[ObservableProperty]
		private string host;

		[ObservableProperty]
		private string queueName;

		[ObservableProperty]
		private string userName;

		[ObservableProperty]
		private string password;

		[ObservableProperty]
		private string? lastSent;

		[RelayCommand]
		private async Task SendAsync()
		{
			// basic validations
			if (string.IsNullOrWhiteSpace(Message))
			{
				LastSent = "Please enter a message.";
				return;
			}

			var hostName = string.IsNullOrWhiteSpace(Host) ? "localHost" : Host;
			var qName = string.IsNullOrWhiteSpace(QueueName) ? "testQueue" : QueueName;
			var user = string.IsNullOrWhiteSpace(UserName) ? "guest" : UserName; //add default user if needed
			var pass = string.IsNullOrWhiteSpace(Password) ? "guest" : Password; //add default password if needed
			var payload = Message ?? string.Empty;

			try
			{
				var factory = new ConnectionFactory
				{
					HostName = hostName,
					UserName = user,
					Password = pass,
					// VirtualHost, AutomaticRecovery, RequestedHeartbeat, etc. can be set here.
				};

				// Using 'using var' to ensure proper disposal of connection and channel
				using var connection = await factory.CreateConnectionAsync();
				using var channel = await connection.CreateChannelAsync();

				// Declare the queue (idempotent operation)
				/*await channel.QueueDeclareAsync(queue: qName, durable: false, exclusive: false, autoDelete: false, arguments: null);*/

				// Convert message to byte array
				var body = Encoding.UTF8.GetBytes(payload);


				// Publish async
				await channel.BasicPublishAsync(exchange: string.Empty,
												routingKey: qName,
												body: body);

				LastSent = $"[x] Sent: {payload}";
				Message = string.Empty;
			}
			catch (Exception ex)
			{
				LastSent = $"Error sending message: {ex.Message}";
			}
		}
	}
}