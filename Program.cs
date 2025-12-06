using Avans.StatisticalRobot;
using RobotProject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


// Create the Host for dependency injection and configuration
AppConfig.Host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddUserSecrets<Program>();
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IConfiguration>(provider => context.Configuration);

        services.AddSingleton<PIRMotion>(O => new PIRMotion(18, 100, 15));
        services.AddSingleton<LCD16x2>(O => new LCD16x2(0x3E));
        services.AddSingleton<Button>(O => new Button(6));
        services.AddSingleton<Led>(O => new Led(5));
        services.AddSingleton<Ultrasonic>(O => new Ultrasonic(16));

        // services.AddSingleton<MqttMessageHandler>();
        // services.AddHostedService(provider => provider.GetService<MqttMessageHandler>());
        // services.AddSingleton<DrivingSystem>();
        // services.AddSingleton<LoggingSystem>();
        // services.AddSingleton<ButtonLedController>();
        // services.AddSingleton<ObstacleDetectionSystem>();
        // services.AddSingleton<InteractionSystem>(o => new InteractionSystem(o.GetService<ButtonLedController>(), 10));
        // services.AddSingleton<CommandHandler>();
        // services.AddSingleton<RobotManager>();
    })
    .Build();

// Access the loaded configuration
AppConfig.Configuration = AppConfig.Host.Services.GetRequiredService<IConfiguration>();


Robot.PlayNotes("a>b<a");

var robotManager = new RobotManager();

await robotManager.Init();

Robot.Wait(5000);
while (true)
{
    robotManager.Update();

    Robot.Wait(100);
}
