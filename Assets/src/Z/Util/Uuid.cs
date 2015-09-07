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
using Random = UnityEngine.Random;

namespace Z.Util {
  public class Uuid {
    private const string rfc4122t = "XXXXXXXX-XXXX-4XXX-XXXX-XXXXXXXXXXX";

    private const string chars = "0123456789abcdef";

    public static string rfc4122(String seed) {
      int oldSeed = Random.seed;
      Random.seed = seed.GetHashCode();
      string uuid = rfc4122();
      Random.seed = oldSeed;
      return uuid;
    }

    public static string rfc4122() {
      string uuid = "";

      for (int i = 0; i < rfc4122t.Length; ++i) {
        char c = rfc4122t[i];

        if (c == 'X') {
          int j = Random.Range(0, chars.Length);
          c = chars[i == 19 ? (j & 0x3) | 0x8 : j & 0xf];
        }

        uuid += c;
      }

      return uuid;
    }

    private const string letterst = "abcdefghijklmnopqrstuvwxyz";

    public static string letters() {
      return letters(36);
    }

    public static string letters(int length) {
      string uuid = "";

      for (int i = 0; i < length; ++i) {
        uuid += letterst[Random.Range(0, letterst.Length)];
      }

      return uuid;
    }

    public static string hex() {
      return hex(36);
    }

    public static string hex(int length) {
      string hex = "";

      for (int i = 0; i < length; ++i) {
        int j = Random.Range(0, chars.Length);
        hex += chars[j];
      }

      return hex;
    }
  }
}
