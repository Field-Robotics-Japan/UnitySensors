# sensor_unity
[![unit04_test](https://github.com/Field-Robotics-Japan/unit04_unity/blob/master/.image/unit04_test.gif)](https://www.youtube.com/watch?v=C1V_L85p0-I)  
Robot sensor packages available on Unity.
You can communicate sensor info via ROS using ROS#.  

The following sensors are added.

- Velodyne Pack (VLP16)
- RGB Camera
- IMU

There are several Prefab and Scene files available for testing each sensor.
Check here for more information.

# How to use (ROS)
## 1. Launch ROS packages
### 1-1 rosbridge
Launch the `rosbridge` with following command.
```bash
$ roslaunch rosbridge_server rosbridge_websocket.launch address:=localhost
```

### 1-2 Sensors
#### 1-2-1 velodyne_pointcloud
Launch the `velodyne_pointcloud` package with following launch file.  
Please create launch file by copy and paste following script.
```xml
<launch>
  <arg name="calibration" default="$(find velodyne_pointcloud)/params/VLP16db.yaml" />
  <arg name="manager" default="velodyne_pointcloud" />
  <arg name="max_range" default="100.0" />
  <arg name="min_range" default="0.9" />

  <node pkg="velodyne_pointcloud" type="cloud_node" name="$(arg manager)">
    <param name="model" value="VLP16"/>
    <param name="calibration" value="$(arg calibration)"/>
    <param name="max_range" value="$(arg max_range)"/>
    <param name="min_range" value="$(arg min_range)"/>
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
1. Just to clone this repo with `git clone` command.
1. Then, open the project file with UnityHub.
1. Finally, run the scene file for the sensor you want to test.

You can try all the sensors in the "`Sensors`" scene file.

# LICENSE
Copyright [2020] Ryodo Tanaka groadpg@gmail.com

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.

## Dependencies
- [RosSharp](https://github.com/siemens/ros-sharp) (Apache2.0 License)
