//////////////////////////////////////////////////////
// MicroSplat
// Copyright (c) Jason Booth
//////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JBooth.MicroSplat;


public partial class MicroSplatTerrainEditor : Editor
{
   class MissingModule
	{
		public string name = null;
		public string link = null;
	}

   class ConvertConfig
   {
      public string name;
      public Texture2D image;
      public string [] keywords;
		public List<MissingModule> missingModules = new List<MissingModule>();
   }

   class IntegrationConfig
   {
      public string name;
      public bool assetInstalled = false;
		public string assetLink;
      public List<MissingModule> missingModules = new List<MissingModule> ();
      public bool include = false;
      public string [] keywords;
   }



   List<ConvertConfig> convertConfigs = null;
   List<IntegrationConfig> integrationConfigs = null;
   Texture2D convertSelectionImg;

   void RequireTriplanar(List<MissingModule> m)
   {
#if !__MICROSPLAT_TRIPLANAR__
				m.Add(new MissingModule() { name = "Triplanar", link = MicroSplatDefines.link_triplanar });
#endif
   }

   void RequireAntiTile (List<MissingModule> m)
   {
#if !__MICROSPLAT_DETAILRESAMPLE__
				m.Add(new MissingModule() { name = "Anti-Tiling", link = MicroSplatDefines.link_antitile });
#endif
   }

   void RequireTessellation(List<MissingModule> m)
   {
#if !__MICROSPLAT_TESSELLATION__
				m.Add(new MissingModule() { name = "Tessellation", link = MicroSplatDefines.link_tessellation });
#endif
   }

   void RequireTextureClusters(List<MissingModule> m)
   {
#if !__MICROSPLAT_TEXTURECLUSTERS__
				m.Add(new MissingModule() { name = "Texture Clusters", link = MicroSplatDefines.link_textureclusters });
#endif
   }

   void RequireStreams(List<MissingModule> m)
   {
#if !__MICROSPLAT_STREAMS__
				m.Add (new MissingModule () { name = "Wetness/Streams", link = MicroSplatDefines.link_streams });
#endif
   }

   void RequireSnow(List<MissingModule> m)
   {
#if !__MICROSPLAT_SNOW__
				m.Add (new MissingModule () { name = "Snow", link = MicroSplatDefines.link_snow });
#endif
   }

   void RequireTerrainHoles(List<MissingModule> m)
   {
#if !__MICROSPLAT_ALPHAHOLE__
      m.Add (new MissingModule () { name = "Terrain Holes", link = MicroSplatDefines.link_alphahole });
#endif
   }



