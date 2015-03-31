using System;
using MicrosoftResearch.Infer.Models;
using MicrosoftResearch.Infer.Maths;
using System.Linq;
using MicrosoftResearch.Infer.Distributions;
using MicrosoftResearch.Infer;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PeriodicMixture {
  public class MultimodalWrappedApproximation {
    public double period = 24.0; 

    public int mixture_count = 2; 
    public int approximation_count = 3;

    public int NumberOfIterations = 2500;

    public IAlgorithm algorithm = new ExpectationPropagation();

    public double [] observedData;



    public MultimodalWrappedApproximation() {
    }



    public void Infer( string filename = null ) {
       

      var meanOffsets = Enumerable.Range( 0, approximation_count ).Select( 
        ii => ii == 0 ? 0 : ( ( ii % 2 == 0 ? 1.0 : -1.0 ) * ( 1 + ( ii - 1 ) / 2 ) ) * period 
      ) ;
      
      var N = new Range( observedData.Count() ).Named( "N" );

      var data = Variable.Array<double>( N ).Named( "data" );
      data.ObservedValue = observedData;

      var mixture_k = new Range( mixture_count ).Named( "mixture_k" );
      var approximation_k = new Range( approximation_count ).Named( "approximation_count" );

      var priorMeanMeans = Variable.Array<double>( mixture_k ).Named( "priorMeans" );
      priorMeanMeans.ObservedValue = Enumerable.Range( 0, mixture_k.SizeAsInt ).Select( i => i * period / mixture_k.SizeAsInt ).ToArray();

      var mixture_means = Variable.Array<double>( mixture_k ).Named( "approximation_means" );
      mixture_means [mixture_k] = Variable.GaussianFromMeanAndPrecision( priorMeanMeans [mixture_k], 1e-2 );

      var mixture_precisions = Variable.Array<double>( mixture_k ).Named( "approximation_precisions" );
      mixture_precisions [mixture_k] = Variable.GammaFromShapeAndScale( 1.0, 1.0 ).ForEach( mixture_k );

      var approximation_meanOffsets = Variable.Array<double>( approximation_k ).Named( "approximation_meanOffsets" );
      approximation_meanOffsets.ObservedValue = meanOffsets.ToArray();

      var mixture_z = Variable.Array<int>( N ).Named( "mixture_z" );
      var mixture_weights = Variable.Dirichlet( mixture_k, Enumerable.Repeat( 1.0, mixture_k.SizeAsInt ).ToArray() );

      var approximation_z = Variable.Array<int>( N ).Named( "approximation_z" );

      using ( Variable.ForEach( N ) ) {
        mixture_z [N] = Variable.Discrete( mixture_weights );

        using ( Variable.Switch( mixture_z [N] ) ) {
          approximation_z [N] = Variable.DiscreteUniform( approximation_k );

          using ( Variable.Switch( approximation_z [N] ) ) {
            if ( algorithm  is ExpectationPropagation )
              Variable.ConstrainBetween( mixture_means [mixture_z [N]], 0.0, period );

            data [N] = Variable.GaussianFromMeanAndPrecision(
              ( mixture_means [mixture_z [N]] + approximation_meanOffsets [approximation_z [N]] ).Named( "approximation_offsetMean" ),
              mixture_precisions [mixture_z [N]]
            );
          }
        }
      }


      var mixture_z_init = new Discrete[N.SizeAsInt]; 
      for ( int i = 0; i < N.SizeAsInt; ++i ) 
        mixture_z_init[i] = Discrete.PointMass( Rand.Int( mixture_k.SizeAsInt ), mixture_k.SizeAsInt ); 
      mixture_z.InitialiseTo( Distribution<int>.Array( mixture_z_init ) ); 



      var ie = new InferenceEngine {
        Algorithm = algorithm,
        NumberOfIterations = NumberOfIterations,
        ShowFactorGraph = false
      };

      var inferredMean = ie.Infer<Gaussian[]>( mixture_means );
      var inferredPrecision = ie.Infer<Gamma []>( mixture_precisions );
      var inferredWeights = ie.Infer<Dirichlet>( mixture_weights );

      if ( filename != null ) {
        var results = new Dictionary<string, object>();
        results ["Data"] = observedData;

        var resList = new List<Dictionary<string, double>>();
        for ( int ii = 0; ii < mixture_k.SizeAsInt; ++ii ) {
          var res = new Dictionary<string, double>();

          res ["Mean"] = inferredMean [ii].GetMean();
          res ["Precision"] = inferredPrecision [ii].GetMean();
          res ["Variance"] = 1.0 / inferredPrecision [ii].GetMean();
          res ["Weight"] = inferredWeights.GetMean() [ii];
          res ["Period"] = period;

          resList.Add( res );
        }

        results ["Params"] = resList;

        Utils.Print( resList ); 

        System.IO.File.WriteAllText( filename, JsonConvert.SerializeObject( results, Formatting.Indented ) );
      }

    }

    public void Print( int numIterations = 5 ) {
      Console.WriteLine( this.GetType().Name ); 
    }
  }
}

