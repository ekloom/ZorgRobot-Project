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

        private readonly LoggingSystem loggingSystem;

        protected readonly MqttMessageHandler mqttMessageHandler;

        // Actuators
        private readonly PIRMotion pIRMotion;

        private LCD16x2 LCD;

        private readonly string robotName;

        public RobotManager()
        {

            pIRMotion = new PIRMotion(18, 100, 15);

            LCD = new LCD16x2(0x3E);

            loggingSystem = new LoggingSystem(LCD);

            buttonLedController = new ButtonLedController(6);

            obstacleDetectionSystem = new ObstacleDetectionSystem(16);

            drivingSystem = new DrivingSystem(obstacleDetectionSystem, loggingSystem);



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
            loggingSystem.LogToLcd($"Welkom! Ik ben {robotName}!");
            mqttMessageHandler.Init();


            mqttMessageHandler.OnMessageReceived += (s, e) =>
            {
                switch (e.ToLower())
                {
                    case "start":
                        drivingSystem.IsFollowingPerson = true;
                        break;
                    case "stop":
                        drivingSystem.IsFollowingPerson = false;
                        drivingSystem.Stop();
                        break;
                    case "reset":
                        drivingSystem.Reset();
                        break;
                }
            };
        }




        public async virtual void Update()
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
                    loggingSystem.LogToLcd("De Noodstop is ingedrukt!");
                    await mqttMessageHandler.SendMessage("Noodstop", TopicType.Alert);
                }

            }

            // if (pIRMotion.Watch() == 1)
            // {
            //     if (drivingSystem.HasPerformedScan && !drivingSystem.IsPersonFound)
            //     {
            //         drivingSystem.StartScanning();
            //     }
            // }
            // else
            // {
            //     // Idle timer: 10 minuten geen motion , {naam}, bent u er nog?
            // }

        }


    }
}
