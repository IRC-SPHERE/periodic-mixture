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
    static void Main( string [] args ) {
      var source = new PeriodicMixture{ N = 100, Mean = new double [] { 1, 13 }, Variance = new double [] { 2, 2 }, Period = 24 }; 
      //var source = new PeriodicSingle{ N = 100, Mean = new [] { 0.0 }, Variance = new [] { 9.0 }, Period = 24 };

      var wm = new WrappedMixture {
        source = source, 
        approximation_count = 3, 
        mixture_count = 2, 
        period = 24 
      };

      wm.Infer(); 
      wm.Print( 50 ); 
    }
  }
}
