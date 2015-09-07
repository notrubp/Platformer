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

namespace Z.Algorithm {
  public static class List {
    public static bool Contains<T>(this IList<T> array, T value) {
      foreach (T t in array) {
        if (t.Equals(value)) {
          return true;
        }
      }

      return false;
    }

    public static string ToString<T>(this IList<T> list) {
      return "[" + Join(list) + "]";
    }

    public static IList<T> Shuffle<T>(this IList<T> list) {
      Random random = new Random();

      for (int i = list.Count - 1; i >= 0; --i) {
        int j = random.Next(0, i);
        Swap(list, i, j);
      }

      return list;
    }

    public static void Swap<T>(this IList<T> list, int i, int j) {
      T k = list[i];
      list[i] = list[j];
      list[j] = k;
    }
    
    public static string Join<T>(this IList<T> list) {
      return Join<T>(list, ",");
    }

    public static string Join<T>(this IList<T> list, string seperator) {
      string joined = "";
      string suffix = "";

      foreach (T t in list) {
        joined += suffix + t.ToString();
        suffix = seperator;
      }

      return joined;
    }

    public static void AddAll<T>(this IList<T> list, T[] array) {
      foreach (T t in array) {
        list.Add(t);
      }
    }

    public static void AddAll<T>(this IList<T> list, IList<T> other) {
      foreach (T t in other) {
        list.Add(t);
      }
    }

    public static IList<T> Rotate<T>(this IList<T> list, int distance) {
      distance = distance % list.Count;
      Reverse(list, 0, list.Count);
      Reverse(list, 0, distance);
      Reverse(list, distance, list.Count);

      return list;
    }

    public static IList<T> Reverse<T>(this IList<T> list) {
      Reverse(list, 0, list.Count);
      return list;
    }

    public static IList<T> Reverse<T>(this IList<T> list, int l, int r) {
      for (int left = l, right = r - 1; left < right; left++, right--) {
        Swap(list, left, right);
      }

      return list;
    }
  }
}
