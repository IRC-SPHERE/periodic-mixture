using System;
using Newtonsoft.Json;

namespace PeriodicMixture {
  public static class Utils {
    public static void Print( object obj, Formatting formatting = Formatting.Indented ) {
      Console.WriteLine( JsonConvert.SerializeObject( obj, formatting ) );
    }
    public static double FractionOfHour(DateTime dt){
      return dt.Hour + (dt.Minute / 60.0) + (dt.Second / 3600.0) + (dt.Millisecond / 3600000.0);
    }
  }
}

