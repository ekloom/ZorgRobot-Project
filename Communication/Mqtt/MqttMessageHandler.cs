namespace RobotProject.Services.Mqtt;

public class MqttMessageHandler
{
    private readonly SimpleMqttClient _mqttClient;

    public event EventHandler<string>? OnMessageReceived;

    public MqttMessageHandler()
    {

        var data = AppConfig.Configuration;

        _mqttClient = new SimpleMqttClient(new SimpleMqttClientConfiguration
        {
            Host = data["Host"],
            Port = Convert.ToInt16(data["Port"]),
            CleanStart = false,
            ClientId = "RobotProject_Robot",
            TimeoutInMs = 5_000,
            UserName = data["UserName"],
            Password = data["Password"],
        });

        _mqttClient.OnMessageReceived += HandleMessage;

    }

    public async Task SendMessage(string messageID, TopicType topicType)
    {
        string topic = "";

        switch (topicType)
        {
            case TopicType.Alert:
                topic = $"/Alert";
                break;
            case TopicType.Info:
                topic = $"/Info";
                break;
        }

        System.Console.WriteLine($"Message being send to MQTT: Topic = {topic}, msgID = {messageID}");
        await _mqttClient.PublishMessage(messageID, topic);
    }

    public async Task SendMessage(string message)
    {
        string topic = "Info";

        System.Console.WriteLine($"Message being send to MQTT: Topic = {topic}, msg = {message}");
        await _mqttClient.PublishMessage(message, topic);
    }

    public async Task Init()
    {
        await _mqttClient.SubscribeToTopic("/Command");
        await SendMessage("Memento is online!");
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
