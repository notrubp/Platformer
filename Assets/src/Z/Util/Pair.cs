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


namespace Z.Util {
  public class Pair<T, U> {
    public T first;

    public U second;

    public Pair() {
    }

    public Pair(T first, U second) {
      this.first = first;
      this.second = second;
    }

    public override int GetHashCode() {
      int seed = first.GetHashCode();
      seed ^= (int) (second.GetHashCode() + 0x9e3779b9 + (seed << 6) + (seed >> 2));
      return seed;
    }

    public override string ToString() {
      return first + "," + second;
    }

    public override bool Equals(object obj) {
      if (object.ReferenceEquals(this, obj)) {
        return true;
      }

      if (obj is Pair<T, U>) {
        Pair<T, U> that = obj as Pair<T, U>;
        return this.first.Equals(that.first) && this.second.Equals(that.second);
      }

      return false;
    }
  }
}
