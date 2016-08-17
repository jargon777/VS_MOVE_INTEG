using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;

namespace VISSIM_MOVES_CONV {
    class Link {
        public int number;
        public int cluster_count;
        public Dictionary<int, InputVehicles> Vehicles;
        public Dictionary<int, VehicleTrajectory> TrajectorySnippet; //contains the trajectory that is on the link.
        public VehiclePath Link_Path;

        public Link(int number) {
            this.number = number;
            this.Vehicles = new Dictionary<int, InputVehicles>();
            this.TrajectorySnippet = new Dictionary<int, VehicleTrajectory>();
            this.Link_Path = new VehiclePath(number); //normally the "number" is the cluster number, here it's the "link number"
        }

        /// <summary>
        /// Repurposes the original clustered analysis for links. Avoids reduplication of vehicles by only saving the result.
        /// </summary>
        public void clustered_analysis(RichTextBox ConsoleBox, string empty_mode) {
            //prepare the vehicles for analysis by adding vehicles with only the trajectories on the link to the vehiclepath.
            foreach(KeyValuePair<int, VehicleTrajectory> Snippet in this.TrajectorySnippet) {
                this.Link_Path.Vehicles_On_Route.Add(new InputVehicles(Snippet.Key, this.Vehicles[Snippet.Key].vehicle_VISSIM_type, Snippet.Value));
            }
            this.Link_Path.AssignClusterGroups(this.cluster_count, ConsoleBox, empty_mode);
        }
    }
}
