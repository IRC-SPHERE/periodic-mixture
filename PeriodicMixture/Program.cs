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
      //Demos.RunDemos(); 
      Demos.PeriodicSingle4();
    }
  }
}
