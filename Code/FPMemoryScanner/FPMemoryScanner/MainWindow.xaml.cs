using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

using Windows.Devices.Enumeration;

namespace FPMemory.Scanner
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private ObservableCollection<BluetoothLEDeviceDisplay> KnownDevices = new ObservableCollection<BluetoothLEDeviceDisplay>();
        private List<DeviceInformation> UnknownDevices = new List<DeviceInformation>();
        private DeviceWatcher deviceWatcher;
        public MainWindow()
        {
            InitializeComponent();
            this.ResultsListView.ItemsSource = KnownDevices;
        }

        private void EnumerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (deviceWatcher == null)
            {
                StartBleDeviceWatcher();
                EnumerateButton.Content = "停止扫描";
                this.StatusText.Content = "Device watcher started.";
            }
            else
            {
                StopBleDeviceWatcher();
                EnumerateButton.Content = "开始扫描";
                this.StatusText.Content = "Device watcher stopped.";
            }

        }

        #region 查找蓝牙设备
        private void StartBleDeviceWatcher()
        {
            string[] requestedProperties = {
                "System.Devices.Aep.DeviceAddress",
                "System.Devices.Aep.IsConnected",
                "System.Devices.Aep.Bluetooth.Le.IsConnectable" };

            string aqsAllBluetoothLEDevices = "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")";

            deviceWatcher =
                DeviceInformation.CreateWatcher(
                    aqsAllBluetoothLEDevices,
                    requestedProperties,
                    DeviceInformationKind.AssociationEndpoint);

            //启动deviceWatcher前注册事件
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Stopped += DeviceWatcher_Stopped;

            KnownDevices.Clear();

            deviceWatcher.Start();

        }

        private void StopBleDeviceWatcher()
        {
            if (deviceWatcher != null)
            {
                // Unregister the event handlers.
                deviceWatcher.Added -= DeviceWatcher_Added;
                deviceWatcher.Updated -= DeviceWatcher_Updated;
                deviceWatcher.Removed -= DeviceWatcher_Removed;
                deviceWatcher.EnumerationCompleted -= DeviceWatcher_EnumerationCompleted;
                deviceWatcher.Stopped -= DeviceWatcher_Stopped;

                // Stop the watcher.
                deviceWatcher.Stop();
                deviceWatcher = null;
            }
        }

        private BluetoothLEDeviceDisplay FindBluetoothLEDeviceDisplay(string id)
        {
            foreach (BluetoothLEDeviceDisplay bleDeviceDisplay in KnownDevices)
            {
                if (bleDeviceDisplay.Id == id)
                {
                    return bleDeviceDisplay;
                }
            }
            return null;
        }

        private DeviceInformation FindUnknownDevices(string id)
        {
            foreach (DeviceInformation bleDeviceInfo in UnknownDevices)
            {
                if (bleDeviceInfo.Id == id)
                {
                    return bleDeviceInfo;
                }
            }
            return null;
        }

        private async void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            await this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == deviceWatcher)
                {
                    this.StatusText.Content = "No longer watching for devices.";
                }
            }));
        }

        private async void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            await this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == deviceWatcher)
                {
                    this.StatusText.Content = $"{KnownDevices.Count} devices found. Enumeration completed.";                        
                }
            }));
        }

        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            await this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                lock (this)
                {
                    Debug.WriteLine(String.Format("Removed {0}{1}", deviceInfoUpdate.Id, ""));

                    
                    if (sender == deviceWatcher)
                    {
                        
                        BluetoothLEDeviceDisplay bleDeviceDisplay = FindBluetoothLEDeviceDisplay(deviceInfoUpdate.Id);
                        if (bleDeviceDisplay != null)
                        {
                            KnownDevices.Remove(bleDeviceDisplay);
                        }

                        DeviceInformation deviceInfo = FindUnknownDevices(deviceInfoUpdate.Id);
                        if (deviceInfo != null)
                        {
                            UnknownDevices.Remove(deviceInfo);
                        }
                    }
                }
            }));

        }

        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            //异步更新UI
            await this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
           {
               lock (this)
               {
                   Debug.WriteLine(String.Format("Updated {0} {1}", deviceInfoUpdate.Id, ""));

                   if (sender == deviceWatcher)
                   {
                       
                       BluetoothLEDeviceDisplay bleDeviceDisplay = FindBluetoothLEDeviceDisplay(deviceInfoUpdate.Id);
                       if (bleDeviceDisplay != null)
                       {

                           bleDeviceDisplay.Update(deviceInfoUpdate);
                           return;
                       }

                       DeviceInformation deviceInfo = FindUnknownDevices(deviceInfoUpdate.Id);
                       if (deviceInfo != null)
                       {
                           deviceInfo.Update(deviceInfoUpdate);

                           if (deviceInfo.Name != String.Empty)
                           {
                               KnownDevices.Add(new BluetoothLEDeviceDisplay(deviceInfo));
                               UnknownDevices.Remove(deviceInfo);
                           }
                       }
                   }

               }
           }
            ));
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            //异步更新UI
            await this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
             {
                 lock (this)
                 {
                     Debug.WriteLine(String.Format("Added {0} {1}", deviceInfo.Id, deviceInfo.Name));

                     if (sender == deviceWatcher)
                     {
                         if (FindBluetoothLEDeviceDisplay(deviceInfo.Id) == null)
                         {
                             if (deviceInfo.Name != string.Empty)
                             {
                                 KnownDevices.Add(new BluetoothLEDeviceDisplay(deviceInfo));
                             }
                             else
                             {
                                 UnknownDevices.Add(deviceInfo);
                             }
                         }

                     }
                 }
             }


            ));

        }

        #endregion
    }
}
