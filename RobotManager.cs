using Avans.StatisticalRobot;
using RobotProject.ControlSystems;
using RobotProject.Controllers;
using RobotProject.Services.Mqtt;


namespace RobotProject
{
    public class RobotManager : IUpdatable
    {
        // private readonly List<IUpdatable> _components;

        // Controllers and systems

        private readonly DrivingSystem drivingSystem;
        protected readonly ButtonLedController buttonLedController;

        protected readonly MqttMessageHandler mqttMessageHandler;

        // Actuators
        private readonly LCD16x2 lCD16X2;

        private readonly string robotName;

        public RobotManager()
        {

            lCD16X2 = new LCD16x2(0x3E);
            // Initialize components

            buttonLedController = new ButtonLedController(6);

            drivingSystem = new DrivingSystem(lCD16X2);

            // mqttMessageHandler = new MqttMessageHandler();

            robotName = "Memento";
        }

        public void Init()
        {
            // Display welcome message
            lCD16X2.SetText($"Welkom! Ik ben {robotName}!");

            // mqttMessageHandler.OnMessageReceived += (s, e) =>
            // {
            //     switch (e)
            //     {
            //         case "Start":
            //             drivingSystem.FollowTarget();
            //             break;
            //         case "Stop":
            //             drivingSystem.Stop();
            //             break;
            //     }
            // };
        }


        public async void Update()
        {
            // Perform component updates
            buttonLedController.Update();
            drivingSystem.Update();

            if (buttonLedController.GetButtonStatus().TimePressed >= 1000)
            {
                // Start met aftellen
                if (buttonLedController.GetButtonStatus().TimePressed >= 5000)
                {
                    drivingSystem.EmergencyStop();
                }
                // await mqttMessageHandler.SendMessage("De noodstopknop is ingedrukt", TopicType.Alert);
            }

        }


    }
}
