namespace FirstToolWin
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    using System.Linq;
    using System.ServiceProcess;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for MyWindowControl.
    /// </summary>
    public partial class MyWindowControl : UserControl
    {
        private SolidColorBrush RedColorBrush = new SolidColorBrush(Color.FromRgb(255, 134, 148));
        private SolidColorBrush GreenColorBrush = new SolidColorBrush(Color.FromRgb(220, 237, 193));
 

        private List<string> BatchFiles = new List<string>()
        {
            @"C:\Users\wreitz\Source\Repos\ShelterSigns\Source\Projects\Luminator.TransitPredictions.Provider.Host\bin\Debug\Luminator.TransitPredictions.Provider.Host.exe",
            @"C:\Users\wreitz\Source\Repos\ShelterSigns\Source\Projects\Luminator.TransitPredictions.Delivery.Host\bin\Debug\Luminator.TransitPredictions.Delivery.Host.exe",
            @"C:\Users\wreitz\Source\Repos\ShelterSigns\Source\Projects\Luminator.ShelterSigns.Repository.Host\bin\Debug\Luminator.ShelterSigns.Repository.Host.exe",
            @"C:\Users\wreitz\Source\Repos\ShelterSigns\Source\Projects\Luminator.Gtfs.DataProvider.Host\bin\Debug\Luminator.Gtfs.DataProvider.Host.exe",
            @"C:\Users\wreitz\source\repos\ShelterSigns\Source\Projects\Luminator.ServiceAlertsDelivery.Host\bin\Debug\Luminator.ServiceAlerts.Delivery.Host.exe",
            @"C:\Users\wreitz\source\repos\ShelterSigns\Source\Projects\Luminator.ServiceAlerts.Provider.Host\bin\Debug\Luminator.ServiceAlerts.Provider.Host.exe",
            @"C:\Users\wreitz\source\repos\ShelterSigns\Source\Projects\Luminator.ShelterSigns.EndOfRoute.Host\bin\Debug\Luminator.ShelterSigns.EndOfRoute.Host.exe"
        };

        private List<string> PredictionProcesses = new List<string>()
         {
             "Luminator.TransitPredictions.Provider.Host",
             "Luminator.Gtfs.DataProvider.Host",
             "Luminator.TransitPredictions.Delivery.Host",
             "Luminator.ShelterSigns.Repository.Host"
        };

        private List<string> ServiceAlertsProcesses = new List<string>()
        {
            "Luminator.ServiceAlerts.Delivery.Host",
            "Luminator.ShelterSigns.Repository.Host",
            "Luminator.ServiceAlerts.Provider.Host"
        };

        private List<string> AllProcesses = new List<string>()
        {
            "Luminator.ShelterSigns.EndOfRoute.Host",
            "Luminator.ServiceAlerts.Delivery.Host",
            "Luminator.ServiceAlerts.Provider.Host",
            "Luminator.Gtfs.DataProvider.Host",
            "Luminator.TransitPredictions.Provider.Host",
            "Luminator.TransitPredictions.Delivery.Host",
            "Luminator.ShelterSigns.Repository.Host"
        };


        /// <summary>
        /// Initializes a new instance of the <see cref="MyWindowControl"/> class.
        /// </summary>
        public MyWindowControl()
        {
            this.InitializeComponent();
            checkForRunningPredictions();
        }

        private void checkForRunningPredictions()
        {
            this.btnStartPredictions.BorderBrush = Process.GetProcessesByName(PredictionProcesses.First())?.Length == 0 ? RedColorBrush : GreenColorBrush;
            this.btnStartAlerts.BorderBrush = Process.GetProcessesByName(ServiceAlertsProcesses.First())?.Length == 0 ? RedColorBrush : GreenColorBrush;

            ServiceController service = new ServiceController("Luminator Shelter Signs Visual Message Board Service");
            this.btnStartWinService.BorderBrush= service.Status.Equals(ServiceControllerStatus.Stopped) || service.Status.Equals(ServiceControllerStatus.StopPending) ? RedColorBrush : GreenColorBrush;
                 
        }

        ///// <summary>
        ///// Handles click on the button by displaying a message box.
        ///// </summary>
        ///// <param name="sender">The event sender.</param>
        ///// <param name="e">The event args.</param>
        //[SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        //[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        //private void button1_Click(object sender, RoutedEventArgs e)
        //{
        //    MessageBox.Show(
        //        string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
        //        "MyWindow");
        //}


        private void btnStopAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StopService("Luminator Shelter Signs Visual Message Board Service", 10000);
                foreach (var process in AllProcesses)
                {
                    Process[] processes = Process.GetProcessesByName(process);
                    if (processes.Length > 0)
                    {
                        foreach (var item in processes)
                        {
                            item.Kill();
                        }
                    }
                }
                checkForRunningPredictions();
            }
            catch (Exception)
            {
            }



        }

        private void btnStartWinService_Click(object sender, RoutedEventArgs e)
        {
            StartService("Luminator Shelter Signs Visual Message Board Service", 10000);
            checkForRunningPredictions();
        }

        private void btnStartAlerts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StartService("Luminator Shelter Signs Visual Message Board Service", 10000);
                foreach (var process in ServiceAlertsProcesses)
                {
                    StartProcessIfNotRunning(process);
                }
                checkForRunningPredictions();
            }
            catch (Exception)
            {
            }
        }

        private void btnStartPredictions_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StartService("Luminator Shelter Signs Visual Message Board Service", 10000);
                foreach (var process in PredictionProcesses)
                {
                    StartProcessIfNotRunning(process);
                }
                checkForRunningPredictions();
            }
            catch (Exception)
            {
            }
        }

        private void StartProcessIfNotRunning(string process)
        {
            if (Process.GetProcessesByName(process).Length == 0)
            {
                ProcessStartInfo info = new ProcessStartInfo(BatchFiles.FirstOrDefault(a => a.Contains(process)));
                info.UseShellExecute = true;
                info.Verb = "runas";
                Process.Start(info);
            }
        }

        public static void StartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                if ((service.Status.Equals(ServiceControllerStatus.Stopped)) || (service.Status.Equals(ServiceControllerStatus.StopPending)))
                {
                    TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void StopService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                if ((service.Status.Equals(ServiceControllerStatus.Running)) || (service.Status.Equals(ServiceControllerStatus.StartPending)))
                {
                    TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void btnShowLogs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
            startInfo.FileName = "cmd.exe";
            string _path = "c:/ShelterSignLogs";
            startInfo.Arguments = string.Format("/C start {0}", _path);
            process.StartInfo = startInfo;
            process.Start();

            }
            catch (Exception)
            {
            }
        }



 
    }
}