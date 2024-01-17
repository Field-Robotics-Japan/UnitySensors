## UnitySensors

[![Acquire activation file](https://github.com/Field-Robotics-Japan/UnitySensorsROSAssets/workflows/Acquire%20activation%20file/badge.svg)](https://github.com/Field-Robotics-Japan/UnitySensorsROSAssets/actions?query=workflow%3A%22Acquire+activation+file%22)
[![CI](https://github.com/Field-Robotics-Japan/UnitySensorsROSAssets/workflows/CI/badge.svg)](https://github.com/Field-Robotics-Japan/UnitySensorsROSAssets/actions?query=workflow%3ACI)

![unity_sensors_ros_assets](.image/unity_sensors_ros_assets.gif)

## Overview
Robot sensor packages available on Unity.
You can communicate sensor info via ROS and ROS2 using ROSTCPConnector and ROSTCPEndpoint.  

The following sensors are added.

- Velodyne 3D LiDAR (Velodyne VLP-16, VLP-16-HiRes, VLP-32, VLS-128)
- Livox 3D LiDAR(Avia, Horizon, Mid40, Mid70, Tele, HAP, Mid360)
- RGB Camera
- RGBD Camera
- IMU
- GNSS
- (GroundTruth)
- (TF)

There are several Prefab and Scene files available for testing each sensor.

## Dependencies
- [RosTCPConnector](https://github.com/Unity-Technologies/ROS-TCP-Connector) (Appache 2.0 LICENSE)

## Package Installation (For using UnitySensors in your project)
1. Using Unity 2021.3 or later, open the Package Manager from `Window` -> `Package Manager`.
2. In the Package Manager window, find and click the + button in the upper lefthand corner of the window. Select `Add package from git URL....`

3. Enter the git URL for the desired package. 
    1. For the UnitySensors, enter `https://github.com/Field-Robotics-Japan/UnitySensors.git?path=/Assets/UnitySensors#v2.0.0b`.
    2. For the UnitySensorsROS, enter `https://github.com/Field-Robotics-Japan/UnitySensorsROS.git?path=/Assets/UnitySensorsROS#v2.0.0b`.
    __Note: UnitySensorsROS does not contain UnitySensors.__
4. Click `Add`.

## Documentation

## LICENSE
Copyright [2020-2024] Ryodo Tanaka (groadpg@gmail.com) and Akiro Harada

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
