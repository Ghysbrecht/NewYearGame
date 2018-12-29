using System;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Linq;

namespace NewYearGame.Logic
{
    class MQTTConnection
    {
        private MqttClient _client;
        private SimpleLogger _log;
        private Action<string, string> subscribedMessageReceivedFunc;

        public MQTTConnection(SimpleLogger log, string connAddress)
        {
            _log = log;
            _client = new MqttClient(connAddress,9001, false, null, null, MqttSslProtocols.None);
        }       

        public void Connect()
        {
            _log.Debug("Connecting to MQTT broker");
            byte code = _client.Connect(Guid.NewGuid().ToString());
            _log.Debug("Connection attempt returned byte > " + code.ToString());
            _client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
        }

        public bool isConnected()
        {
            return _client.IsConnected;
        }

        public void Subscribe(string[] subscriptions)
        {
            foreach(string subscription in subscriptions)
            {
                _log.Debug("Subscribing to topic: " + subscription);
                ushort msgId = _client.Subscribe(
                    new string[] { subscription },
                    new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }
                );
            }
        }

        public void AttachMessageHandlerMethod(Action<string, string> handlerMethod)
        {
            _log.Debug("Attaching messageHandlerMethod");
            subscribedMessageReceivedFunc = handlerMethod;
        }

        public void RemoveConnection()
        {
            _client.Disconnect();
        }

        private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            _log.Debug("Received MQTT message on topic: " + e.Topic + " > " + e.Message);
            string messageString = System.Text.Encoding.Default.GetString(e.Message);
            subscribedMessageReceivedFunc?.Invoke(e.Topic,messageString);
        }
    }
}
