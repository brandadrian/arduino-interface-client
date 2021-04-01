using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using System.Text;

namespace ArduinoInterfaceClient
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private SerialPortManager serialPortManager;
        private List<string> availablePorts;
        private string stringToSend;
        private string stringFromArduino;
        private string comPort;
        private int baudrate;
        private string state;

        public MainWindowViewModel()
        {
            SendCommand = new ActionCommand(OnSendCommand, OnCanExecute);
            ReadCommand = new ActionCommand(OnReadCommand, OnCanExecute);
            DisconnectCommand = new ActionCommand(OnDisconnectCommand);
            ConnectCommand = new ActionCommand(OnConnectCommand);

            serialPortManager = new SerialPortManager();
            serialPortManager.DataReceivedEvent += SerialPortManager_DataReceivedEvent;
            serialPortManager.StateUpdateEvent += SerialPortManager_StateUpdateEvent;
            AvailablePorts = serialPortManager.GetAvailableComPorts(); ;
            ComPort = availablePorts.FirstOrDefault();
            Baudrate = 9600;
            State = "Not connected";
        }

        public List<string> AvailablePorts
        {
            get
            {
                return availablePorts;
            }
            set
            {
                availablePorts = value;
                NotifyPropertyChanged("AvailablePorts");
            }
        }

        public string State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
                NotifyPropertyChanged("State");
            }
        }

        public string StringToSend
        {
            get
            {
                return stringToSend;
            }
            set
            {
                stringToSend = value;
                NotifyPropertyChanged("StringToSend");
            }
        }

        public string StringFromArduino
        {
            get
            {
                return stringFromArduino;
            }
            set
            {
                stringFromArduino = value;
                NotifyPropertyChanged("StringFromArduino");
            }
        }

        public string ComPort
        {
            get
            {
                return comPort;
            }
            set
            {
                comPort = value;
                NotifyPropertyChanged("ComPort");
            }
        }

        public int Baudrate
        {
            get
            {
                return baudrate;
            }
            set
            {
                baudrate = value;
                NotifyPropertyChanged("Baudrate");
            }
        }

        public ICommand DisconnectCommand
        {
            get;
            set;
        }

        public ICommand ConnectCommand
        {
            get;
            set;
        }

        public ICommand SendCommand
        {
            get;
            set;
        }

        public ICommand ReadCommand
        {
            get;
            set;
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnDisconnectCommand(object parameter)
        {
            serialPortManager.Disconnect();
        }

        private void OnConnectCommand(object parameter)
        {
            serialPortManager.Connect(ComPort, Baudrate);
        }

        private void OnSendCommand(object parameter)
        {
            serialPortManager.SendData(StringToSend);
        }

        private void OnReadCommand(object parameter)
        {
            serialPortManager.SendData("readCardCommand#");
        }

        private bool OnCanExecute(object arg)
        {
            return serialPortManager.IsConnected;
        }
        private void SerialPortManager_StateUpdateEvent(object sender, SerialPortManagerStateUpdateEventArgs e)
        {
            State = e.Message;
        }

        private void SerialPortManager_DataReceivedEvent(object sender, SerialPortManagerDataReceivedEventArgs e)
        {
            StringFromArduino = DateTime.Now.ToString("MM.dd.yyyy HH:mm:ss") + "\n" + e.Data + "\n\n" + StringFromArduino;
        }
    }
}
