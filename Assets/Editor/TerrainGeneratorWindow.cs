// 
// TerrainGeneratorWindow.cs
//  
// Author:
//       Peter Bartosch <bartoschp@gmail.com>
// 
// Copyright (c) 2013 Peter Bartosch
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEditor;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Text;

/// <summary>
/// Main window for EarthMake.
/// </summary>
/// <description>
/// This window provides a GUI for using the EarthMake terrain generation classes.  It also provides the
/// ability to save any generated heightmap and parameters, as well as load them (loading needs to be
/// implemented).
/// </description>
public class TerrainGeneratorWindow : EditorWindow
{
	private float _worleyInf;
	private float _cloudInf;
	private int _seed;
	private int _size;
	private bool _showAdvOpt;
	private bool _showNormalOpt;
	private SerializedObject _normalOpt;
	private bool _showWorleyOpt;
	private SerializedObject _worleyOpt;
	private bool _showCloudOpt;
	private bool _specCloudStart;
	private SerializedObject _cloudOpt;
	private string _param;
	private Vector2 _scrollPos;
	private Vector2 _texScrollPos;
	private TerrainGenerator _gen;
	private bool _busy;
	private bool _generated;
	private Texture2D _image;

	//Adds a new menu called "Terrain" and adds an item to it.
	[MenuItem("Window/Terrain Generator")]
	public static void ShowWindow()
	{
		//Show existing window instance.  If one doesn't exist, make one
		Rect winPos = new Rect(100, 100, 768, 768);
		TerrainGeneratorWindow w =
			(TerrainGeneratorWindow)EditorWindow.GetWindowWithRect<TerrainGeneratorWindow>(winPos, false,
																					"Terrain Generator v0.8a");
		w.Init();
	}
	
	private void Init()
	{
		this.minSize = 768 * Vector2.one;
		this.maxSize = 2048 * Vector2.one;
		_showNormalOpt = true;
		_normalOpt = new SerializedObject(ScriptableObject.CreateInstance(typeof(NormalOptions)));
		_showWorleyOpt = false;
		_worleyOpt = new SerializedObject(ScriptableObject.CreateInstance(typeof(WorleyOptions)));
		_showCloudOpt = false;
		_cloudOpt = new SerializedObject(ScriptableObject.CreateInstance(typeof(CloudOptions)));
		_param = "Parameters Used:\n";
		_scrollPos = new Vector2();
		_texScrollPos = new Vector2();
		_gen = new TerrainGenerator();
		_generated = false;
		_gen = new TerrainGenerator();
		_busy = false;
		_generated = false;
	}
	
	private void OnGUI()
	{
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.BeginVertical(GUILayout.MaxWidth(this.minSize.x / 3), GUILayout.ExpandWidth(false));
		ShowNormalOptions();
		EditorGUILayout.Space();
		ShowAdvancedOptions();
		if(!_busy)
			ShowGenerateHeightMap();
		EditorGUILayout.EndVertical();
		Rect r = EditorGUILayout.BeginVertical();
		r.x = 0.0f;
		ShowHeightMapTexture(r);
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
	}
	
	private void ShowNormalOptions()
	{
		EditorGUI.indentLevel = 0;
		_showNormalOpt = EditorGUILayout.Foldout(_showNormalOpt, "Standard Options");
		if(_showNormalOpt)
		{
			ShowSizeSelect();
			ShowMultiplierSelect();
			ShowSeedSelect();
			ShowInfluenceSliders();
			ShowSeamSelect();
			_normalOpt.UpdateIfDirtyOrScript();
		}
		
	}
	
	private void ShowAdvancedOptions()
	{
		EditorGUI.indentLevel = 0;
		_showAdvOpt = EditorGUILayout.Foldout(_showAdvOpt, "Advanced Options");
		if(_showAdvOpt)
		{
			ShowCloudOptions();
			ShowWorleyOptions();
		}
	}
	
