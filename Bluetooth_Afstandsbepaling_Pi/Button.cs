using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Bluetooth_Afstandsbepaling_Pi
{
    class Button
    {
        private GpioController _gpio;

        private const int _buttonStartWatching = 19;
        private const int _buttonStartPublish = 13;
        private const int _buttonStop = 6;

        private GpioPin _gpioButtonStartWatching;
        private GpioPin _gpioButtonStartPublish;
        private GpioPin _gpioButtonStop;
        private Watcher _watcher;
        private Publisher _publisher;

        public Button()
        {
            InitGpio();
            _watcher = new Watcher();
            _publisher = new Publisher();
        }

        private void InitGpio()
        {
            _gpio = GpioController.GetDefault();

            _gpioButtonStartWatching = InitButton(_buttonStartWatching);
            _gpioButtonStartPublish = InitButton(_buttonStartPublish);
            _gpioButtonStop = InitButton(_buttonStop);
        }

        private GpioPin InitButton(int rpiPin)
        {
            GpioPin pin = _gpio.OpenPin(rpiPin);
            pin.SetDriveMode(GpioPinDriveMode.Output);
            pin.Write(GpioPinValue.High);

            if (pin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
            {
                pin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            }
            else
            {
                pin.SetDriveMode(GpioPinDriveMode.Input);
            }
            pin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            pin.ValueChanged += Knop_ValueChanged;

            return pin;
        }

        private void Knop_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.FallingEdge)
            {
                if (sender.PinNumber == _buttonStartWatching)
                {
                    Debug.WriteLine("Button start watching pressed");

                    _watcher.startWatcher();
                }
                else if (sender.PinNumber == _buttonStartPublish)
                {
                    Debug.WriteLine("Start publish");
                    _publisher.publish();
                }
                else if (sender.PinNumber == _buttonStop)
                {
                    Debug.WriteLine("stop");
                    _publisher.stopMeting();
                    _watcher.stopMeting();
                }
            }
        }
    }
}
