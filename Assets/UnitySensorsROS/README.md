# Unity Sensors ROS
Sensor tools for Robotics connecting with ROS and ROS2 environment.

# Environment
### Unity Version
2020.2 or later
### Depend Packages
- Burst : >=1.4.8
- Mathematics : >=1.2.1
- ROS TCP Connector : >=v0.5.0
- UnitySensors : >=0.1.0

# Installation
## Install dependencies
1. Open the package manager from `Window` -> `Package Manager` and select "Add package from git URL..."
2. Enter the following each URLs, respectively.
   - com.unity.burst
   - com.unity.mathematics
   - https://github.com/Unity-Technologies/ROS-TCP-Connector.git?path=/com.unity.robotics.ros-tcp-connector
   - https://github.com/Field-Robotics-Japan/UnitySensors.git
3. Click `Install` buton on the right bottom corner for each depend packages, respectively.

## Install UnitySensors package
1. Open the package manager from `Window` -> `Package Manager` and select "Add package from git URL..."
2. Enter the following URL. If you don't want to use the latest version, substitute your desired version tag where we've put `v0.1.0` in this example:
`https://github.com/Field-Robotics-Japan/UnitySensorsROS.git#v0.1.0`
3. Click `Add`.
