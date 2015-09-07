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
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Ionic.Zlib;
using Z.Network.Internal;
using UnityEngine;

namespace Z.Network {
  public abstract class HttpResponse {
#if UNITY_IOS
    private static Regex CookieRegex = new Regex(@"((?:HttpOnly|Secure|(?:[^\s;]+)=(?:\w+,\s\d{2}-\w+-\d{4}\s\d{2}:\d{2}:\d{2}\s(?:UTC|GMT)|[^\s;]+)))");
#else
#endif

    private byte[] bytes;

    public byte[] Bytes {
      get {
        return bytes;
      }
    }

    private Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>();

    public Dictionary<string, List<string>> Headers {
      get {
        return headers;
      }
    }

    private List<Cookie> cookies = new List<Cookie>();

    public List<Cookie> Cookies {
      get {
        return cookies;
      }
    }

    private HttpRequestStatus status = new HttpRequestStatus();

    public HttpRequestStatus Status {
      get {
        return status;
      }
    }

    public virtual void OnTimeout(WWW www) {
      status.OnTimeout(www);
    }

    public virtual void OnSuccess(WWW www) {
      bytes = www.bytes;

      // Unity does not support multiple response headers of the same name, luckily we can extract the raw
      // string through the assembly.
      PropertyInfo property = typeof(WWW).GetProperty("responseHeadersString", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      string responseHeaders = property.GetValue(www, null) as string;

      string[] lines = responseHeaders.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

      for (int i = 1; i < lines.Length; ++i) {
        string[] kv = lines[i].Split(new char[] { ':' }, 2);

        if (kv.Length >= 2) {
          string k = kv[0];
          string v = kv[1].Trim();

          if (!headers.ContainsKey(k)) {
            headers[k] = new List<string>();
          }

          headers[k].Add(v);
        }
      }

#if UNITY_IOS
      // Unity will, depending on the platform, give back a single Set-Cookie header with all the values joined with a comma. Because the HTTP spec
      // for the Expires parameter has a comma in it this forces us to do some really painful string parsing below.
      if (headers.ContainsKey("Set-Cookie")) {
        foreach (string header in headers["Set-Cookie"]) {
          // Split all possible fields, including possibly multiple cookies.
          MatchCollection matches = CookieRegex.Matches(header);

          // Build the cookie(s) by reading values in-order.
          Cookie cookie = null;

          foreach (Match match in matches) {
            if (match.Success) {
              for (int i = 1; i < match.Groups.Count; ++i) {
                Group group = match.Groups[i];
                string[] kv = group.Value.Split('=');
                string k = kv[0];
                string v = kv.Length > 1 ? kv[1] : null;

                switch (k) {
                  case "Expires":
                    cookie.Expires = DateTime.Parse(v);
                    break;
                  case "Path":
                    cookie.Path = v;
                    break;
                  case "Domain":
                    cookie.Domain = v;
                    break;
                  case "Secure":
                    cookie.IsSecure = true;
                    break;
                  case "HttpOnly":
                    cookie.IsHttpOnly = true;
                    break;
                  default:
                    // New cookie, add the old one.
                    if (cookie != null) {
                      cookies.Add(cookie);
                      cookie = null;
                    }

                    cookie = new Cookie();
                    cookie.Name = k;
                    cookie.Value = v;
                    break;
                }
              }
            }
          }

          // This header has been parsed. Add the final cookie, if any.
          if (cookie != null) {
            cookies.Add(cookie);
            cookie = null;
          }
        }
      }
#else
      if (headers.ContainsKey("Set-Cookie")) {
        foreach (string header in headers["Set-Cookie"]) {
          cookies.Add(new Cookie(header));
        }
      }
#endif

      status.OnSuccess(www);

      if (status.Ok) {
        foreach (Cookie cookie in cookies) {
          Internal.Cookies.SetCookie(cookie);
        }
      }

      if (IsGzipped()) {
        Unzip();
      }
    }

    protected bool IsGzipped() {
      return headers.ContainsKey("Content-Encoding") && headers["Content-Encoding"].Contains("gzip");
    }

    protected void Unzip() {
      using (MemoryStream i = new MemoryStream(bytes)) {
        // Verify gzip header.
        byte[] header = new byte[10];
        if (i.Read(header, 0, header.Length) == 10 && header[0] == 0x1F && header[1] == 0x8B && header[2] == 8) {
          // Back to start.
          i.Seek(0, SeekOrigin.Begin);

          using (MemoryStream o = new MemoryStream()) {
            using (GZipStream gzip = new GZipStream(i, CompressionMode.Decompress)) {
              var buffer = new byte[1024];

              do {
                int read = gzip.Read(buffer, 0, buffer.Length);

                if (read == 0) {
                  break;
                }

                o.Write(buffer, 0, read);
              } while (true);
            }

            this.bytes = o.ToArray();
          }
        }
      }
    }
  }
}
