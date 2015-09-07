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
using System.Reflection;

namespace Z.StringUtil {
  /// <summary>
  /// Utility for expanding and formatting variables within a string.
  /// <example>
  /// <code>
  /// enum AnimalCommand {
  ///   Sit,
  ///   Speak,
  ///   Rollover,
  /// }
  ///
  /// enum AnimalType {
  ///   Dog
  /// }
  ///
  /// class AnimalOptions {
  ///   private Dog[] pals;
  ///
  ///   public Dog[] Pals {
  ///     get {
  ///       return pals;
  ///     }
  ///     set {
  ///       pals = value;
  ///     }
  ///   }
  /// }
  /// 
  /// class Dog {
  ///   public static AnimalType type = AnimalType.Dog;
  ///
  ///   private string name;
  ///
  ///   public string Name {
  ///     get {
  ///       return name;
  ///     }
  ///     set {
  ///       name = value;
  ///     }
  ///   }
  ///
  ///   public int age = 25;
  ///
  ///   private Dictionary<AnimalCommand, bool> commands = new Dictionary<AnimalCommand, bool>();
  ///
  ///   public Dictionary<AnimalCommand, bool> Commands {
  ///     get {
  ///       return commands;
  ///     }
  ///   }
  ///
  ///   private AnimalOptions animalOptions = new AnimalOptions();
  ///
  ///   public AnimalOptions AnimalOptions {
  ///     get {
  ///       return animalOptions;
  ///     }
  ///   }
  /// }
  ///
  /// Dog dog = new Dog();
  /// dog.Name = "Jimmy";
  /// dog.Commands.Add(AnimalCommand.Sit, true);
  /// dog.Commands.Add(AnimalCommand.Speak, true);
  /// dog.Commands.Add(AnimalCommand.Rollover, false);
  ///
  /// Dog other = new Dog();
  /// other.Name = "Spot";
  /// other.age = 5;
  /// other.Commands.Add(AnimalCommand.Sit, false);
  /// other.Commands.Add(AnimalCommand.Speak, false);
  /// other.Commands.Add(AnimalCommand.Rollover, false);
  ///
  /// dog.AnimalOptions.Pals = new Dog[1];
  /// dog.AnimalOptions.Pals[0] = other;
  ///
  /// // Expands to: "Jimmy is 25 Dog years old. Commands: Can he sit? True! His pal's name is Spot."
  /// "$(Name) is $(age) $(type) years old. Commands: Can he sit? $(Commands[Sit])! His pal's name is $(AnimalOptions.Pals[0].Name).".Expand(dog);
  ///
  /// // Expands to: "I have got 999,999,999 problems."
  /// "I have got #(999999999|Grouped) problems.".Expand()
  ///
  /// // Expands to: "You owe me $50."
  /// "You owe me $(Owed|Currency).".Expand(new {
  ///   Owed = 50,
  /// });
  ///
  /// // Expands to: "It is the 13th of May."
  /// "It is the #(13|Ordinal) of May.".Expand();
  ///
  /// class UpperCase : IFormatter {
  ///   public string format(string value, Dictionary<string, string> dictionary) {
  ///     return value.ToUpper();
  ///   }
  /// }
  ///
  /// class Reverse : IFormatter {
  ///   public string format(string value, Dictionary<string, string> dictionary) {
  ///     char[] arr = value.ToCharArray();
  ///     Array.Reverse(arr);
  ///     return new string(arr);
  ///   }
  /// }
  ///
  /// // Expands to: "That was a GREAT salad!"
  /// // Note: The '::' tells the formatter to find the type globally, instead of within the default formatter namespace.
  /// "That was a #(taerg|::UpperCase|::Reverse) salad!".Expand();
  ///
  /// Locale.Strings.Put("SomeString", "This is an english string.");
  ///
  /// // Expands to: "This is an english string."
  /// "$(SomeLocalizedString|)".Expand(new {
  ///   SomeLocalizedString = "%(SomeString)"
  /// });
  ///
  /// // Expands to: "50M"
  /// "#(50000000|Abbreviate?at=TenThousand)".Expand();
  ///
  /// // Expands to: "50,000,000" (because 50000000 is below the Abbreviate threshold of Trillion)
  /// "#(50000000|Abbreviate?at=Trillion)".Expand();
  /// </code>
  /// </example>
  /// </summary>
  public static class VariableExpansion {
    public static string Expand(this string str, params object[] objs) {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();

      foreach (object obj in objs) {
        Store(dictionary, "", obj);
      }

      return Expand(str, dictionary);
    }

