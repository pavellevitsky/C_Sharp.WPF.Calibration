using System;
using System.Threading;
using System.Windows;
using System.Net.Sockets;
using CalibrationModel;
using System.Windows.Media;

namespace CalibrationUI
{
    public partial class MainWindow : Window
    {
        Socket socket;
        Thread thr_readValues;

        public MainWindow()
        {
            InitializeComponent();

            DisableAllButtons();

            session_connect.IsEnabled = true;
            session_connect.Foreground = Brushes.Green;
        }

        private void Button_Click_Connect(object sender, RoutedEventArgs e)
        {
            socket = CalibrationModel.CalibrationModel.Connect(ipTextbox.Text, int.Parse(portTextbox.Text));
            thr_readValues = new Thread(new ThreadStart(ReadValues));
            thr_readValues.Start();

            session_connect.IsEnabled = false;
            session_connect.Foreground = Brushes.Gray;
            session_disconnect.IsEnabled = true;
            session_disconnect.Foreground = Brushes.Red;
            stab_start.IsEnabled = true;
            stab_start.Foreground = Brushes.Green;
            pid_load.IsEnabled = true;
            pid_load.Foreground = Brushes.Black;
            pid_update.IsEnabled = true;
            pid_update.Foreground = Brushes.Black;
            pid_save.IsEnabled = true;
            pid_save.Foreground = Brushes.Black;
            AxisOnYaw.IsEnabled = true;
            AxisOnYaw.Foreground = Brushes.Black;
            AxisOnPitch.IsEnabled = true;
            AxisOnPitch.Foreground = Brushes.Black;
            AxisOnRoll.IsEnabled = true;
            AxisOnRoll.Foreground = Brushes.Black;
            AxisOffYaw.IsEnabled = true;
            AxisOffYaw.Foreground = Brushes.Black;
            AxisOffPitch.IsEnabled = true;
            AxisOffPitch.Foreground = Brushes.Black;
            AxisOffRoll.IsEnabled = true;
            AxisOffRoll.Foreground = Brushes.Black;
            motionTestYaw.IsEnabled = true;
            motionTestYaw.Foreground = Brushes.Black;
            motionTestPitch.IsEnabled = true;
            motionTestPitch.Foreground = Brushes.Black;
            motionTestRoll.IsEnabled = true;
            motionTestRoll.Foreground = Brushes.Black;
            move.IsEnabled = true;
            move.Foreground = Brushes.Black;

            _yawMaxAcc.Text = CalibrationModel.CalibrationModel.ReadMaxAcceleration(socket, Axes.AXIS_YAW).ToString();
            _pitchMaxAcc.Text = CalibrationModel.CalibrationModel.ReadMaxAcceleration(socket, Axes.AXIS_PITCH).ToString();
            _rollMaxAcc.Text = CalibrationModel.CalibrationModel.ReadMaxAcceleration(socket, Axes.AXIS_ROLL).ToString();

            _yawMaxSpeed.Text = CalibrationModel.CalibrationModel.ReadMaxSpeed(socket, Axes.AXIS_YAW).ToString();
            _pitchMaxSpeed.Text = CalibrationModel.CalibrationModel.ReadMaxSpeed(socket, Axes.AXIS_PITCH).ToString();
            _rollMaxSpeed.Text = CalibrationModel.CalibrationModel.ReadMaxSpeed(socket, Axes.AXIS_ROLL).ToString();
        }

        private void Button_Click_Disconnect(object sender, RoutedEventArgs e)
        {
            thr_readValues.Abort();
            CalibrationModel.CalibrationModel.Disconnect(socket);

            DisableAllButtons();
            ClearScreen();

            session_connect.IsEnabled = true;
            session_connect.Foreground = Brushes.Green;
        }

        private void Button_Click_AxisOnYaw(object sender, RoutedEventArgs e)
        {
            CalibrationModel.CalibrationModel.AxisOn(socket, Axes.AXIS_YAW);
        }

        private void Button_Click_AxisOnPitch(object sender, RoutedEventArgs e)
        {
            CalibrationModel.CalibrationModel.AxisOn(socket, Axes.AXIS_PITCH);
        }

