using System;
using System.Net;
using System.Net.Sockets;

namespace CalibrationModel
{
    public enum Axes
    {
        AXIS_YAW = 1,
        AXIS_PITCH = 2,
        AXIS_ROLL = 3
    }

    public enum Commands
    {
        OP_APOS_LD = 0x0109,           // 265
        OP_ASPD = 0x010A,              // 266
        OP_CACC = 0x0130,              // 304
        OP_CSPD = 0x0131,              // 305
        OP_CPOS = 0x0132,              // 306
        OP_UPD = 0x0134,               // 308
        OP_CPR = 0x0138,               // 312
        OP_CPA = 0x0139,               // 313
        OP_SpeedMode = 0x013A,         // 314
        OP_PositionMode = 0x013B,      // 315
        OP_axisOn = 0x013C,            // 316
        OP_axisOff = 0x013D,           // 317
        OP_Reset = 0x013E,             // 318
        OP_isReadyIMU = 0x0601,        // 1537
        OP_getBaseRawYaw = 0x0611,     // 1553 get base yaw value from the IMU sensor
        OP_getBaseRawPitch = 0x0612,   // 1554 get base pitch value from the IMU sensor
        OP_getBaseRawRoll = 0x0613,    // 1555 get base roll value from the IMU sensor
        OP_stabOn = 0x0800,            // 2048
        OP_stabOff = 0x0801,           // 2049
        OP_stabError = 0x0822,         // 2082
        OP_stabSetPoint = 0x082C,      // 2092
        OP_get_cfg_GearRatio = 0x0C10, // 3088
        OP_set_cfg_GearRatio = 0x0C11, // 3089
        OP_get_cfg_MaxSpeed = 0x0C25,  // 3109
        OP_get_cfg_maxAccel = 0x0C42,  // 3138
        OP_setKp = 0x0D04,             // 3332 set Kp value
        OP_setKi = 0x0D05,             // 3333 set Ki value
        OP_savePID = 0x0D07,           // 3335 write current settings of pid to harddrive
        OP_getKp = 0x0D08,             // 3336 read value of Kp
        OP_getKi = 0x0D09,             // 3337 read value of Ki
        OP_loadPID = 0x0D0B,           // 3339 load settings of pid from EEPROM
        OP_updatePID = 0x0D0C,         // 3340 load settings of pid to program
        OP_getErrorsReg = 0x0E01,      // 3585
        OP_getMotorError = 0x0E03,     // 3587
        OP_getSystemError = 0x0E04,    // 3588
        OP_getIMUBaseError = 0x0E06,   // 3590
        OP_TEST_MODE = 0x0A00,
    }

