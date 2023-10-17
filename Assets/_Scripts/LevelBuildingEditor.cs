using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelBuildingScript))]
public class LevelBuildingEditor : Editor
{
    public Texture2D[] buttonImages = new Texture2D[2]; // Adjust the array size as needed

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.LabelField("Buttons with Images");

        // Display an array of button images
        for (int i = 0; i < buttonImages.Length; i++)
        {
            buttonImages[i] = (Texture2D)EditorGUILayout.ObjectField($"{buttonImages[i].name}", buttonImages[i], typeof(Texture2D), false);

            if (buttonImages[i] != null)
            {
                if (GUILayout.Button(buttonImages[i], GUILayout.Width(50), GUILayout.Height(50)))
                {
                    // Button click action for the button at index 'i'
                }
            }
        }
    }

}
