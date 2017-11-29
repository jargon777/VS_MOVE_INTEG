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
    class InputVehicles {
        public int vehicle_number;
        public string vehicle_VISSIM_type;
        private string _converted_vehicle_type;
        public string vehicle_type {
            get {
                return _converted_vehicle_type;
            }
        }
        public VehicleTrajectory Trajectory;

        //CON
        public InputVehicles(int vehicle_number, int second_stamp, double speed, double acceleration, double position, int assoc_link, int assoc_rout, string vehicle_VISSIM_type, Dictionary<string, string> TypeMap) {
            this.vehicle_number = vehicle_number;
            this.vehicle_VISSIM_type = vehicle_VISSIM_type;
            if (TypeMap.ContainsKey(vehicle_VISSIM_type)) this._converted_vehicle_type = TypeMap[vehicle_VISSIM_type];
            else this._converted_vehicle_type = "0"; //assign a type of zero to unmatched vehicles.
            this.Trajectory = new VehicleTrajectory(second_stamp, speed, acceleration, position, assoc_link, assoc_rout);
        }
        public InputVehicles(int vehicle_number, string vehicle_VISSIM_type, VehicleTrajectory trajectory, string converted_vehicle_type) {
            this.vehicle_number = vehicle_number;
            this.Trajectory = trajectory;
            this.vehicle_VISSIM_type = vehicle_VISSIM_type;
            this._converted_vehicle_type = converted_vehicle_type;
        }
    }
    class VehicleTrajectory {
        public List<int> second_stamp;
        public List<int> vehicle_count;
        public List<double> speed;
        public List<double> acceleration;
        public List<double> position;
        public List<int> assoc_link;
        public List<int> assoc_rout;
        public double distance_travelled;
        public bool partial_trajectory;

        //CON
        public VehicleTrajectory(int second_stamp, double speed, double acceleration, double position, int assoc_link, int assoc_rout) {
            this.second_stamp = new List<int>();
            this.speed = new List<double>();
            this.vehicle_count = new List<int>();
            this.acceleration = new List<double>();
            this.position = new List<double>();
            this.assoc_link = new List<int>();
            this.assoc_rout = new List<int>();
            if ((second_stamp) == 0) this.partial_trajectory = true;
            else this.partial_trajectory = false;
            this.second_stamp.Add(second_stamp);
            this.speed.Add(speed);
            this.acceleration.Add(acceleration);
            this.position.Add(position);
            this.assoc_link.Add(assoc_link);
            this.assoc_rout.Add(assoc_rout);
            this.distance_travelled = 0;
        }
        public VehicleTrajectory() {
            this.second_stamp = new List<int>();
            this.speed = new List<double>();
            this.vehicle_count = new List<int>();
            this.acceleration = new List<double>();
            this.position = new List<double>();
            this.assoc_link = new List<int>();
            this.assoc_rout = new List<int>();
        }

        //METH
        public void add_trajectory_element(int second_stamp, double speed, double acceleration, double position, int assoc_link, int assoc_rout) {
            this.second_stamp.Add(second_stamp);
            this.speed.Add(speed);
            this.acceleration.Add(acceleration);
            this.position.Add(position);
            this.assoc_link.Add(assoc_link);
            this.assoc_rout.Add(assoc_rout);
        }
        public double get_average_speed() {
            if (this.speed.Count() == 0) return 0;
            double sum_speed = 0;
            foreach (double speeds in this.speed) {
                sum_speed += speeds;
            }
            return (sum_speed / this.speed.Count());
        }
        public void calculate_distance_travelled() {
            this.distance_travelled = 0; //clear the distance
            if (this.position.Count == 0) { //use the average speed.
                this.distance_travelled = this.get_average_speed() * this.speed.Count; //distance in metres.
            }
            else { //the original distance code. Really unnecesarry since by definition average speed*time = distance. But meh. All that effort can't be wasted.
                for (int i = 1; i < this.position.Count; i++) { //recalculate the distance
                    //if it's on the same link just subtract position.
                    if (this.assoc_link[i] == this.assoc_link[i - 1]) this.distance_travelled += this.position[i] - this.position[i - 1];
                    //well, if not, then use speed. It should be in metres per second, acceleration should be in metres per second squared.
                    else {
                        this.distance_travelled += (this.speed[i] / (this.second_stamp[i] - this.second_stamp[i - 1]))
                            + 0.5 * Math.Pow((this.second_stamp[i] - this.second_stamp[i - 1]), 2) * this.acceleration[i];
                    }
                }
                if (Double.IsInfinity(this.distance_travelled)) Console.WriteLine("Warning, infinity for distance");
            }
        }
    }
}
