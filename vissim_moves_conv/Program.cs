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
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
    class ProgramDelegator {
        private List<VehiclePath> _Paths;
        private Dictionary<int, Link> _Links;
        
        //INPUTS
        private string _in_veh = "";

        //OUTPUTS
        private string _out_sched = "";
        private string _out_links = "";
        private string _out_source_type = "";
        private string _out_cluster_sched = "";
        private string _out_cluster_links = "";
        private string _out_cluster_source_type = "";
        private string _out_cluster_stats = "";
        private int _time_offset = 0;
        private string _countyID = "36029";
        private string _zoneID = "360290";
        private string _roadType = "5";
        private int _kmeans_clusters;
        private int _vehicle_count;
        private string _cluster_mode;
        private string _empty_mode;

        public ProgramDelegator(string in_file, string out_folder, int time_offset, string countyID,
            string zoneID, string roadType, string projectTitle, int kmeans_clusters, string cluster_mode, string empty_mode) {
            this._in_veh = in_file;
            this._out_sched = out_folder + "\\" + projectTitle + "raw_drivesched.csv";
            this._out_links = out_folder + "\\" + projectTitle + "raw_linksched.csv";
            this._out_source_type = out_folder + "\\" + projectTitle + "raw_linksources.csv";
            this._out_cluster_sched = out_folder + "\\" + projectTitle + "clustered_drivesched.csv";
            this._out_cluster_links = out_folder + "\\" + projectTitle + "clustered_linksched.csv";
            this._out_cluster_source_type = out_folder + "\\" + projectTitle + "clustered_linksources.csv";
            this._out_cluster_stats = out_folder + "\\" + projectTitle + "clustered_stats.csv";
            this._countyID = countyID;
            this._zoneID = zoneID;
            this._roadType = roadType;
            this._time_offset = 0 - time_offset; //turn offset into negative.
            this._kmeans_clusters = kmeans_clusters;
            this._vehicle_count = 0;
            this._cluster_mode = cluster_mode;
            this._empty_mode = empty_mode;
        }

        public void Run(RichTextBox ConsoleBox) {
            try {
                Console.WriteLine("VISSIM to MOVES Trajectory Sanitizer...");
                Console.WriteLine("Beginning Import.");
                Console.WriteLine("Importing Vehicles and Their Routes");

                ConsoleBox.Text = "VISSIM to MOVES Trajectory Sanitizer...\nBeginning Import.\n\nImporting Vehicles and Their Routes... "; ConsoleBox.Refresh();

                Tuple<List<VehiclePath>, Dictionary<int, Link>> Result = this._parse_vehicles(this._in_veh, this._time_offset, ConsoleBox, ref this._vehicle_count);
                this._Paths = Result.Item1;
                this._Links = Result.Item2;

                if (this._Paths.Count < 1) {
                    Console.WriteLine("FAILED! NO VEHICLES!");
                    ConsoleBox.AppendText("\nFAILED! NO VEHICLES!"); ConsoleBox.Refresh();
                    return;
                }

                Console.WriteLine("Done Vehicle Import.");

                ConsoleBox.AppendText("\n\nPurging Incomplete Paths... "); ConsoleBox.Refresh();

                this.PurgePartialTrajectories(ConsoleBox);

                if (this._cluster_mode != "NO")
                {
                    ConsoleBox.AppendText("Done!\n\nRunning Trajectory k-Means Clusterer... "); ConsoleBox.Refresh();

                    if (this._cluster_mode == "OD") this._OD_clustering(ConsoleBox);
                    else if (this._cluster_mode == "LN") this._LN_clustering(ConsoleBox);
                }
                ConsoleBox.AppendText("Done!\n\nBeginning Export of Raw Data... "); ConsoleBox.Refresh();
                Console.WriteLine("Beginning Export of Raw Data");

                this._Paths[0].Initialize_IndividualExport(
                    this._out_sched, this._out_links, this._out_source_type, 
                    this._out_cluster_sched, this._out_cluster_links, this._out_cluster_source_type, this._out_cluster_stats, this._cluster_mode, ConsoleBox);
                foreach (VehiclePath Path in this._Paths) {
                    Path.Individual_Export(this._out_sched, this._out_links, this._out_source_type, this._countyID, this._zoneID, this._roadType, ConsoleBox);
                }

                if (this._cluster_mode != "NO")
                {
                    ConsoleBox.AppendText("Done!\n\nBeginning Export of Clustered Data... "); ConsoleBox.Refresh();
                    Console.WriteLine("Beginning Export of Clustered Data");

                    if (this._cluster_mode == "OD")
                    {
                        foreach (VehiclePath Path in this._Paths)
                        {
                            Path.Cluster_Export(this._out_cluster_sched, this._out_cluster_links, this._out_cluster_source_type, this._out_cluster_stats, this._countyID, this._zoneID, this._roadType, ConsoleBox);
                        }
                    }
                    else if (this._cluster_mode == "LN")
                    {
                        foreach (KeyValuePair<int, Link> link in this._Links)
                        {
                            link.Value.Link_Path.Cluster_Export(this._out_cluster_sched, this._out_cluster_links, this._out_cluster_source_type, this._out_cluster_stats, this._countyID, this._zoneID, this._roadType, ConsoleBox);
                        }
                    }
                    //export statistical information
                    ConsoleBox.AppendText("Done.\nExporting Statistical Data... "); ConsoleBox.Refresh();
                    if (this._cluster_mode == "OD")
                    {
                        using (System.IO.StreamWriter matrixSW = new System.IO.StreamWriter(this._out_cluster_stats, true))
                        {
                            string output_line = String.Format("ftest,{0}",
                                this._ODfTestCalc());
                            matrixSW.WriteLine(output_line);
                            output_line = String.Format("total SS,{0}",
                                this._totalsumsquare());
                            matrixSW.WriteLine(output_line);
                        }
                    }
                    Console.WriteLine("Done.");
                }

                ConsoleBox.AppendText("\nDone!"); ConsoleBox.Refresh();
            }
            catch (Exception e) {
                ConsoleBox.AppendText("\n\nEXCEPTION OCCURED, \n  Source: " + e.StackTrace + "\n  Message: " + e.Message); ConsoleBox.Refresh();
                throw;
            }
        }
        private void _LN_clustering(RichTextBox ConsoleBox) {
            this._k_meansLNClusterFactorGenerator(this._Links, this._kmeans_clusters, ConsoleBox);
            foreach (KeyValuePair<int, Link> link in this._Links) {
                link.Value.clustered_analysis(ConsoleBox, this._empty_mode);
            }
        }
        private void _OD_clustering(RichTextBox ConsoleBox) {
            List<int> kmeans_factor_list;
            //generate a list holding the number of clusters to be assigned to each path.
            kmeans_factor_list = this._k_meansODClusterFactorGenerator(this._Paths, this._kmeans_clusters, ConsoleBox);

            //assign the list.
            for (int i = 0; i < kmeans_factor_list.Count; i++) {
                this._Paths[i].AssignClusterGroups(kmeans_factor_list[i], ConsoleBox, this._empty_mode);
            }

            /*
            int test = 0;
            for (int i = 0; i < this._Paths.Count; i++) {
                test += this._Paths[i].Path_Cluster.Count;
            }*/
        }
        private int _totalsumsquare() {
            int sumsquare = 0;
            foreach (VehiclePath path in this._Paths) {
                foreach (VehiclePathClusterGroups cluster in path.Path_Cluster) {
                    cluster.CalculateAllSumSquares();
                    sumsquare += cluster.Sum_Of_Squares.Sum();
                }
            }
            return sumsquare;
        }
        /// <summary>
        /// Estimates the result of an f-test for all the clustered data.
        /// </summary>
        /// <returns>F-test value</returns>
        private double _ODfTestCalc() {
            VehicleTrajectory SampleMean = _ODCalculateSampleMean();
            double explained_variance = 0;
            double unexplained_variance = 0;

            foreach (VehiclePath path in this._Paths) {
                foreach (VehiclePathClusterGroups cluster in path.Path_Cluster) {
                    fTestCalc(SampleMean, path, ref explained_variance, ref unexplained_variance);
                }
            }

            if (unexplained_variance == 0) {
                return 0;
            }
            else return explained_variance/unexplained_variance;
        }
        private VehicleTrajectory _ODCalculateSampleMean() {
            /* This Method is similar to CalculateAverageTrajectory */

            VehicleTrajectory sample_average_trajectory = new VehicleTrajectory();
            int longest_traj = 0;

            //determine what size the average trajectory should be.
            foreach (VehiclePath path in this._Paths) {
                foreach (VehiclePathClusterGroups cluster in path.Path_Cluster) {
                    if (cluster.Average_Trajectory.speed.Count > longest_traj) longest_traj = cluster.Average_Trajectory.speed.Count;
                }
            }

            //initialize average trajectory.
            for (int i = 0; i < longest_traj; i++) {
                sample_average_trajectory.speed.Add(0);
                sample_average_trajectory.second_stamp.Add(i);
            }
            foreach (VehiclePath path in this._Paths) {
                foreach (VehiclePathClusterGroups cluster in path.Path_Cluster) {
                    for (int i = 0; i < cluster.Average_Trajectory.second_stamp.Count; i++) {
                        sample_average_trajectory.speed[i] += cluster.Average_Trajectory.speed[i]; //sum the average speeds.
                    }
                }
            }
            for (int i = 0; i < longest_traj; i++) { //convert the summed speeds to averages giving equal weight to all clusters.
                sample_average_trajectory.speed[i] /= this._kmeans_clusters;
            }
            return sample_average_trajectory;
        }

        /// <summary>
        /// Intermediate calculation for the F-Test.
        /// </summary>
        /// <param name="SampleMean">The Average Trajectory</param>
        /// <param name="path">The Path Group of the Trajectory</param>
        /// <param name="explained_variance">A reference to the Explained Variance Value</param>
        /// <param name="unexplained_variance">A reference to the Unexplained Variance Value</param>
        private void fTestCalc(VehicleTrajectory SampleMean, VehiclePath path, ref double explained_variance, ref double unexplained_variance) {
            foreach (VehiclePathClusterGroups cluster in path.Path_Cluster) {
                double deviation_explained = 0;
                double deviation_unexplained = 0;
                for (int i = 0; i < SampleMean.speed.Count; i++) {

                    if (cluster.Average_Trajectory.speed.Count > i) deviation_explained += Math.Pow(cluster.Average_Trajectory.speed[i] - SampleMean.speed[i], 2);
                    else deviation_explained += Math.Pow(SampleMean.speed[i], 2);
                    if (i == 114) continue;
                }

                foreach (InputVehicles veh in path.Vehicles_On_Route) {
                    for (int i = 0; i < cluster.Average_Trajectory.speed.Count; i++) {
                        if (veh.Trajectory.speed.Count > i) deviation_unexplained += Math.Pow(veh.Trajectory.speed[i] - cluster.Average_Trajectory.speed[i], 2);
                        else deviation_unexplained += Math.Pow(cluster.Average_Trajectory.speed[i], 2);
                    }
                }
                deviation_explained *= cluster.Vehicles_In_Cluster.Count();
                deviation_explained /= (this._kmeans_clusters - 1);
                explained_variance += deviation_explained; //commit deviation to explained variance.

                deviation_unexplained /= (this._vehicle_count - this._kmeans_clusters);
                unexplained_variance += deviation_unexplained; //comit deviation to unexplained.
            }
        }
        private double _LNfTestCalc() {
            VehicleTrajectory SampleMean = _ODCalculateSampleMean();
            double explained_variance = 0;
            double unexplained_variance = 0;
            foreach (KeyValuePair<int, Link> link in this._Links) {
                VehiclePath path = link.Value.Link_Path;
                fTestCalc(SampleMean, path, ref explained_variance, ref unexplained_variance);
            }

            if (unexplained_variance == 0) {
                return 0;
            }
            else return explained_variance/unexplained_variance;
        }
        private VehicleTrajectory _LNCalculateSampleMean() {
            /* This Method is similar to CalculateAverageTrajectory */

            VehicleTrajectory sample_average_trajectory = new VehicleTrajectory();
            int longest_traj = 0;

            //determine what size the average trajectory should be.
            foreach (VehiclePath path in this._Paths) {
                foreach (VehiclePathClusterGroups cluster in path.Path_Cluster) {
                    if (cluster.Average_Trajectory.speed.Count > longest_traj) longest_traj = cluster.Average_Trajectory.speed.Count;
                }
            }

            //initialize average trajectory.
            for (int i = 0; i < longest_traj; i++) {
                sample_average_trajectory.speed.Add(0);
                sample_average_trajectory.second_stamp.Add(i);
            }
            foreach (VehiclePath path in this._Paths) {
                foreach (VehiclePathClusterGroups cluster in path.Path_Cluster) {
                    for (int i = 0; i < cluster.Average_Trajectory.second_stamp.Count; i++) {
                        sample_average_trajectory.speed[i] += cluster.Average_Trajectory.speed[i]; //sum the average speeds.
                    }
                }
            }
            for (int i = 0; i < longest_traj; i++) { //convert the summed speeds to averages giving equal weight to all clusters.
                sample_average_trajectory.speed[i] /= this._kmeans_clusters;
            }
            return sample_average_trajectory;
        }
        /// <summary>
        /// This function iterates through all vehicles and removes those with incomplete paths.
        /// </summary>
        /// <param name="ConsoleBox"></param>
        private void PurgePartialTrajectories(RichTextBox ConsoleBox) {
            List<VehiclePath> Complete_Paths = new List<VehiclePath>();
            foreach (VehiclePath path in this._Paths) {
                List<InputVehicles> Complete_Vehicles = new List<InputVehicles>();
                foreach (InputVehicles veh in path.Vehicles_On_Route) {
                    if (!veh.Trajectory.partial_trajectory) {
                        Complete_Vehicles.Add(veh);
                    }
                }
                if (Complete_Vehicles.Count != 0) {
                    path.Vehicles_On_Route = Complete_Vehicles;
                    Complete_Paths.Add(path);
                }
            }
            if (Complete_Paths.Count != 0) this._Paths = Complete_Paths;
            else {
                ConsoleBox.AppendText("\n\nWARNING! All trajectories have been identified as partial trajectories."); ConsoleBox.Refresh();
            }
        }
        private List<int> _k_meansODClusterFactorGenerator(List<VehiclePath> Paths, int k_means_clusters, RichTextBox ConsoleBox) {
            ConsoleBox.AppendText("\n  Calculating cluster counts for various vehicle paths..."); ConsoleBox.Refresh();

            List<int> k_meansFactors = new List<int>();
            int total_volume = 0;
            int addable_maximum = 0;
            List<int> modable_indexes = new List<int>();

            //check if there are enough clusters.
            if (k_means_clusters < Paths.Count) {
                string message = "\n\nWARNING! Insufficient k-means clusters specified. " + Convert.ToString(Paths.Count) + " Clusters assigned. \n";
                ConsoleBox.AppendText(message); ConsoleBox.Refresh();
                for (int i = 0; i < Paths.Count; i++) {
                    k_meansFactors.Add(1); //assign 1 cluster to every path.
                }
                return k_meansFactors; //exit the function.
            }

            //store the volume on each path and the total volume.
            for (int i = 0; i < Paths.Count; i++ ) {
                k_meansFactors.Add(Paths[i].Vehicles_On_Route.Count);
                total_volume += k_meansFactors[i];
            }


            //iterate over the stored volumes to convert them to factors proportional to volume.
            for (int i = 0; i < k_meansFactors.Count; i++) {
                k_meansFactors[i] = ((k_meansFactors[i] * k_means_clusters) / total_volume);
                if (k_meansFactors[i] == 0) k_meansFactors[i] = 1; //ensure a minimum of 1 cluster is assigned to that path.
                if (k_meansFactors[i] > addable_maximum) addable_maximum = k_meansFactors[i]; //save the maximum value for later use in redistributing.
                modable_indexes.Add(i); //build the list of items.
            }

            //correct the factors so that the sum adds to 200 by removing or adding clusters proportionally, starting at the largest group.
            while (k_meansFactors.Sum() != k_means_clusters) {
                int deviance = k_means_clusters - k_meansFactors.Sum();
                int deviance_scale = (k_meansFactors.Sum() < k_means_clusters) ? 1 : -1;;
                int next_addable_maximum = 0;
                List<int> next_modable_indexes = new List<int>();

                foreach (int index in modable_indexes) {
                    if (k_meansFactors[index] == addable_maximum) { //start by adding onto the highest value number.
                        k_meansFactors[index] += deviance_scale;
                        deviance -= deviance_scale; //reduce te remaining deviance.
                        if (deviance == 0) break; //we've succeeded in levelling the excess.
                        continue;
                    }
                    else if (next_addable_maximum < k_meansFactors[index]) next_addable_maximum = k_meansFactors[index]; //exclude already modified values.

                    next_modable_indexes.Add(index); //keep track of indexes that were not modified, this allows subsequent for loops to be shorter and allows even redistribution.
                }

                //we are done the loop, prepare to reduce the next one.
                if (deviance == 0) break;
                else if (next_addable_maximum > 1) { //we haven't run through all indexes.
                    addable_maximum = next_addable_maximum;
                    modable_indexes = next_modable_indexes;
                }
                else if (next_addable_maximum == 1 && deviance_scale < 0 && (addable_maximum + deviance_scale > 1)) { //oddly we have a case where 1 is the next addable maximum. We don't want to reduce clusters to less than 1 for a path.
                    string message = "\nWARNING! Additional clusters may be required. \n\n";
                    ConsoleBox.AppendText(message); ConsoleBox.Refresh();
                    addable_maximum += deviance_scale; //basically just allow the maximum value to be reduced again. This can happen if too few clusters assigned.
                }
                else { //This should never happen. 
                    string message = "\n\nWARNING! The cluster leveling algorithm was unable to suitably level the field. " + Convert.ToString(k_meansFactors.Sum()) + " clusters were assigned proportionally. \n Ordinary correction of the cluster count will take place. This can happen if the cluster count is too low.";
                    //Just subtract or add the difference to the highest count.
                    while (k_meansFactors.Sum() != k_means_clusters) {
                        int highest_k = 0;
                        int highest_index = 0;
                        int second_highest_k = 0; //allows us to directly subtract to this index.
                        for (int i = 0; i < k_meansFactors.Count; i++) {
                            if (k_meansFactors[i] > highest_k) {
                                highest_k = k_meansFactors[i];
                                highest_index = i;
                            }
                            else if (k_meansFactors[i] > second_highest_k) {
                                second_highest_k = k_meansFactors[i];
                            }
                        }
                        if (k_meansFactors.Sum() > k_means_clusters) {
                            k_meansFactors[highest_index] = (k_meansFactors.Sum() - k_means_clusters > highest_k - second_highest_k) ? second_highest_k - 1 : k_meansFactors[highest_index] + k_means_clusters - k_meansFactors.Sum();
                        }
                        else {
                            k_meansFactors[highest_index] += (k_meansFactors.Sum() - k_means_clusters);
                        }
                    }
                    ConsoleBox.AppendText(message); ConsoleBox.Refresh();
                    break; //basically, throw out of the loop.
                }
            }
            ConsoleBox.AppendText("Done!"); ConsoleBox.Refresh();
            return k_meansFactors;
        }
        private void _k_meansLNClusterFactorGenerator(Dictionary<int, Link> Links, int k_means_clusters, RichTextBox ConsoleBox) {
            ConsoleBox.AppendText("\n  Assigning cluster count to link and checking values..."); ConsoleBox.Refresh();

            foreach (var Link in Links) {
                Link.Value.cluster_count = k_means_clusters;
                if (Link.Value.cluster_count > Link.Value.Vehicles.Count()) { //reduce the cluster count to the vehicle count if there are not enough vehicles.
                    Link.Value.cluster_count = Link.Value.Vehicles.Count();
                }
            }


            /* OLD METHOD THAT JUST DIVIDED THE TOTAL TO ALL LINKS.
            //check if there are enough clusters.
            if (k_means_clusters < Links.Count) {
                string message = "\n\nWARNING! Insufficient k-means clusters specified. " + Convert.ToString(Links.Count) + " Clusters assigned. \n";
                ConsoleBox.AppendText(message); ConsoleBox.Refresh();
                for (int i = 0; i < Links.Count; i++) {
                    Links[i].cluster_count = 1; //assign 1 cluster to every path.
                }
                return; //exit the function.
            }

            //determine the total sum of the volume travelling on all links. This is not the same as the total vehicles as we are double counting vehicles that travel on multiple links.
            foreach (KeyValuePair<int, Link> link in Links) {
                total_volume += link.Value.TrajectorySnippet.Count;
            }
            //iterate over the stored volumes to convert them to factors proportional to volume.
            foreach (KeyValuePair<int, Link> link in Links) {
                link.Value.cluster_count = ((link.Value.TrajectorySnippet.Count * k_means_clusters) / total_volume);
                if (link.Value.cluster_count == 0) link.Value.cluster_count = 1; //ensure a minimum of 1 cluster is assigned to that path.
                if (link.Value.Vehicles.Count() < link.Value.cluster_count) link.Value.cluster_count = link.Value.Vehicles.Count(); //ensure there are not more clusters than vehicles.
                actual_assigned_clusters += link.Value.cluster_count; //keep track of the assigned clusters.
            }
            //level the clusters
            while (actual_assigned_clusters != k_means_clusters) {
                if (actual_assigned_clusters < k_means_clusters) {
                    foreach(var SortedLinks in Links.OrderByDescending(i => i.Value.cluster_count)) {
                        if (SortedLinks.Value.cluster_count < SortedLinks.Value.Vehicles.Count()) { //only assign if this does not over-do the volume.
                            SortedLinks.Value.cluster_count++;
                            actual_assigned_clusters++;
                            if (actual_assigned_clusters == k_means_clusters) break;
                        }
                    }
                }
                else {
                    foreach (var SortedLinks in Links.OrderByDescending(i => i.Value.cluster_count)) {
                        if (SortedLinks.Value.cluster_count <= 1) { 
                            actual_assigned_clusters = k_means_clusters;
                            string message = "\n\nWARNING! Unable to level clusters... debugging required! \n";
                            ConsoleBox.AppendText(message); ConsoleBox.Refresh();
                            break; 
                        } //should never happen...
                        SortedLinks.Value.cluster_count--;
                        actual_assigned_clusters--;
                        if (actual_assigned_clusters == k_means_clusters) break;
                    }
                }
            }*/
            return;
        }

        private Tuple<List<VehiclePath>, Dictionary<int, Link>> _parse_vehicles(string csvloc, int time_offset, RichTextBox ConsoleBox, ref int vehicle_count) {
            List<VehiclePath> Paths_Return = new List<VehiclePath>();
            List<InputVehicles> ActiveVehicles = new List<InputVehicles>();
            List<InputVehicles> InactiveVehicles = new List<InputVehicles>();
            Dictionary<int, Link> Links = new Dictionary<int, Link>();

            using (StreamReader readFile = new StreamReader(csvloc)) {
                Console.Write("  Importing All the Vehicles... ");
                ConsoleBox.AppendText("\n  Importing All the Vehicles... "); ConsoleBox.Refresh();
                string line;
                string[] rowdata;
                char delimeter = ';';
                int simsec_index = -1;
                int no_index = -1;
                int link_index = -1;
                int route_index = -1;
                int pos_index = -1;
                int speed_index = -1;
                int accel_index = -1;
                int vehtype_index = -1;

                line = readFile.ReadLine();
                rowdata = line.Split(delimeter);
                for (int i = 0; i < rowdata.Length; i++) {
                    if (i == 0) { //eliminate the "$VEHICHLE:" label
                        string[] tmp = rowdata[0].Split(':');
                        if (tmp[0] != "$VEHICLE") { //not yet at the vehicle tag, ignore the text.
                            line = readFile.ReadLine();
                            if (line == null) {
                                Console.WriteLine("ERROR. FILE HAS NO VEHICLE DATA. PROGRAM WILL PROBABLY FAIL. YOU SHOULD EXIT.");
                                ConsoleBox.AppendText("\nERROR. FILE HAS NO VEHICLE DATA. PROGRAM WILL FAIL."); ConsoleBox.Refresh();
                                throw new Exception("FILE HAS NO VEHICLE DATA");
                            }
                            rowdata = line.Split(delimeter);
                            i--;
                            continue;
                        }
                        rowdata[0] = tmp[1];
                    }
                    if (rowdata[i] == "SIMSEC") simsec_index = i;
                    else if (rowdata[i] == "NO") no_index = i;
                    else if (rowdata[i] == "LANE\\LINK\\NO") link_index = i;
                    else if (rowdata[i] == "ROUTENO") route_index = i;
                    else if (rowdata[i] == "POS") pos_index = i;
                    else if (rowdata[i] == "SPEED") speed_index = i;
                    else if (rowdata[i] == "ACCELERATION") accel_index = i;
                    else if (rowdata[i] == "VEHTYPE\\CATEGORY") vehtype_index = i;
                }

                //check the essential tags
                if (simsec_index < 0 || no_index < 0 || speed_index < 0 || link_index < 0 || vehtype_index < 0 || accel_index < 0) {
                    Console.WriteLine("VEHICLE DATA DOES NOT HAVE SECONDS, VEHICLE NUMBER, LINK, TYPE, OR SPEED. YOU SHOULD EXIT.");
                    ConsoleBox.AppendText("\nVEHICLE DATA DOES NOT HAVE SECONDS, VEHICLE NUMBER, LINK, ACCELERATION, OR SPEED. YOU SHOULD EXIT."); ConsoleBox.Refresh();
                    throw new Exception("VEHICLE DATA IS MISSING CRITICAL ELEMENTS! MAKE SURE IT HAS SPEED, LINK NUMBER, VEHICLE TYPE, SECONDS, ACCELERATION.");
                }

                while ((line = readFile.ReadLine()) != null) {
                    rowdata = line.Split(delimeter);
                    bool vehicle_exists = false;
                    for (int i = 0; i < ActiveVehicles.Count; i++) {
                        InputVehicles Vehicle = ActiveVehicles[i]; //simplify code by declearing an easy-to-remember reference type.

                        int veh_number;
                        double second_stamp;

                        int.TryParse(rowdata[no_index], out veh_number);
                        double.TryParse(rowdata[simsec_index], out second_stamp);
                        if (Vehicle.vehicle_number == veh_number) {
                            double speed;
                            double acceleration = 0;
                            double position;
                            int assoc_link;
                            int assoc_rout = 0;

                            double.TryParse(rowdata[speed_index], out speed);
                            if (accel_index >= 0) double.TryParse(rowdata[accel_index], out acceleration);
                            double.TryParse(rowdata[pos_index], out position);
                            int.TryParse(rowdata[link_index], out assoc_link);
                            if (route_index >= 0) int.TryParse(rowdata[route_index], out assoc_rout);

                            Vehicle.Trajectory.add_trajectory_element((int)second_stamp + time_offset, speed, acceleration, position, assoc_link, assoc_rout);
                            if (!Links.ContainsKey(assoc_link)) Links.Add(assoc_link, new Link(assoc_link));
                            if (!Links[assoc_link].TrajectorySnippet.ContainsKey(Vehicle.vehicle_number)) {
                                Links[assoc_link].Vehicles.Add(Vehicle.vehicle_number, Vehicle); //add the vehicle's reference to the link's list.
                                Links[assoc_link].TrajectorySnippet.Add(Vehicle.vehicle_number, new VehicleTrajectory());
                            }
                            Links[assoc_link].TrajectorySnippet[Vehicle.vehicle_number].add_trajectory_element((int)second_stamp + time_offset, speed, acceleration, position, assoc_link, assoc_rout);
                            

                            vehicle_exists = true;
                            break;
                        }
                        if (Vehicle.Trajectory.second_stamp.Last() < second_stamp - 15 + time_offset) {
                            //save resources by taking inactive vehicles off the iterative record. Vehicles assumed inactive if not seen for fifteen seconds.
                            InactiveVehicles.Add(Vehicle);
                            ActiveVehicles.RemoveAt(i);
                            i--; //decrement because we removed an element.
                            continue;
                        }
                    }
                    if (!vehicle_exists) {
                        double second_stamp;
                        double speed;
                        double acceleration = 0;
                        double position;
                        string vehicle_type;
                        int assoc_link;
                        int veh_number;
                        int assoc_rout = 0;
                        int.TryParse(rowdata[no_index], out veh_number);
                        double.TryParse(rowdata[simsec_index], out second_stamp);
                        double.TryParse(rowdata[speed_index], out speed);
                        if (accel_index >= 0) double.TryParse(rowdata[accel_index], out acceleration);
                        double.TryParse(rowdata[pos_index], out position);
                        int.TryParse(rowdata[link_index], out assoc_link);
                        if (route_index >= 0) int.TryParse(rowdata[route_index], out assoc_rout);
                        vehicle_type = rowdata[vehtype_index];

                        _vehicle_count++; //keep track of how many vehicles there are.
                        ActiveVehicles.Add(new InputVehicles(veh_number, (int)second_stamp + time_offset, speed, acceleration, position, assoc_link, assoc_rout, vehicle_type));
                        
                        //first instance of vehicle. However, can be repeat re-activated vehicle.
                        if (!Links.ContainsKey(assoc_link)) Links.Add(assoc_link, new Link(assoc_link));
                        //check if reactivated vehicle.
                        if (Links[assoc_link].Vehicles.ContainsKey(veh_number)) { 
                            //The vehicle number is being re-used or is being re-activated despite being off the network for over 1 simulation minute.
                            //Re-assign old vehicle to a new key.
                            ConsoleBox.AppendText("\n Warning, Vehicle Key re-used in file."); ConsoleBox.Refresh();
                            int link_val = 0;
                            do {
                                Random rnd = new Random();
                                link_val = rnd.Next(0, 2147483647);
                            } while (Links[assoc_link].Vehicles.ContainsKey(link_val));
                            Links[assoc_link].Vehicles.Add(link_val, Links[assoc_link].Vehicles[veh_number]);
                            Links[assoc_link].Vehicles[link_val].vehicle_number = link_val;
                            Links[assoc_link].TrajectorySnippet.Add(link_val, Links[assoc_link].TrajectorySnippet[veh_number]);
                            Links[assoc_link].Vehicles.Remove(veh_number);
                            Links[assoc_link].TrajectorySnippet.Remove(veh_number);
                        }
                        Links[assoc_link].Vehicles.Add(veh_number, ActiveVehicles.Last()); //add the vehicle's reference to the link's list.
                        Links[assoc_link].TrajectorySnippet.Add(veh_number, new VehicleTrajectory((int)second_stamp + time_offset, speed, acceleration, position, assoc_link, assoc_rout));
                    }
                }
            }
            //transfer the remaining vehicles to the inactive list. End of simulation.
            foreach (InputVehicles Vehicle in ActiveVehicles) {
                Vehicle.Trajectory.partial_trajectory = true; //mark the vehicle's trajectory as partial.
                InactiveVehicles.Add(Vehicle);
            }
            Console.WriteLine("OK.");
            ConsoleBox.AppendText("OK."); ConsoleBox.Refresh();
            //wipe the active vehicle list for posterity.
            ActiveVehicles = new List<InputVehicles>();

            Console.Write("  Analyzing all the vehicles for paths (this may take some time)... ");
            ConsoleBox.AppendText("\n  Analyzing all the vehicles for paths (this may take some time)... "); ConsoleBox.Refresh();
            foreach (InputVehicles Vehicle in InactiveVehicles) {
                Vehicle.Trajectory.calculate_distance_travelled(); //calculate the distance the vehicle travels.
                List<int> Link_Progression = new List<int>();
                Link_Progression.Add(Vehicle.Trajectory.assoc_link[0]); //populate the progression with the first link.
                for (int i = 1; i < Vehicle.Trajectory.assoc_link.Count; i++) {
                    if (Vehicle.Trajectory.assoc_link[i] != Link_Progression.Last()) Link_Progression.Add(Vehicle.Trajectory.assoc_link[i]);
                }
                bool path_exists = false;
                foreach (VehiclePath Path in Paths_Return) {
                    bool found_link = true;
                    if (Path.Link_Progression.Count != Link_Progression.Count) found_link = false;
                    else {
                        for (int i = 0; i < Link_Progression.Count; i++) {
                            if (Link_Progression[i] != Path.Link_Progression[i]) {
                                found_link = false;
                                break;
                            }
                        }
                    }
                    if (found_link) {
                        Path.Vehicles_On_Route.Add(Vehicle);
                        path_exists = true;
                        break;
                    }
                }
                if (!path_exists) {
                    Paths_Return.Add(new VehiclePath(Link_Progression, Vehicle, Paths_Return.Count));
                }
            }
            ConsoleBox.AppendText("OK."); ConsoleBox.Refresh();

            return Tuple.Create(Paths_Return, Links);
        }
    }
   
}
