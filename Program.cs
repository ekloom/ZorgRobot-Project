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



var RobotManager = new RobotManager();

await RobotManager.Init();

Robot.Wait(5000);
while (true)
{
    RobotManager.Update();

    Robot.Wait(100);
}