using System.Collections.Generic;

namespace Z.Locale {
  public class Strings {
    private static Dictionary<string, string> table = new Dictionary<string, string>();

    public static string Get(string key) {
      return table[key];
    }

    public static void Put(string key, string value) {
      table.Add(key, value);
    }

    public static bool Contains(string key) {
      return table.ContainsKey(key);
    }
  }
}
