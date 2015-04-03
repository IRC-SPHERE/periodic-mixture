using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PeriodicMixture {
  public class Meta
  {
    /// <summary>
    /// Gets or sets the activities.
    /// </summary>
    /// <value>The activities.</value>
    public List<string> Activities { get; set; }

    /// <summary>
    /// Gets or sets the sensors.
    /// </summary>
    /// <value>The sensors.</value>
    public List<string> Sensors { get; set; }

    [JsonProperty(PropertyName = "sensor_types")]
    public Dictionary<string, SensorType> SensorTypes { get; set; }

    public class SensorType
    {
      public string Type { get; set; }
    }
  }
}

