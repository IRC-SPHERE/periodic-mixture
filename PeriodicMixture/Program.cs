using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MicrosoftResearch.Infer.Models;
using MicrosoftResearch.Infer;
using MicrosoftResearch.Infer.Distributions;
using MicrosoftResearch.Infer.Maths;
using Newtonsoft.Json;

namespace PeriodicMixture {
  class Program {
    static void Main( string [] args ) {
      var source = Demos.MixtureSource; 
      source.N = 1000; 
      source.Mean = new [] { 0, 6.0 }; 
      source.Variance = new [] { 4.0, 8.0 }; 
      source.Pk = new [] { 0.4, 0.6 }; 

      var data = source.Generate( "test.json" ); 

      //var wm = new WrappedMixture {
      var wm = new BimodalWrappedApproximation {
        observedData = data, 
        approximation_count = 3, 
        mixture_count = 2, 
        period = 24 
      };

      wm.Infer(); 

      Console.WriteLine( source ); 
      wm.Print( 50 ); 



      /*

      string filename = "../../../Data/" + 
        "r1" + "_" + "sleep" + "_td.json"
      ;

      var data = JsonConvert.DeserializeObject<List<double>>( 
        System.IO.File.ReadAllText(
          string.Format( 
            filename
          )
        )
      ); 

      System.IO.File.WriteAllText( "casas.json", JsonConvert.SerializeObject( data ) );

      Console.WriteLine( filename ); 
      Console.WriteLine( data.Count() ); 

      foreach ( var count in new Int32 [] { 1, 3 } ) {
        Console.WriteLine( "Wrapped approximators: {0}", count ); 

        //var wm = new WrappedMixture {
        var wm = new MixtureWrappedApproximation {
          approximation_count = count, 
          mixture_count = 2, 
          period = 24, 
          observedData = data.ToArray()
        };

        wm.Infer(); 
        wm.Print( 50 ); 
      }

      */
    }
  }
}