   void InitConvertConfigs()
   {
      if (convertConfigs == null)
      {
         integrationConfigs = new List<IntegrationConfig> ();
         convertConfigs = new List<ConvertConfig> ();
         convertBtnStyle = new GUIStyle(GUI.skin.button);
         convertBtnStyle.padding = new RectOffset (0, 0, 0, 0);
         convertSelectionImg = Resources.Load<Texture2D> ("microsplat_prefab_selected");

         {
            var c = new ConvertConfig ();
            c.name = "Performance";
            c.image = Resources.Load<Texture2D>("microsplat_preset_performance");
            c.keywords = new string [] { "_BRANCHSAMPLES", "_BRANCHSAMPLESARG" };
            convertConfigs.Add (c);
         }

         {
            var c = new ConvertConfig ();
            c.name = "Balanced";
            c.image = Resources.Load<Texture2D> ("microsplat_preset_balanced");
            c.keywords = new string [] {
#if __MICROSPLAT_DETAILRESAMPLE__
               "_NORMALNOISE", "_DISTANCERESAMPLE",
#endif
#if __MICROSPLAT_TRIPLANAR__
               "_TRIPLANAR",
#endif
               "_BRANCHSAMPLESARG", 
               "_BRANCHSAMPLES" };
            convertConfigs.Add (c);
            RequireTriplanar (c.missingModules);
            RequireAntiTile (c.missingModules);
			}

         {
            var c = new ConvertConfig ();
            c.name = "Quality";
            c.image = Resources.Load<Texture2D> ("microsplat_preset_quality");
            c.keywords = new string [] {
#if __MICROSPLAT_TEXTURECLUSTERS__
               "_STOCHASTIC",
#endif
#if __MICROSPLAT_DETAILRESAMPLE__
               "_DISTANCERESAMPLE",
               "_DETAILNOISE",
#endif
#if __MICROSPLAT_TRIPLANAR__
               "_TRIPLANAR",
#endif
#if __MICROSPLAT_TESSELLATION__
               "_TESSDISTANCE",
#endif

               "_BRANCHSAMPLES", "_BRANCHSAMPLESAGR" };
            convertConfigs.Add (c);

            RequireTriplanar (c.missingModules);
            RequireAntiTile (c.missingModules);
            RequireTessellation (c.missingModules);
            RequireTextureClusters (c.missingModules);
         }



         // INTERGRATIONS
			{
            var c = new IntegrationConfig ();
            c.name = "Enviro";
            c.assetLink = "https://assetstore.unity.com/packages/tools/particles-effects/enviro-sky-and-weather-33963?aid=25047";

            c.keywords = new string [] {
#if __MICROSPLAT_SNOW__
               "_SNOW",
               "_USEGLOBALSNOWLEVEL",
#endif
#if __MICROSPLAT_STREAMS__
               "_WETNESS",
               "_GLOBALWETNESS",
#endif
               };




#if ENVIRO_HD || ENVIRO_LW || ENVIRO_PRO
            c.assetInstalled = true;
#endif
            RequireStreams (c.missingModules);
            RequireSnow (c.missingModules);
            integrationConfigs.Add(c);
         }

         {
            var c = new IntegrationConfig ();
            c.name = "Weather Maker";
            c.keywords = new string [] {
#if __MICROSPLAT_SNOW__
               "_SNOW",
               "_USEGLOBALSNOWLEVEL",
#endif
#if __MICROSPLAT_STREAMS__
               "_WETNESS",
               "_GLOBALWETNESS",
#endif

            };
            c.assetLink = "https://assetstore.unity.com/packages/tools/particles-effects/weather-maker-unity-weather-system-sky-water-volumetric-clouds-a-60955?aid=25047";
            c.keywords = new string [] { };
            RequireStreams (c.missingModules);
            RequireSnow (c.missingModules);

#if WEATHER_MAKER_PRESENT
            c.assetInstalled = true;
#endif
            integrationConfigs.Add(c);
			}

         

      }
      
   }

   void DrawMissingModule(MissingModule m)
   {
      EditorGUILayout.BeginHorizontal ();
      EditorGUILayout.PrefixLabel (m.name);
      if (GUILayout.Button ("Link"))
      {
         Application.OpenURL (m.link);
      }
      EditorGUILayout.EndHorizontal ();
   }

   bool DrawConvertButton(ConvertConfig c, bool selected)
   {
      EditorGUILayout.BeginVertical ();
      var r = EditorGUILayout.GetControlRect (GUILayout.Width (128), GUILayout.Height(174));
      bool ret = GUI.Button (r, c.image, convertBtnStyle);
      if (selected)
      {
         GUI.DrawTexture (r, convertSelectionImg, ScaleMode.StretchToFill, true);
      }
   
      EditorGUILayout.EndVertical ();
      return ret;
   }

   int selectedConvertConfig = 0;
   GUIStyle convertBtnStyle;

