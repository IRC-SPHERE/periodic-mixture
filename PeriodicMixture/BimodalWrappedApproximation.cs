using System;
using MicrosoftResearch.Infer.Models;
using MicrosoftResearch.Infer.Maths;
using System.Linq;
using MicrosoftResearch.Infer.Distributions;
using MicrosoftResearch.Infer;

namespace PeriodicMixture {
  public class BimodalWrappedApproximation {
    public double period = 24.0; 

    public int mixture_count = 2; 
    public int approximation_count = 3; 

    public double [] observedData; 



    public BimodalWrappedApproximation() {
    }



    public void Infer( string filename = null ) {
      var meanOffsets = Enumerable.Range( 0, approximation_count ).Select( 
        ii => ii == 0 ? 0 : ( ( ii % 2 == 0 ? 1.0 : -1.0 ) * ( 1 + ( ii - 1 ) / 2 ) ) * period 
      ) ;

      var N = new Range( observedData.Count() ).Named( "N" );

      var data = Variable.Array<double>( N ).Named( "data" );
      data.ObservedValue = observedData; 

      var mixture_k = new Range( mixture_count ).Named( "mixture_k" ); 
      var approximation_k = new Range( meanOffsets.Count() ).Named( "approximation_count" ); 

      var approximation_means = Variable.Array<double>( mixture_k ).Named( "approximation_means" ); 
      approximation_means[mixture_k] = Variable.GaussianFromMeanAndPrecision( 0.0, 1e-2 ).ForEach( mixture_k ); 

      var approximation_precisions = Variable.Array<double>( mixture_k ).Named( "approximation_precisions" ); 
      approximation_precisions[mixture_k] = Variable.GammaFromShapeAndScale( 1.0, 1.0 ).ForEach( mixture_k ); 

      var approximation_meanOffsets = Variable.Array<double>( approximation_k ).Named( "approximation_meanOffsets" ); 
      approximation_meanOffsets.ObservedValue = meanOffsets.ToArray(); 

      var mixture_z = Variable.Array<int>( N ).Named( "mixture_z" ); 
      var mixture_weights = Variable.Dirichlet( mixture_k, Enumerable.Repeat( 1.0, mixture_k.SizeAsInt ).ToArray() );

      var approximation_z = Variable.Array<int>( N ).Named( "approximation_z" ); 

      using ( Variable.ForEach( N ) ) {
        mixture_z[N] = Variable.Discrete( mixture_weights ); 

        using ( Variable.Switch( mixture_z[N] ) ) {
          approximation_z[N] = Variable.DiscreteUniform( approximation_k ); 

          using ( Variable.Switch( approximation_z[N] ) ) {
            data[N] = Variable.GaussianFromMeanAndPrecision( 
              ( approximation_means[mixture_z[N]] + approximation_meanOffsets[approximation_z[N]] ).Named( "approximation_offsetMean" ), 
              approximation_precisions[mixture_z[N]] 
            );
          }
        }
      }



      var mixture_z_init = new Discrete[N.SizeAsInt]; 
      var approximation_z_init = new Discrete[N.SizeAsInt];
      for ( int i = 0; i < N.SizeAsInt; ++i ) {
        mixture_z_init[i] = Discrete.PointMass( Rand.Int( mixture_k.SizeAsInt ), mixture_k.SizeAsInt ); 
        approximation_z_init[i] = Discrete.PointMass( Rand.Int( approximation_k.SizeAsInt ), approximation_k.SizeAsInt ); 
      }
      mixture_z.InitialiseTo( Distribution<int>.Array( mixture_z_init ) ); 
      approximation_z.InitialiseTo( Distribution<int>.Array( approximation_z_init ) ); 



      var ie = new InferenceEngine {
        Algorithm = new GibbsSampling(),
        //Algorithm = new ExpectationPropagation(), 
        //Algorithm = new VariationalMessagePassing(),
        NumberOfIterations = 5000,
        ShowFactorGraph = true
      };

      var posteriorMixing = ie.Infer<Dirichlet>( mixture_weights );
      Console.WriteLine( "Mixing params:\n{0}\n{1}\n", mixture_weights, posteriorMixing.GetMean() );
      Console.WriteLine( "Estimated mean:\n{0}\n", ie.Infer( approximation_means ) );
      Console.WriteLine( "Estimated precision:\n{0}\n", ie.Infer( approximation_precisions ) );



    }

    public void Print( int numIterations = 5 ) {
    }
  }
}

