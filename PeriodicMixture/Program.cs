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
      source.Pk = new [] { 0.35, 0.65 }; 
      source.Mean = new [] { 7.0, 0.0 }; 
      source.Variance = new [] { 2.0, 8.0 }; 

      var data = source.Generate();

      var wm = new MultimodalWrappedApproximation {
        period = 24,
        observedData = data, 
        approximation_count = 3, 
        mixture_count = 2, 

        algorithm = new GibbsSampling(),
        NumberOfIterations= 2500
      };

      wm.Infer( "test.json" ); 

      Console.WriteLine( source ); 




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
