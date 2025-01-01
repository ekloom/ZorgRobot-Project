using System;
using System.Diagnostics;
using System.Reflection;
using System.Device.Gpio;
using System.Device.I2c;
using GyroscopeCompass;
using Avans.StatisticalRobot;
using GyroscopeCompass.GyroscopeCompass;

using RobotProject;


var RobotManager = new RobotManager();

RobotManager.Init();

// Thread.Sleep(5000);
Robot.Wait(5000);
while (true)
{
    RobotManager.Update();

    Robot.Wait(200);
}