using System;
using MicrosoftResearch.Infer.Models;
using System.Linq;
using MicrosoftResearch.Infer.Distributions;
using MicrosoftResearch.Infer;
using MicrosoftResearch.Infer.Maths;
using Newtonsoft.Json;

namespace PeriodicMixture {
  public class WrappedMixture {


    public double period = 24.0; 
    public int approximation_count = 3; 
    public int mixture_count = 2; 

    public double [] observedData; 



    private Range N; 
    private VariableArray<double> data; 

    private Range approximation_k; 
    private VariableArray<double> approximation_means; 
    private VariableArray<int> approximation_z; 

    private Range mixture_k; 
    private VariableArray<double> mixture_means; 
    private VariableArray<double> mixture_precisions; 
    private VariableArray<int> mixture_z; 
    private Variable<Vector> mixture_weights; 



    public WrappedMixture() {
    }



    public void Infer() {
      if ( approximation_count % 2 == 0 ) {
        Console.WriteLine( "Warning: incrementing the approximation_count variable (should be odd, but is passed in as even)." );
        approximation_count++; 
      }

      // Dataset parameters
      N = new Range( observedData.Count() ).Named( "N" );
      data = Variable.Array<double>( N ).Named( "data" );
      data.ObservedValue = observedData; 



      // Mixture approximation parameters
      approximation_k = new Range( approximation_count ).Named( "approximation_k" );
      approximation_means = Variable.Array<double>( approximation_k ).Named( "approximation_means" );
      approximation_z = Variable.Array<int>( N ).Named( "approximation_z" ); 

      var approximation_offsets = Enumerable.Range( 0, approximation_count ).Select( 
        ii => ii == 0 ? 0 : ( ii % 2 == 0 ? 1.0 : -1.0 ) * ( 1 + ( ii - 1 ) / 2 ) );
      approximation_means.ObservedValue = approximation_offsets.ToArray(); 



      // Mixture model parameters
      mixture_k = new Range( mixture_count ).Named( "mixture_k" );
      mixture_means = Variable.Array<double>( mixture_k ).Named( "mixture_means" );
      mixture_precisions = Variable.Array<double>( mixture_k ).Named( "mixture_precisions" );
      mixture_z = Variable.Array<int>( N ).Named( "mixture_z" );
      mixture_weights = Variable.Dirichlet( mixture_k, 
        Enumerable.Repeat( 1.0, mixture_k.SizeAsInt ).ToArray() 
      ).Named( "mixture_weights" );

      mixture_means [mixture_k] = Variable.GaussianFromMeanAndPrecision( 0.0, 1e-2 ).ForEach( mixture_k );
      mixture_precisions [mixture_k] = Variable.GammaFromShapeAndScale( 1.0, 1.0 ).ForEach( mixture_k );



      using ( Variable.ForEach( N ) ) {
        mixture_z [N] = Variable.Discrete( mixture_weights );

        using ( Variable.Switch( mixture_z [N] ) ) {
          approximation_z[N] = Variable.DiscreteUniform( approximation_k );

          using ( Variable.Switch( approximation_z[N] ) ) {
            Variable.ConstrainBetween( mixture_means [mixture_z [N]], 0, period ); 
            var componentOffset = ( approximation_means [approximation_z[N]] * period ).Named( "componentOffset" );
            var componentMean = ( mixture_means[mixture_z[N]] + componentOffset ).Named( "componentMean" );
            var componentDistribution = Variable.GaussianFromMeanAndPrecision( 
              componentMean, 
              mixture_precisions[mixture_z[N]] ).Named( "componentDistribution" );

            data [N] = componentDistribution;
          }
        }
      }

      // Break symmetry by evenly distributing means over the period
      var inity = new Gaussian [mixture_k.SizeAsInt];
      for ( int i = 0; i < mixture_k.SizeAsInt; ++i ) 
        inity[i] = Gaussian.FromMeanAndPrecision( i * period / ( mixture_k.SizeAsInt ), 1e-2 );
      mixture_means.InitialiseTo( Distribution<double>.Array( inity ) );
    }

    public void Print( int numIterations = 10 ) {
      // The inference
      var ie = new InferenceEngine {
        Algorithm = new ExpectationPropagation(),
        NumberOfIterations = numIterations,
        ShowFactorGraph = false
      };

      var posteriorMixing = ie.Infer<Dirichlet>(mixture_weights);

      // Print posteriors
      Console.WriteLine( "\nPosterior mixing:\n{0}, mean = {1}\n", mixture_weights, posteriorMixing.GetMean() );
      Console.WriteLine( "Estimated mean:\n{0}\n", ie.Infer( mixture_means ) );
      Console.WriteLine( "Estimated precision:\n{0}\n", ie.Infer( mixture_precisions ) );


      Console.WriteLine(); 
    }
  }
}

