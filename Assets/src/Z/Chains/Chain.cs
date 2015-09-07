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

namespace Z.Chains {
  public class Chain {
    private List<ChainLink> links = new List<ChainLink>();

    private List<ChainLink> snapshot = new List<ChainLink>();

    private List<Exception> causes = new List<Exception>();

    private bool locked = false;

    private Action onComplete;

    public Action OnComplete {
      get {
        return onComplete;
      }
      set {
        onComplete = value;
      }
    }

    private Action<List<Exception>> onFailure;

    public Action<List<Exception>> OnFailure {
      get {
        return onFailure;
      }

      set {
        onFailure = value;
      }
    }

    private bool inOrder;

    /// <summary>
    /// Chain links will run in blocking order, instead of all at once.
    /// </summary>
    public bool InOrder {
      get {
        return inOrder;
      }
      set {
        inOrder = value;
      }
    }

    public Chain() {
    }

    public void Add(ChainLink link) {
      if (locked) {
        throw new InvalidOperationException("Cannot call Add() on a locked Chain.");
      }

      links.Add(link);
      link.Bind(this);
    }

    public void Clear() {
      if (locked) {
        throw new InvalidOperationException("Cannot call Clear() on a locked Chain.");
      }

      links.Clear();
    }

    public void Start() {
      if (locked) {
        throw new InvalidOperationException("Cannot call Start() on a locked Chain.");
      }

      if (links.Count > 0) {
        locked = true;

        if (inOrder) {
          links[0].Start();
        } else {
          // Snapshot, so modification on links is legit.
          snapshot.AddRange(links);

          foreach (ChainLink link in snapshot) {
            link.Start();
          }

          snapshot.Clear();
        }
      } else {
        if (onComplete != null) {
          onComplete();
        }
      }
    }

    public virtual void OnLinkComplete(ChainLink link) {
      if (!locked) {
        throw new InvalidOperationException("ChainLink calling OnLinkComplete() on an unlocked chain.");
      }

      if (links.Remove(link)) {
        link.Bind(null);

        if (links.Count > 0) {
          if (inOrder) {
            links[0].Start();
          }
        } else {
          locked = false;

          if (onComplete != null) {
            onComplete();
          }
        }
      }
    }

    public virtual void OnLinkFailure(ChainLink link, Exception exception) {
      if (!locked) {
        throw new InvalidOperationException("ChainLink calling OnLinkFailure() on an unlocked chain.");
      }

      if (links.Remove(link)) {
        link.Bind(null);

        causes.Add(exception);

        if (links.Count > 0) {
          if (inOrder) {
            // Snapshot, so modification on links is legit.
            snapshot.AddRange(links);

            foreach (ChainLink remainder in links) {
              remainder.OnCancelledDueToFailure();
              remainder.Bind(null);
            }

            snapshot.Clear();

            links.Clear();

            OnLinkFailure();
          }
        } else {
          OnLinkFailure();
        }
      }
    }

    public virtual bool OnLinkAdoptedByAnotherChain(ChainLink link, Chain other) {
      if (links.Remove(link)) {
        link.Bind(null);
      }

      return true;
    }

    private void OnLinkFailure() {
      locked = false;

      List<Exception> causesCopy = new List<Exception>(causes);
      causes.Clear();

      if (onFailure != null) {
        onFailure(causesCopy);
      }
    }
  }
}
