using System;
using MicrosoftResearch.Infer.Models;
using MicrosoftResearch.Infer.Maths;
using System.Linq;
using MicrosoftResearch.Infer.Distributions;
using MicrosoftResearch.Infer;

namespace PeriodicMixture {
  public class UnimodalWrappedApproximation {


    public double period = 24.0; 
    public int approximation_count = 3; 

    public double [] observedData; 



    private Range N; 
    private VariableArray<double> data; 



    public UnimodalWrappedApproximation() {
    }



    public void Infer( string filename = null ) {
      var meanOffsets = Enumerable.Range( 0, approximation_count ).Select( 
        ii => ii == 0 ? 0 : ( ( ii % 2 == 0 ? 1.0 : -1.0 ) * ( 1 + ( ii - 1 ) / 2 ) ) * period 
      ) ;

      N = new Range( observedData.Count() ).Named( "N" );

      data = Variable.Array<double>( N ).Named( "data" );
      data.ObservedValue = observedData; 

      var approximation_k = new Range( approximation_count ).Named( "approximation_count" ); 
      var approximation_mean = Variable.GaussianFromMeanAndPrecision( 0.0, 1e-2 ).Named( "approximation_mean" ); 
      var approximation_precision = Variable.GammaFromShapeAndScale( 1.0, 1.0 ).Named( "approximation_precision" ); 

      var approximation_meanOffsets = Variable.Array<double>( approximation_k ).Named( "approximation_meanOffsets" ); 
      approximation_meanOffsets.ObservedValue = meanOffsets.ToArray(); 

      var approximation_z = Variable.Array<int>( N ).Named( "approximation_z" ); 

      using ( Variable.ForEach( N ) ) {
        approximation_z[N] = Variable.DiscreteUniform( approximation_k ); 

        using ( Variable.Switch( approximation_z[N] ) ) {
          data[N] = Variable.GaussianFromMeanAndPrecision( 
            approximation_mean + approximation_meanOffsets[approximation_z[N]], 
            approximation_precision 
          );
        }
      }

      var ie = new InferenceEngine {
        Algorithm = new GibbsSampling(),
        NumberOfIterations = 2500,
        ShowFactorGraph = true
      };

      Console.WriteLine( "Estimated mean:\n{0}\n", ie.Infer( approximation_mean ) );
      Console.WriteLine( "Estimated precision:\n{0}\n", ie.Infer( approximation_precision ) );
    }

    public void Print( int numIterations = 5 ) {

    }
  }
}

