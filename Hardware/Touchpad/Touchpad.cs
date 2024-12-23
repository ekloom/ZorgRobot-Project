using System;
using System.Device.Gpio;
using System.IO.Ports;
namespace Hardware.Touchpad
{
    class Touchpad
    {
        static SerialPort? serialPort;

        public static char ReadData()
        {
            if (serialPort != null && serialPort.BytesToRead > 0)
            {
                int data = serialPort.ReadByte();
                return data switch
                {
                    0xE1 => '1',
                    0xE2 => '2',
                    0xE3 => '3',
                    0xE4 => '4',
                    0xE5 => '5',
                    0xE6 => '6',
                    0xE7 => '7',
                    0xE8 => '8',
                    0xE9 => '9',
                    0xEA => '*',
                    0xEB => '0',
                    0xEC => '#',
                    _ => ' ',
                };
            }
            return ' ';

        }
        public static void ClosePort()
        {
            serialPort?.Close();
        }

        public static void OpenPort()
        {
            serialPort = new SerialPort("/dev/ttyAMA0", 9600);
            serialPort?.Open();
        }
        public static void PrintData()
        {
            if (serialPort != null && serialPort.BytesToRead > 0)
            {
                int data = serialPort.ReadByte();
                switch (data)
                {
                    case 0xE1:
                        Console.WriteLine("1");
                        break;
                    case 0xE2:
                        Console.WriteLine("2");
                        break;
                    case 0xE3:
                        Console.WriteLine("3");
                        break;
                    case 0xE4:
                        Console.WriteLine("4");
                        break;
                    case 0xE5:
                        Console.WriteLine("5");
                        break;
                    case 0xE6:
                        Console.WriteLine("6");
                        break;
                    case 0xE7:
                        Console.WriteLine("7");
                        break;
                    case 0xE8:
                        Console.WriteLine("8");
                        break;
                    case 0xE9:
                        Console.WriteLine("9");
                        break;
                    case 0xEA:
                        Console.WriteLine("*");
                        break;
                    case 0xEB:
                        Console.WriteLine("0");
                        break;
                    case 0xEC:
                        Console.WriteLine("#");
                        break;
                    default:
                        break;
                }
            }
        }
    }
}