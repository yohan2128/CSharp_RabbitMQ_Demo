using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharp_RabbitMQ_Demo.Views;

namespace CSharp_RabbitMQ_Demo.ViewModels
{
	public partial class MainViewModel : ObservableObject
	{
		[ObservableProperty]
		private ObservableObject currentViewModel;

		private readonly SendMsgViewModel _sendVm;
		private readonly ReceiveMsgViewModel _receiveVm;

		public MainViewModel()
		{

			_sendVm = new SendMsgViewModel();
			_receiveVm = new ReceiveMsgViewModel();

			// Default view-model shown in ContentControl
			CurrentViewModel = _sendVm;
		}
			
		[RelayCommand]
		private void ShowSendMsgView()
		{
			CurrentViewModel = _sendVm;
		}

		[RelayCommand]
		private void ShowReceiveMsgView()
		{
			CurrentViewModel = _receiveVm;
		}
	}
}
