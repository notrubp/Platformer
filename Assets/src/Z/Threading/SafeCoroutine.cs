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
using System.Reflection;
using UnityEngine;

namespace Z.Threading {
  /// <summary>
  /// Utility for safely catching exceptions thrown in a coroutine.
  /// </summary>
  public class SafeCoroutine {
    private Coroutine coroutine;

    /// <summary>
    /// The wrapped coroutine.
    /// </summary>
    public Coroutine Coroutine {
      get {
        return coroutine;
      }
      set {
        coroutine = value;
      }
    }

    private Exception exception;

    /// <summary>
    /// The caught exception, null if one is not thrown.
    /// </summary>
    public Exception Exception {
      get {
        return exception;
      }
    }

    /// <summary>
    /// Wrap a coroutine, will catch and store any thrown exceptions.
    /// </summary>
    /// <param name="component"></param>
    /// <param name="coroutine"></param>
    /// <returns></returns>
    public Coroutine WrapCoroutine(MonoBehaviour component, Func<IEnumerator> coroutine) {
      IEnumerator i = null;

      try {
        i = coroutine();
      } catch (Exception exception) {
        this.exception = exception;
      }

      if (i != null) {
        this.coroutine = component.StartCoroutine(RunCoroutine(i));
        return this.coroutine;
      }

      return null;
    }

    /// <summary>
    /// Wrap a coroutine, will catch and store any thrown exceptions.
    /// </summary>
    /// <typeparam name="P0"></typeparam>
    /// <param name="component"></param>
    /// <param name="coroutine"></param>
    /// <param name="p0"></param>
    /// <returns></returns>
    public Coroutine WrapCoroutine<P0>(MonoBehaviour component, Func<P0, IEnumerator> coroutine, P0 p0) {
      IEnumerator i = null;

      try {
        i = coroutine(p0);
      } catch (Exception exception) {
        this.exception = exception;
      }

      if (i != null) {
        this.coroutine = component.StartCoroutine(RunCoroutine(i));
        return this.coroutine;
      }

      return null;
    }

    /// <summary>
    /// Wrap a coroutine, will catch and store any thrown exceptions.
    /// </summary>
    /// <typeparam name="P0"></typeparam>
    /// <typeparam name="P1"></typeparam>
    /// <param name="component"></param>
    /// <param name="coroutine"></param>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <returns></returns>
    public Coroutine WrapCoroutine<P0, P1>(MonoBehaviour component, Func<P0, P1, IEnumerator> coroutine, P0 p0, P1 p1) {
      IEnumerator i = null;

      try {
        i = coroutine(p0, p1);
      } catch (Exception exception) {
        this.exception = exception;
      }

      if (i != null) {
        this.coroutine = component.StartCoroutine(RunCoroutine(i));
        return this.coroutine;
      }

      return null;
    }

    /// <summary>
    /// Wrap a coroutine, will catch and store any thrown exceptions.
    /// </summary>
    /// <typeparam name="P0"></typeparam>
    /// <typeparam name="P1"></typeparam>
    /// <typeparam name="P2"></typeparam>
    /// <param name="component"></param>
    /// <param name="coroutine"></param>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    public Coroutine WrapCoroutine<P0, P1, P2>(MonoBehaviour component, Func<P0, P1, P2, IEnumerator> coroutine, P0 p0, P1 p1, P2 p2) {
      IEnumerator i = null;

      try {
        i = coroutine(p0, p1, p2);
      } catch (Exception exception) {
        this.exception = exception;
      }

      if (i != null) {
        this.coroutine = component.StartCoroutine(RunCoroutine(i));
        return this.coroutine;
      }

      return null;
    }

    /// <summary>
    /// Wrap a coroutine, will catch and store any thrown exceptions.
    /// </summary>
    /// <typeparam name="P0"></typeparam>
    /// <typeparam name="P1"></typeparam>
    /// <typeparam name="P2"></typeparam>
    /// <typeparam name="P3"></typeparam>
    /// <param name="component"></param>
    /// <param name="coroutine"></param>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <returns></returns>
    public Coroutine WrapCoroutine<P0, P1, P2, P3>(MonoBehaviour component, Func<P0, P1, P2, P3, IEnumerator> coroutine, P0 p0, P1 p1, P2 p2, P3 p3) {
      IEnumerator i = null;

      try {
        i = coroutine(p0, p1, p2, p3);
      } catch (Exception exception) {
        this.exception = exception;
      }

      if (i != null) {
        this.coroutine = component.StartCoroutine(RunCoroutine(i));
        return this.coroutine;
      }

      return null;
    }

    /// <summary>
    /// Wrap a coroutine by name, will catch and store any thrown exceptions.
    /// </summary>
    /// <param name="component"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public Coroutine WrapCoroutineByName(MonoBehaviour component, string name) {
      return WrapCoroutineByName(component, name, null);
    }

    /// <summary>
    /// Wrap a coroutine by name, will catch and store any thrown exceptions.
    /// </summary>
    /// <param name="component"></param>
    /// <param name="name"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public Coroutine WrapCoroutineByName(MonoBehaviour component, string name, object[] parameters) {
      IEnumerator i = null;

      try {
        Type type = component.GetType();
        MethodInfo method = type.GetMethod(name);

        if (method == null) {
          throw new MissingMethodException(type.Name, name);
        }

        i = method.Invoke(component, parameters) as IEnumerator;
      } catch (Exception exception) {
        this.exception = exception;
      }

      if (i != null) {
        this.coroutine = component.StartCoroutine(RunCoroutine(i));
        return this.coroutine;
      }

      return null;
    }

    /// <summary>
    /// Iterate a coroutine and store any thrown exceptions.
    /// </summary>
    /// <param name="coroutine"></param>
    /// <returns></returns>
    public IEnumerator RunCoroutine(IEnumerator coroutine) {
      while (true) {
        try {
          if (!coroutine.MoveNext()) {
            yield break;
          }
        } catch (Exception exception) {
          this.exception = exception;
          yield break;
        }

        yield return coroutine.Current;
      }
    }
  }
}
