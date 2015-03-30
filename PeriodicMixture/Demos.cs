using System;

namespace PeriodicMixture {
  public static class Demos {
    const double period = 24; 
    const int N = 100; 

    public static void RunDemos() {
      PeriodicSingle1(); 
      PeriodicSingle2(); 
      PeriodicSingle3(); 
      PeriodicSingle4(); 
      PeriodicMixture1(); 
      PeriodicMixture2(); 
    }



    public static PeriodicData SingleSource = new PeriodicData{ 
      N = N, 
      Mean = new [] { 
        0.0
      }, 
      Variance = new [] { 
        5.0
      }, 
      Pk = new [] {
        1.0
      },
      Period = period 
    };

    public static void PeriodicSingle1() {
      var data = SingleSource.Generate(); 

      var wm = new WrappedMixture {
        observedData = data, 
        approximation_count = 1, 
        mixture_count = 1, 
        period = period 
      };

      wm.Infer( "PeriodicSingle1.json" ); 
      wm.Print(); 

      Console.WriteLine( "With a single-component GMM, the moments are badly estimated. (if the mean was not near midnight, the approximation might be sufficient)" + "\n\n\n\n\n" );
    }

    public static void PeriodicSingle2() {
      var data = SingleSource.Generate(); 

      var wm = new WrappedMixture {
        observedData = data, 
        approximation_count = 3, 
        mixture_count = 1, 
        period = period 
      };

      wm.Infer( "PeriodicSingle2.json" ); 
      wm.Print(); 

      Console.WriteLine( "By introducing the wrapped approximation, both moments are better estimated." + "\n\n\n\n\n" );
    }

    public static void PeriodicSingle3() {
      var data = SingleSource.Generate(); 

      var wm = new WrappedMixture {
        observedData = data, 
        approximation_count = 1, 
        mixture_count = 2, 
        period = period 
      };

      wm.Infer( "PeriodicSingle3.json" ); 
      wm.Print(); 

      Console.WriteLine( "By approximating the wrapping as a mixture model (where we allow the model to place one component after and another before midnight), \nwe get better estimates, but the moments are still off because the wrapping wasn't accounted for." + "\n\n\n\n\n" );
    }

    public static void PeriodicSingle4() {
      var source = new PeriodicData{ 
        N = N, 
        Mean = new [] { 5.0 }, 
        Variance = new [] { Math.Pow( 8, 2 ) }, 
        Pk = new [] { 1.0 },
        Period = period
      };

      var data = source.Generate(); 

      var wm = new WrappedMixture {
        observedData = data, 
        approximation_count = 3, 
        mixture_count = 1, 
        period = period 
      };

      wm.Infer( "PeriodicSingle4.json" ); 
      wm.Print(50); 

      Console.WriteLine( "Test it with large variance." + "\n\n\n\n\n" );
    }



    public static PeriodicData MixtureSource = new PeriodicData{ 
      N = N, 
      Mean = new double [] { 
        0.0, 
        9.0 
      }, 
      Variance = new double [] { 
        4.0, 
        4.0 
      }, 
      Pk = new double [] {
        0.4, 
        0.6
      },
      Period = period 
    };

    public static void PeriodicMixture1() {
      var data = MixtureSource.Generate(); 

      var wm = new WrappedMixture {
        observedData = data, 
        approximation_count = 1, 
        mixture_count = 2, 
        period = period 
      };

      wm.Infer( "PeriodicMixture1.json" ); 
      wm.Print(); 

      Console.WriteLine( "The training data here are samplled from a mixture model. If we learn a non-wrapped mixture,the moments are poorly approximated.." + "\n\n\n\n\n" );
    }

    public static void PeriodicMixture2() {
      var data = MixtureSource.Generate(); 

      var wm = new WrappedMixture {
        observedData = data, 
        approximation_count = 3, 
        mixture_count = 2, 
        period = period 
      };

      wm.Infer( "PeriodicMixture2.json" ); 
      wm.Print(); 

      Console.WriteLine( "Acounting for the wrapped nature in this mixture model seems to give more optimal results." + "\n\n\n\n\n" );
    }
  }
}

