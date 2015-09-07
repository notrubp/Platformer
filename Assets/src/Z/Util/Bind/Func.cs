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

namespace Z.Util.Bind {
  /// <summary>
  /// Utility for functional programming similar to boost::bind or JavaScript's bind.
  /// <example>
  /// <code>
  /// Func<int, int, int> add = (x, y) => x + y;
  /// Func<int> bound = add.Bind(1, 1);
  /// 
  /// // x is set to 2.
  /// int x = bound();
  /// </code>
  /// </example>
  public static class Func {
    public static Func<TResult> Bind<TResult>(this Func<TResult> f) {
      return () => f();
    }

    public static Func<TResult> Bind<P0, TResult>(this Func<P0, TResult> f, P0 p0) {
      return () => f(p0);
    }

    public static Func<TResult> Bind<P0, P1, TResult>(this Func<P0, P1, TResult> f, P0 p0, P1 p1) {
      return () => f(p0, p1);
    }

    public static Func<TResult> Bind<P0, P1, P2, TResult>(this Func<P0, P1, P2, TResult> f, P0 p0, P1 p1, P2 p2) {
      return () => f(p0, p1, p2);
    }

    public static Func<TResult> Bind<P0, P1, P2, P3, TResult>(this Func<P0, P1, P2, P3, TResult> f, P0 p0, P1 p1, P2 p2, P3 p3) {
      return () => f(p0, p1, p2, p3);
    }

    public static Func<TResult> Bind<P0, P1, P2, P3, P4, TResult>(this Func<P0, P1, P2, P3, P4, TResult> f, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4) {
      return () => f(p0, p1, p2, p3, p4);
    }

    public static Func<TResult> Bind<P0, P1, P2, P3, P4, P5, TResult>(this Func<P0, P1, P2, P3, P4, P5, TResult> f, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5) {
      return () => f(p0, p1, p2, p3, p4, p5);
    }

    public static Func<TResult> Bind<P0, P1, P2, P3, P4, P5, P6, TResult>(this Func<P0, P1, P2, P3, P4, P5, P6, TResult> f, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6) {
      return () => f(p0, p1, p2, p3, p4, p5, p6);
    }

    public static Func<TResult> Bind<P0, P1, P2, P3, P4, P5, P6, P7, TResult>(this Func<P0, P1, P2, P3, P4, P5, P6, P7, TResult> f, P0 p0, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7) {
      return () => f(p0, p1, p2, p3, p4, p5, p6, p7);
    }
  }
}
