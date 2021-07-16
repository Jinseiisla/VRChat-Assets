using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public class PresetEditor : ShaderGUI
{
    // Just some stuff copy paste.
    BindingFlags bindingFlags = BindingFlags.Public |
                                BindingFlags.NonPublic |
                                BindingFlags.Instance |
                                BindingFlags.Static;

    //Styles for the Textures.
    private static class Styles
    {
        public static GUIContent baseText = new GUIContent("Base Texture"); // Name of the Material (Texture)
    }


    MaterialProperty _Diffuse = null; // Basically the property of your shader object. (Can be located on the shader itself.)


    /*
        For if you want to use presets!
    */
    public static List<string> presetsList = new List<string>();
    public static string[] presets;
    int popupIndex = 0;
    string presetText = "";
    string dirPath = "Assets/Jins Shaders/Presets/"; // Preset Path


    /*
        To Help with foldouts, Utilizes bools in an array so you dont have to constantly make a new bool.
    */
    public static Dictionary<Material, JinsToggles> foldouts = new Dictionary<Material, JinsToggles>();

    /* 
        List of Bools and Header Names
        These are numbered correctly so make sure the bools are set to the correct thing for your headers.
        You will see how to detect Headers further.
    */
    JinsToggles toggles = new JinsToggles(
        new bool[]
        {
            false, // Presets
            false // Texture Settings
        },
        new string[]
        {
            "PRESETS", // Header Names
            "Texture Settings"
        }
    );


    /*
        MaterialEditor me is for the editor to function with foldouts and ect.
        MaterialProperty[] props is the material object.
        OnGUI is where we place all the code for our ShaderGUI.
    */

    public override void OnGUI(MaterialEditor me, MaterialProperty[] props)
    {
        /*
            A Bunch of copy paste stuff from Aethras. 
        */
        Material mat = (Material)me.target;

        foreach (var property in GetType().GetFields(bindingFlags))
        {
            if (property.FieldType == typeof(MaterialProperty))
            {
                property.SetValue(this, FindProperty(property.Name, props));
            }
        }

        if (!foldouts.ContainsKey(mat))
            foldouts.Add(mat, toggles);

        DirectoryInfo dir = new DirectoryInfo(dirPath);
        FileInfo[] info = dir.GetFiles();
        foreach (FileInfo f in info)
        {
            if (!f.Name.Contains(".meta") && f.Name.Contains(".mat"))
            {
                Material candidate = (Material)AssetDatabase.LoadAssetAtPath(dirPath + f.Name, typeof(Material));
                if (candidate.shader.name == mat.shader.name)
                {
                    int indOf = f.Name.IndexOf(".");
                    presetsList.Add(f.Name.Substring(0, indOf));
                }
            }
        }
        presets = presetsList.ToArray();

        /*
            End of Copy and Paste. 
        */


        // The Title Of your shader
        JinsStyles.ShurikenHeaderCentered("«  <color=#7af4ff>Preset Editor</color>  <color=#aa0000ff>v1.0.0</color>  »");


        // The beginning of the ShaderGUI Functions
        EditorGUI.BeginChangeCheck();
        {
            GUILayout.Space(15);


            /*
                Foldout
                DoFoldout is the full function of the foldout
                needs to be an if statement for bool

                foldouts is from the JinsToggles above
                mat and me is from the OnGUI
                "PRESETS" is the header we use to put into the Toggles array.

                use this as a copy paste.
            */
            if (JinsStyles.DoFoldout(foldouts, mat, me, "PRESETS"))
            {
                GUILayout.Space(4);
                float buttonWidth = EditorGUIUtility.labelWidth - 5.0f;
                if (JinsStyles.SimpleButton("Capture", buttonWidth, 0))
                {
                    presetText = JinsStyles.ReplaceInvalidChars(presetText);
                    string filePath = dirPath + presetText + ".mat";
                    Material newMat = new Material(mat);
                    AssetDatabase.CreateAsset(newMat, filePath);
                    AssetDatabase.Refresh();
                }
                GUILayout.Space(-17);
                Rect r = EditorGUILayout.GetControlRect();
                r.x += EditorGUIUtility.labelWidth;
                r.width = JinsStyles.GetPropertyWidth();
                presetText = EditorGUI.TextArea(r, presetText);
                if (JinsStyles.SimpleButton("Apply", buttonWidth, 0))
                {
                    string presetPath = dirPath + presets[popupIndex] + ".mat";
                    Material selectedMat = (Material)AssetDatabase.LoadAssetAtPath(presetPath, typeof(Material));
                    mat.CopyPropertiesFromMaterial(selectedMat);
                }
                GUILayout.Space(-17);
                r = EditorGUILayout.GetControlRect();
                r.x += EditorGUIUtility.labelWidth;
                r.width = JinsStyles.GetPropertyWidth();
                popupIndex = EditorGUI.Popup(r, popupIndex, presets);
                GUILayout.Label("Presets are stored in: " + dirPath + "\nPasted Presets from Aethras", EditorStyles.textArea);
            }


            /*
                A more simple Foldout
                PropertyGroup is styled box the surround the properties.
            */
            if (JinsStyles.DoFoldout(foldouts, mat, me, "Texture Settings"))
            {
                JinsStyles.PropertyGroup(() =>
                {
                    me.TexturePropertySingleLine(Styles.baseText, _Diffuse);
                });
            }

            JinsStyles.PropertyGroup(() =>
            {
                JinsStyles.CenteredText("Credit to Aethras for the fact that im using his Editor Styles.\nMochie for creating the foldouts, toggles, and Shuriken Style", 10, 0, 0);
            });


            /*
                PartingLine is white
                PartingLine2 is black
                RenderQueueField(); gets the renderQueue

                EditorGUILayout.BeginHorizontal(); Begins a new horizontal format.
                EditorGUILayout.EndHorizontal(); Ends the horizontal format.

                GUILayout.FlexibleSpace(); Fits it all into the middle.

                imagelinkbutton is just a image button. Must be Texture2D with the right name set inside.

                CenteredImage is just an image you can put.
            */
            JinsStyles.PartingLine2();
            me.RenderQueueField();
            JinsStyles.PartingLine2();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            JinsStyles.imageLinkButton("Github", 70, 30, "https://github.com/NotThisJin");
            JinsStyles.imageLinkButton("Discord", 70, 30, "https://discord.gg/nRjEmxBbPe");
            JinsStyles.imageLinkButton("Twitter", 70, 30, "https://twitter.com/NotThisJin");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            JinsStyles.CenteredImage("demonslayerlogo", 0, 0, 10, 100);

        }

    }
}
