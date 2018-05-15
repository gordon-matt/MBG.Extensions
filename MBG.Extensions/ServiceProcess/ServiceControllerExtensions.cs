using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using MBG.Extensions.Core;

namespace MBG.Extensions.ServiceProcess
{
    public static class ServiceControllerExtensions
    {
        public static void Restart(this ServiceController serviceController)
        {
            if (serviceController.Status.In(
                ServiceControllerStatus.Running,
                ServiceControllerStatus.ContinuePending,
                ServiceControllerStatus.StartPending))
            {
                serviceController.WaitForStatus(ServiceControllerStatus.Running);
                serviceController.Stop();
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                serviceController.Start();
                serviceController.WaitForStatus(ServiceControllerStatus.Running);
            }
            else if (serviceController.Status.In(
                ServiceControllerStatus.PausePending,
                ServiceControllerStatus.Paused))
            {
                serviceController.WaitForStatus(ServiceControllerStatus.Paused);
                serviceController.Stop();
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                serviceController.Start();
                serviceController.WaitForStatus(ServiceControllerStatus.Running);
            }
            else if (serviceController.Status.In(
                ServiceControllerStatus.Stopped,
                ServiceControllerStatus.StopPending))
            {
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                serviceController.Start();
                serviceController.WaitForStatus(ServiceControllerStatus.Running);
            }
        }
    }
}