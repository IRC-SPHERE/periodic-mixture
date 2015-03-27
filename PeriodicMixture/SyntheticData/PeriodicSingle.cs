using System;
using MicrosoftResearch.Infer.Distributions;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace PeriodicMixture {
  public class PeriodicSingle : IPeriodicGenerator {
    public string filename = "Samples.json"; 

    public int N { get; set; }
    public double [] Mean {get; set;}
    public double [] Variance { get; set; }
    public double Period { get; set; }

    public double PY1 = 0.4;

    public double [] Generate() {
      var dist = new WrappedGaussian( Mean[0], Variance[0], Period );

      var data = new double [N];
      var samples = new List<double>();

      var trueB = new Bernoulli( PY1 );

      for ( int j = 0; j < N; j++ ) {
        var sample = dist.Sample(); 

        samples.Add( sample );
        data [j] = sample;
      } 

      System.IO.File.WriteAllText( filename, JsonConvert.SerializeObject( samples ) );

      return data;
    }

  }
}