        private void Button_Click_AxisOnRoll(object sender, RoutedEventArgs e)
        {
            CalibrationModel.CalibrationModel.AxisOn(socket, Axes.AXIS_ROLL);
        }

        private void Button_Click_AxisOffYaw(object sender, RoutedEventArgs e)
        {
            CalibrationModel.CalibrationModel.AxisOff(socket, Axes.AXIS_YAW);
        }

        private void Button_Click_AxisOffPitch(object sender, RoutedEventArgs e)
        {
            CalibrationModel.CalibrationModel.AxisOff(socket, Axes.AXIS_PITCH);
        }

        private void Button_Click_AxisOffRoll(object sender, RoutedEventArgs e)
        {
            CalibrationModel.CalibrationModel.AxisOff(socket, Axes.AXIS_ROLL);
        }

        private void Button_Click_StabStart(object sender, RoutedEventArgs e)
        {
            CalibrationModel.CalibrationModel.StabOn(socket);

            stab_start.IsEnabled = false;
            stab_start.Foreground = Brushes.Gray;
            stab_stop.IsEnabled = true;
            stab_stop.Foreground = Brushes.Red;
            // button_calibrate.IsEnabled = true;
            // button_calibrate.Foreground = Brushes.Green;
        }

        private void Button_Click_StabStop(object sender, RoutedEventArgs e)
        {
            CalibrationModel.CalibrationModel.StabOff(socket);

            stab_start.IsEnabled = true;
            stab_start.Foreground = Brushes.Green;
            stab_stop.IsEnabled = false;
            stab_stop.Foreground = Brushes.Gray;

            _pitchSetPoint.Text = "";
            _rollSetPoint.Text = "";

            _pitchErr.Text = "";
            _rollErr.Text = "";
        }

        private void Button_Click_Move(object sender, RoutedEventArgs e)
        {
            CalibrationModel.CalibrationModel.Movement(socket);
        }

        private void Button_Click_MotionTestYaw(object sender, RoutedEventArgs e)
        {
            CalibrationModel.CalibrationModel.TestMotors(socket, Axes.AXIS_YAW);
        }

        private void Button_Click_MotionTestPitch(object sender, RoutedEventArgs e)
        {
            CalibrationModel.CalibrationModel.TestMotors(socket, Axes.AXIS_PITCH);
        }

        private void Button_Click_MotionTestRoll(object sender, RoutedEventArgs e)
        {
            CalibrationModel.CalibrationModel.TestMotors(socket, Axes.AXIS_ROLL);
        }

        private void Button_Click_LoadParams(object sender, RoutedEventArgs e)
        {
            CalibrationModel.CalibrationModel.LoadPid(socket);

            _yawKpUpd.Text = CalibrationModel.CalibrationModel.ReadStabParamsKp(socket, Axes.AXIS_YAW).ToString();
            _yawKiUpd.Text = CalibrationModel.CalibrationModel.ReadStabParamsKi(socket, Axes.AXIS_YAW).ToString();
            _yawRatioUpd.Text = CalibrationModel.CalibrationModel.ReadAxisRatio(socket, Axes.AXIS_YAW).ToString();

            _pitchKpUpd.Text = CalibrationModel.CalibrationModel.ReadStabParamsKp(socket, Axes.AXIS_PITCH).ToString();
            _pitchKiUpd.Text = CalibrationModel.CalibrationModel.ReadStabParamsKi(socket, Axes.AXIS_PITCH).ToString();
            _pitchRatioUpd.Text = CalibrationModel.CalibrationModel.ReadAxisRatio(socket, Axes.AXIS_PITCH).ToString();

            _rollKpUpd.Text = CalibrationModel.CalibrationModel.ReadStabParamsKp(socket, Axes.AXIS_ROLL).ToString();
            _rollKiUpd.Text = CalibrationModel.CalibrationModel.ReadStabParamsKi(socket, Axes.AXIS_ROLL).ToString();
            _rollRatioUpd.Text = CalibrationModel.CalibrationModel.ReadAxisRatio(socket, Axes.AXIS_ROLL).ToString();
        }