   bool DoConvertGUI(MicroSplatTerrain t)
   {
      if (t.templateMaterial == null)
      {
         InitConvertConfigs ();
         using (new GUILayout.VerticalScope (GUI.skin.box))
         {
            EditorGUILayout.LabelField ("Select a Template:");

            EditorGUILayout.BeginHorizontal ();
            for (int i = 0; i < convertConfigs.Count; ++i)
            {
               var c = convertConfigs [i];
               if (DrawConvertButton (c, i == selectedConvertConfig))
               {
                  selectedConvertConfig = i;
               }
            }
            EditorGUILayout.EndHorizontal ();

            var selectedConfig = convertConfigs [selectedConvertConfig];
            if (selectedConfig.missingModules.Count > 0)
            {
               EditorGUILayout.HelpBox ("You are missing some MicroSplat modules needed by this template. The preset may still be used but some features will not be enabled. Missing modules are listed below:", MessageType.Warning);
               for (int i = 0; i < selectedConfig.missingModules.Count; ++i)
               {
                  var m = selectedConfig.missingModules [i];
                  DrawMissingModule (m);
               }
            }

            EditorGUILayout.LabelField ("Select any integrations you want to add:");
            // integrations
            for (int i = 0; i < integrationConfigs.Count; ++i)
            {
               
               var ic = integrationConfigs [i];
               if (!ic.assetInstalled)
               {
                  EditorGUILayout.BeginHorizontal ();
                  EditorGUILayout.LabelField (ic.name, GUILayout.Width (120));
                  EditorGUILayout.LabelField ("Not Installed", GUILayout.Width (120));
                  if (GUILayout.Button ("Link", GUILayout.Width (120)))
                  {
                     Application.OpenURL (ic.assetLink);
                  }
                  EditorGUILayout.EndHorizontal ();
               }
               else
               {
                  EditorGUILayout.BeginHorizontal ();
                  ic.include = EditorGUILayout.Toggle (ic.include, GUILayout.Width (20));
                  EditorGUILayout.LabelField (ic.name);
                  EditorGUILayout.EndHorizontal ();
                  if (ic.include && ic.missingModules.Count > 0)
                  {
                     using (new GUILayout.VerticalScope (GUI.skin.box))
                     {
                        EditorGUILayout.HelpBox ("Some MicroSplat modules requested by this module are not installed. Some or all features of the integration will not be active.", MessageType.Warning);
                        for (int j = 0; j < ic.missingModules.Count; ++j)
                        {
                           var m = ic.missingModules [j];
                           DrawMissingModule (m);
                        }
                     }
                  }
               }
            }

            if (GUILayout.Button ("Convert to MicroSplat"))
            {
               // get all terrains in selection, not just this one, and treat as one giant terrain
               var objs = Selection.gameObjects;
               List<Terrain> terrains = new List<Terrain> ();
               for (int i = 0; i < objs.Length; ++i)
               {
                  Terrain ter = objs [i].GetComponent<Terrain> ();
                  if (ter != null)
                  {
                     terrains.Add (ter);
                  }
                  Terrain [] trs = objs [i].GetComponentsInChildren<Terrain> ();
                  for (int x = 0; x < trs.Length; ++x)
                  {
                     if (!terrains.Contains (trs [x]))
                     {
                        terrains.Add (trs [x]);
                     }
                  }
               }

               Terrain terrain = t.GetComponent<Terrain> ();
               int texcount = 16;
#if UNITY_2018_3_OR_NEWER
               texcount = terrain.terrainData.terrainLayers.Length;
#else
               texcount = terrain.terrainData.splatPrototypes.Length;
#endif
               List<string> keywords = new List<string> (selectedConfig.keywords);
               if (texcount <= 4)
               {
                  keywords.Add ("_MAX4TEXTURES");
               }
               else if (texcount <= 8)
               {
                  keywords.Add ("_MAX8TEXTURES");
               }
               else if (texcount <= 12)
               {
                  keywords.Add ("_MAX12TEXTURES");
               }
               else if (texcount <= 20)
               {
                  keywords.Add ("_MAX20TEXTURES");
               }
               else if (texcount <= 24)
               {
                  keywords.Add ("_MAX24TEXTURES");
               }
               else if (texcount <= 28)
               {
                  keywords.Add ("_MAX28TEXTURES");
               }
               else if (texcount > 28)
               {
                  keywords.Add ("_MAX32TEXTURES");
               }

               for (int i = 0; i < integrationConfigs.Count; ++i)
               {
                  var ic = integrationConfigs [i];
                  if (ic.include)
                  {
                     keywords.AddRange (ic.keywords);
                  }
               }

               // setup this terrain
               t.templateMaterial = MicroSplatShaderGUI.NewShaderAndMaterial (terrain, keywords.ToArray ());

               var config = TextureArrayConfigEditor.CreateConfig (terrain);
               t.templateMaterial.SetTexture ("_Diffuse", config.diffuseArray);
               t.templateMaterial.SetTexture ("_NormalSAO", config.normalSAOArray);

               t.propData = MicroSplatShaderGUI.FindOrCreatePropTex (t.templateMaterial);
#if UNITY_2018_3_OR_NEWER
               if (terrain.terrainData.terrainLayers.Length > 0)
               {
                  var uvScale = terrain.terrainData.terrainLayers[0].tileSize;
                  var uvOffset = terrain.terrainData.terrainLayers[0].tileOffset;

                  uvScale = MicroSplatRuntimeUtil.UnityUVScaleToUVScale(uvScale, terrain);
                  uvOffset.x = uvScale.x / terrain.terrainData.size.x * 0.5f * uvOffset.x;
                  uvOffset.y = uvScale.y / terrain.terrainData.size.x * 0.5f * uvOffset.y;
                  Vector4 scaleOffset = new Vector4(uvScale.x, uvScale.y, uvOffset.x, uvOffset.y);
                  t.templateMaterial.SetVector("_UVScale", scaleOffset);

               }
#else
               if (terrain.terrainData.splatPrototypes.Length > 0)
               {
                  var uvScale = terrain.terrainData.splatPrototypes [0].tileSize;
                  var uvOffset = terrain.terrainData.splatPrototypes [0].tileOffset;

                  uvScale = MicroSplatRuntimeUtil.UnityUVScaleToUVScale (uvScale, terrain);
                  uvOffset.x = uvScale.x / terrain.terrainData.size.x * 0.5f * uvOffset.x;
                  uvOffset.y = uvScale.y / terrain.terrainData.size.x * 0.5f * uvOffset.y;
                  Vector4 scaleOffset = new Vector4 (uvScale.x, uvScale.y, uvOffset.x, uvOffset.y);
                  t.templateMaterial.SetVector ("_UVScale", scaleOffset);

               }
#endif
               // we need to set a few things on the material if certain features are enabled.
               // Test for property existence as module might not be installed and feature was culled. 
               if (System.Array.Exists (selectedConfig.keywords, x => x == "_TRIPLANAR"))
               {
                  if (t.templateMaterial.HasProperty ("_TriplanarUVScale"))
                  {
                     t.templateMaterial.SetVector ("_TriplanarUVScale", new Vector4 (0.25f, 0.25f, 0, 0));
                  }
               }

               if (System.Array.Exists(selectedConfig.keywords, x => x == "_NORMALNOISE"))
               {
                  if (t.templateMaterial.HasProperty ("_NormalNoise"))
                  {
                     t.templateMaterial.SetTexture ("_NormalNoise", MicroSplatUtilities.GetAutoTexture ("microsplat_def_detail_normal_01"));
                  }
               }

               if (System.Array.Exists (selectedConfig.keywords, x => x == "_DETAILNOISE"))
               {
                  if (t.templateMaterial.HasProperty ("_DetailNoise"))
                  {
                     t.templateMaterial.SetTexture ("_DetailNoise", MicroSplatUtilities.GetAutoTexture ("microsplat_def_detail_noise"));
                  }
               }

               // now make sure others all have the same settings as well.
               for (int i = 0; i < terrains.Count; ++i)
               {
                  var nt = terrains [i];
                  var mgr = nt.GetComponent<MicroSplatTerrain> ();
                  if (mgr == null)
                  {
                     mgr = nt.gameObject.AddComponent<MicroSplatTerrain> ();
                  }
                  mgr.templateMaterial = t.templateMaterial;

                  if (mgr.propData == null)
                  {
                     mgr.propData = MicroSplatShaderGUI.FindOrCreatePropTex (mgr.templateMaterial);
                  }
               }
               

               Selection.SetActiveObjectWithContext (config, config);
               t.keywordSO = MicroSplatUtilities.FindOrCreateKeywords (t.templateMaterial);

               t.keywordSO.keywords.Clear ();
               t.keywordSO.keywords = new List<string> (keywords);
               
               MicroSplatTerrain.SyncAll ();

               // trun on draw instanced if enabled and tessellation is disabled, unless render loop is LWRP/URP in which case it does work..
#if UNITY_2018_3_OR_NEWER
               if (t.keywordSO != null && (!t.keywordSO.IsKeywordEnabled("_TESSDISTANCE") || t.keywordSO.IsKeywordEnabled("_MSRENDERLOOP_UNITYLD")))
               {
                  for (int i = 0; i < terrains.Count; ++i)
                  {
                     var nt = terrains [i];
                     var mgr = nt.GetComponent<MicroSplatTerrain> ();
                     if (mgr != null && mgr.keywordSO != null && !mgr.keywordSO.IsKeywordEnabled("_MSRENDERLOOP_UNITYLD"))
                     {
                        nt.drawInstanced = true;
                     }
                  }
               }
#endif
               return true;
            }
         }
      }
      return false;
   }
}
