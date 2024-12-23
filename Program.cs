using Avans.StatisticalRobot;
using GyroscopeCompass.GyroscopeCompass;
using Hardware.Touchpad;
using Speaker;
using System.Device.I2c;
using RobotProject;


var RobotManager = new RobotManager();

// Thread.Sleep(5000);
Robot.Wait(5000);
while (true)
{
    RobotManager.Update();

    Robot.Wait(200);
}