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
    class VehiclePathClusterGroups {
        public List<InputVehicles> Vehicles_In_Cluster;
        public VehicleTrajectory Average_Trajectory;
        public List<int> Sum_Of_Squares;
        public Dictionary<string, double> SourceTypeDistribution {
            get {
                return this._CalculateSourceTypeDistribution();
            }
        }

        /// <summary>
        /// Creates a cluster and populates it with a predetermined list and then calculates the average trajectory for those vehicles. Used when preparing all the vehicles for the cluster analysis.
        /// </summary>
        /// <param name="Vehicles_In_Cluster">The vehicles which will make up the cluster.</param>
        public VehiclePathClusterGroups(List<InputVehicles> Vehicles_In_Cluster) {
            this.Vehicles_In_Cluster = Vehicles_In_Cluster;
            this.Average_Trajectory = _CalculateAverageTrajectory();
        }
        /// <summary>
        /// Creates a Cluster group from one vehicle. Used when seeding the initial k-means clusters.
        /// </summary>
        /// <param name="initial_vehicle">The initial vehicle for this k-means cluster.</param>
        public VehiclePathClusterGroups(InputVehicles initial_vehicle) {
            this.Vehicles_In_Cluster = new List<InputVehicles>();
            //add initial vehicle and assign it as the average trajectory.
            this.Vehicles_In_Cluster.Add(initial_vehicle);
            this.Average_Trajectory = initial_vehicle.Trajectory;
        }
        public void RecalculateAverageTrajectory() {
            if (this.Vehicles_In_Cluster.Count != 0) this.Average_Trajectory = _CalculateAverageTrajectory(); //avoids dropping the cluster.
        }
        private Dictionary<string, double> _CalculateSourceTypeDistribution() {
            Dictionary<string, double> SourceTypeDist = new Dictionary<string, double>();
            List<string> keys; //for iterating

            foreach (InputVehicles veh in this.Vehicles_In_Cluster) {
                if (SourceTypeDist.ContainsKey(veh.vehicle_type)) {
                    SourceTypeDist[veh.vehicle_type]++; //increment the total count for that element.
                }
                else SourceTypeDist.Add(veh.vehicle_type, 1); //add the type.
            }
            keys = new List<string>(SourceTypeDist.Keys);
            foreach (string key in keys) {
                SourceTypeDist[key] /= ((double)this.Vehicles_In_Cluster.Count); //convert the counts to proportions
            }

            return SourceTypeDist;
        }
        public void CalculateAllSumSquares() {
            this.Sum_Of_Squares = new List<int>(); //re-initialize the list.
            for (int i = 0; i < this.Vehicles_In_Cluster.Count; i++) {
                Sum_Of_Squares.Add(this.CalculateSumSquare(this.Vehicles_In_Cluster[i].Trajectory));
            }
        }
        public int CalculateSumSquare(VehicleTrajectory Trajectory) {
            double sum_square = 0;
            for (int i = 0; i < this.Average_Trajectory.speed.Count; i++) {
                //if a speed exists use it to calculate the sum of square. Else treat it as a zero entry.
                if (i < Trajectory.speed.Count) sum_square += Math.Pow((this.Average_Trajectory.speed[i] - Trajectory.speed[i]), 2);
                else sum_square += Math.Pow((this.Average_Trajectory.speed[i]), 2);
            }

            return (int)sum_square;
        }
        private int _CalculateSumSquareDEPRECATED(VehicleTrajectory Trajectory) {
            double sum_square = 0;
            int i_traj = 0;
            int div_break = this.Average_Trajectory.speed.Count;
            int num_entries_to_double = this.Average_Trajectory.speed.Count - Trajectory.speed.Count;
            int extra_entries_added = 0;
            if (Trajectory.speed.Count != this.Average_Trajectory.speed.Count) { //allow the modulo to be zero when we need to insert numbers.
                div_break = num_entries_to_double;
                div_break = Trajectory.speed.Count / div_break;
            }
            for (int i = 0; i < Trajectory.speed.Count; i++) {
                if (i % div_break == 0 && extra_entries_added < num_entries_to_double) {
                    extra_entries_added++;
                    //since path is shorter, increment the i_traj and allow a double entry.
                    sum_square += Math.Pow((this.Average_Trajectory.speed[i_traj] - Trajectory.speed[i]), 2);
                    i_traj++;
                }
                sum_square += Math.Pow((this.Average_Trajectory.speed[i_traj] - Trajectory.speed[i]), 2);
                i_traj++;
            }
            for (; i_traj < this.Average_Trajectory.speed.Count; i_traj++) { //fill up the remaining spots with the last entry.
                sum_square += Math.Pow((this.Average_Trajectory.speed[i_traj] - Trajectory.speed.Last()), 2);
            }

            return (int)sum_square;
        }
        private VehicleTrajectory _CalculateAverageTrajectory() {
            /* Trajectories may differ by a few seconds. To do the k-means clustering, we will normalise the trajectories to the longest one.
             * This longest trajectory will form the basis for the error calculations. 
             * Modulo is used to double-count certain entries for "short" paths at even intervals. */

            VehicleTrajectory average_trajectory = new VehicleTrajectory();
            int longest_traj = 0;

            foreach (InputVehicles veh in this.Vehicles_In_Cluster) {
                if (veh.Trajectory.second_stamp.Count > longest_traj) longest_traj = veh.Trajectory.second_stamp.Count;
            }

            //initialize average trajectory.
            for (int i = 0; i < longest_traj; i++) {
                average_trajectory.speed.Add(0);
                average_trajectory.second_stamp.Add(i);
                average_trajectory.vehicle_count.Add(0);
            }
            foreach (InputVehicles veh in this.Vehicles_In_Cluster) {
                for (int i = 0; i < veh.Trajectory.second_stamp.Count; i++) {
                    average_trajectory.speed[i] += veh.Trajectory.speed[i]; //sum the average speeds.
                    average_trajectory.vehicle_count[i]++; //keep track of the number of vehicles for a more accurate average.
                }
            }
            for (int i = 0; i < longest_traj; i++) { //convert the summed speeds to averages giving equal weight to all vehicles.
                //THIS method, or a varient, may be used in the future...
                //if (average_trajectory.vehicle_count[i] != 0) average_trajectory.speed[i] /= average_trajectory.vehicle_count[i];
                //else average_trajectory.speed[i] /= this.Vehicles_In_Cluster.Count;
                average_trajectory.speed[i] /= this.Vehicles_In_Cluster.Count;
                
            }
            return average_trajectory;
        }
    }
    class VehiclePath {
        public int num;
        public List<int> Link_Progression;
        //public List<int> Route_Progression; //unused
        //public List<double> Distance_To_Link;
        //public double Average_Distance;
        public List<VehiclePathClusterGroups> Path_Cluster;
        public List<InputVehicles> Vehicles_On_Route;
        public List<VehicleTrajectory> InitialTrajectories;
        private int local_kmeans_runs = 399; //indicates the maximum number of times averages are recalculated.
        private double local_stopping_cond = 0.15; //ratio modifier (+/-) indicating the maximum sum of square difference between successive iterations before halting is met.
        private double _mph_conversion_factor = 2.23694;
        private double _mile_conversion_factor = 0.000621371;
        /// <summary>
        /// Creates a new instance of VehiclePath.
        /// </summary>
        /// <param name="Link_Progression">A list of the links the vehicles traverse on.</param>
        /// <param name="Vehicle">A seed vehicle.</param>
        /// <param name="num">A unique identifier for the path.</param>
        public VehiclePath(List<int> Link_Progression, InputVehicles Vehicle, int num) {
            this.Link_Progression = Link_Progression;
            this.Vehicles_On_Route = new List<InputVehicles>();
            this.InitialTrajectories = new List<VehicleTrajectory>();
            this.Vehicles_On_Route.Add(Vehicle);
            this.num = num;
        }
        public VehiclePath(int num) {
            this.Vehicles_On_Route = new List<InputVehicles>();
            this.InitialTrajectories = new List<VehicleTrajectory>();
            this.num = num;
        }

        /// <summary>
        /// Assigns the cluster groups. Run after all vehicles loaded in paths.
        /// </summary>
        /// <param name="cluster_count">The number of k-means clusters to create.</param>
        public void AssignClusterGroups(int cluster_count, RichTextBox ConsoleBox, string empty_mode) {
            Random randomizer = new Random();
            ConsoleBox.AppendText("\n   Initializing Cluster Groups..."); ConsoleBox.Refresh();
            this._InitialiseClusterGroups(cluster_count);
            ConsoleBox.AppendText("\n   Beginning Iterations..."); ConsoleBox.Refresh();
            int prvSS = 0;
            int curSS = 0;
            int iterator = 0;
            int maximum_empty = 0; //the minimum empty represents the most detailed clustering we could achieve if we reach the max iterations and need to reduce the count.
            for (iterator = 0; iterator < this.local_kmeans_runs; iterator++) {
                curSS = 0;
                double ratio;

                this._DestributeVehicles();
                for (int ii = 0; ii < this.Path_Cluster.Count; ii++) {
                    if (iterator == 0) this.InitialTrajectories.Add(this.Path_Cluster[ii].Average_Trajectory); //save the initial trajectory.
                    VehicleTrajectory old_avg = this.Path_Cluster[ii].Average_Trajectory;
                    this.Path_Cluster[ii].RecalculateAverageTrajectory();
                    curSS += this.Path_Cluster[ii].CalculateSumSquare(old_avg);
                }

                //check the stopping condition. Check it as a +/- bounds to the stopping cond.
                if (prvSS == 0 && curSS == 0) ratio = 1; //since this represents no change in the SS, consider it as unity.
                else ratio = ((double)curSS) / prvSS;
                if (ratio <= 1 + local_stopping_cond && ratio >= 1 - local_stopping_cond) {
                    if (this._CheckClusterForEmpty(ref maximum_empty)) break;
                }
                else if (iterator % (35) == 0 && iterator != 0) { //jostle the clusters a little.
                    //select a trajectory at random and average it with the current and initial seeds of the individual clusters to shift the seed conditions to improve the chance of convergence.
                    for (int ii = 0; ii < this.Path_Cluster.Count; ii++) {
                        int random = randomizer.Next(this.Vehicles_On_Route.Count);
                        InputVehicles rand = this.Vehicles_On_Route[random];
                        InputVehicles o_traj = new InputVehicles(0, 0, 0, 0, 0, 0, 0, "DUMMY", new Dictionary<string, string>()); //add dummy vehicles to allow averaging.
                        InputVehicles c_traj = new InputVehicles(0, 0, 0, 0, 0, 0, 0, "DUMMY", new Dictionary<string, string>());
                        o_traj.Trajectory = this.InitialTrajectories[ii];
                        c_traj.Trajectory = this.Path_Cluster[ii].Average_Trajectory;
                        this.Path_Cluster[ii].Vehicles_In_Cluster = new List<InputVehicles>(); //erase the container of vehicles.
                        this.Path_Cluster[ii].Vehicles_In_Cluster.Add(rand);
                        this.Path_Cluster[ii].Vehicles_In_Cluster.Add(o_traj);
                        this.Path_Cluster[ii].Vehicles_In_Cluster.Add(c_traj);
                        this.Path_Cluster[ii].RecalculateAverageTrajectory(); //recalculate the average with these three trajectories.
                    }
                }
                else {
                    this._CheckClusterForEmpty(ref maximum_empty); //check for empty clusters and correct it.
                }
                prvSS = curSS;
            }
            if (iterator == this.local_kmeans_runs && empty_mode == "RW") { //deals with empty clusters after the maximum has been reached.
                ConsoleBox.AppendText("  Maximum iterations reached, removing zero clusters and re-balancing"); ConsoleBox.Refresh();
                cluster_count -= maximum_empty;
                AssignClusterGroups(cluster_count, ConsoleBox, empty_mode); //recursively call to rebalance.
            }
            else {
                ConsoleBox.AppendText(" OK. Stopping after " + iterator + " iterations."); ConsoleBox.Refresh();
            }
            return;
        }
        /// <summary>
        /// check for empty clusters and correct this by assigning it a mean from the largest cluster
        /// </summary>
        /// <returns></returns>
        private bool _CheckClusterForEmpty(ref int maximum_empty) {
            List<int> emptyClusters = new List<int>();
            int largest_cluster = 0;
            int largest_cluster_size = 0;
            for (int i = 0; i < this.Path_Cluster.Count; i++) {
                if (this.Path_Cluster[i].Vehicles_In_Cluster.Count == 0) emptyClusters.Add(i);
                else if (this.Path_Cluster[i].Vehicles_In_Cluster.Count > largest_cluster_size) {
                    largest_cluster_size = this.Path_Cluster[i].Vehicles_In_Cluster.Count;
                    largest_cluster = i;
                }
            }

            if (emptyClusters.Count == 0) return true;
            else {
                if (emptyClusters.Count > maximum_empty) maximum_empty = emptyClusters.Count /2 + 1; //retains some clusters in the hopes of a better balance.
                List<int> Sorted_Sum_Of_Squares;
                List<int> Sorted_Sum_Pull_Indices;
                int cluster_interval;
                this.Path_Cluster[largest_cluster].CalculateAllSumSquares(); //recalculate sum of squares for largest cluster
                //perform a sort of initilization.
                Sorted_Sum_Of_Squares = this.Path_Cluster[largest_cluster].Sum_Of_Squares;
                Sorted_Sum_Of_Squares.Sort(); //sort them.

                //the idea is identical to the seeding process, redistribute the largest cluster into smaller clusters to use up empty clusters.
                Sorted_Sum_Pull_Indices = new List<int>();
                cluster_interval = Sorted_Sum_Of_Squares.Count / emptyClusters.Count;
                if (cluster_interval == 0) {
                    cluster_interval = 1;
                    while (Sorted_Sum_Of_Squares.Count < emptyClusters.Count) {
                        emptyClusters.RemoveAt(0);
                    }
                }//can happen if there are too many clusters or if mpt empigh volume. Basically all we do is just assign some of the emptys for now.

                /****************** SEEDING ***********************************/
                //iterate over the first half the list, if there are more than one clusters, using the modulus to save the elements.
                if (emptyClusters.Count == 1) {
                    Sorted_Sum_Pull_Indices.Add(Sorted_Sum_Of_Squares[0]);  //doesn't really matter which one is seeded, if there's only one then it'll be all of them.
                }
                else {
                    for (int i = 0; i < Sorted_Sum_Of_Squares.Count / 2; i++) {
                        if (i % cluster_interval == 0) Sorted_Sum_Pull_Indices.Add(Sorted_Sum_Of_Squares[i]);
                        if (Sorted_Sum_Pull_Indices.Count >= emptyClusters.Count / 2) break; //should only be half of them.
                    }
                    for (int i = 0; ; i++) {
                        if (i % cluster_interval == 0) Sorted_Sum_Pull_Indices.Add(Sorted_Sum_Of_Squares[Sorted_Sum_Of_Squares.Count - 1 - i]);
                        if (Sorted_Sum_Pull_Indices.Count >= emptyClusters.Count) break;
                    }
                }

                //assign the paths and prepare the returnable list of unassigned vehicles.
                for (int i = 0; i < Sorted_Sum_Pull_Indices.Count; ) {
                    for (int ii = 0; ii < this.Path_Cluster[largest_cluster].Sum_Of_Squares.Count; ii++) {
                        if (Sorted_Sum_Pull_Indices[i] == this.Path_Cluster[largest_cluster].Sum_Of_Squares[ii]) {
                            this.Path_Cluster[emptyClusters[i]].Average_Trajectory = this.Path_Cluster[largest_cluster].Vehicles_In_Cluster[ii].Trajectory; //assign the trajectory to be the new average for this cluster.
                            this.Path_Cluster[largest_cluster].Sum_Of_Squares[ii] = -1; //mark the sum for later.
                            i++; //increment the outside iterator.
                            break; //break the inside loop to check the outside condition.
                        }
                    }
                }
                return false;
            }
        }
        private void _ReInitialiseClusterGroups() {
            //clear the old vehicles assigned to list.
            for (int i = 0; i < Path_Cluster.Count; i++) {
                Path_Cluster[i].Vehicles_In_Cluster = new List<InputVehicles>();
            }
        }
        private void _DestributeVehicles() {
            this._ReInitialiseClusterGroups(); //clear the list of vehicles first.
            foreach (InputVehicles veh in this.Vehicles_On_Route) {
                int lowest_sum_square_index = 0;
                int lowest_sum_square = Int32.MaxValue; //initialize value to largest possible.
                for (int i = 0; i < this.Path_Cluster.Count; i++) {
                    int sum_square = this.Path_Cluster[i].CalculateSumSquare(veh.Trajectory);
                    if (sum_square < lowest_sum_square) {
                        lowest_sum_square_index = i;
                        lowest_sum_square = sum_square;
                    }
                }
                this.Path_Cluster[lowest_sum_square_index].Vehicles_In_Cluster.Add(veh); //add the vehicle to best cluster identified.
            }
        }

        /// <summary>
        /// Initialises the cluster group using the class's Vehicles_On_Route. One vehicle is assigned to each cluster. Vehicles are selected to evenly represent differences in the sum of squares from a zero reference condition (maximum difference from each other).
        /// </summary>
        /// <param name="cluster_count">The number of k-means clusters to initialise.</param>
        /// <returns>A list of vehicles which have not been assigned a cluster.</returns>
        private void _InitialiseClusterGroups(int cluster_count) {
            this.Path_Cluster = new List<VehiclePathClusterGroups>(); //init the list.
            //List<InputVehicles> Unassigned_Vehicles = new List<InputVehicles>();
            List<int> Sorted_Sum_Of_Squares;
            List<int> Sorted_Sum_Pull_Indices;
            VehiclePathClusterGroups VehicleSample;
            int cluster_interval = 0;

            //calculate average trajectory for all vehicles.
            VehicleSample = new VehiclePathClusterGroups(this.Vehicles_On_Route);

            //find the sum of squares for all entities.
            VehicleSample.CalculateAllSumSquares();

            //assign those sum of squares and sort them.
            Sorted_Sum_Of_Squares = VehicleSample.Sum_Of_Squares;
            Sorted_Sum_Of_Squares.Sort(); //sort them.

            //the idea is we now seed the initial paths based on the raw difference in sum of squares from the overall average.
            Sorted_Sum_Pull_Indices = new List<int>();
            cluster_interval = Sorted_Sum_Of_Squares.Count / cluster_count;
            if (cluster_interval == 0) Console.WriteLine("WHOA");

            /****************** SEEDING ***********************************/
            //iterate over the first half the list, if there are more than one clusters, using the modulus to save the elements.
            if (cluster_count == 1) {
                Sorted_Sum_Pull_Indices.Add(Sorted_Sum_Of_Squares[0]);  //doesn't really matter which one is seeded, if there's only one then it'll be all of them.
            }
            else {
                for (int i = 0; i < Sorted_Sum_Of_Squares.Count / 2; i++) {
                    if (i % cluster_interval == 0) Sorted_Sum_Pull_Indices.Add(Sorted_Sum_Of_Squares[i]);
                    if (Sorted_Sum_Pull_Indices.Count >= cluster_count / 2) break; //should only be half of them.
                }
                for (int i = 0; ; i++) {
                    if (i % cluster_interval == 0) Sorted_Sum_Pull_Indices.Add(Sorted_Sum_Of_Squares[Sorted_Sum_Of_Squares.Count - 1 - i]);
                    if (Sorted_Sum_Pull_Indices.Count >= cluster_count) break;
                }
            }

            //assign the paths and prepare the returnable list of unassigned vehicles.
            for (int i = 0; i < Sorted_Sum_Pull_Indices.Count; ) {
                for (int ii = 0; ii < VehicleSample.Sum_Of_Squares.Count; ii++) {
                    if (Sorted_Sum_Pull_Indices[i] == VehicleSample.Sum_Of_Squares[ii]) {
                        this.Path_Cluster.Add(new VehiclePathClusterGroups(VehicleSample.Vehicles_In_Cluster[ii]));
                        VehicleSample.Sum_Of_Squares[ii] = -1; //mark the sum for later.
                        i++; //increment the outside iterator.
                        break; //break the inside loop to check the outside condition.
                    }
                }
            }

            return;
        }
        /// <summary>
        /// Creates the files that will be exported, overwriting any if they exist.
        /// </summary>
        /// <param name="sched_loc">Path for ordinary drive schedule.</param>
        /// <param name="link_loc">Path for ordinary link list.</param>
        /// <param name="source_loc">Path for ordinary source type distribution.</param>
        /// <param name="cluster_sched_loc">Path for clustered drive schedule.</param>
        /// <param name="cluster_link_loc">Path for clustered link list.</param>
        /// <param name="cluster_source_loc">Path for clustered source type distribution.</param>
        /// <param name="cluster_stats_loc">Path for clustered statistical information.</param>
        /// <param name="ConsoleBox">Area to update with progress.</param>
        public void Initialize_IndividualExport(
                string sched_loc,
                string link_loc,
                string source_loc,
                string cluster_sched_loc,
                string cluster_link_loc,
                string cluster_source_loc,
                string cluster_stats_loc,
                string cluster_mode,
                RichTextBox ConsoleBox) {
            Console.Write("\n  Checking if output files Exists and Writing Headers... ");
            ConsoleBox.AppendText("\n  Checking if output files Exists and Writing Headers... "); ConsoleBox.Refresh();
            //Normal Data
            using (System.IO.StreamWriter matrixSW = new System.IO.StreamWriter(sched_loc, false)) {
                matrixSW.WriteLine("linkID,secondID,speed,grade");
            }
            using (System.IO.StreamWriter matrixSW = new System.IO.StreamWriter(link_loc, false)) {
                matrixSW.WriteLine("linkID,countyID,zoneID,roadTypeID,linkLength,linkVolume,linkAvgSpeed,linkDescription,linkAvgGrade");
            }
            using (System.IO.StreamWriter matrixSW = new System.IO.StreamWriter(source_loc, false)) {
                matrixSW.WriteLine("linkID,sourceTypeID,sourceTypeHourFraction");
            }
            if (cluster_mode != "NO")
            { //prepare cluster data if requested.
              //Clustered Data
                using (System.IO.StreamWriter matrixSW = new System.IO.StreamWriter(cluster_sched_loc, false))
                {
                    matrixSW.WriteLine("linkID,secondID,speed,grade");
                }
                using (System.IO.StreamWriter matrixSW = new System.IO.StreamWriter(cluster_link_loc, false))
                {
                    matrixSW.WriteLine("linkID,countyID,zoneID,roadTypeID,linkLength,linkVolume,linkAvgSpeed,linkDescription,linkAvgGrade");
                }
                using (System.IO.StreamWriter matrixSW = new System.IO.StreamWriter(cluster_source_loc, false))
                {
                    matrixSW.WriteLine("linkID,sourceTypeID,sourceTypeHourFraction");
                }
                using (System.IO.StreamWriter matrixSW = new System.IO.StreamWriter(cluster_stats_loc, false))
                {
                    matrixSW.WriteLine("STATISTIC, value, note");
                }
            }
            Console.WriteLine("OK.");
            ConsoleBox.AppendText("OK."); ConsoleBox.Refresh();
        }

        public void Cluster_Export(string cluster_sched_loc, string cluster_link_loc, string cluster_source_loc, string cluster_stats_loc, string countyID, string zoneID, string roadType, RichTextBox ConsoleBox) {
            Console.Write("\n  Exporting Schedule and Link for Cluster... ");
            ConsoleBox.AppendText("\n  Exporting Schedule and Link for Cluster... "); ConsoleBox.Refresh();
            for (int i = 0; i < this.Path_Cluster.Count; i++) {
                this.Path_Cluster[i].Average_Trajectory.calculate_distance_travelled(); //update the distance travelled before export.
                string output_line;

                //write the average trajectory.
                using (System.IO.StreamWriter matrixSW = new System.IO.StreamWriter(cluster_sched_loc, true)) {
                    for (int ii = 0; ii < this.Path_Cluster[i].Average_Trajectory.second_stamp.Count; ii++) {
                        output_line = String.Format("{0},{1},{2},{3}",
                            (i << 16) + this.num + 1,
                            this.Path_Cluster[i].Average_Trajectory.second_stamp[ii] - this.Path_Cluster[i].Average_Trajectory.second_stamp[0],
                            this.Path_Cluster[i].Average_Trajectory.speed[ii] * this._mph_conversion_factor,
                            0); //convert to mph. Align second stamps to start at zero. Grade is set to zero. Link number is i << 16 + num + 1 (to avoid zero and mask for link numbers)

                        matrixSW.WriteLine(output_line);
                    }
                }

                //export the summary of volume on the path.
                this.Path_Cluster[i].Average_Trajectory.calculate_distance_travelled();
                output_line = String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                    (i << 16) + this.num + 1, countyID, zoneID, roadType, this.Path_Cluster[i].Average_Trajectory.distance_travelled * this._mile_conversion_factor,
                    this.Path_Cluster[i].Vehicles_In_Cluster.Count, this.Path_Cluster[i].Average_Trajectory.get_average_speed() * this._mph_conversion_factor, "vehicle_clust_link", 0);
                using (System.IO.StreamWriter matrixSW = new System.IO.StreamWriter(cluster_link_loc, true)) {
                    matrixSW.WriteLine(output_line);
                }

                //export the proportion of each type of vehicle.
                using (System.IO.StreamWriter matrixSW = new System.IO.StreamWriter(cluster_source_loc, true)) {
                    Dictionary<string, double> SourceTypeDist = this.Path_Cluster[i].SourceTypeDistribution;
                    foreach (KeyValuePair<string, double> entry in SourceTypeDist) {
                        output_line = String.Format("{0},{1},{2}",
                            (i << 16) + this.num + 1, entry.Key, entry.Value);
                        matrixSW.WriteLine(output_line);
                    }
                }

                /* Temporary method to make a cluster vehicles graph.
                using (System.IO.StreamWriter matrixSW = new System.IO.StreamWriter(cluster_source_loc + "test", true)) {
                    Dictionary<string, double> SourceTypeDist = this.Path_Cluster[i].SourceTypeDistribution;
                    output_line = String.Format("{0}", (i << 16) + this.num + 1);
                    foreach (InputVehicles Vehicle in this.Path_Cluster[i].Vehicles_In_Cluster) {
                        string addendum = String.Format(",{0}", Vehicle.vehicle_number);
                        output_line = String.Concat(output_line, addendum);
                    }
                    matrixSW.WriteLine(output_line);
                }*/
            }
            ConsoleBox.AppendText("OK."); ConsoleBox.Refresh();
        }
        public void Individual_Export(string sched_loc, string link_loc, string source_loc, string countyID, string zoneID, string roadType, RichTextBox ConsoleBox) {
            Console.Write("\n  Exporting Schedule and Link for Path... ");
            ConsoleBox.AppendText("\n  Exporting Schedule and Link for Path... "); ConsoleBox.Refresh();
            foreach (InputVehicles Vehicle in this.Vehicles_On_Route) {
                string output_line;
                using (System.IO.StreamWriter matrixSW = new System.IO.StreamWriter(sched_loc, true)) {
                    for (int i = 0; i < Vehicle.Trajectory.second_stamp.Count; i++) {
                        output_line = String.Format("{0},{1},{2},{3}",
                            Vehicle.vehicle_number, Vehicle.Trajectory.second_stamp[i] - Vehicle.Trajectory.second_stamp[0], Vehicle.Trajectory.speed[i] * this._mph_conversion_factor, 0); //convert to mph. Align second stamps to start at zero.

                        matrixSW.WriteLine(output_line);
                    }
                }
                output_line = String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                    Vehicle.vehicle_number, countyID, zoneID, roadType, Vehicle.Trajectory.distance_travelled * this._mile_conversion_factor, 1, Vehicle.Trajectory.get_average_speed() * this._mph_conversion_factor, "vehicle_gen_link", 0);
                using (System.IO.StreamWriter matrixSW = new System.IO.StreamWriter(link_loc, true)) {
                    matrixSW.WriteLine(output_line);
                }
                output_line = String.Format("{0},{1},{2}",
                    Vehicle.vehicle_number, Vehicle.vehicle_type, 1);
                using (System.IO.StreamWriter matrixSW = new System.IO.StreamWriter(source_loc, true)) {
                    matrixSW.WriteLine(output_line);
                }
            }
            ConsoleBox.AppendText("OK."); ConsoleBox.Refresh();
        }
    }
}
