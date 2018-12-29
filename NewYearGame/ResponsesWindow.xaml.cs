using NewYearGame.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NewYearGame
{
    /// <summary>
    /// Interaction logic for ResponsesWindow.xaml
    /// </summary>
    public partial class ResponsesWindow : Window
    {
        private SimpleLogger _log;
        private MQTTConnection _mqtt;
        public ResponsesWindow(SimpleLogger log)
        {
            InitializeComponent();
            _log = log;
            _log.Debug("ResponsesWindow object created");
            ConnectionAddressTextBox.Text = DefaultValues.MQTT_ADRESS;
        }

        public void CreateMQTTConnection()
        {
            _log.Debug("Creating MQTT Connection");
            if (ConnectionAddressTextBox.Text == "")
            {
                _log.Error("Connection addres was empty when trying to create the MQTT connection");
                AddLineToTextBlock("Empty connection address!");
            }

            try
            {
                if(_mqtt != null)
                {
                    _log.Debug("MQTT Connection already exists, removing...");
                    _mqtt.RemoveConnection();
                    _mqtt = null;
                }

                _mqtt = new MQTTConnection(_log, ConnectionAddressTextBox.Text);
                _mqtt.Connect();
                _mqtt.Subscribe(DefaultValues.TEAMNAMES);
                _mqtt.AttachMessageHandlerMethod(HandlePlayerMessage);
            }
            catch(Exception e)
            {
                _log.Error("Excpetion thrown when trying to setup MQTT connection > " + e.ToString());
                AddLineToTextBlock("Exception occured! Check logging...");
            }
            

            if (_mqtt != null && _mqtt.isConnected())
            {
                _log.Info("Connection established!");
                AddLineToTextBlock("Connection established!");
            }
            else
            {
                _log.Error("Connection failed!");
                AddLineToTextBlock("Connection Failed!");
            }
        }

        public void HandlePlayerMessage(string team, string message)
        {
            if (message.Length > DefaultValues.MAX_MESSAGE_LENGTH)
            {
                message = "MESSAGE TOO LONG!";
            }

            AddLineToTextBlock(DateTime.Now.ToShortTimeString() + " | " + team + " > " + message);
        }

        public void AddLineToTextBlock(string content)
        {
            Dispatcher.BeginInvoke((Action)(() => ResponsesTextBlock.Text = content + "\n" + ResponsesTextBlock.Text));
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            _log.Debug("Click on subscribe button...");
            CreateMQTTConnection();
            _log.Debug("Click handled!");
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ResponsesTextBlock.Text = "CLEARED!";
        }
    }
}
