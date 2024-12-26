using System;

namespace RobotProject.Services.Mqtt;

public class MqttMessageProcessingService
{
  private readonly SimpleMqttClient _mqttClient;

  public MqttMessageProcessingService(SimpleMqttClient mqttClient)
  {

    _mqttClient = mqttClient;

    _mqttClient.OnMessageReceived += HandleMessage;

  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    await _mqttClient.SubscribeToTopic("#");
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private void HandleMessage(object sender, SimpleMqttMessage args)
  {
    // save je data mbv je repo
    // Wllicht wil je ook je data opslaan in een database?
    Console.WriteLine($"Incoming MQTT message on {args.Topic}:{args.Message}");

  }
}
