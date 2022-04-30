using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32.TaskScheduler;
using System.Security.Principal;
using System.Security.Permissions;

namespace DataTools.Scheduler
{
    /// <summary>
    /// Tools to add and remove the current assembly from the system startup sequence.
    /// </summary>
    public static class TaskTool
    {
        /// <summary>
        /// Enable the current assembly to run on system startup.
        /// </summary>
        /// <param name="args">Optional arguments to pass to the program on startup.</param>
        /// <param name="runLevel">Optional <see cref="TaskRunLevel"/> to use to start the application.</param>
        public static void EnableOnStartup(string? args = null, TaskRunLevel runLevel = TaskRunLevel.LUA)
        {            
            var task = TaskService.Instance.NewTask();
            var exec = AppDomain.CurrentDomain.BaseDirectory + "\\" + AppDomain.CurrentDomain.FriendlyName + ".exe";

            var asm = AppDomain.CurrentDomain.FriendlyName;

            task.Triggers.Add(new LogonTrigger());

            if (args != null)
                task.Actions.Add(exec, args);
            else
                task.Actions.Add(exec);

            task.Principal.RunLevel = runLevel;

            TaskService.Instance.RootFolder.RegisterTaskDefinition(asm, task);
        }

        /// <summary>
        /// Remove the current application from the system startup tasks.
        /// </summary>
        public static void DisableOnStartup()
        {
            var asm = AppDomain.CurrentDomain.FriendlyName;
            TaskService.Instance.RootFolder.DeleteTask(asm, false);
        }

        /// <summary>
        /// Gets a value indicating whether or not the current assembly is registered to run on system startup.
        /// </summary>
        /// <returns></returns>
        public static bool GetIsEnabled()
        {
            var asm = AppDomain.CurrentDomain.FriendlyName;

            foreach (var t in TaskService.Instance.RootFolder.Tasks)
            {
                if (t.Name == asm) return true;
            }

            return false;
        }

    }

}
