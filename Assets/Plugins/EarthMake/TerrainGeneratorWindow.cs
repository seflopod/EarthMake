// 
// TerrainGeneratorWindow.cs
//  
// Author:
//       Peter Bartosch <bartoschp@gmail.com>

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
	private static readonly Vector2 MIN_SIZE = new Vector2(576, 384);
	private static readonly Vector2 MAX_SIZE = new Vector2(3072, 2048);

	//Adds a new menu called "Terrain" and adds an item to it.
	[MenuItem("Ogre Toast/EarthMake")]
	public static void ShowWindow()
	{
		//Show existing window instance.  If one doesn't exist, make one
		Rect winPos = new Rect(100f, 100f, MIN_SIZE.x, MIN_SIZE.y);
		var w = (TerrainGeneratorWindow)EditorWindow.GetWindowWithRect<TerrainGeneratorWindow>(winPos, false, "EarthMake v0.9a", true);
		w.init();
	}

	#region texture_tracking_fields
	private TerrainGenerator tgGenerator;
	private bool bIsBusy;
	private bool bIsGenerated;
	private Texture2D texGenerated;
	#endregion

	#region serialized_options_fields
	private bool bShouldShowNormalOptions;
	private SerializedObject soNormalOptions;
	private SerializedProperty spNormalSize;
	private SerializedProperty spNormalSeed;
	private SerializedProperty spNormalVoronoiInf;
	private SerializedProperty spNormalCloudInf;
	private SerializedProperty spNormalShowSeams;

	private bool bShouldShowAdvancedOptions;
	private bool bShouldShowCloudOptions;
	private bool bCanSpecifyCloudStartValues;
	private SerializedObject soCloudOptions;
	private SerializedProperty spCloudUpperLeftStart;
	private SerializedProperty spCloudLowerLeftStart;
	private SerializedProperty spCloudLowerRightStart;
	private SerializedProperty spCloudUpperRightStart;

	private bool bShouldShowVoronoiOptions;
	private SerializedObject soVoronoiOptions;
	private SerializedProperty spVoronoiMetric;
	private SerializedProperty spVoronoiCombiner;
	private SerializedProperty spVoronoiFeaturePoints;
	private SerializedProperty spVoronoiSubregions;
	private SerializedProperty spVoronoiMultiplier;
	#endregion

	#region other_fields
	private string mParametersUsed;
	private Vector2 mScrollPos;
	private Vector2 mTextureScrollPos;
	#endregion
	
	private void init()
	{
		this.minSize = MIN_SIZE;
		this.maxSize = MAX_SIZE;

		//create options objects
		soNormalOptions = new SerializedObject(ScriptableObject.CreateInstance(typeof(NormalOptions)));
		soCloudOptions = new SerializedObject(ScriptableObject.CreateInstance(typeof(CloudOptions)));
		soVoronoiOptions = new SerializedObject(ScriptableObject.CreateInstance(typeof(VoronoiOptions)));

		//init normal properties
		spNormalSize = soNormalOptions.FindProperty("size");
		spNormalSeed = soNormalOptions.FindProperty("seed");
		spNormalVoronoiInf = soNormalOptions.FindProperty("voronoiInf");
		spNormalCloudInf = soNormalOptions.FindProperty("cloudInf");
		spNormalShowSeams = soNormalOptions.FindProperty("showSeams");

		//init cloud properties
		spCloudUpperLeftStart = soCloudOptions.FindProperty("upperLeftStart");
		spCloudLowerLeftStart = soCloudOptions.FindProperty("lowerLeftStart");
		spCloudLowerRightStart = soCloudOptions.FindProperty("lowerRightStart");
		spCloudUpperRightStart = soCloudOptions.FindProperty("upperRightStart");

		//init voronoi properties
		spVoronoiMetric = soVoronoiOptions.FindProperty("metric");
		spVoronoiCombiner = soVoronoiOptions.FindProperty("combiner");
		spVoronoiFeaturePoints = soVoronoiOptions.FindProperty("numberOfFeaturePoints");
		spVoronoiSubregions = soVoronoiOptions.FindProperty("numberOfSubregions");
		spVoronoiMultiplier = soVoronoiOptions.FindProperty("multiplier");

		//should we show the options right away?
		bShouldShowNormalOptions = true;
		bShouldShowCloudOptions = false;
		bShouldShowVoronoiOptions = false;



		//prep the output
		mParametersUsed = "Parameters Used:\n";
		tgGenerator = new TerrainGenerator();

		mScrollPos = new Vector2();
		mTextureScrollPos = new Vector2();

		bIsGenerated = false;
		bIsBusy = false;
	}
	
	private void OnGUI()
	{
		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.BeginVertical(GUILayout.MaxWidth(this.minSize.x / 3), GUILayout.ExpandWidth(false));
			{
				showNormalOptions();
				showAdvancedOptions();
				if(!bIsBusy)
				{
					showGenerateHeightMapBtn();
				}
			}
			EditorGUILayout.EndVertical();

			Rect r = EditorGUILayout.BeginVertical();
			{
				r.x = 0.0f;
				showTexture(r);
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndHorizontal();
	}

	#region standard_options
	private void showNormalOptions()
	{
		soNormalOptions.Update();
		bShouldShowNormalOptions = EditorGUILayout.Foldout(bShouldShowNormalOptions, "Standard Options");
		if(bShouldShowNormalOptions)
		{
			EditorGUI.indentLevel++;
			showSizeSelect();
			showSeedSelect();
			showInfluenceSliders();
			showSeamSelect();
			soNormalOptions.UpdateIfDirtyOrScript();
			EditorGUI.indentLevel--;
		}
	}

	private void showSizeSelect()
	{
		int curSize = spNormalSize.intValue;
		
		curSize = EditorGUILayout.IntField("Size", curSize);
		curSize = Mathf.Clamp(curSize, 1, 2048);

		if(!Mathf.IsPowerOfTwo(curSize))
		{	//we are only dealing with power of two textures, so if it doesn't
			//fit, go to the nearest
			curSize = Mathf.ClosestPowerOfTwo(curSize);
		}

		spNormalSize.intValue = curSize;
		soNormalOptions.ApplyModifiedProperties();
	}
	
	private void showMultiplierSelect()
	{
		EditorGUILayout.PropertyField(spVoronoiMultiplier, false);
		spVoronoiMultiplier.floatValue = Mathf.Clamp(spVoronoiMultiplier.floatValue, 0f, Mathf.Infinity);

		soNormalOptions.ApplyModifiedProperties();
	}
	
	private void showSeedSelect()
	{
		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.PropertyField(spNormalSeed, false);
			if(UnityEngine.GUILayout.Button("Randomize Seed"))
			{
				spNormalSeed.intValue = (int)LCGRandom.Instance.Next();
			}
		}
		EditorGUILayout.EndHorizontal();
		soNormalOptions.ApplyModifiedProperties();
	}
	
	private void showInfluenceSliders()
	{
		float voronoiInf = spNormalVoronoiInf.floatValue;
		float cloudInf = spNormalCloudInf.floatValue;

		cloudInf = EditorGUILayout.Slider("Cloud Fractal Influence", 1 - voronoiInf, 0.0f, 1.0f);
		voronoiInf = EditorGUILayout.Slider("Voronoi Diagram Influence", 1 - cloudInf, 0.0f, 1.0f);
		
		spNormalCloudInf.floatValue = cloudInf;
		spNormalVoronoiInf.floatValue = voronoiInf;

		soNormalOptions.ApplyModifiedProperties();
	}
	
	private void showSeamSelect()
	{
		spNormalShowSeams.boolValue = EditorGUILayout.ToggleLeft("Show Seams", spNormalShowSeams.boolValue);	
		soNormalOptions.ApplyModifiedProperties();
	}
	#endregion

	#region advanced_options
	private void showAdvancedOptions()
	{
		bShouldShowAdvancedOptions = EditorGUILayout.Foldout(bShouldShowAdvancedOptions, "Advanced Options");
		if(bShouldShowAdvancedOptions)
		{
			EditorGUI.indentLevel++;
			showCloudOptions();
			showVoronoiOptions();
			EditorGUI.indentLevel--;
		}
	}

	/// <summary>
	/// Shows the cloud options.
	/// </summary>
	/// <description>
	/// The method is a bit...ugly.  This is unavoidable for the way I'm dealing
	/// with the editor GUI stuff.  Basicall it just shows the ability for the
	/// user to specify the starting values for each corner of the fractal.
	/// </description>
	private void showCloudOptions()
	{
		bShouldShowCloudOptions = EditorGUILayout.Foldout(bShouldShowCloudOptions, "Cloud Fractal Options");
		if(bShouldShowCloudOptions)
		{
			EditorGUI.indentLevel++;

			bCanSpecifyCloudStartValues = EditorGUILayout.BeginToggleGroup("Specify Start Values", bCanSpecifyCloudStartValues);
			{
				float tmp;
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.BeginVertical();
					{
						EditorGUILayout.LabelField("Upper Left Start");
						EditorGUILayout.BeginHorizontal();
						{
							tmp = spCloudUpperLeftStart.floatValue;
							tmp = Mathf.Clamp01(EditorGUILayout.FloatField(tmp));
							if(GUILayout.Button("Random"))
							{
								tmp = LCGRandom.Instance.NextPct();
							}
							spCloudUpperLeftStart.floatValue = tmp;
						}
						EditorGUILayout.EndHorizontal();
					}
					EditorGUILayout.EndVertical();

					EditorGUILayout.BeginVertical();
					{
						EditorGUILayout.LabelField("Upper Right Start");
						EditorGUILayout.BeginHorizontal();
						{
							tmp = spCloudUpperRightStart.floatValue;
							tmp = Mathf.Clamp01(EditorGUILayout.FloatField(tmp));
							if(GUILayout.Button("Random"))
							{
								tmp = LCGRandom.Instance.NextPct();
							}
							spCloudUpperRightStart.floatValue = tmp;
						}
						EditorGUILayout.EndHorizontal();
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();
					
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.BeginVertical();
					{
						EditorGUILayout.LabelField("Lower Left Start");
						EditorGUILayout.BeginHorizontal();
						{
							tmp = spCloudLowerLeftStart.floatValue;
							tmp = Mathf.Clamp01(EditorGUILayout.FloatField(tmp));
							if(GUILayout.Button("Random"))
							{
								tmp = LCGRandom.Instance.NextPct();
							}
							spCloudLowerLeftStart.floatValue = tmp;
						}
						EditorGUILayout.EndHorizontal();
					}
					EditorGUILayout.EndVertical();
					
					EditorGUILayout.BeginVertical();
					{
						EditorGUILayout.LabelField("Lower Right Start");
						EditorGUILayout.BeginHorizontal();
						{
							tmp = spCloudLowerRightStart.floatValue;
							tmp = Mathf.Clamp01(EditorGUILayout.FloatField(tmp));
							if(GUILayout.Button("Random"))
							{
								tmp = LCGRandom.Instance.NextPct();
							}
							spCloudLowerRightStart.floatValue = tmp;
						}
						EditorGUILayout.EndHorizontal();
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndToggleGroup();
			
			soCloudOptions.ApplyModifiedProperties();
			soCloudOptions.UpdateIfDirtyOrScript();
			EditorGUI.indentLevel--;
		}
	}
	
	private void showVoronoiOptions()
	{
		bShouldShowVoronoiOptions = EditorGUILayout.Foldout(bShouldShowVoronoiOptions, "Voronoi Map Options");
		if(bShouldShowVoronoiOptions)
		{
			EditorGUI.indentLevel++;

			EditorGUILayout.PropertyField(spVoronoiMetric, false);
			EditorGUILayout.PropertyField(spVoronoiCombiner, false);
			EditorGUILayout.PropertyField(spVoronoiFeaturePoints, false);
			EditorGUILayout.PropertyField(spVoronoiSubregions, false);
			showMultiplierSelect();

			soVoronoiOptions.ApplyModifiedProperties();
			soVoronoiOptions.UpdateIfDirtyOrScript();
			EditorGUI.indentLevel--;
		}
	}
	#endregion
	
	#region other_disp
	private void showGenerateHeightMapBtn()
	{
		bool clicked = false, exportPNG = false;
		EditorGUILayout.BeginHorizontal();
		{
			clicked = GUILayout.Button("Generate Heightmap");
			if(bIsGenerated)
			{
				exportPNG = GUILayout.Button("Save As PNG");
			}
		}
		EditorGUILayout.EndHorizontal();

		if(!bIsBusy && clicked)
		{
			bIsBusy = true;
			
			tgGenerator.NormalOpt = (NormalOptions)soNormalOptions.targetObject;
			tgGenerator.CloudOpt = (CloudOptions)soCloudOptions.targetObject;
			tgGenerator.VoronoiOpt = (VoronoiOptions)soVoronoiOptions.targetObject;
			
			mParametersUsed = "Parameters Used:\n";
			mParametersUsed += tgGenerator.NormalOpt;
			mParametersUsed += "\n";
			mParametersUsed += tgGenerator.CloudOpt;
			mParametersUsed += "\n";
			mParametersUsed += tgGenerator.VoronoiOpt;
			
			tgGenerator.CreateNewHeightMap();
			texGenerated = tgGenerator.GetAsTexture2D();
			
			bIsGenerated = true;
			bIsBusy = false;
		}
		mScrollPos = EditorGUILayout.BeginScrollView(mScrollPos, false, true);
		GUILayout.TextArea(mParametersUsed);
		EditorGUILayout.EndScrollView();
		
		if(!bIsBusy && exportPNG)
		{
			bIsBusy = true;
			File.WriteAllBytes("texture.png", texGenerated.EncodeToPNG());
			HeightMapFileIO.Write("all_data", (NormalOptions)soNormalOptions.targetObject,
									(CloudOptions)soCloudOptions.targetObject,
			                      (VoronoiOptions)soVoronoiOptions.targetObject, texGenerated);
			NormalOptions no = ScriptableObject.CreateInstance<NormalOptions>();
			CloudOptions co = ScriptableObject.CreateInstance<CloudOptions>();
			VoronoiOptions vo = ScriptableObject.CreateInstance<VoronoiOptions>();
			Texture2D t = new Texture2D(2, 2);
			HeightMapFileIO.Read("all_data.emb", ref no, ref co, ref vo, ref t);
			Debug.Log(no + "\n\n" + co + "\n\n" + vo);
			File.WriteAllBytes("new_texture.png", t.EncodeToPNG());
			bIsBusy = false;
		}
	}
	
	private void showTexture(Rect bounds)
	{
		if(bIsGenerated)
		{	//if the texture has been generated, then show it.

			//the size of each side is the largest between passed bounds width,
			//2/3 of the window and the texture width.
			float s = Mathf.Max(Mathf.Max(2 * this.minSize.x / 3, bounds.width), texGenerated.width);
			bounds.width = s / 2;
			bounds.height = s / 2;
			mTextureScrollPos = EditorGUILayout.BeginScrollView(mTextureScrollPos, true, true);

			//draw the texture and scale it to fit the bounds we have.
			EditorGUI.DrawPreviewTexture(bounds, texGenerated, null, ScaleMode.ScaleToFit);

			EditorGUILayout.EndScrollView();
		}
	}
	#endregion
}