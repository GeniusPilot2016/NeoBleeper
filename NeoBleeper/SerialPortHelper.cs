using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace NeoBleeper
{
    public static class SerialPortHelper
    {
        private static SerialPort _activePort;
        private static readonly object _lockObject = new object();
        private static bool _isMotorPlaying = false;
        private static bool _isBuzzerPlaying = false;
        private static CancellationTokenSource _motorCts;
        private static CancellationTokenSource _buzzerCts;

        public static string[] GetAvailablePortNames()
        {
            return SerialPort.GetPortNames();
        }

        public static string[] GetPortNames()
        {
            return SerialPort.GetPortNames();
        }

        public static bool IsAnyPortThatIsMicrocontrollerAvailable() // Checks if any connected serial port is a microcontroller (Arduino, Raspberry Pi or ESP32)
        {
            string[] portNames = SerialPort.GetPortNames();
            foreach (string portName in portNames)
            {
                try
                {
                    using (SerialPort port = new SerialPort(portName))
                    {
                        port.Open();
                        // Send a simple command to identify the microcontroller
                        port.WriteLine("IDENTIFY");
                        string response = port.ReadLine();
                        if (response.Contains("Arduino") || response.Contains("Raspberry Pi") || response.Contains("ESP32"))
                        {
                            return true;
                        }
                    }
                }
                catch
                {
                    // Ignore any exceptions and continue checking other ports
                }
            }
            return false;
        }

        private static SerialPort GetConnectedMicrocontroller()
        {
            if (_activePort != null && _activePort.IsOpen)
                return _activePort;

            string[] portNames = SerialPort.GetPortNames();
            foreach (string portName in portNames)
            {
                try
                {
                    SerialPort port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
                    port.ReadTimeout = 1000;
                    port.WriteTimeout = 1000;
                    port.Open();

                    // Send identify command
                    port.WriteLine("IDENTIFY");
                    string response = port.ReadLine();

                    if (response.Contains("Arduino") || response.Contains("Raspberry Pi") || response.Contains("ESP32"))
                    {
                        _activePort = port;
                        return port;
                    }
                    else
                    {
                        port.Close();
                        port.Dispose();
                    }
                }
                catch
                {
                    // Continue to next port
                }
            }

            throw new Exception("No compatible microcontroller found");
        }

        public static async Task PlaySoundUsingMotor(int frequency, int duration, bool nonStopping)
        {
            await Task.Run(() =>
            {
                frequency *= (int)Math.Pow(2, TemporarySettings.MicrocontrollerSettings.stepperMotorOctave);
                int RPM = (int)(frequency / 60.0 * 1000); // Convert frequency in Hz to RPM

                try
                {
                    lock (_lockObject)
                    {
                        if (_isMotorPlaying)
                        {
                            _motorCts?.Cancel();
                        }

                        _motorCts = new CancellationTokenSource();
                        _isMotorPlaying = true;
                    }

                    SerialPort port = GetConnectedMicrocontroller();

                    // Send command to microcontroller
                    port.WriteLine($"MOTOR:{RPM}");

                    // If not non-stopping, wait for completion and then reset state
                    HighPrecisionSleep.Sleep(duration);
                    if (!nonStopping)
                    {
                        StopMotorSound().Wait();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error playing motor sound: {ex.Message}", Logger.LogTypes.Error);
                    lock (_lockObject)
                    {
                        _isMotorPlaying = false;
                    }
                }
            });
        }

        public static async Task PlaySoundUsingBuzzer(int frequency, int duration, bool nonStopping)
        {
            await Task.Run(() =>
            {
                try
                {
                    lock (_lockObject)
                    {
                        if (_isBuzzerPlaying)
                        {
                            _buzzerCts?.Cancel();
                        }

                        _buzzerCts = new CancellationTokenSource();
                        _isBuzzerPlaying = true;
                    }

                    SerialPort port = GetConnectedMicrocontroller();

                    // Send command to microcontroller
                    port.WriteLine($"BUZZER:{frequency}");
                    HighPrecisionSleep.Sleep(duration);
                    if (!nonStopping)
                    {
                        StopBuzzerSound().Wait();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error playing buzzer sound: {ex.Message}", Logger.LogTypes.Error);
                    lock (_lockObject)
                    {
                        _isBuzzerPlaying = false;
                    }
                }
            });
        }

        public static async Task StopMotorSound()
        {
            await Task.Run(() =>
            {
                try
                {
                    lock (_lockObject)
                    {
                        if (!_isMotorPlaying) return;

                        _motorCts?.Cancel();
                        _isMotorPlaying = false;
                    }

                    SerialPort port = GetConnectedMicrocontroller();
                    port.WriteLine("STOP:MOTOR");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error stopping motor sound: {ex.Message}");
                }
            });
        }

        public static async Task StopBuzzerSound()
        {
            await Task.Run(() =>
            {
                try
                {
                    lock (_lockObject)
                    {
                        if (!_isBuzzerPlaying) return;

                        _buzzerCts?.Cancel();
                        _isBuzzerPlaying = false;
                    }

                    SerialPort port = GetConnectedMicrocontroller();
                    port.WriteLine("STOP:BUZZER");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error stopping buzzer sound: {ex.Message}");
                }
            });
        }
    }
}