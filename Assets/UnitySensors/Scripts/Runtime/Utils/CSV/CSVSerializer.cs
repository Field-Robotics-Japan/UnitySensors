
using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CSVSerializer
{
    static public T[] Deserialize<T>(string text)
    {
        return (T[])CreateArray(typeof(T), ParseCSV(text));
    }

    static public T[] Deserialize<T>(List<string[]> rows)
    {
        return (T[])CreateArray(typeof(T), rows);
    }

    static public T DeserializeIdValue<T>(string text, int id_col = 0, int value_col = 1)
    {
        return (T)CreateIdValue(typeof(T), ParseCSV(text), id_col, value_col);
    }

    static public T DeserializeIdValue<T>(List<string[]> rows, int id_col=0, int value_col=1)
    {
        return (T)CreateIdValue(typeof(T), rows, id_col, value_col);
    }

    static private object CreateArray(Type type, List<string[]> rows)
    {
        Array array_value = Array.CreateInstance(type, rows.Count - 1);
        Dictionary<string, int> table = new Dictionary<string, int>();

        for (int i = 0; i < rows[0].Length; i++)
        {
            string id = rows[0][i];
            string id2 = "";
            for (int j = 0; j < id.Length; j++)
            {
                if ((id[j] >= 'a' && id[j] <= 'z') || (id[j] >= '0' && id[j] <= '9'))
                    id2 += ((char)id[j]).ToString();
                else if (id[j] >= 'A' && id[j] <= 'Z')
                    id2 += ((char)(id[j] - 'A' + 'a')).ToString();
            }

            table.Add(id, i);
            if (!table.ContainsKey(id2))
                table.Add(id2, i);
        }

        for (int i = 1; i < rows.Count; i++)
        {
            object rowdata = Create(rows[i], table, type);
            array_value.SetValue(rowdata, i-1);
        }
        return array_value;
    }

    static object Create(string[] cols, Dictionary<string, int> table, Type type)
    {
        object v = Activator.CreateInstance(type);

        FieldInfo[] fieldinfo = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo tmp in fieldinfo)
        {
            if (table.ContainsKey(tmp.Name))
            {
                int idx = table[tmp.Name];
                if (idx < cols.Length)
                    SetValue(v, tmp, cols[idx]);
            }
        }
        return v;
    }

    static void SetValue(object v, FieldInfo fieldinfo, string value)
    {
        if (value == null || value == "")
            return;

        if (fieldinfo.FieldType.IsArray)
        {
            Type elementType = fieldinfo.FieldType.GetElementType();
            string[] elem = value.Split(',');
            Array array_value = Array.CreateInstance(elementType, elem.Length);
            for (int i = 0; i < elem.Length; i++)
            {
                if (elementType == typeof(string))
                    array_value.SetValue(elem[i], i);
                else
                    array_value.SetValue(Convert.ChangeType(elem[i], elementType), i);
            }
            fieldinfo.SetValue(v, array_value);
        }
        else if (fieldinfo.FieldType.IsEnum)
            fieldinfo.SetValue(v, Enum.Parse(fieldinfo.FieldType, value.ToString()));
        else if (value.IndexOf('.') != -1 &&
            (fieldinfo.FieldType == typeof(Int32) || fieldinfo.FieldType == typeof(Int64) || fieldinfo.FieldType == typeof(Int16)))
        {
            float f = (float)Convert.ChangeType(value, typeof(float));
            fieldinfo.SetValue(v, Convert.ChangeType(f, fieldinfo.FieldType));
        }
#if UNITY_EDITOR
        else if (fieldinfo.FieldType == typeof(UnityEngine.Sprite))
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(value.ToString());
            fieldinfo.SetValue(v, sprite);
        }
#endif
        else if (fieldinfo.FieldType == typeof(string))
            fieldinfo.SetValue(v, value);
        else
            fieldinfo.SetValue(v, Convert.ChangeType(value, fieldinfo.FieldType));
    }

    static object CreateIdValue(Type type, List<string[]> rows, int id_col=0, int val_col=1)
    {
        object v = Activator.CreateInstance(type);

        Dictionary<string, int> table = new Dictionary<string, int>();

        for (int i = 1; i < rows.Count; i++)
        {
            if (rows[i][id_col].Length > 0)
                table.Add(rows[i][id_col].TrimEnd(' '), i);
        }

        FieldInfo[] fieldinfo = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo tmp in fieldinfo)
        {
            if (table.ContainsKey(tmp.Name))
            {
                int idx = table[tmp.Name];
                if (rows[idx].Length > val_col)
                    SetValue(v, tmp, rows[idx][val_col]);
            }
            else
            {
                Debug.Log("Miss " + tmp.Name);
            }
        }
        return v;
    }

    static public List<string[]> ParseCSV(string text, char separator = ',')
    {
        List<string[]> lines = new List<string[]>();
        List<string> line = new List<string>();
        StringBuilder token = new StringBuilder();
        bool quotes = false;

        for (int i = 0; i < text.Length; i++)
        {
            if (quotes == true)
            {
                if ((text[i] == '\\' && i + 1 < text.Length && text[i + 1] == '\"') || (text[i] == '\"' && i + 1 < text.Length && text[i + 1] == '\"'))
                {
                    token.Append('\"');
                    i++;
                }
                else if (text[i] == '\\' && i + 1 < text.Length && text[i + 1] == 'n')
                {
                    token.Append('\n');
                    i++;
                }
                else if (text[i] == '\"')
                {
                    line.Add(token.ToString());
                    token = new StringBuilder();
                    quotes = false;
                    if (i + 1 < text.Length && text[i + 1] == separator)
                        i++;
                }
                else
                {
                    token.Append(text[i]);
                }
            }
            else if (text[i] == '\r' || text[i] == '\n')
            {
                if (token.Length > 0)
                {
                    line.Add(token.ToString());
                    token = new StringBuilder();
                }
                if (line.Count > 0)
                {
                    lines.Add(line.ToArray());
                    line.Clear();
                }
            }
            else if (text[i] == separator)
            {
                line.Add(token.ToString());
                token = new StringBuilder();
            }
            else if (text[i] == '\"')
            {
                quotes = true;
            }
            else
            {
                token.Append(text[i]);
            }
        }

        if (token.Length > 0)
        {
            line.Add(token.ToString());
        }
        if (line.Count > 0)
        {
            lines.Add(line.ToArray());
        }
        return lines;
    }
}