    public static class CalibrationModel
    {
        public static Socket Connect(string ip_address, int port_number)
        {
            try
            {
                IPAddress ipAddr = IPAddress.Parse(ip_address);
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port_number);

                // Creation TCP/IP Socket using Socket Class Costructor 
                Socket socket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    socket.Connect(localEndPoint);
                }

                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }

                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }

                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

                return socket;
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return null;
        }

        public static void Disconnect(Socket socket)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        public static void Movement(Socket sender)
        {
            byte[] messageReceived = new byte[32];

            SendCommandByte(sender, (int)Commands.OP_TEST_MODE, 0, 0, messageReceived, false);
        }

        public static void AxisOn(Socket socket, Axes axis)
        {
            byte[] messageReceived = new byte[32];

            SendCommandByte(socket, (int)Commands.OP_axisOn, (byte)axis, 0, messageReceived, true);
        }

        public static void AxisOff(Socket socket, Axes axis)
        {
            byte[] messageReceived = new byte[32];

            SendCommandByte(socket, (int)Commands.OP_axisOff, (byte)axis, 0, messageReceived, true);
        }

        public static void StabOn(Socket socket)
        {
            byte[] messageReceived = new byte[32];

            SendCommandByte(socket, (int)Commands.OP_stabOn, (byte)Axes.AXIS_PITCH, 0, messageReceived, true);
        }

        public static void StabOff(Socket socket)
        {
            byte[] messageReceived = new byte[32];

            SendCommandByte(socket, (int)Commands.OP_stabOff, (byte)Axes.AXIS_PITCH, 0, messageReceived, true);
        }

        public static void TestMotors(Socket sender, Axes axis)
        {
            byte[] messageReceived = new byte[32];

            SendCommandByte(sender, (int)Commands.OP_axisOn, (byte)axis, 0, messageReceived, true);

            SendCommandByte(sender, (int)Commands.OP_PositionMode, (byte)axis, 0, messageReceived, true);

            SendCommandByte(sender, (int)Commands.OP_CPR, (byte)axis, 0, messageReceived, true);

            SendCommandFloat(sender, (int)Commands.OP_CACC, (byte)axis, 100, messageReceived);

            SendCommandFloat(sender, (int)Commands.OP_CSPD, (byte)axis, 5, messageReceived);

            SendCommandFloat(sender, (int)Commands.OP_CPOS, (byte)axis, 10, messageReceived);

            SendCommandByte(sender, (int)Commands.OP_UPD, (byte)axis, 0, messageReceived, true);
        }

        public static float ReadStabParamsKp(Socket sender, Axes axis)
        {
            byte[] messageReceived = new byte[32];
            SendCommandByte(sender, (int)Commands.OP_getKp, (byte)axis, 0, messageReceived, true);
            return Array2Float(messageReceived, 7);
        }

        public static float ReadStabParamsKi(Socket sender, Axes axis)
        {
            byte[] messageReceived = new byte[32];
            SendCommandByte(sender, (int)Commands.OP_getKi, (byte)axis, 0, messageReceived, true);
            return Array2Float(messageReceived, 7);
        }

        public static void WriteStabParamsKp(Socket sender, Axes axis, float value)
        {
            byte[] messageReceived = new byte[32];
            SendCommandFloat(sender, (int)Commands.OP_setKp, (byte)axis, value, messageReceived);
        }

        public static void WriteStabParamsKi(Socket sender, Axes axis, float value)
        {
            byte[] messageReceived = new byte[32];
            SendCommandFloat(sender, (int)Commands.OP_setKi, (byte)axis, value, messageReceived);
        }

        public static Int16 ReadMaxAcceleration(Socket sender, Axes axis)
        {
            byte[] messageReceived = new byte[32];
            SendCommandByte(sender, (int)Commands.OP_get_cfg_maxAccel, (byte)axis, 0, messageReceived, true);
            return Array2Int(messageReceived, 7);
        }

        public static Int16 ReadMaxSpeed(Socket sender, Axes axis)
        {
            byte[] messageReceived = new byte[32];
            SendCommandByte(sender, (int)Commands.OP_get_cfg_MaxSpeed, (byte)axis, 0, messageReceived, true);
            return Array2Int(messageReceived, 7);
        }

        public static float ReadAxisRatio(Socket sender, Axes axis)
        {
            byte[] messageReceived = new byte[32];
            SendCommandByte(sender, (int)Commands.OP_get_cfg_GearRatio, (byte)axis, 0, messageReceived, true);
            return Array2Float(messageReceived, 7);
        }

        public static void WriteAxisRatio(Socket sender, Axes axis, float value)
        {
            byte[] messageReceived = new byte[32];
            SendCommandFloat(sender, (int)Commands.OP_set_cfg_GearRatio, (byte)axis, value, messageReceived);
        }

        public static float ReadActualPosition(Socket sender, Axes axis)
        {
            byte[] messageReceived = new byte[32];
            SendCommandByte(sender, (int)Commands.OP_APOS_LD, (byte)axis, 0, messageReceived, true);
            return Array2Float(messageReceived, 7);
        }

        public static float ReadImuBaseYaw(Socket sender)
        {
            byte[] messageReceived = new byte[32];
            SendCommandByte(sender, (int)Commands.OP_getBaseRawYaw, 0, 0, messageReceived, true);
            return Array2Float(messageReceived, 7);
        }

        public static float ReadImuBasePitch(Socket sender)
        {
            byte[] messageReceived = new byte[32];
            SendCommandByte(sender, (int)Commands.OP_getBaseRawPitch, 0, 0, messageReceived, true);
            return Array2Float(messageReceived, 7);
        }

        public static float ReadImuBaseRoll(Socket sender)
        {
            byte[] messageReceived = new byte[32];
            SendCommandByte(sender, (int)Commands.OP_getBaseRawRoll, 0, 0, messageReceived, true);
            return Array2Float(messageReceived, 7);
        }

        public static float ReadStabError(Socket sender, Axes axis)
        {
            byte[] messageReceived = new byte[32];
            SendCommandByte(sender, (int)Commands.OP_stabError, (byte)axis, 0, messageReceived, true);
            return Array2Float(messageReceived, 7);
        }

        public static float ReadSetPoint(Socket sender, Axes axis)
        {
            byte[] messageReceived = new byte[32];
            SendCommandByte(sender, (int)Commands.OP_stabSetPoint, (byte)axis, 0, messageReceived, true);
            return Array2Float(messageReceived, 7);
        }

        public static void LoadPid(Socket sender)
        {
            byte[] messageReceived = new byte[32];
            SendCommandByte(sender, (int)Commands.OP_loadPID, 0, 0, messageReceived, true);
        }

        public static void SavePid(Socket sender)
        {
            byte[] messageReceived = new byte[32];
            SendCommandByte(sender, (int)Commands.OP_savePID, 0, 0, messageReceived, true);
        }

        public static short ReadErrorsRegister(Socket sender)
        {
            byte[] messageReceived = new byte[32];
            SendCommandByte(sender, (int)Commands.OP_getErrorsReg, 0, 0, messageReceived, true);
            return Array2Int(messageReceived, 7);
        }

        public static String ReadMotorError(Socket sender)
        {
            byte[] messageReceived = new byte[64];
            SendCommandByte(sender, (int)Commands.OP_getMotorError, 0, 0, messageReceived, true);
            return Array2String(messageReceived, 7, messageReceived[2] - 4);
        }

        public static String ReadSystemError(Socket sender)
        {
            byte[] messageReceived = new byte[32];
            SendCommandByte(sender, (int)Commands.OP_getSystemError, 0, 0, messageReceived, true);
            return Array2String(messageReceived, 7, messageReceived[2] - 4);
        }

        private static byte SendCommandByte(Socket sender, int opcode, byte axis, byte data, byte[] messageReceived, bool ack_required)
        {
            byte[] messageSent = { 0x50, 0x54, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            messageSent[4] = axis;
            messageSent[5] = (byte)(opcode >> 8);
            messageSent[6] = (byte)(opcode & 0xFF);
            messageSent[7] = data;
            messageSent[8] = CalculateCrc(messageSent, sizeof(byte));

            Console.WriteLine("COMMAND | {0} | {1} | data:{2:D}", Enum.GetName(typeof(Commands), opcode), Enum.GetName(typeof(Axes), axis), data);
            Console.WriteLine("PACKET : " + $"{string.Join(" ", messageSent)}");
            sender.Send(messageSent);

            if (ack_required)
            {
                sender.Receive(messageReceived);
                Console.WriteLine("REPLY  : " + $"{string.Join(" ", messageReceived)}");
            }

            return messageReceived[0];
        }

        public static byte SendCommandFloat(Socket sender, int opcode, byte axis, float data, byte[] messageReceived)
        {
            byte[] messageSent = { 0x50, 0x54, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] array = BitConverter.GetBytes(data);

            messageSent[4] = axis;
            messageSent[5] = (byte)(opcode >> 8);    // OpCode High byte
            messageSent[6] = (byte)(opcode & 0xFF);  // OpCode Low byte
            messageSent[7] = array[3];
            messageSent[8] = array[2];
            messageSent[9] = array[1];
            messageSent[10] = array[0];
            messageSent[11] = CalculateCrc(messageSent, sizeof(float));

            Console.WriteLine("COMMAND | {0} | {1} | data:{2:F}", Enum.GetName(typeof(Commands), opcode), Enum.GetName(typeof(Axes), axis), data);
            Console.WriteLine("PACKET : " + $"{string.Join(" ", messageSent)}");
            sender.Send(messageSent);
            sender.Receive(messageReceived);
            Console.WriteLine("REPLY  : " + $"{string.Join(" ", messageReceived)}");

            return messageReceived[0];
        }

        private static Int16 Array2Int(byte[] value, byte startIndex)
        {
            return (short)(value[startIndex+1] + (value[startIndex] << 8));
        }

        private static float Array2Float(byte[] value, byte startIndex)
        {
            byte[] array = { value[startIndex + 3], value[startIndex + 2], value[startIndex + 1], value[startIndex] };
            return BitConverter.ToSingle(array, 0);
        }

        private static String Array2String(byte[] value, byte startIndex, int length)
        {
            byte[] str = new byte[length];
            Buffer.BlockCopy(value, startIndex, str, 0, length);
            return System.Text.Encoding.Default.GetString(str);
        }

        private static byte CalculateCrc(byte[] packet, int size)
        {
            byte crc = 0;

            for (int i = 2; i < size + 7; i++)
            {
                crc = (byte)(crc + packet[i]);
            }

            return crc;
        }
    }
}
