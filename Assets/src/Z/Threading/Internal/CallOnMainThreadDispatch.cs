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
using Z.Exceptions;
using Z.Logging;
using UnityEngine;

namespace Z.Threading.Internal {
  [AddComponentMenu("")]
  public class CallOnMainThreadDispatch : MonoBehaviour {
    private static CallOnMainThreadDispatch instance;

    private static object __lock = new object();

    public static CallOnMainThreadDispatch Instance {
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

          var gameObject = new GameObject("CallOnMainThreadDispatch", typeof(CallOnMainThreadDispatch));
          gameObject.hideFlags = HideFlags.HideAndDontSave;
          instance = gameObject.GetComponent<CallOnMainThreadDispatch>();
        }

        return instance;
      }
    }

    private Queue callbacks = Queue.Synchronized(new Queue());

    public Queue Callbacks {
      get {
        return callbacks;
      }
    }

    public void OnApplicationQuit() {
      Destroy(gameObject);
    }

    public void Update() {
      while (callbacks.Count > 0) {
        Action callback = callbacks.Dequeue() as Action;
        try {
          callback();
        } catch (Exception exception) {
          ExceptionReporter.Enqueue(exception);
          Log.Error(Channel.Exception, exception.Message + "\n" + exception.StackTrace);
        }
      }
    }
  }
}