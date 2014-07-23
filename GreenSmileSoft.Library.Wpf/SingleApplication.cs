using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace GreenSmileSoft.Library.Wpf
{
    /// <summary>
    /// 책임: WPF Windows Application이 오직 하나만 실행하게 합니다.
    /// </summary>
    public static class SingleApplication
    {
        /// <summary>
        /// 같은 이름의 어플리케이션이 실행중일때 경고를 출력할지 정합니다.
        /// </summary>
        private static bool isAlert;

        public static void Set(bool isAlert = true)
        {
            SingleApplication.isAlert = isAlert;
            Run(SingleInstanceModes.ForEveryUser);
        }

        private static void Run(SingleInstanceModes singleInstanceModes)
        {
            var appName = Application.Current.GetType().Assembly.ManifestModule.ScopeName;

            var windowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var keyUserName = windowsIdentity != null ? windowsIdentity.User.ToString() : String.Empty;

            // Be careful! Max 260 chars!
            var eventWaitHandleName = string.Format(
                "{0}{1}",
                appName,
                singleInstanceModes == SingleInstanceModes.ForEveryUser ? keyUserName : String.Empty
                );

            try
            {
                using (var eventWaitHandle = EventWaitHandle.OpenExisting(eventWaitHandleName))
                {
                    // It informs first instance about other startup attempting.
                    eventWaitHandle.Set();
                }

                // Let's terminate this posterior startup.
                // For that exit no interceptions.
                if (true == isAlert)
                {
                    MessageBox.Show(string.Format("Already {0} is run!!", appName));
                }
                Environment.Exit(0);
            }
            catch
            {

                using (var eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, eventWaitHandleName))
                {
                    ThreadPool.RegisterWaitForSingleObject(eventWaitHandle, OtherInstanceAttemptedToStart, null, Timeout.Infinite, false);
                }

                RemoveApplicationsStartupDeadlockForStartupCrushedWindows();
            }
        }

        private static void OtherInstanceAttemptedToStart(Object state, Boolean timedOut)
        {
            RemoveApplicationsStartupDeadlockForStartupCrushedWindows();
            Application.Current.Dispatcher.BeginInvoke(new Action(() => { try { Application.Current.MainWindow.Activate(); } catch { } }));
        }

        internal static DispatcherTimer AutoExitAplicationIfStartupDeadlock;

        /// <summary>
        /// Бывают случаи, когда при старте произошла ошибка и ни одно окно не появилось.
        /// При этом второй инстанс приложения уже не запустить, а этот не закрыть, кроме как через панель задач. Deedlock своего рода получился.
        /// </summary>
        internal static void RemoveApplicationsStartupDeadlockForStartupCrushedWindows()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                AutoExitAplicationIfStartupDeadlock =
                    new DispatcherTimer(
                        TimeSpan.FromSeconds(6),
                        DispatcherPriority.ApplicationIdle,
                        (o, args) =>
                        {
                            if (0 == Application.Current.Windows.Cast<Window>().Count(window => !Double.IsNaN(window.Left)))
                            {
                                // For that exit no interceptions.
                                Environment.Exit(0);
                            }
                        },
                        Application.Current.Dispatcher
                    );
            }),
                DispatcherPriority.ApplicationIdle
                );
        }
    }

    internal enum SingleInstanceModes
    {
        /// <summary>
        /// Do nothing.
        /// </summary>
        NotInited = 0,

        /// <summary>
        /// Every user can have own single instance.
        /// </summary>
        ForEveryUser,
    }
}
