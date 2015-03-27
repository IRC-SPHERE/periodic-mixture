using System;
using Newtonsoft.Json;

namespace PeriodicMixture {
  public static class Utils {
    public static void Print( object obj, Formatting formatting = Formatting.Indented ) {
      Console.WriteLine( JsonConvert.SerializeObject( obj, formatting ) );
    }
  }
}

