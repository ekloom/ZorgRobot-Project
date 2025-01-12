using System;

namespace RobotProject.Services.Mqtt;

public class MqttMessageHandler
{
  private readonly SimpleMqttClient _mqttClient;

  public event EventHandler<string>? OnMessageReceived;

  public MqttMessageHandler()
  {

    _mqttClient = SimpleMqttClient.CreateSimpleMqttClientForHiveMQ("RobotProject");

    _mqttClient.OnMessageReceived += HandleMessage;

  }

  public async Task SendMessage(string message, TopicType topicType)
  {
    string topic = "";

    switch (topicType)
    {
      case TopicType.Alert:
        topic = "Alert";
        break;
      case TopicType.Info:
        topic = "Info";
        break;
    }

    System.Console.WriteLine($"Message being send to MQTT: Topic = {topic}, Message = {message}");
    await _mqttClient.PublishMessage(message, topic);
  }

  public async Task Init()
  {
    await _mqttClient.SubscribeToTopic("Command");
    SendMessage("Memento is online!", TopicType.Info);
  }

  private void HandleMessage(object sender, SimpleMqttMessage? args)
  {
    Console.WriteLine($"Command received: {args.Topic} {args.Message}");
    OnMessageReceived?.Invoke(sender, args.Message);
  }
}

public enum TopicType
{
  Alert,
  Info,
}
