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
using System.Text.RegularExpressions;

namespace Z.Network.Internal {
  public class Cookie {
    private static Regex CookieRegex = new Regex(@"\s*([^=]+)(?:=?((?:.|\n)*))");

    private string name = "";

    public string Name {
      get {
        return name;
      }
      set {
        name = value;
      }
    }

    private string value = "";

    public string Value {
      get {
        return value;
      }
      set {
        this.value = value;
      }
    }

    private string path = null;

    public string Path {
      get {
        return path;
      }
      set {
        path = value;
      }
    }

    private string domain = null;

    public string Domain {
      get {
        return domain;
      }
      set {
        domain = value;
      }
    }

    private DateTime expires = DateTime.MaxValue;

    public DateTime Expires {
      get {
        return expires;
      }
      set {
        expires = value;
      }
    }

    private bool isSecure = false;

    public bool IsSecure {
      get {
        return isSecure;
      }
      set {
        isSecure = value;
      }
    }

    private bool isHttpOnly = false;

    public bool IsHttpOnly {
      get {
        return isHttpOnly;
      }
      set {
        isHttpOnly = value;
      }
    }

    public Cookie() {
    }

    public Cookie(string cookie) {
      if (cookie == null || cookie.Equals(string.Empty)) {
        throw new InvalidCookieException("");
      }

      string[] values = cookie.Split(';');

      if (values.Length < 1) {
        throw new InvalidCookieException(cookie);
      }

      Extract(values[0], out name, out value);

      for (int i = 1; i < values.Length; ++i) {
        string k;
        string v;

        Extract(values[i], out k, out v);

        switch (k) {
        case "Expires":
          expires = DateTime.Parse(v);
          break;
        case "Path":
          this.path = v;
          break;
        case "Domain":
          this.domain = v;
          break;
        case "Secure":
          this.isSecure = true;
          break;
        case "HttpOnly":
          this.isHttpOnly = true;
          break;
        default:
          // Nothing to do here...
          break;
        }
      }
    }

    public override bool Equals(object obj) {
      if (object.ReferenceEquals(this, obj)) {
        return true;
      }

      if (obj is Cookie) {
        Cookie that = obj as Cookie;

        return this.name.Equals(that.name) &&
          this.value.Equals(that.value) &&
          ((this.path == null && that.path == null) || this.path.Equals(that.path)) &&
          ((this.domain == null && that.domain == null) || this.domain.Equals(that.domain)) &&
          this.expires.Equals(that.expires) &&
          this.isSecure.Equals(that.isSecure) &&
          this.isHttpOnly.Equals(that.isHttpOnly);
      }

      return false;
    }

    public override int GetHashCode() {
      int seed = name.GetHashCode();

      if (domain != null) {
        seed ^= domain.GetHashCode();
      }

      if (path != null) {
        seed ^= path.GetHashCode();
      }

      return seed;
    }

    public override string ToString() {
      Query cookie = new Query();
      cookie.Prefix = "";
      cookie.SubPrefix = ";";

      cookie.Table.Add(name, value);

      if (path != null) {
        cookie.Table.Add("Path", path);
      }

      if (domain != null) {
        cookie.Table.Add("Domain", domain);
      }

      if (expires != DateTime.MaxValue) {
        cookie.Table.Add("Expires", expires.ToString());
      }

      if (isSecure) {
        cookie.Table.Add("Secure", "");
      }

      if (isHttpOnly) {
        cookie.Table.Add("HttpOnly", "");
      }

      return cookie.ToString();
    }

    private void Extract(string cookie, out string k, out string v) {
      Match match = CookieRegex.Match(cookie);

      if (match.Success) {
        if (match.Groups.Count < 2) {
          throw new InvalidCookieException(cookie);
        }

        k = match.Groups[1].Value;

        if (match.Groups.Count < 3) {
          v = "";
        } else {
          v = match.Groups[2].Value;
        }
      } else {
        throw new InvalidCookieException(cookie);
      }
    }
  }
}