        private void Button_Click_UpdateParams(object sender, RoutedEventArgs e)
        {
            CalibrationModel.CalibrationModel.WriteStabParamsKp(socket, Axes.AXIS_YAW, float.Parse(_yawKpUpd.Text));
            CalibrationModel.CalibrationModel.WriteStabParamsKi(socket, Axes.AXIS_YAW, float.Parse(_yawKiUpd.Text));
            CalibrationModel.CalibrationModel.WriteStabParamsKp(socket, Axes.AXIS_PITCH, float.Parse(_pitchKpUpd.Text));
            CalibrationModel.CalibrationModel.WriteStabParamsKi(socket, Axes.AXIS_PITCH, float.Parse(_pitchKiUpd.Text));
            CalibrationModel.CalibrationModel.WriteStabParamsKp(socket, Axes.AXIS_ROLL, float.Parse(_rollKpUpd.Text));
            CalibrationModel.CalibrationModel.WriteStabParamsKi(socket, Axes.AXIS_ROLL, float.Parse(_rollKiUpd.Text));

            CalibrationModel.CalibrationModel.WriteAxisRatio(socket, Axes.AXIS_YAW, int.Parse(_yawRatioUpd.Text));
            CalibrationModel.CalibrationModel.WriteAxisRatio(socket, Axes.AXIS_PITCH, int.Parse(_pitchRatioUpd.Text));
            CalibrationModel.CalibrationModel.WriteAxisRatio(socket, Axes.AXIS_ROLL, int.Parse(_rollRatioUpd.Text));
        }

        private void Button_Click_SaveParams(object sender, RoutedEventArgs e)
        {
            CalibrationModel.CalibrationModel.SavePid(socket);
        }

        private void ReadValues()
        {
            while (true)
            {
                Thread.Sleep(500);

                short errorRegister = CalibrationModel.CalibrationModel.ReadErrorsRegister(socket);

                Dispatcher.Invoke(() =>
                {
                    _yawKp.Text = CalibrationModel.CalibrationModel.ReadStabParamsKp(socket, Axes.AXIS_YAW).ToString();
                    _yawKi.Text = CalibrationModel.CalibrationModel.ReadStabParamsKi(socket, Axes.AXIS_YAW).ToString();
                    _yawRatio.Text = CalibrationModel.CalibrationModel.ReadAxisRatio(socket, Axes.AXIS_YAW).ToString();
                    _yawApos.Text = CalibrationModel.CalibrationModel.ReadActualPosition(socket, Axes.AXIS_YAW).ToString("F");
                    _yawImuBase.Text = CalibrationModel.CalibrationModel.ReadImuBaseYaw(socket).ToString("F");
                    _yawErr.Text = CalibrationModel.CalibrationModel.ReadStabError(socket, Axes.AXIS_YAW).ToString("F");

                    _pitchKp.Text = CalibrationModel.CalibrationModel.ReadStabParamsKp(socket, Axes.AXIS_PITCH).ToString();
                    _pitchKi.Text = CalibrationModel.CalibrationModel.ReadStabParamsKi(socket, Axes.AXIS_PITCH).ToString();
                    _pitchRatio.Text = CalibrationModel.CalibrationModel.ReadAxisRatio(socket, Axes.AXIS_PITCH).ToString();
                    _pitchApos.Text = CalibrationModel.CalibrationModel.ReadActualPosition(socket, Axes.AXIS_PITCH).ToString("F");
                    _pitchImuBase.Text = CalibrationModel.CalibrationModel.ReadImuBasePitch(socket).ToString("F");
                    _pitchErr.Text = CalibrationModel.CalibrationModel.ReadStabError(socket, Axes.AXIS_PITCH).ToString("F");

                    _rollKp.Text = CalibrationModel.CalibrationModel.ReadStabParamsKp(socket, Axes.AXIS_ROLL).ToString();
                    _rollKi.Text = CalibrationModel.CalibrationModel.ReadStabParamsKi(socket, Axes.AXIS_ROLL).ToString();
                    _rollRatio.Text = CalibrationModel.CalibrationModel.ReadAxisRatio(socket, Axes.AXIS_ROLL).ToString();
                    _rollApos.Text = CalibrationModel.CalibrationModel.ReadActualPosition(socket, Axes.AXIS_ROLL).ToString("F");
                    _rollImuBase.Text = CalibrationModel.CalibrationModel.ReadImuBaseRoll(socket).ToString("F");
                    _rollErr.Text = CalibrationModel.CalibrationModel.ReadStabError(socket, Axes.AXIS_ROLL).ToString("F");

                    _errorRegister.Text = Convert.ToString(errorRegister, 2).PadLeft(8, '0');
                    _errorMotor.Text = CalibrationModel.CalibrationModel.ReadMotorError(socket).ToString();
                    _errorSystem.Text = CalibrationModel.CalibrationModel.ReadSystemError(socket).ToString();

                    if (stab_start.IsEnabled)
                    {
                        _yawSetPoint.Text = CalibrationModel.CalibrationModel.ReadSetPoint(socket, Axes.AXIS_YAW).ToString("F");
                        _pitchSetPoint.Text = CalibrationModel.CalibrationModel.ReadSetPoint(socket, Axes.AXIS_PITCH).ToString("F");
                        _rollSetPoint.Text = CalibrationModel.CalibrationModel.ReadSetPoint(socket, Axes.AXIS_ROLL).ToString("F");
                    }
                });
            }
        }

