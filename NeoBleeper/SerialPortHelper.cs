// NeoBleeper - AI-enabled tune creation software using the system speaker (aka PC Speaker) on the motherboard
// Copyright (C) 2023 GeniusPilot2016
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using System.IO.Ports;

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
        public static string ArduinoFirmwareCode = "#include <Arduino.h>\r\n\r\nvoid setup() {\r\n  Serial.begin(9600);\r\n  pinMode(8, OUTPUT); // Buzzer pin\r\n  pinMode(9, OUTPUT); // Motor direction pin\r\n  pinMode(10, OUTPUT); // Motor step pin\r\n}\r\n\r\nvoid loop() {\r\n  if (Serial.available() > 0) {\r\n    String command = Serial.readStringUntil('\\n');\r\n    command.trim();\r\n\r\n    if (command == \"IDENTIFY\") {\r\n      Serial.println(\"Arduino\");\r\n    } \r\n    else if (command.startsWith(\"MOTOR:\")) {\r\n      int rpm = command.substring(6).toInt();\r\n      if (rpm > 0) {\r\n        // Convert RPM to step delay\r\n        unsigned long stepDelay = 60000000UL / (rpm * 200); // 200 steps per revolution\r\n        \r\n        // Set motor direction\r\n        digitalWrite(9, HIGH);\r\n        \r\n        // Start stepping motor\r\n        for (int i = 0; i < 200; i++) {\r\n          digitalWrite(10, HIGH);\r\n          delayMicroseconds(stepDelay / 2);\r\n          digitalWrite(10, LOW);\r\n          delayMicroseconds(stepDelay / 2);\r\n        }\r\n        \r\n        Serial.print(\"Motor running at RPM: \");\r\n        Serial.println(rpm);\r\n      }\r\n    } \r\n    else if (command.startsWith(\"BUZZER:\")) {\r\n      int frequency = command.substring(7).toInt();\r\n      if (frequency > 0 && frequency <= 20000) {\r\n        tone(8, frequency);\r\n        Serial.print(\"Buzzer playing at frequency: \");\r\n        Serial.println(frequency);\r\n      }\r\n    } \r\n    else if (command == \"STOP:MOTOR\") {\r\n      digitalWrite(9, LOW);\r\n      digitalWrite(10, LOW);\r\n      Serial.println(\"Motor stopped\");\r\n    } \r\n    else if (command == \"STOP:BUZZER\") {\r\n      noTone(8);\r\n      Serial.println(\"Buzzer stopped\");\r\n    }\r\n  }\r\n}";
        public static string RaspberryPiFirmwareCode = "import serial\r\nimport time\r\nimport threading\r\nimport RPi.GPIO as GPIO\r\nfrom gpiozero import PWMOutputDevice\r\n\r\n# GPIO setup\r\nGPIO.setmode(GPIO.BCM)\r\nBUZZER_PIN = 18\r\nMOTOR_STEP_PIN = 20\r\nMOTOR_DIR_PIN = 21\r\n\r\nGPIO.setup(MOTOR_STEP_PIN, GPIO.OUT)\r\nGPIO.setup(MOTOR_DIR_PIN, GPIO.OUT)\r\n\r\n# PWM for buzzer\r\nbuzzer = PWMOutputDevice(BUZZER_PIN)\r\n\r\n# Global state\r\nmotor_running = False\r\nbuzzer_running = False\r\nmotor_thread = None\r\nbuzzer_thread = None\r\n\r\ndef motor_control(rpm):\r\n    global motor_running\r\n    if rpm <= 0:\r\n        return\r\n    \r\n    steps_per_rev = 200\r\n    step_delay = 60.0 / (rpm * steps_per_rev)\r\n    \r\n    GPIO.output(MOTOR_DIR_PIN, GPIO.HIGH)\r\n    \r\n    while motor_running:\r\n        GPIO.output(MOTOR_STEP_PIN, GPIO.HIGH)\r\n        time.sleep(step_delay / 2)\r\n        GPIO.output(MOTOR_STEP_PIN, GPIO.LOW)\r\n        time.sleep(step_delay / 2)\r\n\r\ndef handle_command(command):\r\n    global motor_running, buzzer_running, motor_thread, buzzer_thread\r\n    \r\n    if command == \"IDENTIFY\":\r\n        return \"Raspberry Pi\"\r\n    \r\n    elif command.startswith(\"MOTOR:\"):\r\n        try:\r\n            rpm = int(command.split(\":\")[1])\r\n            if motor_running:\r\n                motor_running = False\r\n                if motor_thread:\r\n                    motor_thread.join()\r\n            \r\n            motor_running = True\r\n            motor_thread = threading.Thread(target=motor_control, args=(rpm,))\r\n            motor_thread.start()\r\n            return f\"Motor running at RPM: {rpm}\"\r\n        except ValueError:\r\n            return \"Invalid RPM value\"\r\n    \r\n    elif command.startswith(\"BUZZER:\"):\r\n        try:\r\n            frequency = int(command.split(\":\")[1])\r\n            if 0 < frequency <= 20000:\r\n                buzzer_running = True\r\n                buzzer.frequency = frequency\r\n                buzzer.value = 0.5  # 50% duty cycle\r\n                return f\"Buzzer playing at frequency: {frequency}\"\r\n            else:\r\n                return \"Frequency out of range\"\r\n        except ValueError:\r\n            return \"Invalid frequency value\"\r\n    \r\n    elif command == \"STOP:MOTOR\":\r\n        motor_running = False\r\n        if motor_thread:\r\n            motor_thread.join()\r\n        GPIO.output(MOTOR_STEP_PIN, GPIO.LOW)\r\n        GPIO.output(MOTOR_DIR_PIN, GPIO.LOW)\r\n        return \"Motor stopped\"\r\n    \r\n    elif command == \"STOP:BUZZER\":\r\n        buzzer_running = False\r\n        buzzer.off()\r\n        return \"Buzzer stopped\"\r\n    \r\n    return \"Unknown command\"\r\n\r\ndef cleanup():\r\n    global motor_running, buzzer_running\r\n    motor_running = False\r\n    buzzer_running = False\r\n    buzzer.close()\r\n    GPIO.cleanup()\r\n\r\nif __name__ == \"__main__\":\r\n    try:\r\n        ser = serial.Serial('/dev/ttyS0', 9600, timeout=1)\r\n        ser.flush()\r\n        \r\n        print(\"Raspberry Pi ready for commands\")\r\n        \r\n        while True:\r\n            if ser.in_waiting > 0:\r\n                command = ser.readline().decode('utf-8').strip()\r\n                if command:\r\n                    response = handle_command(command)\r\n                    ser.write((response + \"\\n\").encode('utf-8'))\r\n                    \r\n    except KeyboardInterrupt:\r\n        print(\"Shutting down...\")\r\n    except Exception as e:\r\n        print(f\"Error: {e}\")\r\n    finally:\r\n        cleanup()";
        public static string ESP32FirmwareCode = "#include <Arduino.h>\r\n\r\n// Pin definitions\r\n#define BUZZER_PIN 25\r\n#define MOTOR_STEP_PIN 26\r\n#define MOTOR_DIR_PIN 27\r\n#define BUZZER_CHANNEL 0\r\n\r\n// Global variables\r\nbool motorRunning = false;\r\nbool buzzerRunning = false;\r\nTaskHandle_t motorTaskHandle = NULL;\r\n\r\nvoid motorTask(void* parameter) {\r\n  int rpm = *(int*)parameter;\r\n  delete (int*)parameter;\r\n  \r\n  if (rpm <= 0) {\r\n    vTaskDelete(NULL);\r\n    return;\r\n  }\r\n  \r\n  int stepsPerRev = 200;\r\n  unsigned long stepDelay = (60UL * 1000000UL) / (rpm * stepsPerRev); // in microseconds\r\n  \r\n  digitalWrite(MOTOR_DIR_PIN, HIGH);\r\n  \r\n  while (motorRunning) {\r\n    digitalWrite(MOTOR_STEP_PIN, HIGH);\r\n    delayMicroseconds(stepDelay / 2);\r\n    digitalWrite(MOTOR_STEP_PIN, LOW);\r\n    delayMicroseconds(stepDelay / 2);\r\n  }\r\n  \r\n  digitalWrite(MOTOR_STEP_PIN, LOW);\r\n  motorTaskHandle = NULL;\r\n  vTaskDelete(NULL);\r\n}\r\n\r\nvoid setup() {\r\n  Serial.begin(9600);\r\n  \r\n  // Initialize pins\r\n  pinMode(BUZZER_PIN, OUTPUT);\r\n  pinMode(MOTOR_STEP_PIN, OUTPUT);\r\n  pinMode(MOTOR_DIR_PIN, OUTPUT);\r\n  \r\n  // Initialize PWM for buzzer\r\n  ledcSetup(BUZZER_CHANNEL, 1000, 8); // 1000 Hz, 8-bit resolution\r\n  ledcAttachPin(BUZZER_PIN, BUZZER_CHANNEL);\r\n  \r\n  Serial.println(\"ESP32 ready for commands\");\r\n}\r\n\r\nvoid loop() {\r\n  if (Serial.available() > 0) {\r\n    String command = Serial.readStringUntil('\\n');\r\n    command.trim();\r\n\r\n    if (command == \"IDENTIFY\") {\r\n      Serial.println(\"ESP32\");\r\n    } \r\n    else if (command.startsWith(\"MOTOR:\")) {\r\n      int rpm = command.substring(6).toInt();\r\n      \r\n      // Stop existing motor task\r\n      if (motorTaskHandle != NULL) {\r\n        motorRunning = false;\r\n        vTaskDelay(pdMS_TO_TICKS(10)); // Wait for task to finish\r\n      }\r\n      \r\n      if (rpm > 0) {\r\n        motorRunning = true;\r\n        int* rpmPtr = new int(rpm);\r\n        xTaskCreate(motorTask, \"MotorTask\", 2048, rpmPtr, 1, &motorTaskHandle);\r\n        Serial.print(\"Motor running at RPM: \");\r\n        Serial.println(rpm);\r\n      }\r\n    } \r\n    else if (command.startsWith(\"BUZZER:\")) {\r\n      int frequency = command.substring(7).toInt();\r\n      \r\n      if (frequency > 0 && frequency <= 20000) {\r\n        buzzerRunning = true;\r\n        ledcSetup(BUZZER_CHANNEL, frequency, 8);\r\n        ledcWrite(BUZZER_CHANNEL, 128); // 50% duty cycle\r\n        Serial.print(\"Buzzer playing at frequency: \");\r\n        Serial.println(frequency);\r\n      }\r\n    } \r\n    else if (command == \"STOP:MOTOR\") {\r\n      motorRunning = false;\r\n      if (motorTaskHandle != NULL) {\r\n        vTaskDelay(pdMS_TO_TICKS(10)); // Wait for task to finish\r\n      }\r\n      digitalWrite(MOTOR_STEP_PIN, LOW);\r\n      digitalWrite(MOTOR_DIR_PIN, LOW);\r\n      Serial.println(\"Motor stopped\");\r\n    } \r\n    else if (command == \"STOP:BUZZER\") {\r\n      buzzerRunning = false;\r\n      ledcWrite(BUZZER_CHANNEL, 0); // Stop PWM\r\n      ledcDetachPin(BUZZER_PIN);\r\n      Serial.println(\"Buzzer stopped\");\r\n    }\r\n  }\r\n}";
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
                        port.ReadTimeout = 1000; // 1 second timeout
                        port.Open();
                        port.WriteLine("IDENTIFY");
                        try
                        {
                            string response = port.ReadLine();
                            if (response.Contains("Arduino") || response.Contains("Raspberry Pi") || response.Contains("ESP32"))
                            {
                                return true;
                            }
                        }
                        catch (TimeoutException)
                        {
                            // Skip if no response
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
                frequency *= (int)Math.Pow(2, TemporarySettings.MicrocontrollerSettings.stepperMotorOctave - 2);
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

                    try
                    {
                        port.WriteLine($"MOTOR:{RPM}");
                    }
                    catch (TimeoutException)
                    {
                        Logger.Log("Motor command write timed out.", Logger.LogTypes.Error);
                        lock (_lockObject)
                        {
                            _isMotorPlaying = false;
                        }
                        return;
                    }

                    HighPrecisionSleep.Sleep(duration);
                    if (!nonStopping)
                    {
                        StopMotorSound().Wait();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error playing motor sound: {ex.Message}", Logger.LogTypes.Error);
                    if (!IsAnyPortThatIsMicrocontrollerAvailable())
                    {
                        TemporarySettings.MicrocontrollerSettings.useMicrocontroller = false; // Disable microcontroller usage if none is available
                    }
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
                frequency *= (int)Math.Pow(2, TemporarySettings.MicrocontrollerSettings.stepperMotorOctave - 2);
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

                    try
                    {
                        port.WriteLine($"BUZZER:{frequency}");
                    }
                    catch (TimeoutException)
                    {
                        Logger.Log("Buzzer command write timed out.", Logger.LogTypes.Error);
                        lock (_lockObject)
                        {
                            _isBuzzerPlaying = false;
                        }
                        return;
                    }

                    HighPrecisionSleep.Sleep(duration);
                    if (!nonStopping)
                    {
                        StopBuzzerSound().Wait();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error playing buzzer sound: {ex.Message}", Logger.LogTypes.Error);
                    if (!IsAnyPortThatIsMicrocontrollerAvailable())
                    {
                        TemporarySettings.MicrocontrollerSettings.useMicrocontroller = false; // Disable microcontroller usage if none is available
                    }
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
                    try
                    {
                        port.WriteLine("STOP:MOTOR");
                    }
                    catch (TimeoutException)
                    {
                        Logger.Log("Motor stopping command timed out.", Logger.LogTypes.Error);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error stopping motor sound: {ex.Message}", Logger.LogTypes.Error);
                    if (!IsAnyPortThatIsMicrocontrollerAvailable())
                    {
                        TemporarySettings.MicrocontrollerSettings.useMicrocontroller = false; // Disable microcontroller usage if none is available
                    }
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
                    try
                    {
                        port.WriteLine("STOP:BUZZER");
                    }
                    catch (TimeoutException)
                    {
                        Logger.Log("Buzzer stopping command timed out.", Logger.LogTypes.Error);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error stopping buzzer sound: {ex.Message}", Logger.LogTypes.Error);
                    if (!IsAnyPortThatIsMicrocontrollerAvailable())
                    {
                        TemporarySettings.MicrocontrollerSettings.useMicrocontroller = false; // Disable microcontroller usage if none is available
                    }
                }
            });
        }
    }
}