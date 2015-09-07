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
using System.Text.RegularExpressions;
using Z.Logging;
using Z.Locale;
using Z.Network;
using Z.StringUtil.Format;

namespace Z.StringUtil {
  public class VariableExpander {
    // For brevity of format strings.
    private static string DefaultFormatterNamespace = typeof(IFormatter).Namespace;

    private static Regex VariablesRegex = new Regex(@"\$\((.*?)\)");

    private static Regex StringsRegex = new Regex(@"\%\((.*?)\)");

    private static Regex LiteralsRegex = new Regex(@"\#\((.*?)\)");

    private string value;

    public string Value {
      get {
        return value;
      }
    }

    private Dictionary<string, object> dictionary;

    private bool dirty = true;

    private int depth = 0;

    private int maxDepth = 5;

    public int MaxDepth {
      get {
        return maxDepth;
      }
      set {
        maxDepth = value;
      }
    }

    public VariableExpander(string value) {
      this.value = value;
    }

    public VariableExpander(string value, Dictionary<string, object> dictionary) {
      this.value = value;
      this.dictionary = dictionary;
    }

    public void Run() {
      while (dirty) {
        dirty = false;

        Strings();
        Variables();
        Literals();

        if (++depth > maxDepth) {
          break;
        }
      }
    }

    public bool Test() {
      return VariablesRegex.Match(value).Success || StringsRegex.Match(value).Success || LiteralsRegex.Match(value).Success;
    }

    private string Strings() {
      Match match = StringsRegex.Match(value);

      while (match.Success) {
        string k = match.Groups[1].Value;
        string v = Locale.Strings.Get(k);

        if (v == null) {
          v = k;

          Log.Warn(Channel.StringUtil, "Could not find locale string \"" + k + "\".");
        }

        // Replace with value.
        value = StringsRegex.Replace(value, v, 1);

        dirty = true;

        // Re-evaluate.
        match = StringsRegex.Match(value);
      }

      return value;
    }

    private string Variables() {
      Match match = VariablesRegex.Match(value);

      while (match.Success) {
        string k = match.Groups[1].Value;
        object v = null;
        string f = null;

        int split = k.IndexOf('|');
        if (split != -1) {
          string b = k;
          k = b.Substring(0, split);
          f = b.Substring(split + 1);
        }

        if (dictionary != null && dictionary.ContainsKey(k)) {
          v = dictionary[k];
        }

        if (v == null) {
          v = string.Empty;

          Log.Warn(Channel.StringUtil, "Could not expand variable \"" + k + "\".");
        }

        string s = v.ToString();

        try {
          if (f != null && !f.Equals(string.Empty)) {
            s = Format(s, f);
          }
        } catch (Exception exception) {
          Log.Warn(Channel.StringUtil, "Could not format variable \"" + k + "\" because of exception: \"" + exception.Message + "\".");
        }

        // Replace '$' with '$$' to avoid the submatches.
        s = s.Replace("$", "$$");

        // Replace with value.
        value = VariablesRegex.Replace(value, s, 1);

        dirty = true;

        // Re-evaluate.
        match = VariablesRegex.Match(value);
      }

      return value;
    }

    private string Literals() {
      Match match = LiteralsRegex.Match(value);

      while (match.Success) {
        string v = match.Groups[1].Value;
        string f = null;

        int split = v.IndexOf('|');
        if (split != -1) {
          string b = v;
          v = b.Substring(0, split);
          f = b.Substring(split + 1);
        }

        try {
          if (f != null) {
            v = Format(v, f);
          }
        } catch (Exception exception) {
          Log.Warn(Channel.StringUtil, "Could not format variable \"" + v + "\" because of exception: \"" + exception.Message + "\".");
        }

        // Replace with value.
        value = LiteralsRegex.Replace(value, v, 1);

        // Re-evaluate.
        match = LiteralsRegex.Match(value);
      }

      return value;
    }

    private static string Format(string v, string f) {
      if (v == null || v.Equals(String.Empty)) {
        return v;
      }

      foreach (string uri in f.Split('|')) {
        string t = uri;
        string q = null;

        int split = uri.IndexOf('?');
        if (split != -1) {
          t = uri.Substring(0, split);
          q = uri.Substring(split);
        }

        v = Format(v, t, new Query(q));
      }

      return v;
    }

    private static string Format(string v, string t, Query q) {
      IFormatter formatter = null;

      string path = t.StartsWith("::") ? t.Substring(2) : DefaultFormatterNamespace + "." + t;

      if (Reified.Table.ContainsKey(path)) {
        formatter = Reified.Table[path];
      } else {
        // Global scope formatters that start with ::, instead of looking in DefaultFormatterNamespace.
        Type type = Type.GetType(path);

        if (type == null) {
          throw new MissingFormatterException("Could not find formatter of type \"" + path + "\".");
        }

        formatter = (IFormatter) Activator.CreateInstance(type);

        Reified.Table.Add(path, formatter);
      }

      v = formatter.format(v, q.Table);

      return v;
    }
  }
}