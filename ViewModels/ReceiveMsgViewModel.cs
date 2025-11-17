using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace CSharp_RabbitMQ_Demo.ViewModels
{
	public partial class ReceiveMsgViewModel : ObservableObject
	{
		public ObservableCollection<string> Messages { get; } = new ObservableCollection<string>();

		[ObservableProperty]
		private string host;

		[ObservableProperty]
		private string queueName;

		[ObservableProperty]
		private string userName;

		[ObservableProperty]
		private string password;

		private bool IsReceiving { get; set; }

		// Keep connection/channel/consumer as fields so we can stop them later
		private IConnection? _connection;
		private IChannel? _channel;
		private string? _consumerTag;
		private AsyncEventingBasicConsumer? _consumer;

		[RelayCommand]
		private async Task StartService()
		{
			if (IsReceiving)
			{
				OnMessageReceived(" [!] Already receiving.");
				return;
			}

			var hostName = string.IsNullOrWhiteSpace(Host) ? "localHost" : Host;
			var qName = string.IsNullOrWhiteSpace(QueueName) ? "testQueue" : QueueName;
			var user = string.IsNullOrWhiteSpace(UserName) ? "guest" : UserName; //add default user if needed
			var pass = string.IsNullOrWhiteSpace(Password) ? "guest" : Password; //add default password if needed

			try
			{
				var factory = new ConnectionFactory { HostName = hostName, UserName = user, Password = pass };

				// keep the connection and channel alive as fields
				_connection = await factory.CreateConnectionAsync();
				_channel = await _connection.CreateChannelAsync();

				OnMessageReceived(" [*] Waiting for messages.");

				// Set up the consumer
				_consumer = new AsyncEventingBasicConsumer(_channel);
				_consumer.ReceivedAsync += (model, ea) =>
				{
					var body = ea.Body.ToArray();
					var message = Encoding.UTF8.GetString(body);
					OnMessageReceived($" [x] Received {message}");
					return Task.CompletedTask;
				};

				// BasicConsumeAsync returns the consumer tag
				_consumerTag = await _channel.BasicConsumeAsync(qName, autoAck: true, consumer: _consumer);
				IsReceiving = true;
			}
			catch (Exception ex)
			{
				OnMessageReceived($" [!] Failed to start receiving: {ex.Message}");
				// Clean up any partially created resources
				await CleanupConnectionAsync();
			}
		}

		[RelayCommand]
		private async Task StopService()
		{
			if (!IsReceiving)
			{
				OnMessageReceived(" [!] Not currently receiving.");
				return;
			}

			try
			{
				if (_channel != null && !string.IsNullOrEmpty(_consumerTag))
				{
					try
					{
						// Cancel the consumer on the server side
						await _channel.BasicCancelAsync(_consumerTag);
					}
					catch
					{
						// ignore cancel exceptions, proceed to cleanup
					}
				}
			}
			finally
			{
				await CleanupConnectionAsync();
				IsReceiving = false;
				OnMessageReceived(" [*] Stopped receiving messages.");
			}
		}

		private async Task CleanupConnectionAsync()
		{
			// Detach event handlers first to avoid callbacks during dispose
			if (_consumer != null)
			{
				_consumer.ReceivedAsync -= (model, ea) =>
				{
					var body = ea.Body.ToArray();
					var message = Encoding.UTF8.GetString(body);
					OnMessageReceived($" [x] Received {message}");
					return Task.CompletedTask;
				};
			}

			try
			{
				_channel?.CloseAsync();
			}
			catch { }

			try
			{
				_connection?.CloseAsync();
			}
			catch { }

			try
			{
				_channel?.Dispose();
			}
			catch { }

			try
			{
				_connection?.Dispose();
			}
			catch { }

			_consumerTag = null;
			_consumer = null;
			_channel = null;
			_connection = null;

			// small yield to keep async method truly asynchronous
			await Task.Yield();
		}

		private void OnMessageReceived(string msg)
		{
			// Ensure update happens on UI thread
			if (Application.Current?.Dispatcher?.CheckAccess() == true)
			{
				Messages.Insert(0, msg);
			}
			else
			{
				Application.Current?.Dispatcher?.Invoke(() => Messages.Insert(0, msg));
			}
		}

		[RelayCommand]
		private void Refresh()
		{
			Messages.Clear();
			OnMessageReceived(" [*] Messages cleared.");
		}
	}
}