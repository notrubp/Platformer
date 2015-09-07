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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MiniJSON;
using Z.Exceptions;
using Z.Network.Internal;
using Z.Threading;
using UnityEngine;

namespace Z.Network {
  public class HttpRequest<T> where T : HttpResponse, new() {
    private HttpMethod method;

    public HttpMethod Method {
      get {
        return method;
      }
      set {
        method = value;
      }
    }

    private string uri;

    public string Uri {
      get {
        return uri;
      }
      set {
        uri = value;
      }
    }

    private Query query;

    public Query Query {
      get {
        return query;
      }
      set {
        query = value;
      }
    }

    private double timeout = 60000;

    public double Timeout {
      get {
        return timeout;
      }
      set {
        timeout = value;
      }
    }

    private Dictionary<string, string> headers = new Dictionary<string, string>();

    public Dictionary<string, string> Headers {
      get {
        return headers;
      }
    }

    private byte[] data;

    public byte[] Data {
      get {
        return data;
      }
      set {
        data = value;
      }
    }

    private bool synchronous = false;

    public bool Synchronous {
      get {
        return synchronous;
      }
      set {
        synchronous = value;
      }
    }

    private volatile bool isPending = true;

    public bool IsPending {
      get {
        return isPending;
      }
    }

    private bool cancelled = false;

    public bool Cancelled {
      get {
        return cancelled;
      }
    }

    private T response;

    public T Response {
      get {
        return response;
      }
    }

    private Action<T> completeHandler;

    public Action<T> CompleteHandler {
      get {
        return completeHandler;
      }
      set {
        completeHandler = value;
      }
    }

    public HttpRequest(HttpMethod method, string uri) {
      this.method = method;
      this.uri = uri;

      headers.Add("User-Agent", Http.UserAgent);
      headers.Add("Accept-Encoding", "gzip, deflate");
    }

    public virtual void Send() {
      isPending = true;

      StartCoroutineOnMainThread.Enqueue(SendCoroutine);

      if (synchronous) {
        while (isPending) {
          // Block.
        }
      }
    }

    public virtual void Cancel() {
      cancelled = true;
    }

    public void SetQuery(Query query) {
      this.query = query;
    }

    public void SetData(Query form) {
      headers["Content-Type"] = "application/x-www-form-urlencoded;charset=utf8";
      form.Prefix = "";
      data = Encoding.UTF8.GetBytes(form.ToString());
    }

    public void SetData(object json) {
      headers["Content-Type"] = "application/json;charset=utf8";
      data = Encoding.UTF8.GetBytes(Json.Serialize(json));
    }

    public void SetData(string text) {
      headers["Content-Type"] = "text/plain;charset=utf8";
      data = Encoding.UTF8.GetBytes(text);
    }

    private IEnumerator SendCoroutine() {
      string finalUri = uri;

      // Add query to final URI.
      if (query != null) {
        if (uri.Contains("?") && !uri.EndsWith("&")) {
          this.query.Prefix = "&";
        }

        finalUri = uri + query;
      }

      if (method == HttpMethod.Post && data == null) {
        // Unity will not do HTTP POST when data is null.
        SetData("\r\n");
      }

      DateTime timeout = DateTime.Now.AddMilliseconds(this.timeout);

      // Get cookies for this domain and merge them into the request headers.
      List<PublicCookie> cookies = Cookies.GetCookies(new Uri(finalUri));

      if (headers.ContainsKey("Cookie")) {
        headers["Cookie"] += (headers["Cookie"].EndsWith(";") ? "" : ";") + Algorithm.List.Join(cookies, ";");
      } else {
        headers.Add("Cookie", Algorithm.List.Join(cookies, ";"));
      }

      // Start the HTTP request.
      WWW www = new WWW(finalUri, data, headers);

      while (!www.isDone) {
        // Check for cancellation.
        if (cancelled) {
          // Kill the request.
          www.Dispose();

          isPending = false;

          // Exit the coroutine.
          yield break;
        }

        // Check for timeout.
        if (timeout < DateTime.Now) {
          OnTimeout(www);

          // Kill the request.
          www.Dispose();

          // Exit the coroutine.
          yield break;
        }

        // Continue pending.
        yield return 0;
      }

      OnSuccess(www);
    }

    private void OnTimeout(WWW www) {
      response = new T();

      try {
        response.OnTimeout(www);
      } catch (Exception exception) {
        ExceptionReporter.Enqueue(exception);
      }

      if (completeHandler != null) {
        CallOnMainThread.Enqueue(() => {
          completeHandler(response);
        });
      }

      isPending = false;
    }

    private void OnSuccess(WWW www) {
      response = new T();

      try {
        response.OnSuccess(www);
      } catch (Exception exception) {
        ExceptionReporter.Enqueue(exception);
      }

      if (completeHandler != null) {
        CallOnMainThread.Enqueue(() => {
          completeHandler(response);
        });
      }

      isPending = false;
    }
  }
}
