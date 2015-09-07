/**
 * MIT License
 * Copyright (c) 2015 notrubp@gmail.com
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR 
 * IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *
 * @license MIT
 * @copyright notrubp@gmail.com 2015
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Z.Logging;
using Z.Threading;
using Z.Util;
using UnityEngine;

namespace Z.Exceptions.Internal {
  [AddComponentMenu("")]
  public class ExceptionReporterDispatch : MonoBehaviour {
    private static ExceptionReporterDispatch instance;

    private static object __lock = new object();

    public static ExceptionReporterDispatch Instance {
      get {
        if (instance != null) {
          return instance;
        }

        lock (__lock) {
          if (instance != null) {
            return instance;
          }

          if (!Threads.IsCurrentThreadMainThread()) {
            throw new InvalidOperationException("Instance must be created on main thread.");
          }

          GameObject gameObject = new GameObject("ExceptionReporterDispatch", typeof(ExceptionReporterDispatch));
          gameObject.hideFlags = HideFlags.HideAndDontSave;
          instance = gameObject.GetComponent<ExceptionReporterDispatch>();
        }

        return instance;
      }
    }

    private Queue exceptions = Queue.Synchronized(new Queue());

    private bool isScheduled = false;

    public bool IsScheduled {
      get {
        return isScheduled;
      }
    }

    private bool shouldSchedule = false;

    public bool ShouldSchedule {
      get {
        return shouldSchedule;
      }
    }

    public bool CanSchedule {
      get {
        return !isScheduled;
      }
    }

    private bool debugPopupsEnabled = false;

    public bool DebugPopupsEnabled {
      get {
        return debugPopupsEnabled;
      }
      set {
        debugPopupsEnabled = value;
      }
    }

    private bool logHandledExceptions = false;

    public bool LogHandledExceptions {
      get {
        return logHandledExceptions;
      }
      set {
        logHandledExceptions = value;
      }
    }

    private Dictionary<string, object> template;

    public void Awake() {
      Application.logMessageReceivedThreaded += (string condition, string stackTrace, LogType type) => {
        if (type == LogType.Exception) {
          Enqueue(new Pair<string, string>(condition, stackTrace));
        }
      };

      template = new Dictionary<string, object>();
      template["os"] = SystemInfo.operatingSystem;
      template["version"] = Application.version;
      template["unityversion"] = Application.unityVersion;
      template["level"] = Application.loadedLevelName;

      Dictionary<string, object> device = new Dictionary<string, object>();
      device["model"] = SystemInfo.deviceModel;
      device["name"] = SystemInfo.deviceName;
      device["type"] = SystemInfo.deviceType.ToString();
      device["uuid"] = SystemInfo.deviceUniqueIdentifier;

      template["device"] = device;

      Dictionary<string, object> graphics = new Dictionary<string, object>();
      graphics["deviceid"] = SystemInfo.graphicsDeviceID;
      graphics["name"] = SystemInfo.graphicsDeviceName;
      graphics["vendor"] = SystemInfo.graphicsDeviceVendor;
      graphics["vendorid"] = SystemInfo.graphicsDeviceVendorID;
      graphics["version"] = SystemInfo.graphicsDeviceVersion;

      template["graphics"] = graphics;
    }

    public void OnApplicationQuit() {
      Destroy(gameObject);
    }

    public void Update() {
      if (ShouldSchedule && CanSchedule) {
        Schedule();
      }
    }

    public void Enqueue(Exception exception) {
      exceptions.Enqueue(exception);
      TrySchedule();
    }

    public void Enqueue(Pair<string, string> exception) {
      exceptions.Enqueue(exception);
      TrySchedule();
    }

    private void TrySchedule() {
      if (CanSchedule) {
        Schedule();
      } else {
        shouldSchedule = true;
      }
    }

    private void Schedule() {
      isScheduled = true;
      shouldSchedule = false;

      ThreadPool.QueueUserWorkItem((object state) => {
        Flush();
      });
    }

    public void Flush() {
      while (exceptions.Count > 0) {
        Exception exception = null;
        string message = null;
        string stackTrace = null;

        object obj = exceptions.Dequeue();

        if (obj is Exception) {
          exception = obj as Exception;
          message = exception.Message;
          stackTrace = exception.StackTrace;
        } else if (obj is Pair<string, string>) {
          Pair<string, string> pair = obj as Pair<string, string>;
          message = pair.first;
          stackTrace = pair.second;
        } else {
          // Should never happen but whatever...
          continue;
        }

        // Try out any registered handlers for this type.
        if (exception != null && ExceptionHandlers.TryHandlers(exception) && !logHandledExceptions) {
          continue;
        }

        Dictionary<string, object> report = new Dictionary<string, object>(template);

        report["time"] = (long) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

        /*if (UserInfoCache.Me != null) {
          report["identityId"] = UserInfoCache.Me.IdentityId;
        }*/

        string clazz = null;

        if (exception != null) {
          clazz = exception.GetType().Name;
        } else {
          string[] split = message.Split(new char[] { ':' }, 2);
          clazz = split[0];
          message = split[1].Trim();
        }

        report["class"] = clazz;
        report["message"] = message;

        List<object> trace = new List<object>();
        foreach (string s in stackTrace.Split('\n')) {
          string s1 = s;
          s1 = s1.Replace("  at ", "");
          s1 = s1.Trim();

          if (!s.Equals(String.Empty)) {
            trace.Add(s1);
          }
        }

        report["trace"] = trace;

        if (debugPopupsEnabled) {
          /*CallOnMainThread.Enqueue(() => {
            GlobalPopup popup = GlobalPopup.Make("OkPopup");
            popup.Header.text = "Error";
            popup.Message.text = clazz + ": " + message + "\n" + stackTrace;
          });*/
        }

        if (exception != null) {
          CallOnMainThread.Enqueue(() => {
            Log.Error(Channel.Exception, clazz + ": " + message + "\n" + stackTrace);
          });
        }

        // Not really much we can do if the client has no idea where to report to yet...
        /*if (Registry.Contains("report.exception")) {
          ReportExceptionRequest request = new ReportExceptionRequest(report);
          request.Synchronous = true;
          request.Send();
        }*/
      }

      isScheduled = false;
    }
  }
}