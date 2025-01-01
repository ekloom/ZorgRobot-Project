using System;

namespace RobotProject.Services.Mqtt;

public class MqttMessageProcessingService
{
  private readonly SimpleMqttClient _mqttClient;

  public event EventHandler<string> OnMessageReceived;

  public MqttMessageProcessingService()
  {

    _mqttClient = new SimpleMqttClient(new SimpleMqttClientConfiguration());

    _mqttClient.OnMessageReceived += HandleMessage;

  }

  public async Task SendMessage(string message, TopicType topicType)
  {
    string topic = "";

    switch (topicType)
    {
      case TopicType.Alert:
        topic = "/Alert";
        break;
      case TopicType.Info:
        topic = "/Info";
        break;
    }

    await _mqttClient.PublishMessage(message, topic);
  }

  public async Task Init()
  {
    await _mqttClient.SubscribeToTopic("/Command");
  }

  private void HandleMessage(object sender, SimpleMqttMessage args)
  {
    // save je data mbv je repo
    // Wllicht wil je ook je data opslaan in een database?
    Console.WriteLine($"Incoming Command on {args.Topic}:{args.Message}");
    OnMessageReceived?.Invoke(sender, args.Message);
  }
}

public enum TopicType
{
  Alert,
  Info,
}
