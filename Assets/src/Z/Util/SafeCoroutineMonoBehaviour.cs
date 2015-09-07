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
using Z.Threading;
using UnityEngine;

namespace Z.Util {
  public static class SafeCoroutineMonoBehaviour {
    public static Coroutine SafeStartCoroutine(this MonoBehaviour component, Func<IEnumerator> coroutine) {
      return component.StartCoroutine(Wrap(component, coroutine));
    }

    public static Coroutine SafeStartCoroutine<P0>(this MonoBehaviour component, Func<P0, IEnumerator> coroutine, P0 p0) {
      return component.StartCoroutine(Wrap(component, coroutine, p0));
    }

    public static Coroutine SafeStartCoroutine<P0, P1>(this MonoBehaviour component, Func<P0, P1, IEnumerator> coroutine, P0 p0, P1 p1) {
      return component.StartCoroutine(Wrap(component, coroutine, p0, p1));
    }

    public static Coroutine SafeStartCoroutine<P0, P1, P2>(this MonoBehaviour component, Func<P0, P1, P2, IEnumerator> coroutine, P0 p0, P1 p1, P2 p2) {
      return component.StartCoroutine(Wrap(component, coroutine, p0, p1, p2));
    }

    public static Coroutine SafeStartCoroutine<P0, P1, P2, P3>(this MonoBehaviour component, Func<P0, P1, P2, P3, IEnumerator> coroutine, P0 p0, P1 p1, P2 p2, P3 p3) {
      return component.StartCoroutine(Wrap(component, coroutine, p0, p1, p2, p3));
    }

    private static IEnumerator Wrap(MonoBehaviour component, Func<IEnumerator> coroutine) {
      SafeCoroutine safeCoroutine = new SafeCoroutine();

      IEnumerator i = null;

      try {
        i = coroutine();
      } catch (Exception exception) {
        Handle(exception);
      }

      if (i != null) {
        safeCoroutine.Coroutine = component.StartCoroutine(safeCoroutine.RunCoroutine(i));
        yield return safeCoroutine.Coroutine;
      }

      if (safeCoroutine.Exception != null) {
        Handle(safeCoroutine.Exception);
      }
    }

    private static IEnumerator Wrap<P0>(MonoBehaviour component, Func<P0, IEnumerator> coroutine, P0 p0) {
      SafeCoroutine safeCoroutine = new SafeCoroutine();

      IEnumerator i = null;

      try {
        i = coroutine(p0);
      } catch (Exception exception) {
        Handle(exception);
      }

      if (i != null) {
        safeCoroutine.Coroutine = component.StartCoroutine(safeCoroutine.RunCoroutine(i));
        yield return safeCoroutine.Coroutine;
      }

      if (safeCoroutine.Exception != null) {
        Handle(safeCoroutine.Exception);
      }
    }

    private static IEnumerator Wrap<P0, P1>(MonoBehaviour component, Func<P0, P1, IEnumerator> coroutine, P0 p0, P1 p1) {
      SafeCoroutine safeCoroutine = new SafeCoroutine();

      IEnumerator i = null;

      try {
        i = coroutine(p0, p1);
      } catch (Exception exception) {
        Handle(exception);
      }

      if (i != null) {
        safeCoroutine.Coroutine = component.StartCoroutine(safeCoroutine.RunCoroutine(i));
        yield return safeCoroutine.Coroutine;
      }

      if (safeCoroutine.Exception != null) {
        Handle(safeCoroutine.Exception);
      }
    }

    private static IEnumerator Wrap<P0, P1, P2>(MonoBehaviour component, Func<P0, P1, P2, IEnumerator> coroutine, P0 p0, P1 p1, P2 p2) {
      SafeCoroutine safeCoroutine = new SafeCoroutine();

      IEnumerator i = null;

      try {
        i = coroutine(p0, p1, p2);
      } catch (Exception exception) {
        Handle(exception);
      }

      if (i != null) {
        safeCoroutine.Coroutine = component.StartCoroutine(safeCoroutine.RunCoroutine(i));
        yield return safeCoroutine.Coroutine;
      }

      if (safeCoroutine.Exception != null) {
        Handle(safeCoroutine.Exception);
      }
    }

    private static IEnumerator Wrap<P0, P1, P2, P3>(MonoBehaviour component, Func<P0, P1, P2, P3, IEnumerator> coroutine, P0 p0, P1 p1, P2 p2, P3 p3) {
      SafeCoroutine safeCoroutine = new SafeCoroutine();

      IEnumerator i = null;

      try {
        i = coroutine(p0, p1, p2, p3);
      } catch (Exception exception) {
        Handle(exception);
      }

      if (i != null) {
        safeCoroutine.Coroutine = component.StartCoroutine(safeCoroutine.RunCoroutine(i));
        yield return safeCoroutine.Coroutine;
      }

      if (safeCoroutine.Exception != null) {
        Handle(safeCoroutine.Exception);
      }
    }

    private static void Handle(Exception exception) {
      ExceptionReporter.Enqueue(exception);
      Log.Error(Channel.Exception, exception.Message + "\n" + exception.StackTrace);
    }
  }
}