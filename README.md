# UnitySensorsROSAssets

[![Acquire activation file](https://github.com/Field-Robotics-Japan/UnitySensorsROSAssets/workflows/Acquire%20activation%20file/badge.svg)](https://github.com/Field-Robotics-Japan/UnitySensorsROSAssets/actions?query=workflow%3A%22Acquire+activation+file%22)
[![CI](https://github.com/Field-Robotics-Japan/UnitySensorsROSAssets/workflows/CI/badge.svg)](https://github.com/Field-Robotics-Japan/UnitySensorsROSAssets/actions?query=workflow%3ACI)

![unity_sensors_ros_assets](.image/unity_sensors_ros_assets.gif)
Robot sensor packages available on Unity.
You can communicate sensor info via ROS and ROS2 using ROSTCPConnector and ROSTCPEndpoint.  

The following sensors are added.

- 3D LiDAR (Velodyne VLP-16)
- 2D LiDAR (Hokuyo UST-30LX)
- RGB Camera (Logitech C910)
- IMU

There are several Prefab and Scene files available for testing each sensor.
Check [this directory](https://github.com/Field-Robotics-Japan/sensors_unity/tree/develop/Assets/Scenes).

# How to use (ROS)
## 1. Launch ROS packages
### 1-1 ros_tcp_endpoint
Launch the `ros_tcp_endpoint` with following command.
```bash
roslaunch ros_tcp_endpoint endpoint.launch 
```

### 1-2 Sensors
#### 1-2-1 velodyne_pointcloud
Launch the `velodyne_pointcloud` package with following launch file.  
Please create launch file by copy and paste following script.  
**The version of `velodyne_pointcloud` package should be later than 1.6.0.**  
If you fail to lanch following launch file, please download velodyne package directoly into your workspace, build it, and launch them again.  
https://github.com/ros-drivers/velodyne
```xml
<launch>
  <arg name="calibration" default="$(find velodyne_pointcloud)/params/VLP16db.yaml" />
  <arg name="manager" default="velodyne_pointcloud" />
  <arg name="max_range" default="100.0" />
  <arg name="min_range" default="0.1" />
  <arg name="frame_id" default="velodyne" />

  <node pkg="velodyne_pointcloud" type="transform_node" name="$(arg manager)">
    <param name="model" value="VLP16"/>
    <param name="calibration" value="$(arg calibration)"/>
    <param name="max_range" value="$(arg max_range)"/>
    <param name="min_range" value="$(arg min_range)"/>
    <param name="frame_id" value="$(arg frame_id)"/>
    <!-- <param name="view_direction" value="0"/> -->
    <!-- <param name="view_width" value="360"/> -->
  </node>
</launch>
```

#### 1-2-2 RGB Camera
Image communication between Unity and ROS is done using JPEG compression.  
You can set the topic name in the Unity inspector window.  

**Note** :  
Message type is sensor_msgs/CompressedImage.
So, the topic name must end with "/compressed".

In order to handle sensor_msgs/CompressedImage,  
launch the `image_transport` package with following launch file.  
Please create launch file by copy and paste following script.

```xml
<launch>
    <node name="image_republish" pkg="image_transport" 
          type="republish" args="compressed raw">
      <remap from="input" to="/image_raw/compressed" />
      <remap from="output" to="/image_exp" />
    </node>
</launch>
```
If you want to check the images in Rviz, you don't need to use the above launch file.

#### 1-2-3 IMU
It can be used in the same way as a normal IMU sensor.

## 2. Clone and run on Unity
1. Just to clone this repo with `git clone --recursive https://github.com/Field-Robotics-Japan/UnitySensorsROSAssets.git` command.
1. Then, open the project file with UnityHub.
1. Finally, run the scene file for the sensor you want to test.

You can try all the sensors in the "`Sensors`" scene file.

# LICENSE
Copyright [2020-2021] Ryodo Tanaka groadpg@gmail.com

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.

## Dependencies
- [RosTCPConnector](https://github.com/Unity-Technologies/ROS-TCP-Connector) (Appache 2.0 LICENSE)
