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
  /// Func<int, int, int> add = (x, y) => x + y;
  /// Func<int, Func<int, int>> curried = add.Curry();
  /// Func<int, int> add1 = curried(1);
  ///
  /// // x is set to 2.
  /// int x = add1(1);
  /// </code>
  /// </example>
  public static class Func {
    public static Func<P0, TResult> Curry<P0, TResult>(this Func<P0, TResult> f) {
      return p0 => f(p0);
    }

    public static Func<P0, Func<P1, TResult>> Curry<P0, P1, TResult>(this Func<P0, P1, TResult> f) {
      return p0 => p1 => f(p0, p1);
    }

    public static Func<P0, Func<P1, Func<P2, TResult>>> Curry<P0, P1, P2, TResult>(this Func<P0, P1, P2, TResult> f) {
      return p0 => p1 => p2 => f(p0, p1, p2);
    }

    public static Func<P0, Func<P1, Func<P2, Func<P3, TResult>>>> Curry<P0, P1, P2, P3, TResult>(this Func<P0, P1, P2, P3, TResult> f) {
      return p0 => p1 => p2 => p3 => f(p0, p1, p2, p3);
    }

    public static Func<P0, Func<P1, Func<P2, Func<P3, Func<P4, TResult>>>>> Curry<P0, P1, P2, P3, P4, TResult>(this Func<P0, P1, P2, P3, P4, TResult> f) {
      return p0 => p1 => p2 => p3 => p4 => f(p0, p1, p2, p3, p4);
    }

    public static Func<P0, Func<P1, Func<P2, Func<P3, Func<P4, Func<P5, TResult>>>>>> Curry<P0, P1, P2, P3, P4, P5, TResult>(this Func<P0, P1, P2, P3, P4, P5, TResult> f) {
      return p0 => p1 => p2 => p3 => p4 => p5 => f(p0, p1, p2, p3, p4, p5);
    }

    public static Func<P0, Func<P1, Func<P2, Func<P3, Func<P4, Func<P5, Func<P6, TResult>>>>>>> Curry<P0, P1, P2, P3, P4, P5, P6, TResult>(this Func<P0, P1, P2, P3, P4, P5, P6, TResult> f) {
      return p0 => p1 => p2 => p3 => p4 => p5 => p6 => f(p0, p1, p2, p3, p4, p5, p6);
    }

    public static Func<P0, Func<P1, Func<P2, Func<P3, Func<P4, Func<P5, Func<P6, Func<P7, TResult>>>>>>>> Curry<P0, P1, P2, P3, P4, P5, P6, P7, TResult>(this Func<P0, P1, P2, P3, P4, P5, P6, P7, TResult> f) {
      return p0 => p1 => p2 => p3 => p4 => p5 => p6 => p7 => f(p0, p1, p2, p3, p4, p5, p6, p7);
    }
  }
}