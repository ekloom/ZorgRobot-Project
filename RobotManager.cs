using Avans.StatisticalRobot;
using RobotProject.ControlSystems;
using RobotProject.Controllers;
using RobotProject.Services.Mqtt;
using Microsoft.Extensions.DependencyInjection;
using Speaker;
using RobotProject.Services.Datainterface;
using RobotProject.Communication.Datainterface;


namespace RobotProject
{
    public class RobotManager : IUpdatable
    {
        // private readonly List<IUpdatable> _components;

        // Controllers and systems
        private readonly DrivingSystem _drivingSystem;
        private readonly InteractionSystem _interactionSystem;

        protected readonly ButtonLedController _buttonLedController;
        private readonly ObstacleDetectionSystem _obstacleDetectionSystem;

        private readonly LoggingSystem _loggingSystem;
        private readonly PIRMotion _MotionDetector;
        protected readonly MqttMessageHandler _mqttMessageHandler;
        private readonly CommandHandler _commandHandler;
        private readonly SqlInterface _sqlInterface;
        private readonly SqlTaskRepo sqlTaskRepo;

        private List<AgendaTaskModel> ListOfTask;

        bool robotStarted;

        public RobotManager() : base()
        {

            _MotionDetector = new PIRMotion(18, 100, 15);

            _mqttMessageHandler = new MqttMessageHandler();

            _loggingSystem = new LoggingSystem(AppConfig.Host.Services.GetService<LCD16x2>());

            _buttonLedController = new ButtonLedController(AppConfig.Host.Services.GetService<Button>(), AppConfig.Host.Services.GetService<Led>());

            _obstacleDetectionSystem = new ObstacleDetectionSystem(AppConfig.Host.Services.GetService<Ultrasonic>());

            _drivingSystem = new DrivingSystem(_obstacleDetectionSystem, _loggingSystem, _MotionDetector);

            _interactionSystem = new InteractionSystem(_buttonLedController, 10);

            _commandHandler = new CommandHandler();

            _sqlInterface = new SqlInterface();

            sqlTaskRepo = new SqlTaskRepo();

            // _components = new List<IUpdatable>{
            //     _obstacleDetectionSystem,
            //     _buttonLedController,
            //     _drivingSystem,
            //     _interactionSystem
            // };


        }

        public async Task Init()
        {
            // Display welcome message
            _loggingSystem.LogToLcd($"Welkom! Ik ben Memento!");
            await _mqttMessageHandler.Init();



            ListOfTask = sqlTaskRepo.UpdateList();

            _commandHandler.AddCommand("start", () =>
            {
                _drivingSystem.DrivingMode = DrivingMode.Autonome;
                robotStarted = true;
            });
            _commandHandler.AddCommand("stop", () =>
            {
                robotStarted = false;
                _drivingSystem.DrivingMode = DrivingMode.Idle;
            });
            _commandHandler.AddCommand("reset", () =>
            {
                _drivingSystem.Reset();
                _loggingSystem.LogToLcd($"Welkom! Ik ben Memento!");
                robotStarted = false;
            });

            _commandHandler.AddCommand("update", () =>
            {
                ListOfTask = sqlTaskRepo.UpdateList();
            });


            _mqttMessageHandler.OnMessageReceived += (s, e) =>
            {

                string command = e.ToLower();

                // _interactionSystem.Query("#");

                // this is for the interaction logic
                if (command.Contains("#"))
                {
                    // Disect till the #

                    // Option: #request id number ("Ben je er nog?")
                }
                else
                {
                    _commandHandler.ExecuteCommand(command);
                }

            };
        }



        private DateTime _lastAlarmTime = DateTime.MinValue; // Tracks last alarm time
        private DateTime _lastFlickerTime = DateTime.MinValue; // Tracks last flicker time
        private bool _isLedOn = false; // Tracks LED state

        private DateTime _lastNotificationTime = DateTime.MinValue;
        private bool _isNotificationActive = false;
        private DateTime _timeoutEndTime = DateTime.MinValue;


        AgendaTaskModel activeTask = null; // Houd de actieve taak bij

        bool sentAlert = false;

