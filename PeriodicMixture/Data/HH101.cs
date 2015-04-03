using System;
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
  public class HH101 {
    public static double [] LoadData( string MetaDataFilename, string DataFilename, string outputPath ) {
      Meta meta;

      using (var reader = new StreamReader(File.OpenRead(MetaDataFilename))) {
        meta = JsonConvert.DeserializeObject<Meta>(reader.ReadToEnd());
      }

      var activities = meta.Activities.ToDictionary(ia => ia, ia => new List<DateTime>());
      var sensors = meta.Sensors.ToDictionary(ia => ia, ia => new List<DateTime>());


      using (var reader = new StreamReader(File.OpenRead(DataFilename)))
      {
        bool first = true;
        while (!reader.EndOfStream)
        {
          var line = reader.ReadLine();

          if (first)
          {
            first = false;
            continue;
          }

          var values = line.Split(',');

          // date_time,sensor,reading,activity,D001,D002,T102,T103,T101,MA016,D003,M009,LS010,M010,LS011,M007,LS008,M008,LS009,M011,LS012,LS001,LS002,M001,LS016,T105,T104,LS003,M002,LS004,M003,LS005,M004,LS006,M005,LS007,MA013,LS013,M012,MA015,LS015,MA014,LS014,M006,Enter_Home,Personal_Hygiene,Wash_Lunch_Dishes,Leave_Home,Cook_Dinner,Eat_Dinner,Cook_Lunch,Eat_Lunch,Relax,Read,Phone,Evening_Meds,Eat_Breakfast,Watch_TV,Cook,Wash_Breakfast_Dishes,Eat,Wash_Dishes,Toilet,Groom,Sleep_Out_Of_Bed,Work_At_Table,Morning_Meds,Cook_Breakfast,Bed_Toilet_Transition,Wash_Dinner_Dishes,Bathe,Entertain_Guests,Sleep,Dress
          //int index = int.Parse(values[0]);
          var dateTime = DateTime.Parse(values[0]);
          var sensor = values[1];
          var reading = values[2];
          var activity = values[3];

          // Console.WriteLine("{0,3} {1,10}, {2,10}, {3,10}, {4,10}", index, dateTime, sensor, reading, activity);

          for (int i = 0; i < meta.Sensors.Count; i++)
          {
            // only binary for now
            if (meta.SensorTypes[meta.Sensors[i].Substring(0, 1)].Type != "Binary")
            {
              continue;
            }

            bool b = bool.Parse(values[i + 5]);
            if (b)
            {
              sensors[meta.Sensors[i]].Add(dateTime);
            }
          }

          for (int i = 0; i < meta.Activities.Count; i++)
          {
            bool b = bool.Parse(values[i + 4 + meta.Sensors.Count]);
            if (b)
            {
              activities[meta.Activities[i]].Add(dateTime);
            }
          }
        }
      }

      Utils.Print( activities.Select( aa => new { name = aa.Key, count = aa.Value.Count() } ) );

      foreach ( var kv in activities ) {
        Console.WriteLine( kv.Key ); 

        var filename = string.Format(
          "{0}/{1}.json", outputPath, kv.Key.ToLower()
        ); 

        var filecontents = JsonConvert.SerializeObject( 
          kv.Value.Select( Utils.FractionOfHour ).OrderBy( ia => Guid.NewGuid() )
        );

        System.IO.File.WriteAllText( filename, filecontents ); 
      }
      
      return activities[""].Select( Utils.FractionOfHour )
        .OrderBy(ia => Guid.NewGuid()).ToArray();
    }
  }
}

