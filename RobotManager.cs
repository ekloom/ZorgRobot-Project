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
        private readonly CommandHandler commandHandler;
        protected readonly ButtonLedController buttonLedController;
        private readonly ObstacleDetectionSystem obstacleDetectionSystem;

        private readonly LoggingSystem loggingSystem;

        protected readonly MqttMessageHandler mqttMessageHandler;


        bool isInitializing;

        public RobotManager() : base()
        {

            // pIRMotion = new PIRMotion(18, 100, 15);

            mqttMessageHandler = new MqttMessageHandler();

            loggingSystem = new LoggingSystem(0x3E);

            buttonLedController = new ButtonLedController(6);

            obstacleDetectionSystem = new ObstacleDetectionSystem(16);

            drivingSystem = new DrivingSystem(obstacleDetectionSystem, loggingSystem);

            interactionSystem = new InteractionSystem(buttonLedController, 10);

            commandHandler = new CommandHandler();


            _components = new List<IUpdatable>{
                obstacleDetectionSystem,
                buttonLedController,
                drivingSystem,
                interactionSystem
            };


        }

        public async Task Init()
        {
            // Display welcome message
            loggingSystem.LogToLcd($"Welkom! Ik ben Memento!");
            await mqttMessageHandler.Init();

            isInitializing = true;


            commandHandler.AddCommand("start", () => drivingSystem.DrivingMode = DrivingMode.Autonome);
            commandHandler.AddCommand("stop", () => drivingSystem.DrivingMode = DrivingMode.Idle);
            commandHandler.AddCommand("reset", () => drivingSystem.Reset());


            mqttMessageHandler.OnMessageReceived += (s, e) =>
            {

                string command = e.ToLower();

                interactionSystem.Query("#");

                // this is for the interaction logic
                if (command.Contains("#"))
                {
                    // Disect till the #

                    // Option: #request id number ("Ben je er nog?")
                }
                else
                {
                    commandHandler.ExecuteCommand(command);
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

            if (isInitializing)
            {
                // initializing manouvre


                isInitializing = false;
            }

            if (interactionSystem.Response != null)
            {
                await mqttMessageHandler.SendMessage(interactionSystem.Response, TopicType.Info);
            }
        }


    }
}