        public void Update()
        {
            _obstacleDetectionSystem.Update();
            _buttonLedController.Update();
            _interactionSystem.Update();
            _drivingSystem.Update();
            _loggingSystem.Update();

            if (_buttonLedController.GetButtonStatus().isButtonPressed)
            {
                _drivingSystem.DrivingMode = DrivingMode.Idle;
            }
            else if (robotStarted)
            {
                _drivingSystem.DrivingMode = DrivingMode.Autonome;
            }

            DateTime now = DateTime.Now;


            foreach (var task in ListOfTask)
            {
                // Combine the task's date and time
                DateTime taskStart = task.Date.Date + task.StartTime.TimeOfDay;
                DateTime taskEnd = task.Date.Date + task.EndTime.TimeOfDay;

                if (now >= taskStart && now <= taskEnd && now >= _timeoutEndTime)
                {
                    // Start the notification period if not already active
                    if (!_isNotificationActive)
                    {
                        _isNotificationActive = true;
                        activeTask = task;

                        // Add task title to LCD queue
                        _loggingSystem.LogToLcd($"{task.Title} Druk twee keer op de knop voor de bevestiging", true);
                    }

                    // Play the notification sound every 5 seconds
                    if (_isNotificationActive && (now - _lastNotificationTime).TotalSeconds >= 5)
                    {
                        Robot.PlayNotes("g>g>a");
                        _lastNotificationTime = now;
                    }

                    // Laat de LED knipperen
                    if (_isNotificationActive && (now - _lastFlickerTime).TotalMilliseconds >= 500)
                    {
                        _isLedOn = !_isLedOn;
                        _buttonLedController.LedOn(_isLedOn);
                        _lastFlickerTime = now; // Update de flicker-tijd
                    }
                }
            }


            // Controleer of er twee keer op de knop is gedrukt
            if (_buttonLedController.GetButtonStatus().TimesPressed >= 2 && _isNotificationActive && activeTask != null)
            {
                // Stop de notificatie en reset de robot
                _isNotificationActive = false;
                _buttonLedController.LedOn(false); // Zet de LED uit
                _drivingSystem.Reset();
                _loggingSystem.LogToLcd($"Welkom! Ik ben Memento!");
                robotStarted = false;
                _timeoutEndTime = DateTime.Now.AddMinutes(5); // Stel de timeout in

                // Verzend de taak via MQTT
                string mqttMessage = $"Taak geaccepteerd: {activeTask.Title} van {activeTask.StartTime:HH:mm} tot {activeTask.EndTime:HH:mm} op {activeTask.Date:yyyy-MM-dd}";

                _mqttMessageHandler.SendMessage(mqttMessage, TopicType.Info);
            }


            if (_buttonLedController.GetButtonStatus().PressingDuration >= 1000)
            {
                int duration = _buttonLedController.GetButtonStatus().PressingDuration;
                int flickerInterval = 500;
                bool isLedOn = (duration / flickerInterval) % 2 == 0;
                _buttonLedController.LedOn(isLedOn);

                if (_buttonLedController.GetButtonStatus().PressingDuration >= 3000)
                {
                    _buttonLedController.LedOn(isLedOn);
                    _drivingSystem.EmergencyStop();
                    _loggingSystem.LogToLcd("De Noodstop is ingedrukt!");

                    // Generate a unique ID
                    var generatedID = Guid.NewGuid();

                    var alertModel = new AlertModel()
                    {
                        Date = DateTime.Now,
                        Id = generatedID,
                        Title = "Noodstop"
                    };

                    if (!sentAlert)
                    {
                        sentAlert = true;
                        _sqlInterface.SaveData<AlertModel>("Alerts", alertModel);
                        _mqttMessageHandler.SendMessage($"msgID:{generatedID}", TopicType.Alert);
                    }


                }
            }

            if (_MotionDetector.Watch() == 1)
            {
                _drivingSystem.SetSpeed(0.1);
            }
            else
            {
                _drivingSystem.SetSpeed();
            }

            if (_drivingSystem.isEmergencyStop)
            {
                if ((now - _lastFlickerTime).TotalMilliseconds >= 500)
                {
                    _isLedOn = !_isLedOn;
                    _buttonLedController.LedOn(_isLedOn);
                    Robot.LEDs((byte)(_isLedOn ? 255 : 0), 0, 0);
                    _lastFlickerTime = now;
                }

                if ((now - _lastAlarmTime).TotalMilliseconds >= 3000)
                {
                    Robot.PlayNotes("c>c<c>c<c>c<c");
                    _lastAlarmTime = now;
                }
            }

            if (_interactionSystem.Response != null)
            {
                _mqttMessageHandler.SendMessage(_interactionSystem.Response, TopicType.Info);
            }
        }






    }
}
