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
using System.Collections.Generic;
using Z.Logging.Internal;
using Z.Threading;

namespace Z.Logging {
  public class Log {
    private static bool enabled = true;

    public static bool Enabled {
      get {
        return enabled;
      }
      set {
        enabled = value;
      }
    }

    private static Channel channels = Channel.Application | Channel.Exception;

    public static Channel Channels {
      get {
        return channels;
      }
      set {
        channels = value;
      }
    }

    private static Severity severityLevel = Severity.Debug;

    public static Severity SeverityLevel {
      get {
        return severityLevel;
      }
      set {
        severityLevel = value;
      }
    }

    private static HashSet<Type> typesFilter = new HashSet<Type>();

    public static HashSet<Type> TypesFilter {
      get {
        return typesFilter;
      }
    }

    public static void Error(Channel channel, string message) {
      Error(channel, null, message);
    }

    public static void Error(Channel channel, Type type, string message) {
      if (IsNotFiltered(channel, Severity.Error, type)) {
        LogOrEnqueueMessage(Severity.Error, DateTime.Now.ToString("hh:mm:ss:ffff") + " - [Error] [" + channel + "] " + (type != null ? "[" + type.FullName + "] " : String.Empty) + message);
      }
    }

    public static void Warn(Channel channel, string message) {
      Warn(channel, null, message);
    }

    public static void Warn(Channel channel, Type type, string message) {
      if (IsNotFiltered(channel, Severity.Warn, type)) {
        LogOrEnqueueMessage(Severity.Warn, DateTime.Now.ToString("hh:mm:ss:ffff") + " - [Warn] [" + channel + "] " + (type != null ? "[" + type.FullName + "] " : String.Empty) + message);
      }
    }

    public static void Info(Channel channel, string message) {
      Info(channel, null, message);
    }

    public static void Info(Channel channel, Type type, string message) {
      if (IsNotFiltered(channel, Severity.Info, type)) {
        LogOrEnqueueMessage(Severity.Info, DateTime.Now.ToString("hh:mm:ss:ffff") + " - [Info] [" + channel + "] " + (type != null ? "[" + type.FullName + "] " : String.Empty) + message);
      }
    }

    public static void Debug(Channel channel, string message) {
      Debug(channel, null, message);
    }

    public static void Debug(Channel channel, Type type, string message) {
      if (IsNotFiltered(channel, Severity.Debug, type)) {
        LogOrEnqueueMessage(Severity.Debug, DateTime.Now.ToString("hh:mm:ss:ffff") + " - [Debug] [" + channel + "] " + (type != null ? "[" + type.FullName + "] " : String.Empty) + message);
      }
    }

    private static bool IsNotFiltered(Channel channel, Severity severity, Type type) {
      return enabled && (channels & channel) == channel && severity <= severityLevel && (type == null || !typesFilter.Contains(type));
    }

    private static void LogOrEnqueueMessage(Severity severity, string message) {
      if (Threads.IsCurrentThreadMainThread()) {
        switch (severity) {
        case Severity.Error:
          UnityEngine.Debug.LogError(message);
          break;
        case Severity.Warn:
          UnityEngine.Debug.LogWarning(message);
          break;
        case Severity.Info:
        case Severity.Debug:
          UnityEngine.Debug.Log(message);
          break;
        }
      } else {
        LogDispatch.Instance.Messages.Enqueue("[" + System.Threading.Thread.CurrentThread.Name + "] " + message);
      }
    }
  }
}
