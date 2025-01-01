
using RobotProject.Util;

namespace RobotProject.ControlSystems
{
    internal class MotorController : IUpdatable
    {

        private bool isEmergencyStop;

        private short CurrentMotorSpeedL;
        private short CurrentMotorSpeedR;

        private MotorMode _motorMode;

        short _targetSpeedR;
        short _targetSpeedL;

        public MotorController()
        {
            _motorMode = MotorMode.stop;
        }


        public void Stop()
        {
            _motorMode = MotorMode.stop;
        }

        public void EmergencyStop()
        {
            isEmergencyStop = true;
            _motorMode = MotorMode.stop;
            MotorFuntions.SetMotorSpeed(0, 0);
        }

        public void SetTargetSpeed(MotorMode motorMode, short targetSpeedR, short targetSpeedL)
        {
            _targetSpeedR = targetSpeedR;
            _targetSpeedL = targetSpeedL;
            _motorMode = motorMode;
        }


        public void Update()
        {
            // Handle periodic updates if necessary
            if (!isEmergencyStop)
            {

                switch (_motorMode)
                {
                    case MotorMode.stop:
                        if (CurrentMotorSpeedL != 0 || CurrentMotorSpeedR != 0)
                        {
                            MotorFuntions.EaseOutMotors(ref CurrentMotorSpeedL, ref CurrentMotorSpeedR, 0, 0);
                        }
                        else
                        {
                            MotorFuntions.SetMotorSpeed(CurrentMotorSpeedL, CurrentMotorSpeedR);
                        }
                        break;
                    case MotorMode.Forward:
                        if (CurrentMotorSpeedL != _targetSpeedL || CurrentMotorSpeedR != _targetSpeedR)
                        {
                            MotorFuntions.EaseOutMotors(ref CurrentMotorSpeedL, ref CurrentMotorSpeedR, _targetSpeedL, _targetSpeedR);
                        }
                        else
                        {
                            MotorFuntions.SetMotorSpeed(CurrentMotorSpeedL, CurrentMotorSpeedR);
                        }
                        break;
                    case MotorMode.Backwards:
                        if (CurrentMotorSpeedL != 0 || CurrentMotorSpeedR != 0)
                        {
                            MotorFuntions.EaseOutMotors(ref CurrentMotorSpeedL, ref CurrentMotorSpeedR, 0, 0);
                        }

                        if (CurrentMotorSpeedL != _targetSpeedL || CurrentMotorSpeedR != _targetSpeedR)
                        {
                            MotorFuntions.EaseOutMotors(ref CurrentMotorSpeedL, ref CurrentMotorSpeedR, _targetSpeedL, _targetSpeedR);
                        }
                        else
                        {
                            MotorFuntions.SetMotorSpeed(CurrentMotorSpeedL, CurrentMotorSpeedR);
                        }
                        break;

                }
            }
        }
    }
    public enum MotorMode
    {
        stop = 0,
        Forward,
        Backwards,
        idle

    }


}
