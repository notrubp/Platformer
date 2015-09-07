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

using System.Text.RegularExpressions;
using UnityEngine;

namespace Z.Network {
  public class HttpRequestStatus {
    private string uri = "";

    private bool ok = false;

    public bool Ok {
      get {
        return ok;
      }
    }

    private int code = 0;

    public int Code {
      get {
        return code;
      }
    }

    private string reason;

    public string Reason {
      get {
        return reason;
      }
    }

    private bool isIncomplete = false;

    public bool IsIncomplete {
      get {
        return isIncomplete;
      }
    }

    private bool timedOut = false;

    public bool TimedOut {
      get {
        return timedOut;
      }
    }

    private bool notAuthorized = false;

    public bool NotAuthorized {
      get {
        return notAuthorized;
      }
    }

    private bool failed = false;

    public bool Failed {
      get {
        return failed;
      }
    }

    public virtual void OnTimeout(WWW www) {
      this.uri = www.url;

      timedOut = true;
      failed = true;
    }

    public virtual void OnSuccess(WWW www) {
      this.uri = www.url;

      if (!www.isDone) {
        isIncomplete = true;
        return;
      }

      if (www.responseHeaders.ContainsKey("STATUS")) {
        // http://www.w3.org/Protocols/rfc2616/rfc2616-sec6.html
        Regex regex = new Regex(@"^HTTP\/[\d.]+\s+(\d+)\s+(.*)$");
        Match match = regex.Match(www.responseHeaders["STATUS"]);

        if (match.Success) {
          int.TryParse(match.Groups[1].Value, out code);
          reason = match.Groups[1].Value;

          switch (code) {
          case 200:
            ok = true;
            break;
          case 401:
          case 403:
            notAuthorized = true;
            failed = true;
            break;
          default:
            failed = true;
            break;
          }
        } else {
          timedOut = true;
          failed = true;
        }
      } else {
        timedOut = true;
        failed = true;
      }
    }

    public void Throw() {
      if (isIncomplete) {
        throw new RequestIsNotCompleteException();
      }

      if (timedOut) {
        throw new RequestTimedOutException();
      }

      if (notAuthorized) {
        throw new NotAuthorizedException();
      }

      if (failed) {
        throw new RequestFailedException(uri, code);
      }
    }
  }
}
