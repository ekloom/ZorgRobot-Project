using Avans.StatisticalRobot;
using RobotProject.ControlSystems;
using RobotProject.Controllers;
using RobotProject.Services.Mqtt;


namespace RobotProject
{
    public class RobotManager : IUpdatable
    {
        private readonly List<IUpdatable> _components;

        // Controllers and systems

        private readonly DrivingSystem drivingSystem;
        protected readonly ButtonLedController buttonLedController;

        private readonly ObstacleDetectionSystem obstacleDetectionSystem;

        protected readonly MqttMessageHandler mqttMessageHandler;

        // Actuators
        private readonly LCD16x2 lCD16X2;
        private readonly PIRMotion pIRMotion;

        private readonly string robotName;

        public RobotManager()
        {

            lCD16X2 = new LCD16x2(0x3E);
            pIRMotion = new PIRMotion(18, 100, 15);


            // Initialize components

            buttonLedController = new ButtonLedController(6);

            obstacleDetectionSystem = new ObstacleDetectionSystem(16);

            drivingSystem = new DrivingSystem(lCD16X2, obstacleDetectionSystem);

            mqttMessageHandler = new MqttMessageHandler();
            _components = new List<IUpdatable>{
                obstacleDetectionSystem,
                buttonLedController,
                drivingSystem
            };

            robotName = "Memento";
        }

        public void Init()
        {
            // Display welcome message
            lCD16X2.SetText($"Welkom! Ik ben {robotName}!");
            mqttMessageHandler.Init();


            mqttMessageHandler.OnMessageReceived += (s, e) =>
            {
                switch (e.ToLower())
                {
                    case "start":
                        drivingSystem.IsFollowingTarget = true;
                        break;
                    case "stop":
                        drivingSystem.IsFollowingTarget = false;
                        drivingSystem.Stop();
                        break;
                    case "reset":
                        drivingSystem.Reset();
                        break;
                }
            };
        }


        public async void Update()
        {
            // Perform component updates
            foreach (var component in _components)
            {
                component.Update();
            }

            // System.Console.WriteLine(pIRMotion.Watch());

            if (buttonLedController.GetButtonStatus().PressingDuration >= 1000)
            {
                // Start met aftellen


                if (buttonLedController.GetButtonStatus().PressingDuration >= 3000)
                {
                    drivingSystem.EmergencyStop();
                    await mqttMessageHandler.SendMessage("De noodstopknop is ingedrukt", TopicType.Alert);
                }

            }

        }


    }
}