	#region normal_opt_disp
	private void ShowSizeSelect()
	{
		EditorGUI.indentLevel = 1;
		int size = ((NormalOptions)_normalOpt.targetObject).size;
		
		size = EditorGUILayout.IntField("Size", size);
		size = (size < 0) ? -size : size;
		if(!Mathf.IsPowerOfTwo(size))
			size = Mathf.NextPowerOfTwo(size);
		
		_normalOpt.FindProperty("size").intValue = size;
		
		_normalOpt.ApplyModifiedProperties();
	}
	
	private void ShowMultiplierSelect()
	{
		EditorGUI.indentLevel = 1;

		EditorGUILayout.PropertyField(_normalOpt.FindProperty("multiplier"), false);
		float mult = _normalOpt.FindProperty("multiplier").floatValue;
		if(mult < 0)
		{
			mult *= -1;
			_normalOpt.FindProperty("multiplier").floatValue = mult;
		}
		
		_normalOpt.ApplyModifiedProperties();
	}
	
	private void ShowSeedSelect()
	{
		EditorGUI.indentLevel = 1;
		int seed = ((NormalOptions)_normalOpt.targetObject).seed;
		EditorGUILayout.BeginHorizontal();
		seed = EditorGUILayout.IntField("Seed", seed);
		if(UnityEngine.GUILayout.Button("Randomize Seed"))
			seed = (int)LCGRandom.GlobalInstance.Next();
		EditorGUILayout.EndHorizontal();
		
		_normalOpt.FindProperty("seed").intValue = seed;
		_normalOpt.ApplyModifiedProperties();
	}
	
	private void ShowInfluenceSliders()
	{
		EditorGUI.indentLevel = 1;
		float worleyInf = _normalOpt.FindProperty("worleyInf").floatValue;
		float cloudInf = _normalOpt.FindProperty("cloudInf").floatValue;
		
		cloudInf = EditorGUILayout.Slider("Cloud Fractal Influence", 1 - worleyInf, 0.0f, 1.0f);
		worleyInf = EditorGUILayout.Slider("Worley Noise Influence", 1 - cloudInf, 0.0f, 1.0f);
		
		_normalOpt.FindProperty("cloudInf").floatValue = cloudInf;
		_normalOpt.FindProperty("worleyInf").floatValue = worleyInf;
		
		_normalOpt.ApplyModifiedProperties();
	}
	
	private void ShowSeamSelect()
	{
		EditorGUI.indentLevel = 1;
		EditorGUILayout.PropertyField(_normalOpt.FindProperty("showSeams"), false);		
		_normalOpt.ApplyModifiedProperties();
	}
	#endregion
	
	#region adv_opt_disp
	private void ShowCloudOptions()
	{
		EditorGUI.indentLevel = 1;

		_showCloudOpt = EditorGUILayout.Foldout(_showCloudOpt, "Cloud Fractal Options");
		if(_showCloudOpt)
		{
			EditorGUI.indentLevel = 2;
			_specCloudStart = EditorGUILayout.BeginToggleGroup("Specify Start Values", _specCloudStart);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Slider(_cloudOpt.FindProperty("upperLeftStart"), 0.0f, 1.0f);
			if(GUILayout.Button("Random"))
				_cloudOpt.FindProperty("upperLeftStart").floatValue = (LCGRandom.GlobalInstance.NextPct());
			EditorGUILayout.EndHorizontal();
				
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Slider(_cloudOpt.FindProperty("lowerLeftStart"), 0.0f, 1.0f);
			if(GUILayout.Button("Random"))
				_cloudOpt.FindProperty("lowerLeftStart").floatValue = (LCGRandom.GlobalInstance.NextPct());
			EditorGUILayout.EndHorizontal();
				
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Slider(_cloudOpt.FindProperty("lowerRightStart"), 0.0f, 1.0f);
			if(GUILayout.Button("Random"))
				_cloudOpt.FindProperty("lowerRightStart").floatValue = (LCGRandom.GlobalInstance.NextPct());
			EditorGUILayout.EndHorizontal();
				
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Slider(_cloudOpt.FindProperty("upperRightStart"), 0.0f, 1.0f);
			if(GUILayout.Button("Random"))
				_cloudOpt.FindProperty("upperRightStart").floatValue = (LCGRandom.GlobalInstance.NextPct());
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndToggleGroup();
			
			_cloudOpt.ApplyModifiedProperties();
			_cloudOpt.UpdateIfDirtyOrScript();
		}
	}
	
