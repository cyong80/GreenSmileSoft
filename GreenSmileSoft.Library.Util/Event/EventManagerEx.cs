using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GreenSmileSoft.Library.Util.Event
{
    public static class EventManagerEx
    {
        public static void OnEvent<T>(object callee, EventHandler<T> evt, T args, SynchronizationContext context) where T : EventArgs
        {
            if (evt == null) { return; }
            if (args == null) { return; }
            if (callee == null) { return; }
            if (context == null)
            {
                evt(callee, args);
            }
            else
            {
                context.Post(postEvent<T>, new object[] { evt, args, callee });
            }
        }

        public static void OnEvent(object callee, EventHandler evt, EventArgs args, SynchronizationContext context)
        {
            if (evt == null) { return; }
            if (args == null) { return; }
            if (callee == null) { return; }

            if (context == null)
            {
                evt(callee, args);
            }
            else
            {
                context.Post(postEvent, new object[] { evt, args, callee });
            }
        }

        public static void OnEvent(object callee, Action action)
        {
            SynchronizationContext context = SynchronizationContext.Current;
            if (callee == null) { return; }
            if(context == null)
            {
                action();
            }
            else
            {
                EventHandler evt = (sender, args) => { action(); };
                context.Post(postEvent, new object[] { evt, EventArgs.Empty, callee });
            }
        }

        private static void postEvent(object data)
        {
            object[] dataArray = (object[])data;
            EventHandler evt = dataArray[0] as EventHandler;
            EventArgs args = dataArray[1] as EventArgs;
            object callee = dataArray[2];
            evt(callee, args);
        }

        private static void postEvent<T>(object data) where T : EventArgs
        {
            object[] dataArray = (object[])data;
            EventHandler<T> evt = dataArray[0] as EventHandler<T>;
            T args = dataArray[1] as T;
            object callee = dataArray[2];
            evt(callee, args);
        }
    }
}
