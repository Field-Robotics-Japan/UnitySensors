using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnitySensors;
using UnitySensors.ROS;

class URDF2TFConverter : EditorWindow
{
    private enum Mode
    {
        FromTextAsset,
        FromFilePath
    }

    private Mode _mode;
    private string _filePath;
    private TextAsset _urdfFile;

    [MenuItem("UnitySensorsROS/Generate TF Objects...")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(URDF2TFConverter));
    }

    private void OnGUI()
    {
        GUILayout.Label("Setting", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        _mode = (Mode)EditorGUILayout.EnumPopup("Source", _mode);

        if (_mode == Mode.FromTextAsset)
        {
            _urdfFile = EditorGUILayout.ObjectField("URDF File", _urdfFile, typeof(TextAsset), true) as TextAsset;
        }
        else
        {
            _filePath = EditorGUILayout.TextField("URDF File Path", _filePath);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate TF Objects"))
        {
            if (_mode == Mode.FromTextAsset && !_urdfFile) return;
            Generate();
        }
    }

    private void Generate()
    {
        XmlDocument doc = new XmlDocument();

        if(_mode == Mode.FromTextAsset)
        {
            doc.LoadXml(_urdfFile.text);
        }
        else
        {
            doc.Load(_filePath);
        }

        XmlNode robot_node = doc.SelectSingleNode("robot");
        GameObject robot_obj = new GameObject();
        Transform robot_trans = robot_obj.transform;
        string robot_name = robot_node.Attributes.GetNamedItem("name").Value;
        robot_obj.name = robot_name;

        Dictionary<string, Transform> links = new Dictionary<string, Transform>();
        Dictionary<string, TFSensor> tfs = new Dictionary<string, TFSensor>();
        links.Add(robot_name, robot_trans);

        XmlNodeList link_nodes = robot_node.SelectNodes("link");
        for (int i = 0; i < link_nodes.Count; i++)
        {
            GameObject link_obj = new GameObject();
            string link_name = link_nodes[i].Attributes.GetNamedItem("name").Value;
            link_obj.name = link_name;
            links.Add(link_name, link_obj.transform);
            TFSensor tf = link_obj.AddComponent<TFSensor>();
            tf.frame_id = link_name;
            tfs.Add(link_name, tf);
            if (i == 0) link_obj.AddComponent<TFPublisher>();
        }

        XmlNodeList joint_nodes = robot_node.SelectNodes("joint");
        for (int i = 0; i < joint_nodes.Count; i++)
        {
            string parent_name = joint_nodes[i].SelectSingleNode("parent").Attributes.GetNamedItem("link").Value;
            string child_name = joint_nodes[i].SelectSingleNode("child").Attributes.GetNamedItem("link").Value;
            links[child_name].parent = links[parent_name];
            tfs[parent_name].AddChild(tfs[child_name]);

            XmlNode origin_node = joint_nodes[i].SelectSingleNode("origin");
            if (origin_node != null)
            {
                XmlNode xyz_node = origin_node.Attributes.GetNamedItem("xyz");
                if (xyz_node != null)
                {
                    string[] pos_str = xyz_node.Value.Split(' ');
                    Vector3 pos = new Vector3(-float.Parse(pos_str[1]), float.Parse(pos_str[2]), float.Parse(pos_str[0]));
                    links[child_name].localPosition = pos;
                }
                else
                {
                    links[child_name].localPosition = Vector3.zero;
                }

                XmlNode rpy_node = origin_node.Attributes.GetNamedItem("rpy");
                if (rpy_node != null)
                {
                    string[] rot_str = rpy_node.Value.Split(' ');
                    Vector3 rot = new Vector3(-float.Parse(rot_str[1]), float.Parse(rot_str[2]), float.Parse(rot_str[0]));
                    links[child_name].localEulerAngles = rot * Mathf.Rad2Deg;
                }
                else
                {
                    links[child_name].localEulerAngles = Vector3.zero;
                }
            }
        }

        foreach (Transform link in links.Values)
        {
            if (link.parent) continue;
            link.parent = robot_trans;
        }
    }
}
