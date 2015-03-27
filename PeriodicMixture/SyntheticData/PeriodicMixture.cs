using System;
using MicrosoftResearch.Infer.Distributions;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace PeriodicMixture {
  public interface IPeriodicGenerator {
    int N {
      get; 
    }

    double [] Mean { 
      get; 
    }

    double [] Variance { 
      get; 
    }

    double Period {
      get; 
    }

    double [] Generate(); 
  }

  public class PeriodicMixture : IPeriodicGenerator {
    public static string filename = "MixtureSamples.json"; 

    public int N { get; set; }
    public double Period { get; set; }
    public double [] Mean {get; set;}
    public double [] Variance { get; set; }

    public double PY1 = 0.4;

    public double [] Generate() {
      var dists = Enumerable.Range( 0, 2 ).Select(
        ii => new WrappedGaussian( Mean[ii], Variance[ii], Period )
      );

      var data = new double [N];
      var samples = new List<double>();

      var trueB = new Bernoulli( PY1 );

      for ( int j = 0; j < N; j++ ) {
        var sample = trueB.Sample() ? dists.First().Sample() : dists.Last().Sample();

        samples.Add( sample );
        data [j] = sample;
      } 

      System.IO.File.WriteAllText( filename, JsonConvert.SerializeObject( samples ) );

      return data;
    }

  }
}

