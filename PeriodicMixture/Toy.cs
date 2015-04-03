using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MicrosoftResearch.Infer.Models;
using MicrosoftResearch.Infer;
using MicrosoftResearch.Infer.Distributions;
using MicrosoftResearch.Infer.Maths;
using Newtonsoft.Json;
using System.IO;



namespace PeriodicMixture {
  public class Toy {
    static void TopMain( string [] args ) {
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

    }
  }
}