    private static void Store(Dictionary<string, object> dictionary, string fqlName, object obj) {
      if (obj == null) {
        if (fqlName.Equals(String.Empty)) {
          fqlName = "anon_" + dictionary.Count;
        }

        dictionary.Add(fqlName, "null");
      } else {
        Type type = obj.GetType();

        if (type.IsClass) {
          if (obj is String) {
            if (fqlName.Equals(String.Empty)) {
              fqlName = "anon_" + dictionary.Count;
            }

            dictionary.Add(fqlName, obj.ToString());
          } else if (obj is IDictionary) {
            StoreDictionary(dictionary, fqlName, obj as IDictionary);
          } else if (obj is IEnumerable) {
            StoreEnumerable(dictionary, fqlName, obj as IEnumerable);
          } else {
            if (!fqlName.Equals(String.Empty)) {
              fqlName = fqlName + ".";
            }

            StoreClass(dictionary, fqlName, obj);
          }
        } else if (type.IsArray) {
          StoreArray(dictionary, fqlName, obj as Array);
        } else {
          if (fqlName.Equals(String.Empty)) {
            fqlName = "anon_" + dictionary.Count;
          }

          dictionary.Add(fqlName, obj.ToString());
        }
      }
    }

    private static void StoreDictionary(Dictionary<string, object> dictionary, string fqlName, IDictionary obj) {
      foreach (DictionaryEntry entry in obj) {
        Store(dictionary, fqlName + "[" + entry.Key + "]", entry.Value);
      }
    }

    private static void StoreEnumerable(Dictionary<string, object> dictionary, string fqlName, IEnumerable obj) {
      IEnumerator e = obj.GetEnumerator();
      for (int i = 0; e.MoveNext(); ++i) {
        Store(dictionary, fqlName + "[" + i + "]", e.Current);
      }
    }

    private static void StoreClass(Dictionary<string, object> dictionary, string fqlName, object obj) {
      Type type = obj.GetType();

      // Collect public fields and values.
      FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
      foreach (FieldInfo field in fields) {
        string fqlFieldName = fqlName + field.Name;

        if (!dictionary.ContainsKey(fqlName)) {
          object value = field.GetValue(obj);
          Store(dictionary, fqlFieldName, value);
        }
      }

      // Collect public properties and values.
      PropertyInfo[] properties = type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
      foreach (PropertyInfo property in properties) {
        if (property.CanRead) {
          string fqlPropertyName = fqlName + property.Name;

          if (!dictionary.ContainsKey(fqlName)) {
            object value = property.GetValue(obj, null);
            Store(dictionary, fqlPropertyName, value);
          }
        }
      }
    }

    private static void StoreArray(Dictionary<string, object> dictionary, string fqlName, object obj) {
      object[] arr = (object[]) obj;
      for (int i = 0; i < arr.Length; ++i) {
        Store(dictionary, fqlName + "[" + i + "]", arr[i]);
      }
    }

    public static string Expand(this string str, Dictionary<string, object> dictionary) {
      VariableExpander exp = new VariableExpander(str, dictionary);
      exp.Run();

      return exp.Value;
    }

    public static bool HasExpansion(this string str) {
      VariableExpander exp = new VariableExpander(str);
      return exp.Test();
    }
  }
}
