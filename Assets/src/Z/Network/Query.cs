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

namespace Z.Network {
  public class Query {
    private Dictionary<string, string> table;

    public Dictionary<string, string> Table {
      get {
        return table;
      }
    }

    private string prefix = "?";

    public string Prefix {
      get {
        return prefix;
      }
      set {
        prefix = value;
      }
    }

    private string subPrefix = "&";

    public string SubPrefix {
      get {
        return subPrefix;
      }
      set {
        subPrefix = value;
      }
    }

    private string separator = "=";

    public string Separator {
      get {
        return separator;
      }
      set {
        separator = value;
      }
    }

    public Query() {
      this.table = new Dictionary<string, string>();
    }

    public Query(params KeyValuePair<string, string>[] arguments)
      : this() {
      foreach (KeyValuePair<string, string> pair in arguments) {
        table.Add(pair.Key, pair.Value);
      }
    }

    public Query(string query)
      : this() {
      if (query != null && query != String.Empty) {
        if (query[0] == '?') {
          query = query.Substring(1);
        }

        string[] split = query.Split('&');

        foreach (string value in split) {
          string[] kv = value.Split(new char[] { '=' }, 2);

          if (kv.Length > 0) {
            string k = kv[0];
            string v = "";

            if (kv.Length > 1) {
              v = kv[1];
            }

            table.Add(k, v);
          }
        }
      }
    }

    public Query(Dictionary<string, string> table) {
      this.table = table;
    }

    public override string ToString() {
      string value = "";

      string pre = prefix;

      foreach (KeyValuePair<string, string> pair in table) {
        value += pre;
        value += pair.Key;

        if (pair.Value != null && !pair.Value.Equals(String.Empty)) {
          value += separator;
          value += pair.Value;
        }

        pre = subPrefix;
      }

      return value;
    }
  }
}
