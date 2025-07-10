<div align="center">

# UnitySensors

[![][github-release-shield]][github-release-link]
[![][external-unity-shield]][external-unity-link]
[![][external-ros-shield]][external-ros-link]
[![][github-workflow-shield]][github-workflow-link] <br>
[![][github-contributors-shield]][github-contributors-link]
[![][github-forks-shield]][github-forks-link]
[![][github-stars-shield]][github-stars-link]
[![][github-issues-shield]][github-issues-link]
[![][github-license-shield]][github-license-link]

![unity_sensors_ros_assets](.image/unity_sensors_ros_assets.gif)

</div>

## 🔍 Overview

UnitySensos is a projet that regroup two Unity3D packages that allow you to **easily** use Unity3D as robotic simulation !
There are several Prefab and Scene files available for testing each sensor.

**1. UnitySensor**

The following sensors are available inside :

- Velodyne 3D LiDAR (Velodyne VLP-16, VLP-16-HiRes, VLP-32, HDL-32E, VLS-128)
- Livox 3D LiDAR(Avia, Horizon, Mid40, Mid70, Tele, HAP, Mid360)
- RGB Camera
- RGBD Camera
- Panoramic Camera
- Fisheye Camera (Equidistant and [EUCM][external-EUCM-link] models with adjustable parameters)
- IMU
- GNSS
- (GroundTruth)
- (TF)

Both the `Panoramic Camera` and `Fisheye Camera` are based on cubemap projection. 
To avoid seams between cubemap faces, you may need to disable some post-processing effects and use fixed exposure.

**2. UnitySensorROS**

This package is responsible to make the link between sensor and ROS by serializing sensor raw data and sending to them to ROS using [ROS-TCP-Connector][external-RosTCPConnector-link] package.
To receive the data in ROS take a look at [ROS-TCP-Endpoint][external-RosTCPEndpoint-link].

## 🚀 Quick start

### Inside Unity3D

> \[!NOTE]
>
> UnitySensors is made for Unity 2022.3 or later

1. Open the Package Manager from `Window` -> `Package Manager`.
2. In the Package Manager window, find and click the + button in the upper lefthand corner of the window. Select `Add package from git URL....`

3. Enter the git URL for the desired package.
    1. For the UnitySensors, enter `https://github.com/Field-Robotics-Japan/UnitySensors.git?path=/Packages/UnitySensors#v2.0.5`.
    2. For the UnitySensorsROS, enter `https://github.com/Field-Robotics-Japan/UnitySensors.git?path=/Packages/UnitySensorsROS#v2.0.5`.
    __Note: UnitySensorsROS does not contain UnitySensors.__
4. Click `Add`.
5. Import sample assets of UnitySensors and UnitySensorsROS from `Window` -> `Package Manager` -> `UnitySensors` or `UnitySensorsROS` -> `Import Sample`.

### Inside ROS workspace

1. Download lastest release of [ROS-TCP-Endpoint][external-RosTCPEndpoint-release-link].
2. Build your workspace.
3. Launch ROS endpoint node.

### Migrating to URP/HDRP

Please refer to [Migrating to URP/HDRP](MigrateToURP&HDRP.md).

## 🤝 Contributing

<div align="center">

A huge thank you to everyone who is helping to improve UnitySensors !

[![contributors badge][github-contributors-img]][github-contributors-link]

</div>

## 🔗 Dependencies
- [ROS-TCP-Connector][external-RosTCPConnector-link] (Appache 2.0 LICENSE)

## 📄 LICENSE
Copyright [2020-2024] Ryodo Tanaka (groadpg@gmail.com) and Akiro Harada

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.

<!-- LINK GROUP -->

[external-EUCM-link]: https://github.com/ethz-asl/kalibr/wiki/supported-models
[external-unity-shield]: https://img.shields.io/badge/Unity3D-%3E%202022.3-blue?style=flat-square&logo=unity
[external-unity-link]: https://unity.com/
[external-ros-shield]: https://img.shields.io/badge/ROS-1%7C2-blue?style=flat-square&logo=ros
[external-ros-link]: https://www.ros.org/
[external-RosTCPConnector-link]: https://github.com/Unity-Technologies/ROS-TCP-Connector
[external-RosTCPEndpoint-link]: https://github.com/Unity-Technologies/ROS-TCP-Endpoint
[external-RosTCPEndpoint-release-link]: https://github.com/Unity-Technologies/ROS-TCP-Endpoint/releases
[github-workflow-shield]: https://img.shields.io/github/actions/workflow/status/Field-Robotics-Japan/UnitySensors/main.yml?style=flat-square&logo=github&label=CI%20check
[github-workflow-link]: https://github.com/Field-Robotics-Japan/UnitySensors/actions/workflows/main.yml
[github-contributors-img]: https://readme-contribs.as93.net/contributors/Field-Robotics-Japan/UnitySensors?avatarSize=40&shape=circle
[github-contributors-link]: https://github.com/Field-Robotics-Japan/UnitySensors/graphs/contributors
[github-contributors-shield]: https://img.shields.io/github/contributors/Field-Robotics-Japan/UnitySensors?color=B2FFA3&style=flat-square
[github-forks-link]: https://github.com/Field-Robotics-Japan/UnitySensors/network/members
[github-forks-shield]: https://img.shields.io/github/forks/Field-Robotics-Japan/UnitySensors?color=8ae8ff&style=flat-square
[github-issues-link]: https://github.com/Field-Robotics-Japan/UnitySensors/issues
[github-issues-shield]: https://img.shields.io/github/issues/Field-Robotics-Japan/UnitySensors?color=FFDBFA&style=flat-square
[github-license-link]: https://github.com/Field-Robotics-Japan/UnitySensors/blob/main/LICENSE
[github-license-shield]: https://img.shields.io/github/license/Field-Robotics-Japan/UnitySensors?color=FFADAD&style=flat-square
[github-stars-link]: https://github.com/Field-Robotics-Japan/UnitySensors/network/stargazers
[github-stars-shield]: https://img.shields.io/github/stars/Field-Robotics-Japan/UnitySensors?color=F9DC5F&style=flat-square
[github-release-link]: https://github.com/Field-Robotics-Japan/UnitySensors/releases
[github-release-shield]: https://img.shields.io/github/v/release/Field-Robotics-Japan/UnitySensors?color=9BF6FF&logo=github&style=flat-square

