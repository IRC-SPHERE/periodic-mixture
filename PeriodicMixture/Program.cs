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
    const double Period = 24.0;



    public static double [] GeneratePeriodicData( int nData, double mean, double variance ) {
      var dist = new WrappedGaussian( mean, variance, Period );

      var data = new double [nData];
      var samples = new List<double>();

      for ( int j = 0; j < nData; j++ ) {
        var sample = dist.Sample();

        samples.Add( sample );
        data [j] = sample;
      }

      System.IO.File.WriteAllText(
        "samples.json",
        JsonConvert.SerializeObject(
          samples
        )
      );

      return data;
    }


    public static double [] GeneratePeriodicMixtureData( int nData ) {
      var dist1 = new WrappedGaussian( 0, 6, Period );
      var dist2 = new WrappedGaussian( 15, 6, Period );

      var data = new double [nData];
      var samples = new List<double>();

      double truePi = 0.6;
      Bernoulli trueB = new Bernoulli( truePi );

      for ( int j = 0; j < nData; j++ ) {
        var sample = trueB.Sample() ? dist1.Sample() : dist2.Sample();

        samples.Add( sample );
        data [j] = sample;
      } 

      System.IO.File.WriteAllText(
        "samples.json",
        JsonConvert.SerializeObject(
          samples
        )
      );

      return data;
    }



    static void Main( string [] args ) {
      int N = 300;

      var k = new Range( 5 ).Named( "NumWrapped" );

      var n = new Range( N ).Named( "n" );
      var data = Variable.Array<double>( n ).Named( "x" );

      var meanOffset = Variable.Array<double>( k ).Named( "meanOffset" );
      meanOffset.ObservedValue = new double [] { 0, 1, -1, 2, -2 };

      var kMixture = new Range( 2 ).Named( "kMixture" );
      var meansMixture = Variable.Array<double>( kMixture ).Named( "meansMixture" );

      var precisionsMixture = Variable.Array<double>( kMixture ).Named( "precisionsMixture" );
      meansMixture [kMixture] = Variable.GaussianFromMeanAndPrecision( 0.0, 1e-4 ).ForEach( kMixture );
      precisionsMixture [kMixture] = Variable.GammaFromShapeAndScale( 1.0, 1.0 ).ForEach( kMixture );

      Gaussian [] inity = new Gaussian [kMixture.SizeAsInt];
      for ( int i = 0; i < inity.Length; ++i )
        inity[i] = Gaussian.FromMeanAndVariance( i == 0 ? 6 : 18, 3 );
      VariableArray<Gaussian> initVar = Variable.Observed( inity, kMixture );
      meansMixture.InitialiseTo( Distribution<double>.Array( inity ) );

      var weightsMixture = Variable.Dirichlet( kMixture, new double [] { 1, 1 } ).Named( "weightsMixture" );
      var zMixture = Variable.Array<int>( n ).Named( "zMixture" );


      using ( Variable.ForEach( n ) ) {
        zMixture [n] = Variable.Discrete( weightsMixture );

        using ( Variable.Switch( zMixture [n] ) ) {
          var z = Variable.DiscreteUniform( k );

          using ( Variable.Switch( z ) ) {
            Variable.ConstrainBetween( meansMixture [zMixture [n]], 0, Period ); 
            var componentOffset = ( meanOffset [z] * Period ).Named( "componentOffset" );
            var componentMean = ( meansMixture[zMixture[n]] + componentOffset ).Named( "componentMean" );
            var componentDistribution = Variable.GaussianFromMeanAndPrecision( componentMean, precisionsMixture[zMixture[n]] ).Named( "componentDistribution" );

            data [n] = componentDistribution;
          }
        }
      }

      using ( Variable.ForEach( kMixture ) ) 
        Variable.ConstrainBetween( meansMixture[kMixture], 0, Period ); 


      // Attach some generated data
      var observedData = GeneratePeriodicMixtureData( N );
      Console.WriteLine( "Min: {0}\nMax: {1}", observedData.Min(), observedData.Max() );

      data.ObservedValue = observedData;


      // The inference
      InferenceEngine ie = new InferenceEngine {
        Algorithm = new ExpectationPropagation(),
        NumberOfIterations = 50,
        ShowFactorGraph = false
      };

      Console.WriteLine( "Dist over alpha=\n{0}\n", ie.Infer( weightsMixture ) );
      Console.WriteLine( "Dist over means=\n{0}\n", ie.Infer( meansMixture ) );
      Console.WriteLine( "Dist over precs=\n{0}\n", ie.Infer( precisionsMixture ) );
    }




    /// <summary>
    /// Generates a data set from a particular true model.
    /// </summary>
    public static double [] GenerateData( int nData ) {
      var trueM1 = 2.0;
      var trueM2 = 7.0;

      var trueP1 = 1.0;
      var trueP2 = 1.0 / 3.0;

      var trueVG1 = Gaussian.FromMeanAndPrecision( trueM1, trueP1 );
      var trueVG2 = Gaussian.FromMeanAndPrecision( trueM2, trueP2 );

      double truePi = 0.6;
      Bernoulli trueB = new Bernoulli( truePi );

      // Restart the infer.NET random number generator
      Rand.Restart( 12347 );
      var data = new double [nData];

      for ( int j = 0; j < nData; j++ ) {
        bool bSamp = trueB.Sample();
        data [j] = bSamp ? trueVG1.Sample() : trueVG2.Sample();
      }

      return data;
    }
  }
}
