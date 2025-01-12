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
        private readonly InteractionSystem interactionSystem;
        protected readonly ButtonLedController buttonLedController;

        private readonly ObstacleDetectionSystem obstacleDetectionSystem;

        private readonly LoggingSystem loggingSystem;

        protected readonly MqttMessageHandler mqttMessageHandler;

        // Actuators
        private readonly PIRMotion pIRMotion;

        private LCD16x2 LCD;

        public RobotManager() : base()
        {

            pIRMotion = new PIRMotion(18, 100, 15);

            LCD = new LCD16x2(0x3E);

            loggingSystem = new LoggingSystem(LCD);

            buttonLedController = new ButtonLedController(6);

            obstacleDetectionSystem = new ObstacleDetectionSystem(16);

            drivingSystem = new DrivingSystem(obstacleDetectionSystem, loggingSystem);

            interactionSystem = new InteractionSystem(buttonLedController, 10);

            mqttMessageHandler = new MqttMessageHandler();

            _components = new List<IUpdatable>{
                obstacleDetectionSystem,
                buttonLedController,
                drivingSystem,
                interactionSystem
            };


        }

        public void Init()
        {
            // Display welcome message
            loggingSystem.LogToLcd($"Welkom! Ik ben Memento!");
            mqttMessageHandler.Init();


            mqttMessageHandler.OnMessageReceived += (s, e) =>
            {

                string command = e.ToLower();
                // 
                interactionSystem.Query("#");

                // this is for the interaction logic
                if (command.Contains("#"))
                {
                    // Disect till the #
                    // Use switch case

                    // Option: #request id number ("Ben je er nog?")
                }

                /**/
                switch (command)
                {
                    case "start":
                        drivingSystem.DrivingMode = DrivingMode.Autonome;
                        break;
                    case "stop":
                        drivingSystem.DrivingMode = DrivingMode.Idle;
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

            if (interactionSystem.Response != null)
            {
                await mqttMessageHandler.SendMessage(interactionSystem.Response, TopicType.Info);
            }


        }


    }
}
