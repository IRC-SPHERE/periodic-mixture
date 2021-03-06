﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MicrosoftResearch.Infer.Models;
using MicrosoftResearch.Infer;
using MicrosoftResearch.Infer.Distributions;
using MicrosoftResearch.Infer.Maths;
using Newtonsoft.Json;
using System.IO;

namespace PeriodicMixture {
  class Program {
    const string DataPath = "../../../Data/";

    static void Main( string [] args ) {
      var data = JsonConvert.DeserializeObject<double[]>( 
        System.IO.File.ReadAllText( DataPath + "wash_lunch_dishes.json" )
      ); 

      Console.WriteLine( "{0} datapoints...", data.Count() ); 

      var wm = new MultimodalWrappedApproximation {
        period = 24,
        observedData = data, 
        approximation_count = 3, 
        mixture_count = 2, 

        algorithm = new ExpectationPropagation(),
        NumberOfIterations = 50
      };

      wm.Infer( "hh101.json" ); 
    }
  }
}