        private void DisableAllButtons()
        {
            session_connect.IsEnabled = false;
            session_connect.Foreground = Brushes.Gray;
            session_disconnect.IsEnabled = false;
            session_disconnect.Foreground = Brushes.Gray;
            AxisOnYaw.IsEnabled = false;
            AxisOnYaw.Foreground = Brushes.Gray;
            AxisOnPitch.IsEnabled = false;
            AxisOnPitch.Foreground = Brushes.Gray;
            AxisOnRoll.IsEnabled = false;
            AxisOnRoll.Foreground = Brushes.Gray;
            AxisOffYaw.IsEnabled = false;
            AxisOffYaw.Foreground = Brushes.Gray;
            AxisOffPitch.IsEnabled = false;
            AxisOffPitch.Foreground = Brushes.Gray;
            AxisOffRoll.IsEnabled = false;
            AxisOffRoll.Foreground = Brushes.Gray;
            motionTestYaw.IsEnabled = false;
            motionTestYaw.Foreground = Brushes.Gray;
            motionTestPitch.IsEnabled = false;
            motionTestPitch.Foreground = Brushes.Gray;
            motionTestRoll.IsEnabled = false;
            motionTestRoll.Foreground = Brushes.Gray;
            move.IsEnabled = false;
            move.Foreground = Brushes.Gray;
            stab_start.IsEnabled = false;
            stab_start.Foreground = Brushes.Gray;
            stab_stop.IsEnabled = false;
            stab_stop.Foreground = Brushes.Gray;
            pid_load.IsEnabled = false;
            pid_load.Foreground = Brushes.Gray;
            pid_update.IsEnabled = false;
            pid_update.Foreground = Brushes.Gray;
            pid_save.IsEnabled = false;
            pid_save.Foreground = Brushes.Gray;
        }

        private void ClearScreen()
        {
            _yawKp.Text = "";
            _yawKi.Text = "";
            _yawApos.Text = "";
            _yawImuBase.Text = "";
            _yawErr.Text = "";
            _yawMaxAcc.Text = "";
            _yawMaxSpeed.Text = "";
            _yawRatio.Text = "";
            _yawSetPoint.Text = "";

            _pitchKp.Text = "";
            _pitchKi.Text = "";
            _pitchApos.Text = "";
            _pitchImuBase.Text = "";
            _pitchErr.Text = "";
            _pitchMaxAcc.Text = "";
            _pitchMaxSpeed.Text = "";
            _pitchRatio.Text = "";
            _pitchSetPoint.Text = "";

            _rollKp.Text = "";
            _rollKi.Text = "";
            _rollApos.Text = "";
            _rollImuBase.Text = "";
            _rollErr.Text = "";
            _rollMaxAcc.Text = "";
            _rollMaxSpeed.Text = "";
            _rollRatio.Text = "";
            _rollSetPoint.Text = "";

            _errorRegister.Text = "";
            _errorMotor.Text = "";
            _errorSystem.Text = "";
        }
    }
}
