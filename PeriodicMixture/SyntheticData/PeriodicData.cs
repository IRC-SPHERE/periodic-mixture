using System;
using MicrosoftResearch.Infer.Distributions;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace PeriodicMixture {
  public interface IPeriodicData {
    int N {
      get; 
    }

    double [] Mean { 
      get; 
    }

    double [] Variance { 
      get; 
    }

    double [] Pk {
      get; 
    }

    double Period {
      get; 
    }

    double [] Generate( string filename = null ); 
  }



  public class PeriodicData : IPeriodicData {
    public int N { get; set; }
    public double Period { get; set; }
    public double [] Mean {get; set;}
    public double [] Variance { get; set; }
    public double [] Pk { get; set; }

    public double [] Generate( string filename = null ) {
      var K = Mean.Count(); 

      var dists = Enumerable.Range( 0, K ).Select(
        ii => new WrappedGaussian( Mean[ii], Variance[ii], Period )
      );

      var data = new double [N];
      var samples = new List<double>();

      var component = new Discrete( Pk );

      for ( int j = 0; j < N; j++ ) {
        var k = component.Sample(); 
        var sample = dists.ElementAt( k ).Sample();

        samples.Add( sample );
        data [j] = sample;
      } 

      if ( filename != null )
        System.IO.File.WriteAllText( filename, JsonConvert.SerializeObject( samples ) );

      return data;
    }


    public override string ToString() {
      return string.Format( "True parameters:\n  N         = {0}\n  Period    = {1}\n  Mean      = {2}\n  Variance  = {3}\n  Precision = {4}\n  Pk        = {5}\n\n", 
        N, Period,
        JsonConvert.SerializeObject( Mean ),
        JsonConvert.SerializeObject( Variance ),
        JsonConvert.SerializeObject( Variance.Select( vv => 1.0 / vv ) ), 
        JsonConvert.SerializeObject( Pk ));
    }
  }
}