	private void ShowWorleyOptions()
	{
		EditorGUI.indentLevel = 1;
		_showWorleyOpt = EditorGUILayout.Foldout(_showWorleyOpt, "Worley Noise Options");
		if(_showWorleyOpt)
		{
			EditorGUI.indentLevel = 2;
			EditorGUILayout.Slider(_worleyOpt.FindProperty("zoomLevel"), 0.5f, 20.0f);
			EditorGUILayout.PropertyField(_worleyOpt.FindProperty("metric"), false);
			EditorGUILayout.PropertyField(_worleyOpt.FindProperty("combiner"), false);
			_worleyOpt.ApplyModifiedProperties();
			_worleyOpt.UpdateIfDirtyOrScript();
		}
	}
	#endregion
	
	#region other_disp
	private void ShowGenerateHeightMap()
	{
		EditorGUILayout.BeginHorizontal();
		bool clicked = GUILayout.Button("Generate Heightmap");
		bool exportPNG = false;
		if(_generated)
			exportPNG = GUILayout.Button("Save As PNG");
		EditorGUILayout.EndHorizontal();
		if(!_busy && clicked)
		{
			_busy = true;
			
			_gen.NormalOpt = (NormalOptions)_normalOpt.targetObject;
			_gen.CloudOpt = (CloudOptions)_cloudOpt.targetObject;
			_gen.WorleyOpt = (WorleyOptions)_worleyOpt.targetObject;
			
			_param = "Parameters Used:\n";
			_param += _gen.NormalOpt;
			_param += "\n";
			_param += _gen.CloudOpt;
			_param += "\n";
			_param += _gen.WorleyOpt;
			
			_gen.CreateNewHeightMap();
			_image = _gen.GetAsTexture2D();
			
			_generated = true;
			_busy = false;
		}
		_scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, true);
		GUILayout.TextArea(_param);
		EditorGUILayout.EndScrollView();
		
		if(!_busy && exportPNG)
		{
			_busy = true;
			File.WriteAllBytes("texture.png", _image.EncodeToPNG());
			HeightMapFileIO.Write("all_data", (NormalOptions)_normalOpt.targetObject,
									(CloudOptions)_cloudOpt.targetObject,
									(WorleyOptions)_worleyOpt.targetObject, _image);
			NormalOptions no = ScriptableObject.CreateInstance<NormalOptions>();
			CloudOptions co = ScriptableObject.CreateInstance<CloudOptions>();
			WorleyOptions wo = ScriptableObject.CreateInstance<WorleyOptions>();
			Texture2D t = new Texture2D(2, 2);
			HeightMapFileIO.Read("all_data.emb", ref no, ref co, ref wo, ref t);
			Debug.Log(no + "\n\n" + co + "\n\n" + wo);
			File.WriteAllBytes("new_texture.png", t.EncodeToPNG());
			_busy = false;
		}
	}
	
	private void ShowHeightMapTexture(Rect bounds)
	{
		if(_generated)
		{
			float s = Mathf.Max(Mathf.Max(2 * this.minSize.x / 3, bounds.width), _image.width);
			bounds.width = s;
			bounds.height = s;
			_texScrollPos = EditorGUILayout.BeginScrollView(_texScrollPos, true, true);
			EditorGUI.DrawPreviewTexture(bounds, _image, null, ScaleMode.ScaleToFit);
			EditorGUILayout.EndScrollView();
		}
	}
	#endregion
}