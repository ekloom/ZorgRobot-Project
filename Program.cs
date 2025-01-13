using Avans.StatisticalRobot;
using RobotProject;
using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RobotProject.Services.Mqtt;
using System.Diagnostics;
using System.Device.Gpio;
using System.Device.I2c;
using GyroscopeCompass;
using GyroscopeCompass.GyroscopeCompass;



// var builder = new ConfigurationBuilder()
// .SetBasePath(Directory.GetCurrentDirectory())
//             .AddUserSecrets<Program>()
//             ;

// IConfiguration config = builder.Build();


// System.Console.WriteLine(config.GetSection("HiveMQ").Exists());

var RobotManager = new RobotManager();

await RobotManager.Init();

Robot.Wait(5000);
while (true)
{
    RobotManager.Update();

    Robot.Wait(100);
}