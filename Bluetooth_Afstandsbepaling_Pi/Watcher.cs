using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;

namespace Bluetooth_Afstandsbepaling_Pi
{
    class Watcher
    {
        private BluetoothLEAdvertisementWatcher watcher;
        private int advertisementCount;
        private List<double> rssiList;
        private Distance distance;

        public Watcher()
        {
            watcher = new BluetoothLEAdvertisementWatcher();
            advertisementCount = 0;
            rssiList = new List<double>();
            distance = new Distance();
            this.Watch();
        }

        public void Watch()
        {
            //watcher = new BluetoothLEAdvertisementWatcher();
            watcher.ScanningMode = BluetoothLEScanningMode.Active;

            watcher.Received += OnAdvertisementReceived;

            var manufacturerData = new BluetoothLEManufacturerData();
            manufacturerData.CompanyId = 0xFFFE;

            // Make sure that the buffer length can fit within an advertisement payload (~20 bytes). 
            // Otherwise you will get an exception.
            var writer = new DataWriter();
            writer.WriteString("Hello World");
            manufacturerData.Data = writer.DetachBuffer();

            watcher.AdvertisementFilter.Advertisement.ManufacturerData.Add(manufacturerData);

            watcher.Start();
        }

        public void startWatcher()
        {
            watcher.Start();
        }

        private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
        {
            // The received signal strength indicator (RSSI)
            Int16 rssi = eventArgs.RawSignalStrengthInDBm;
            distance.CalculateDistance(rssi);
            advertisementCount += 1;

            if (advertisementCount == 20)
            {
                //Debug.WriteLine(GetRssi());
                advertisementCount = 0;
                rssiList.Clear();
                if (_checkIfWatcherIsActive())
                {
                    watcher.Stop();
                }
            }
        }

        private bool _checkIfWatcherIsActive()
        {
            if (watcher.Status == BluetoothLEAdvertisementWatcherStatus.Started)
            {
                return true;
            }
            return false;
        }

        public void stopMeting()
        {
            if (watcher.Status != BluetoothLEAdvertisementWatcherStatus.Stopped)
            {
                watcher.Stop();
                Debug.WriteLine("Watcher status: " + watcher.Status);
            }
        }
    }
}
