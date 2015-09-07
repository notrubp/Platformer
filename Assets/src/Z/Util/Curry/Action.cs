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

namespace Z.Util.Curry {
  /// <summary>
  /// Utility for function currying.
  /// <example>
  /// <code>
  /// Action<int, int> print = (x, y) => System.Console.WriteLine(x + ", " + y);
  /// Func<int, Action<int>> curried = print.Curry();
  /// Action<int> print1 = curried(1);
  ///
  /// // Prints "1, 1" to the console.
  /// print1(1);
  /// </code>
  /// </example>
  public static class Action {
    public static Action<P0> Curry<P0>(this Action<P0> f) {
      return p0 => f(p0);
    }

    public static Func<P0, Action<P1>> Curry<P0, P1>(this Action<P0, P1> f) {
      return p0 => p1 => f(p0, p1);
    }

    public static Func<P0, Func<P1, Action<P2>>> Curry<P0, P1, P2>(this Action<P0, P1, P2> f) {
      return p0 => p1 => p2 => f(p0, p1, p2);
    }

    public static Func<P0, Func<P1, Func<P2, Action<P3>>>> Curry<P0, P1, P2, P3>(this Action<P0, P1, P2, P3> f) {
      return p0 => p1 => p2 => p3 => f(p0, p1, p2, p3);
    }

    public static Func<P0, Func<P1, Func<P2, Func<P3, Action<P4>>>>> Curry<P0, P1, P2, P3, P4>(this Action<P0, P1, P2, P3, P4> f) {
      return p0 => p1 => p2 => p3 => p4 => f(p0, p1, p2, p3, p4);
    }

    public static Func<P0, Func<P1, Func<P2, Func<P3, Func<P4, Action<P5>>>>>> Curry<P0, P1, P2, P3, P4, P5>(this Action<P0, P1, P2, P3, P4, P5> f) {
      return p0 => p1 => p2 => p3 => p4 => p5 => f(p0, p1, p2, p3, p4, p5);
    }

    public static Func<P0, Func<P1, Func<P2, Func<P3, Func<P4, Func<P5, Action<P6>>>>>>> Curry<P0, P1, P2, P3, P4, P5, P6>(this Action<P0, P1, P2, P3, P4, P5, P6> f) {
      return p0 => p1 => p2 => p3 => p4 => p5 => p6 => f(p0, p1, p2, p3, p4, p5, p6);
    }

    public static Func<P0, Func<P1, Func<P2, Func<P3, Func<P4, Func<P5, Func<P6, Action<P7>>>>>>>> Curry<P0, P1, P2, P3, P4, P5, P6, P7>(this Action<P0, P1, P2, P3, P4, P5, P6, P7> f) {
      return p0 => p1 => p2 => p3 => p4 => p5 => p6 => p7 => f(p0, p1, p2, p3, p4, p5, p6, p7);
    }
  }
}
