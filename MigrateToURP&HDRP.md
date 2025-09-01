# Migrating To URP/HDRP
Only the sensors or visualizers written with **custom shaders** need to be migrated to URP/HDRP. The rest of them will work without any changes. 

Which means the **Depth Camera**, **RGBD Camera**, **DepthBuffer Lidar** and the **Point Cloud Visualizers** need to be migrated.

In addition to the shader, the post-processing steps are also different in URP/HDRP. In the built-in pipeline, the post-processing is applied in the `OnRenderImage` method of the script. In URP/HDRP, the post-processing steps are different.

> \[!NOTE]
>
> Now only support migrating the **Depth Camera** and **RGBD Camera** to URP/HDRP.

## Migrating to URP

1. Create the URP project

2. Install the [ROS-TCP-Connector](https://github.com/Unity-Technologies/ROS-TCP-Connector) package

3. Install this package and import sample assets

4. Update the materials from `Window` -> `Rendering` -> `Render Pipeline Converter`. Tick the `Rendering Settings` and `Material Upgrade` then click `Initialize And Convert`

   <img src=".image/URP_convert_materials.png" style="zoom:50%;" />

5. Open `DepthCamera` Shader Graph, change `Active Targets` from `Built-in` to `Univsrsal`, then change the material to `Fullscreen` and click `Save Asset`

   ![](.image/URP_ShaderGraph.png)

6. Go to the `Settings` folder, create a new `URP Universal Render` asset called `URP-DepthCamera-Renderer`, and add the `Full Screen Pass Renderer Feture` on it

7. On the `Full Screen Pass Renderer Feture`, select the `DepthCamera` Shader Graph as the `Pass Material`.

   ![](.image/URP_post_process.png)

8. Add the `URP-DepthCamera-Renderer` to the `Renderer List` of `Default Render Pipeline Asset`, such as the `URP-HighFidelity` asset. And tick the `Depth Texture` selection

   ![](.image/URP_render_list.png)

9. Select the `DepthCamera` and `RGBDCamera` prefabs, set their renderer to  `URP-DepthCamera-Renderer`

   ![](.image/URP_camera_renderer.png)

10. The **Depth Camera** and **RGBD Camera** should work normally in URP

    ![](.image/URP_final_result.png)


## Migrating to HDRP

Migrating to HDRP is relatively simple. You need to:

1. Create the HDRP project

2. Install the [ROS-TCP-Connector](https://github.com/Unity-Technologies/ROS-TCP-Connector) package

3. Install this package and import sample assets

4. Update the materials from `Window` -> `Rendering` -> `HDRP Wizard`. Click `Convert All Built-in Materials to HDRP`

   ![](.image/HDRP_convert_materials.png)

5. Open `DepthCamera` Shader Graph, change `Active Targets` from `Built-in` to `HDRP`, then change the material to `Fullscreen` and click `Save Asset`

   ![](.image/HDRP_ShaderGraph.png)

6. Add a custom pass volume in the scene via `Volume` -> `Custom Pass`. Set the `Mode`to `Camera`, drag the  **Depth Camera** or **RGBD Camera** in the scene to the `Target Camera`. Add the `Full Screen Custom Pass` in the `Custom Passes` list. Set the `FullScreen Material` to `DepthCamera` Shader Graph.

   ![](.image/HDRP_custom_pass.png)

7. The **Depth Camera** and **RGBD Camera** should work normally in HDRP

   ![](.image/HDRP_final_result.png)