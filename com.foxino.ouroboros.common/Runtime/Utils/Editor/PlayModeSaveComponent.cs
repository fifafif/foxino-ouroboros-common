using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class PlayModeSaveComponent
{
    public class ComponentData
    {
        public int Id;
        public readonly List<SerializedProperty> props = new List<SerializedProperty>(32);

        public override string ToString()
        {
            return string.Format("[ComponentData] Id={0}, Obj={1}", Id, props.Count);
        }
    }

    public static List<ComponentData> objs = new List<ComponentData>();

    static PlayModeSaveComponent()
    {
        EditorApplication.playModeStateChanged += LogPlayModeState;
    }

    private static void LogPlayModeState(PlayModeStateChange state)
    {
        Debug.Log(state);

        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            objs.Clear();
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            foreach (var data in objs)
            {
                var comp = EditorUtility.InstanceIDToObject(data.Id);
                var so = new SerializedObject(comp);

                foreach (var prop in data.props)
                {
                    so.CopyFromSerializedProperty(prop);
                }

                so.ApplyModifiedProperties();
            }
        }
    }

    [MenuItem("CONTEXT/Component/Save In Play Mode")]
    public static void SaveComponent(MenuCommand menuCommand)
    {
        var rigid = menuCommand.context;

        SerializedObject obj = new SerializedObject(rigid);

        var data = new ComponentData
        {
            Id = rigid.GetInstanceID()
        };

        var sp = obj.GetIterator();

        while (sp.NextVisible(true))
        {
            Debug.Log(sp.name);

            if (sp.propertyType == SerializedPropertyType.ObjectReference)
            {
                if (sp.objectReferenceValue == null
                    && sp.objectReferenceInstanceIDValue != 0)
                {
                    //ShowError(context, go, c.GetType().Name, ObjectNames.NicifyVariableName(sp.name));
                }
            }
            else
            {
                data.props.Add(sp.Copy());
            }
        }

        objs.Add(data);
        
        Debug.Log("Saving: " + data);
    }
}
