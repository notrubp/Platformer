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

using System.Collections.Generic;
using System.Globalization;
using System;

namespace Z.StringUtil.Format {
  public class Abbreviation {
    private static Abbreviation thousand = new Abbreviation("K", 3, 3);

    public static Abbreviation Thousand {
      get {
        return thousand;
      }
    }

    private static Abbreviation tenThousand = new Abbreviation("K", 3, 4);

    public static Abbreviation TenThousand {
      get {
        return tenThousand;
      }
    }

    private static Abbreviation hundredThousand = new Abbreviation("K", 3, 5);

    public static Abbreviation HundredThousand {
      get {
        return hundredThousand;
      }
    }

    private static Abbreviation million = new Abbreviation("M", 6, 6);

    public static Abbreviation Million {
      get {
        return million;
      }
    }

    private static Abbreviation tenMillion = new Abbreviation("M", 6, 7);

    public static Abbreviation TenMillion {
      get {
        return tenMillion;
      }
    }

    private static Abbreviation hundredMillion = new Abbreviation("M", 6, 8);

    public static Abbreviation HundredMillion {
      get {
        return hundredMillion;
      }
    }

    private static Abbreviation billion = new Abbreviation("B", 9, 9);

    public static Abbreviation Billion {
      get {
        return billion;
      }
    }

    private static Abbreviation tenBillion = new Abbreviation("B", 9, 10);

    public static Abbreviation TenBillion {
      get {
        return tenBillion;
      }
    }

    private static Abbreviation hundredBillion = new Abbreviation("B", 9, 11);

    public static Abbreviation HundredBillion {
      get {
        return hundredBillion;
      }
    }

    private static Abbreviation trillion = new Abbreviation("T", 12, 12);

    public static Abbreviation Trillion {
      get {
        return trillion;
      }
    }

    private static Abbreviation quadrillion = new Abbreviation("P", 15, 15);

    public static Abbreviation Quadrillion {
      get {
        return quadrillion;
      }
    }

    private static Abbreviation quintillion = new Abbreviation("E", 18, 18);

    public static Abbreviation Quintillion {
      get {
        return quintillion;
      }
    }

    private static Abbreviation sextillion = new Abbreviation("Z", 21, 21);

    public static Abbreviation Sextillion {
      get {
        return sextillion;
      }
    }

    private static Abbreviation septillion = new Abbreviation("Y", 24, 24);

    public static Abbreviation Septillion {
      get {
        return septillion;
      }
    }

    private static Dictionary<string, Abbreviation> dictionary = new Dictionary<string, Abbreviation> {
      { "Thousand", thousand },
      { "TenThousand", tenThousand },
      { "HundredThousand", hundredThousand },
      { "Million", million },
      { "TenMillion", tenMillion },
      { "HundredMillion", hundredMillion },
      { "Billion", billion },
      { "TenBillion", tenBillion },
      { "HundredBillion", hundredBillion },
      { "Trillion", trillion },
      { "Quadrillion", quadrillion },
      { "Quintillion", quintillion },
      { "Sextillion", sextillion },
      { "Septillion", septillion }
    };

    public static Dictionary<string, Abbreviation> Dictionary {
      get {
        return dictionary;
      }
    }

    private static Abbreviation[] ordered = {
      thousand,
      tenThousand,
      hundredThousand,
      million,
      tenMillion,
      hundredMillion,
      billion,
      tenBillion,
      hundredBillion,
      trillion,
      quadrillion,
      quintillion,
      sextillion,
      septillion 
    };

    public static Abbreviation[] Ordered {
      get {
        return ordered;
      }
    }

    private string symbol;

    public string Symbol {
      get {
        return symbol;
      }
    }

    private double truncate;

    public double Truncate {
      get {
        return truncate;
      }
    }

    private double exponent;

    public double Exponent {
      get {
        return exponent;
      }
    }

    protected Abbreviation(string symbol, double truncate, double exponent) {
      this.symbol = symbol;
      this.truncate = truncate;
      this.exponent = exponent;
    }
  }

  public class Abbreviate : IFormatter {
    private NumberFormatInfo info = new CultureInfo("en-US", false).NumberFormat;

    public string format(string value, Dictionary<string, string> dictionary) {
      if (value == null || value.Equals(string.Empty)) {
        return "NaN";
      }

      Abbreviation threshold = Abbreviation.TenThousand;

      if (dictionary.ContainsKey("at")) {
        string at = dictionary["at"];

        if (Abbreviation.Dictionary.ContainsKey(at)) {
          threshold = Abbreviation.Dictionary[at];
        }
      }

      double number = double.Parse(value);
      double exponent = System.Math.Log10(number);

      Abbreviation abbr = null;

      for (int i = Abbreviation.Ordered.Length - 1; i >= 0; --i) {
        Abbreviation a = Abbreviation.Ordered[i];

        if (a.Exponent < threshold.Exponent) {
          break;
        }

        if (a.Exponent <= exponent) {
          abbr = a;
          break;
        }
      }

      if (abbr != null) {
        double shifted = number / System.Math.Pow(10, abbr.Truncate);

        // Only show the tenth if it's > 0
        info.NumberDecimalDigits = Math.Round(shifted, 1) % 1.0 > 0 ? 1 : 0;

        return shifted.ToString("N", info) + abbr.Symbol;
      }

      info.NumberDecimalDigits = 0;

      return number.ToString("N", info);
    }
  }
}
