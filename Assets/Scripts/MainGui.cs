﻿using System;
using System.Collections.Generic;
using SFB;
using UnityEngine;

public enum PropChannelMap
{
    None,
    Height,
    Metallic,
    Smoothness,
    Edge,
    Ao,
    AoEdge
}


public class MainGui : MonoBehaviour
{
    public static MainGui Instance;

    private readonly ExtensionFilter[] _imageLoadFilter =
    {
        new ExtensionFilter("Image Files", "png", "jpg", "jpeg", "tga", "bmp", "exr")
    };

    private readonly ExtensionFilter[] _imageSaveFilter =
    {
        new ExtensionFilter("Image Files", "png", "jpg", "jpeg", "tga", "exr")
    };

    private MapType _activeMapType;
    public Texture2D AoMap;
    public Texture2D DiffuseMap;
    public Texture2D DiffuseMapOriginal;
    public Texture2D EdgeMap;

    private const float GamaCorrection = 2.2f;

    public RenderTexture HdHeightMap;
    public Texture2D HeightMap;
    private string _lastDirectory = "";
    public Texture2D MetallicMap;
    public Texture2D NormalMap;

    public Texture2D PropertyMap;
    public Texture2D SmoothnessMap;

    public Texture2D TextureBlack;
    public Texture2D TextureGrey;
    public Texture2D TextureNormal;
    private Texture2D _textureToSave;
    public Texture2D TextureWhite;

    public AlignmentGui AlignmentGuiScript;

    public GameObject AoFromNormalGuiObject;
    private AOFromNormalGui _aoFromNormalGuiScript;

    private bool _busySaving;

    private bool _clearTextures;

    public GameObject CommandListExecutorObject;
    private CommandListExecutor _commandListExecutorScript;
    public Cubemap[] CubeMaps;

    public GameObject EdgeFromNormalGuiObject;
    private EdgeFromNormalGui _edgeFromNormalGuiScript;

    public GameObject EditDiffuseGuiObject;
    private EditDiffuseGui _editDiffuseGuiScript;
    private bool _exrSelected;
    public Material FullMaterial;

    public Material FullMaterialRef;

    public GameObject HeightFromDiffuseGuiObject;
    private HeightFromDiffuseGui _heightFromDiffuseGuiScript;

    public bool HideGui;
    private bool _jpgSelected;

    public GameObject MaterialGuiObject;
    private MaterialGui _materialGuiScript;

    public GameObject MetallicGuiObject;
    private MetallicGui _metallicGuiScript;

    public GameObject NormalFromHeightGuiObject;
    private NormalFromHeightGui _normalFromHeightGuiScript;

    private List<GameObject> _objectsToUnhide;
    private char _pathChar = '/';
    private bool _pngSelected = true;

    public GameObject PostProcessGuiObject;
    public PropChannelMap PropBlue = PropChannelMap.None;
    private bool _propBlueChoose;
    private Material _propertyCompMaterial;

    private Shader _propertyCompShader;
    public PropChannelMap PropGreen = PropChannelMap.None;
    private bool _propGreenChoose;

    public PropChannelMap PropRed = PropChannelMap.None;
    private bool _propRedChoose;

    public string QuicksavePathAo = "";
    public string QuicksavePathDiffuse = "";
    public string QuicksavePathEdge = "";
    public string QuicksavePathHeight = "";
    public string QuicksavePathMetallic = "";
    public string QuicksavePathNormal = "";
    public string QuicksavePathProperty = "";
    public string QuicksavePathSmoothness = "";

    //public Material skyboxMaterial;
    public ReflectionProbe ReflectionProbe;
    public Material SampleMaterial;
    public Material SampleMaterialRef;

    public GameObject SaveLoadProjectObject;
    private SaveLoadProject _saveLoadProjectScript;
    private int _selectedCubemap;
    public FileFormat SelectedFormat;

    public GameObject SettingsGuiObject;
    private SettingsGui _settingsGuiScript;

    public GameObject SmoothnessGuiObject;
    private SmoothnessGui _smoothnessGuiScript;

    public GameObject SuggestionGuiObject;

    public GameObject TestObject;
    public GameObject TestObjectBox;
    public GameObject TestObjectCube;
    public GameObject TestObjectCylinder;
    public GameObject TestObjectSphere;

    private Texture2D _textureToLoad;
    private bool _tgaSelected;

    private Material _thisMaterial;

    public GameObject TilingTextureMakerGuiObject;
    private TilingTextureMakerGui _tilingTextureMakerGuiScript;

    private string toolsWindowTitle = "Texture Tools";
    private static readonly int CorrectionId = Shader.PropertyToID("_GamaCorrection");
    private static readonly int MainTexId = Shader.PropertyToID("_MainTex");
    private static readonly int GlobalCubemapId = Shader.PropertyToID("_GlobalCubemap");
    private static readonly int DisplacementMapId = Shader.PropertyToID("_DisplacementMap");
    private static readonly int DiffuseMapId = Shader.PropertyToID("_DiffuseMap");
    private static readonly int NormalMapId = Shader.PropertyToID("_NormalMap");
    private static readonly int MetallicMapId = Shader.PropertyToID("_MetallicMap");
    private static readonly int SmoothnessMapId = Shader.PropertyToID("_SmoothnessMap");
    private static readonly int AoMapId = Shader.PropertyToID("_AOMap");
    private static readonly int EdgeMapId = Shader.PropertyToID("_EdgeMap");
    private static readonly int TilingId = Shader.PropertyToID("_Tiling");

    private void Start()
    {
        _lastDirectory = Application.dataPath;
        Instance = this;

        HeightMap = null;
        HdHeightMap = null;
        DiffuseMap = null;
        DiffuseMapOriginal = null;
        NormalMap = null;
        MetallicMap = null;
        SmoothnessMap = null;
        EdgeMap = null;
        AoMap = null;

        _propertyCompShader = Shader.Find("Hidden/Blit_Property_Comp");
        _propertyCompMaterial = new Material(_propertyCompShader);

        Shader.SetGlobalFloat(CorrectionId, GamaCorrection);

        FullMaterial = new Material(FullMaterialRef.shader);
        FullMaterial.CopyPropertiesFromMaterial(FullMaterialRef);

        SampleMaterial = new Material(SampleMaterialRef.shader);
        SampleMaterial.CopyPropertiesFromMaterial(SampleMaterialRef);

        _heightFromDiffuseGuiScript = HeightFromDiffuseGuiObject.GetComponent<HeightFromDiffuseGui>();
        _normalFromHeightGuiScript = NormalFromHeightGuiObject.GetComponent<NormalFromHeightGui>();
        _edgeFromNormalGuiScript = EdgeFromNormalGuiObject.GetComponent<EdgeFromNormalGui>();
        _aoFromNormalGuiScript = AoFromNormalGuiObject.GetComponent<AOFromNormalGui>();
        _editDiffuseGuiScript = EditDiffuseGuiObject.GetComponent<EditDiffuseGui>();
        _metallicGuiScript = MetallicGuiObject.GetComponent<MetallicGui>();
        _smoothnessGuiScript = SmoothnessGuiObject.GetComponent<SmoothnessGui>();
        _materialGuiScript = MaterialGuiObject.GetComponent<MaterialGui>();
        _tilingTextureMakerGuiScript = TilingTextureMakerGuiObject.GetComponent<TilingTextureMakerGui>();
        _saveLoadProjectScript = SaveLoadProjectObject.GetComponent<SaveLoadProject>();
        _commandListExecutorScript = CommandListExecutorObject.GetComponent<CommandListExecutor>();
        _settingsGuiScript = SettingsGuiObject.GetComponent<SettingsGui>();

        _settingsGuiScript.LoadSettings();

        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.WindowsPlayer)
            _pathChar = '\\';

        TestObject.GetComponent<Renderer>().material = FullMaterial;
        SetMaterialValues();

        ReflectionProbe.RenderProbe();
    }

    public void SetPreviewMaterial(Texture2D textureToPreview)
    {
        CloseWindows();
        if (textureToPreview == null) return;
        FixSizeMap(textureToPreview);
        SampleMaterial.SetTexture(MainTexId, textureToPreview);
        TestObject.GetComponent<Renderer>().material = SampleMaterial;
    }

    public void SetPreviewMaterial(RenderTexture textureToPreview)
    {
        CloseWindows();
        if (textureToPreview == null) return;
        FixSizeMap(textureToPreview);
        SampleMaterial.SetTexture(MainTexId, textureToPreview);
        TestObject.GetComponent<Renderer>().material = SampleMaterial;
    }

    public void SetMaterialValues()
    {
        Shader.SetGlobalTexture(GlobalCubemapId, CubeMaps[_selectedCubemap]);

        FullMaterial.SetTexture(DisplacementMapId, HeightMap != null ? HeightMap : TextureGrey);

        if (DiffuseMap != null)
            FullMaterial.SetTexture(DiffuseMapId, DiffuseMap);
        else if (DiffuseMapOriginal != null)
            FullMaterial.SetTexture(DiffuseMapId, DiffuseMapOriginal);
        else
            FullMaterial.SetTexture(DiffuseMapId, TextureGrey);

        FullMaterial.SetTexture(NormalMapId, NormalMap != null ? NormalMap : TextureNormal);

        FullMaterial.SetTexture(MetallicMapId, MetallicMap != null ? MetallicMap : TextureBlack);

        FullMaterial.SetTexture(SmoothnessMapId, SmoothnessMap != null ? SmoothnessMap : TextureBlack);

        FullMaterial.SetTexture(AoMapId, AoMap != null ? AoMap : TextureWhite);

        FullMaterial.SetTexture(EdgeMapId, EdgeMap != null ? EdgeMap : TextureGrey);

        TestObject.GetComponent<Renderer>().material = FullMaterial;

        FullMaterial.SetVector(TilingId, new Vector4(1, 1, 0, 0));
    }

    public void CloseWindows()
    {
        _heightFromDiffuseGuiScript.Close();
        _normalFromHeightGuiScript.Close();
        _edgeFromNormalGuiScript.Close();
        _aoFromNormalGuiScript.Close();
        _editDiffuseGuiScript.Close();
        _metallicGuiScript.Close();
        _smoothnessGuiScript.Close();
        _tilingTextureMakerGuiScript.Close();
        AlignmentGuiScript.Close();
        MaterialGuiObject.SetActive(false);
        PostProcessGuiObject.SetActive(false);
    }

    private void HideWindows()
    {
        _objectsToUnhide = new List<GameObject>();

        if (HeightFromDiffuseGuiObject.activeSelf) _objectsToUnhide.Add(HeightFromDiffuseGuiObject);

        if (NormalFromHeightGuiObject.activeSelf) _objectsToUnhide.Add(NormalFromHeightGuiObject);

        if (EdgeFromNormalGuiObject.activeSelf) _objectsToUnhide.Add(EdgeFromNormalGuiObject);

        if (AoFromNormalGuiObject.activeSelf) _objectsToUnhide.Add(AoFromNormalGuiObject);

        if (EditDiffuseGuiObject.activeSelf) _objectsToUnhide.Add(EditDiffuseGuiObject);

        if (MetallicGuiObject.activeSelf) _objectsToUnhide.Add(MetallicGuiObject);

        if (SmoothnessGuiObject.activeSelf) _objectsToUnhide.Add(SmoothnessGuiObject);

        if (MaterialGuiObject.activeSelf) _objectsToUnhide.Add(MaterialGuiObject);

        if (PostProcessGuiObject.activeSelf) _objectsToUnhide.Add(PostProcessGuiObject);

        if (TilingTextureMakerGuiObject.activeSelf) _objectsToUnhide.Add(TilingTextureMakerGuiObject);

        HeightFromDiffuseGuiObject.SetActive(false);
        NormalFromHeightGuiObject.SetActive(false);
        EdgeFromNormalGuiObject.SetActive(false);
        AoFromNormalGuiObject.SetActive(false);
        EditDiffuseGuiObject.SetActive(false);
        MetallicGuiObject.SetActive(false);
        SmoothnessGuiObject.SetActive(false);
        MaterialGuiObject.SetActive(false);
        PostProcessGuiObject.SetActive(false);
        TilingTextureMakerGuiObject.SetActive(false);
    }

    private void ShowFullMaterial()
    {
        CloseWindows();
        FixSize();
        MaterialGuiObject.SetActive(true);
        _materialGuiScript.Initialize();
    }

    private static void Fullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    private void OnGUI()
    {
        //==================================================//
        // 					Unhidable Buttons				//
        //==================================================//

        if (GUI.Button(new Rect(Screen.width - 80, Screen.height - 40, 70, 30), "Quit")) Application.Quit();

        GUI.enabled = false;
        if (Screen.fullScreen)
        {
            if (GUI.Button(new Rect(Screen.width - 190, Screen.height - 40, 100, 30), "Windowed")) Fullscreen();
        }
        else
        {
            if (GUI.Button(new Rect(Screen.width - 190, Screen.height - 40, 100, 30), "Full Screen")) Fullscreen();
        }

        GUI.enabled = true;

        if (GUI.Button(new Rect(Screen.width - 260, 10, 140, 30), "Make Suggestion"))
            SuggestionGuiObject.SetActive(true);

        if (HideGui == false)
        {
            if (GUI.Button(new Rect(Screen.width - 110, 10, 100, 30), "Hide Gui"))
            {
                HideGui = true;
                HideWindows();
            }
        }
        else
        {
            if (!GUI.Button(new Rect(Screen.width - 110, 10, 100, 30), "Show Gui")) return;
            HideGui = false;
            foreach (var objToHide in _objectsToUnhide)
                objToHide.SetActive(true);

            return;
        }

        //==================================================//
        // 						Main Gui					//
        //==================================================//


        const int spacingX = 130;

        var offsetX = 20;
        var offsetY = 20;


        //==============================//
        // 			Height Map			//
        //==============================//

        GUI.Box(new Rect(offsetX, offsetY, 110, 250), "Height Map");

        if (HeightMap != null) GUI.DrawTexture(new Rect(offsetX + 5, offsetY + 25, 100, 100), HeightMap);

        // Paste 
        if (GUI.Button(new Rect(offsetX + 5, offsetY + 130, 20, 20), "P"))
        {
            _activeMapType = MapType.Height;
            PasteFile();
        }

        GUI.enabled = HeightMap != null;

        // Copy
        if (GUI.Button(new Rect(offsetX + 30, offsetY + 130, 20, 20), "C"))
        {
            _textureToSave = HeightMap;
            CopyFile();
        }

        GUI.enabled = true;

        // Open
        if (GUI.Button(new Rect(offsetX + 60, offsetY + 130, 20, 20), "O")) OpenTextureFile(MapType.Height);

        GUI.enabled = HeightMap != null;

        // Save
        if (GUI.Button(new Rect(offsetX + 85, offsetY + 130, 20, 20), "S")) SaveTextureFile(MapType.Height);


        if (HeightMap == null || QuicksavePathHeight == "")
            GUI.enabled = false;
        else
            GUI.enabled = true;

        // Quick Save
        if (GUI.Button(new Rect(offsetX + 15, offsetY + 160, 80, 20), "Quick Save"))
        {
            _textureToSave = HeightMap;
            SaveFile(QuicksavePathProperty);
        }

        GUI.enabled = HeightMap != null;

        if (GUI.Button(new Rect(offsetX + 15, offsetY + 190, 80, 20), "Preview")) SetPreviewMaterial(HeightMap);

        GUI.enabled = true;

        if (DiffuseMapOriginal == null && DiffuseMap == null && NormalMap == null)
            GUI.enabled = false;
        else
            GUI.enabled = true;

        if (GUI.Button(new Rect(offsetX + 5, offsetY + 220, 50, 20), "Create"))
        {
            CloseWindows();
            FixSize();
            HeightFromDiffuseGuiObject.SetActive(true);
            _heightFromDiffuseGuiScript.NewTexture();
            _heightFromDiffuseGuiScript.DoStuff();
        }

        GUI.enabled = true;

        GUI.enabled = HeightMap != null;

        if (GUI.Button(new Rect(offsetX + 60, offsetY + 220, 45, 20), "Clear"))
        {
            ClearTexture(MapType.Height);
            CloseWindows();
            SetMaterialValues();
            FixSize();
        }

        GUI.enabled = true;


        //==============================//
        // 			Diffuse Map			//
        //==============================//

        GUI.Box(new Rect(offsetX + spacingX, offsetY, 110, 250), "Diffuse Map");

        if (DiffuseMap != null)
            GUI.DrawTexture(new Rect(offsetX + spacingX + 5, offsetY + 25, 100, 100), DiffuseMap);
        else if (DiffuseMapOriginal != null)
            GUI.DrawTexture(new Rect(offsetX + spacingX + 5, offsetY + 25, 100, 100), DiffuseMapOriginal);

        // Paste 
        if (GUI.Button(new Rect(offsetX + spacingX + 5, offsetY + 130, 20, 20), "P"))
        {
            _activeMapType = MapType.DiffuseOriginal;
            PasteFile();
        }

        if (DiffuseMapOriginal == null && DiffuseMap == null)
            GUI.enabled = false;
        else
            GUI.enabled = true;

        // Copy
        if (GUI.Button(new Rect(offsetX + spacingX + 30, offsetY + 130, 20, 20), "C"))
        {
            if (DiffuseMap != null)
                _textureToSave = DiffuseMap;
            else
                _textureToSave = DiffuseMapOriginal;

            CopyFile();
        }

        GUI.enabled = true;

        // Open
        if (GUI.Button(new Rect(offsetX + spacingX + 60, offsetY + 130, 20, 20), "O"))
            OpenTextureFile(MapType.DiffuseOriginal);

        if (DiffuseMapOriginal == null && DiffuseMap == null)
            GUI.enabled = false;
        else
            GUI.enabled = true;

        // Save
        if (GUI.Button(new Rect(offsetX + spacingX + 85, offsetY + 130, 20, 20), "S")) SaveTextureFile(MapType.Diffuse);

        if (DiffuseMapOriginal == null && DiffuseMap == null || QuicksavePathDiffuse == "")
            GUI.enabled = false;
        else
            GUI.enabled = true;

        // Quick Save
        if (GUI.Button(new Rect(offsetX + spacingX + 15, offsetY + 160, 80, 20), "Quick Save"))
        {
            if (DiffuseMap != null)
                _textureToSave = DiffuseMap;
            else
                _textureToSave = DiffuseMapOriginal;

            SaveFile(QuicksavePathDiffuse);
        }

        if (DiffuseMapOriginal == null && DiffuseMap == null)
            GUI.enabled = false;
        else
            GUI.enabled = true;

        if (GUI.Button(new Rect(offsetX + spacingX + 15, offsetY + 190, 80, 20), "Preview"))
        {
            if (DiffuseMap != null)
                SetPreviewMaterial(DiffuseMap);
            else
                SetPreviewMaterial(DiffuseMapOriginal);
        }

        GUI.enabled = DiffuseMapOriginal != null;

        if (GUI.Button(new Rect(offsetX + spacingX + 5, offsetY + 220, 50, 20), "Edit"))
        {
            CloseWindows();
            FixSize();
            EditDiffuseGuiObject.SetActive(true);
            _editDiffuseGuiScript.NewTexture();
            _editDiffuseGuiScript.DoStuff();
        }

        if (DiffuseMapOriginal == null && DiffuseMap == null)
            GUI.enabled = false;
        else
            GUI.enabled = true;

        if (GUI.Button(new Rect(offsetX + spacingX + 60, offsetY + 220, 45, 20), "Clear"))
        {
            ClearTexture(MapType.Diffuse);
            CloseWindows();
            SetMaterialValues();
            FixSize();
        }

        GUI.enabled = true;


        //==============================//
        // 			Normal Map			//
        //==============================//

        GUI.Box(new Rect(offsetX + spacingX * 2, offsetY, 110, 250), "Normal Map");

        if (NormalMap != null)
            GUI.DrawTexture(new Rect(offsetX + spacingX * 2 + 5, offsetY + 25, 100, 100), NormalMap);

        // Paste 
        if (GUI.Button(new Rect(offsetX + spacingX * 2 + 5, offsetY + 130, 20, 20), "P"))
        {
            _activeMapType = MapType.Normal;
            PasteFile();
        }

        GUI.enabled = NormalMap != null;

        // Copy
        if (GUI.Button(new Rect(offsetX + spacingX * 2 + 30, offsetY + 130, 20, 20), "C"))
        {
            _textureToSave = NormalMap;
            CopyFile();
        }

        GUI.enabled = true;

        //Open
        if (GUI.Button(new Rect(offsetX + spacingX * 2 + 60, offsetY + 130, 20, 20), "O"))
            OpenTextureFile(MapType.Normal);

        GUI.enabled = NormalMap != null;

        // Save
        if (GUI.Button(new Rect(offsetX + spacingX * 2 + 85, offsetY + 130, 20, 20), "S"))
            SaveTextureFile(MapType.Normal);

        if (NormalMap == null || QuicksavePathNormal == "")
            GUI.enabled = false;
        else
            GUI.enabled = true;

        // Quick Save
        if (GUI.Button(new Rect(offsetX + spacingX * 2 + 15, offsetY + 160, 80, 20), "Quick Save"))
        {
            _textureToSave = NormalMap;
            SaveFile(QuicksavePathNormal);
        }

        GUI.enabled = NormalMap != null;

        if (GUI.Button(new Rect(offsetX + spacingX * 2 + 15, offsetY + 190, 80, 20), "Preview"))
            SetPreviewMaterial(NormalMap);

        GUI.enabled = HeightMap != null;

        if (GUI.Button(new Rect(offsetX + spacingX * 2 + 5, offsetY + 220, 50, 20), "Create"))
        {
            CloseWindows();
            FixSize();
            NormalFromHeightGuiObject.SetActive(true);
            _normalFromHeightGuiScript.NewTexture();
            _normalFromHeightGuiScript.DoStuff();
        }

        GUI.enabled = NormalMap != null;

        if (GUI.Button(new Rect(offsetX + spacingX * 2 + 60, offsetY + 220, 45, 20), "Clear"))
        {
            ClearTexture(MapType.Normal);
            CloseWindows();
            SetMaterialValues();
            FixSize();
        }

        GUI.enabled = true;


        //==============================//
        // 			Metallic Map		//
        //==============================//

        GUI.Box(new Rect(offsetX + spacingX * 3, offsetY, 110, 250), "Metallic Map");

        if (MetallicMap != null)
            GUI.DrawTexture(new Rect(offsetX + spacingX * 3 + 5, offsetY + 25, 100, 100), MetallicMap);

        // Paste 
        if (GUI.Button(new Rect(offsetX + spacingX * 3 + 5, offsetY + 130, 20, 20), "P"))
        {
            _activeMapType = MapType.Metallic;
            PasteFile();
        }

        GUI.enabled = MetallicMap != null;

        // Copy
        if (GUI.Button(new Rect(offsetX + spacingX * 3 + 30, offsetY + 130, 20, 20), "C"))
        {
            _textureToSave = MetallicMap;
            CopyFile();
        }

        GUI.enabled = true;

        //Open
        if (GUI.Button(new Rect(offsetX + spacingX * 3 + 60, offsetY + 130, 20, 20), "O"))
            OpenTextureFile(MapType.Metallic);

        GUI.enabled = MetallicMap != null;

        // Save
        if (GUI.Button(new Rect(offsetX + spacingX * 3 + 85, offsetY + 130, 20, 20), "S"))
            SaveTextureFile(MapType.Metallic);

        if (MetallicMap == null || QuicksavePathMetallic == "")
            GUI.enabled = false;
        else
            GUI.enabled = true;

        // Quick Save
        if (GUI.Button(new Rect(offsetX + spacingX * 3 + 15, offsetY + 160, 80, 20), "Quick Save"))
        {
            _textureToSave = MetallicMap;
            SaveFile(QuicksavePathMetallic);
        }

        GUI.enabled = MetallicMap != null;

        if (GUI.Button(new Rect(offsetX + spacingX * 3 + 15, offsetY + 190, 80, 20), "Preview"))
            SetPreviewMaterial(MetallicMap);

        if (DiffuseMapOriginal == null && DiffuseMap == null)
            GUI.enabled = false;
        else
            GUI.enabled = true;

        if (GUI.Button(new Rect(offsetX + spacingX * 3 + 5, offsetY + 220, 50, 20), "Create"))
        {
            CloseWindows();
            FixSize();

            MetallicGuiObject.SetActive(true);
            _metallicGuiScript.NewTexture();
            _metallicGuiScript.DoStuff();
        }

        GUI.enabled = MetallicMap != null;

        if (GUI.Button(new Rect(offsetX + spacingX * 3 + 60, offsetY + 220, 45, 20), "Clear"))
        {
            ClearTexture(MapType.Metallic);
            CloseWindows();
            SetMaterialValues();
            FixSize();
        }

        GUI.enabled = true;


        //==============================//
        // 		Smoothness Map			//
        //==============================//

        GUI.Box(new Rect(offsetX + spacingX * 4, offsetY, 110, 250), "Smoothness Map");

        if (SmoothnessMap != null)
            GUI.DrawTexture(new Rect(offsetX + spacingX * 4 + 5, offsetY + 25, 100, 100), SmoothnessMap);

        // Paste 
        if (GUI.Button(new Rect(offsetX + spacingX * 4 + 5, offsetY + 130, 20, 20), "P"))
        {
            _activeMapType = MapType.Smoothness;
            PasteFile();
        }

        GUI.enabled = SmoothnessMap != null;

        // Copy
        if (GUI.Button(new Rect(offsetX + spacingX * 4 + 30, offsetY + 130, 20, 20), "C"))
        {
            _textureToSave = SmoothnessMap;
            CopyFile();
        }

        GUI.enabled = true;

        //Open
        if (GUI.Button(new Rect(offsetX + spacingX * 4 + 60, offsetY + 130, 20, 20), "O"))
            OpenTextureFile(MapType.Smoothness);

        GUI.enabled = SmoothnessMap != null;

        // Save
        if (GUI.Button(new Rect(offsetX + spacingX * 4 + 85, offsetY + 130, 20, 20), "S"))
            SaveTextureFile(MapType.Smoothness);

        if (SmoothnessMap == null || QuicksavePathSmoothness == "")
            GUI.enabled = false;
        else
            GUI.enabled = true;

        // Quick Save
        if (GUI.Button(new Rect(offsetX + spacingX * 4 + 15, offsetY + 160, 80, 20), "Quick Save"))
        {
            _textureToSave = SmoothnessMap;
            SaveFile(QuicksavePathSmoothness);
        }

        GUI.enabled = SmoothnessMap != null;

        if (GUI.Button(new Rect(offsetX + spacingX * 4 + 15, offsetY + 190, 80, 20), "Preview"))
            SetPreviewMaterial(SmoothnessMap);

        if (DiffuseMapOriginal == null && DiffuseMap == null)
            GUI.enabled = false;
        else
            GUI.enabled = true;

        if (GUI.Button(new Rect(offsetX + spacingX * 4 + 5, offsetY + 220, 50, 20), "Create"))
        {
            CloseWindows();
            FixSize();
            SmoothnessGuiObject.SetActive(true);
            _smoothnessGuiScript.NewTexture();
            _smoothnessGuiScript.DoStuff();
        }

        GUI.enabled = SmoothnessMap != null;

        if (GUI.Button(new Rect(offsetX + spacingX * 4 + 60, offsetY + 220, 45, 20), "Clear"))
        {
            ClearTexture(MapType.Smoothness);
            CloseWindows();
            SetMaterialValues();
            FixSize();
        }

        GUI.enabled = true;


        //==============================//
        // 			Edge Map			//
        //==============================//

        GUI.Box(new Rect(offsetX + spacingX * 5, offsetY, 110, 250), "Edge Map");

        if (EdgeMap != null) GUI.DrawTexture(new Rect(offsetX + spacingX * 5 + 5, offsetY + 25, 100, 100), EdgeMap);

        // Paste 
        if (GUI.Button(new Rect(offsetX + spacingX * 5 + 5, offsetY + 130, 20, 20), "P"))
        {
            _activeMapType = MapType.Edge;
            PasteFile();
        }

        GUI.enabled = EdgeMap != null;

        // Copy
        if (GUI.Button(new Rect(offsetX + spacingX * 5 + 30, offsetY + 130, 20, 20), "C"))
        {
            _textureToSave = EdgeMap;
            CopyFile();
        }

        GUI.enabled = true;

        //Open
        if (GUI.Button(new Rect(offsetX + spacingX * 5 + 60, offsetY + 130, 20, 20), "O"))
            OpenTextureFile(MapType.Edge);

        GUI.enabled = EdgeMap != null;

        // Save
        if (GUI.Button(new Rect(offsetX + spacingX * 5 + 85, offsetY + 130, 20, 20), "S"))
            SaveTextureFile(MapType.Edge);

        if (EdgeMap == null || QuicksavePathEdge == "")
            GUI.enabled = false;
        else
            GUI.enabled = true;

        // Quick Save
        if (GUI.Button(new Rect(offsetX + spacingX * 5 + 15, offsetY + 160, 80, 20), "Quick Save"))
        {
            _textureToSave = EdgeMap;
            SaveFile(QuicksavePathEdge);
        }

        GUI.enabled = EdgeMap != null;

        if (GUI.Button(new Rect(offsetX + spacingX * 5 + 15, offsetY + 190, 80, 20), "Preview"))
            SetPreviewMaterial(EdgeMap);

        GUI.enabled = NormalMap != null;

        if (GUI.Button(new Rect(offsetX + spacingX * 5 + 5, offsetY + 220, 50, 20), "Create"))
        {
            CloseWindows();
            FixSize();
            EdgeFromNormalGuiObject.SetActive(true);
            _edgeFromNormalGuiScript.NewTexture();
            _edgeFromNormalGuiScript.DoStuff();
        }

        GUI.enabled = EdgeMap != null;

        if (GUI.Button(new Rect(offsetX + spacingX * 5 + 60, offsetY + 220, 45, 20), "Clear"))
        {
            ClearTexture(MapType.Edge);
            CloseWindows();
            SetMaterialValues();
            FixSize();
        }

        GUI.enabled = true;

        //==============================//
        // 			AO Map				//
        //==============================//

        GUI.Box(new Rect(offsetX + spacingX * 6, offsetY, 110, 250), "AO Map");

        if (AoMap != null) GUI.DrawTexture(new Rect(offsetX + spacingX * 6 + 5, offsetY + 25, 100, 100), AoMap);


        // Paste 
        if (GUI.Button(new Rect(offsetX + spacingX * 6 + 5, offsetY + 130, 20, 20), "P"))
        {
            _activeMapType = MapType.AO;
            PasteFile();
        }

        GUI.enabled = AoMap != null;

        // Copy
        if (GUI.Button(new Rect(offsetX + spacingX * 6 + 30, offsetY + 130, 20, 20), "C"))
        {
            _textureToSave = AoMap;
            CopyFile();
        }

        GUI.enabled = true;

        //Open
        if (GUI.Button(new Rect(offsetX + spacingX * 6 + 60, offsetY + 130, 20, 20), "O")) OpenTextureFile(MapType.AO);

        GUI.enabled = AoMap != null;

        // Save
        if (GUI.Button(new Rect(offsetX + spacingX * 6 + 85, offsetY + 130, 20, 20), "S")) SaveTextureFile(MapType.AO);

        if (AoMap == null || QuicksavePathAo == "")
            GUI.enabled = false;
        else
            GUI.enabled = true;

        // Quick Save
        if (GUI.Button(new Rect(offsetX + spacingX * 6 + 15, offsetY + 160, 80, 20), "Quick Save"))
        {
            _textureToSave = AoMap;
            SaveFile(QuicksavePathAo);
        }

        GUI.enabled = AoMap != null;

        if (GUI.Button(new Rect(offsetX + spacingX * 6 + 15, offsetY + 190, 80, 20), "Preview"))
            SetPreviewMaterial(AoMap);

        if (NormalMap == null && HeightMap == null)
            GUI.enabled = false;
        else
            GUI.enabled = true;

        if (GUI.Button(new Rect(offsetX + spacingX * 6 + 5, offsetY + 220, 50, 20), "Create"))
        {
            CloseWindows();
            FixSize();
            AoFromNormalGuiObject.SetActive(true);
            _aoFromNormalGuiScript.NewTexture();
            _aoFromNormalGuiScript.DoStuff();
        }

        GUI.enabled = AoMap != null;

        if (GUI.Button(new Rect(offsetX + spacingX * 6 + 60, offsetY + 220, 45, 20), "Clear"))
        {
            ClearTexture(MapType.AO);
            CloseWindows();
            SetMaterialValues();
            FixSize();
        }

        GUI.enabled = true;


        //==============================//
        // 		Map Saving Options		//
        //==============================//

        offsetX = offsetX + spacingX * 7;

        GUI.Box(new Rect(offsetX, offsetY, 230, 250), "Saving Options");

        GUI.Label(new Rect(offsetX + 20, offsetY + 20, 100, 25), "File Format");

        _pngSelected = GUI.Toggle(new Rect(offsetX + 30, offsetY + 60, 80, 20), _pngSelected, "PNG");
        if (_pngSelected) SetFormat(FileFormat.png);

        _jpgSelected = GUI.Toggle(new Rect(offsetX + 30, offsetY + 80, 80, 20), _jpgSelected, "JPG");
        if (_jpgSelected) SetFormat(FileFormat.jpg);

        _tgaSelected = GUI.Toggle(new Rect(offsetX + 30, offsetY + 100, 80, 20), _tgaSelected, "TGA");
        if (_tgaSelected) SetFormat(FileFormat.tga);

        _exrSelected = GUI.Toggle(new Rect(offsetX + 30, offsetY + 120, 80, 20), _exrSelected, "EXR");
        if (_exrSelected) SetFormat(FileFormat.exr);

        // Flip Normal Map Y
        GUI.enabled = NormalMap != null;

        if (GUI.Button(new Rect(offsetX + 10, offsetY + 145, 100, 25), "Flip Normal Y")) FlipNormalMapY();

        GUI.enabled = true;

        //Save Project
        if (GUI.Button(new Rect(offsetX + 10, offsetY + 180, 100, 25), "Save Project"))
        {
            var defaultName = "baseName.mtz";
            var path = StandaloneFileBrowser.SaveFilePanel("Save Project", _lastDirectory, defaultName, "mtz");
            if (path.IsNullOrEmpty()) return;

            var lastBar = path.LastIndexOf(_pathChar);
            _lastDirectory = path.Substring(0, lastBar + 1);

            _saveLoadProjectScript.SaveProject(path, SelectedFormat);
        }

        //Load Project
        if (GUI.Button(new Rect(offsetX + 10, offsetY + 215, 100, 25), "Load Project"))
        {
            var path = StandaloneFileBrowser.OpenFilePanel("Load Project", _lastDirectory, "mtz", false);
            if (path[0].IsNullOrEmpty()) return;

            var lastBar = path[0].LastIndexOf(_pathChar);
            _lastDirectory = path[0].Substring(0, lastBar + 1);

            _saveLoadProjectScript.LoadProject(path[0]);
        }

        //======================================//
        //			Property Map Settings		//
        //======================================//

        GUI.Label(new Rect(offsetX + 130, offsetY + 20, 100, 25), "Property Map");

        if (_propRedChoose)
            GUI.enabled = false;
        else
            GUI.enabled = true;

        GUI.Label(new Rect(offsetX + 100, offsetY + 45, 20, 20), "R:");
        if (GUI.Button(new Rect(offsetX + 120, offsetY + 45, 100, 25), PCM2String(PropRed, "Red None")))
        {
            _propRedChoose = true;
            _propGreenChoose = false;
            _propBlueChoose = false;
        }

        GUI.enabled = !_propGreenChoose;

        GUI.Label(new Rect(offsetX + 100, offsetY + 80, 20, 20), "G:");
        if (GUI.Button(new Rect(offsetX + 120, offsetY + 80, 100, 25), PCM2String(PropGreen, "Green None")))
        {
            _propRedChoose = false;
            _propGreenChoose = true;
            _propBlueChoose = false;
        }

        GUI.enabled = !_propBlueChoose;

        GUI.Label(new Rect(offsetX + 100, offsetY + 115, 20, 20), "B:");
        if (GUI.Button(new Rect(offsetX + 120, offsetY + 115, 100, 25), PCM2String(PropBlue, "Blue None")))
        {
            _propRedChoose = false;
            _propGreenChoose = false;
            _propBlueChoose = true;
        }

        GUI.enabled = true;

        var propBoxOffsetX = offsetX + 250;
        var propBoxOffsetY = 20;
        if (_propRedChoose || _propGreenChoose || _propBlueChoose)
        {
            GUI.Box(new Rect(propBoxOffsetX, propBoxOffsetY, 150, 245), "Map for Channel");
            var chosen = false;
            var chosenPCM = PropChannelMap.None;

            if (GUI.Button(new Rect(propBoxOffsetX + 10, propBoxOffsetY + 30, 130, 25), "None"))
            {
                chosen = true;
                chosenPCM = PropChannelMap.None;
            }

            if (GUI.Button(new Rect(propBoxOffsetX + 10, propBoxOffsetY + 60, 130, 25), "Height"))
            {
                chosen = true;
                chosenPCM = PropChannelMap.Height;
            }

            if (GUI.Button(new Rect(propBoxOffsetX + 10, propBoxOffsetY + 90, 130, 25), "Metallic"))
            {
                chosen = true;
                chosenPCM = PropChannelMap.Metallic;
            }

            if (GUI.Button(new Rect(propBoxOffsetX + 10, propBoxOffsetY + 120, 130, 25), "Smoothness"))
            {
                chosen = true;
                chosenPCM = PropChannelMap.Smoothness;
            }

            if (GUI.Button(new Rect(propBoxOffsetX + 10, propBoxOffsetY + 150, 130, 25), "Edge"))
            {
                chosen = true;
                chosenPCM = PropChannelMap.Edge;
            }

            if (GUI.Button(new Rect(propBoxOffsetX + 10, propBoxOffsetY + 180, 130, 25), "Ambient Occlusion"))
            {
                chosen = true;
                chosenPCM = PropChannelMap.Ao;
            }

            if (GUI.Button(new Rect(propBoxOffsetX + 10, propBoxOffsetY + 210, 130, 25), "AO + Edge"))
            {
                chosen = true;
                chosenPCM = PropChannelMap.AoEdge;
            }

            if (chosen)
            {
                if (_propRedChoose) PropRed = chosenPCM;

                if (_propGreenChoose) PropGreen = chosenPCM;

                if (_propBlueChoose) PropBlue = chosenPCM;

                _propRedChoose = false;
                _propGreenChoose = false;
                _propBlueChoose = false;
            }
        }

        if (GUI.Button(new Rect(offsetX + 120, offsetY + 150, 100, 40), "Save\r\nProperty Map"))
        {
            ProcessPropertyMap();
            SaveTextureFile(MapType.Property);
        }

        if (QuicksavePathProperty == "") GUI.enabled = false;

        if (GUI.Button(new Rect(offsetX + 120, offsetY + 200, 100, 40), "Quick Save\r\nProperty Map"))
        {
            ProcessPropertyMap();
            _textureToSave = PropertyMap;
            SaveFile(QuicksavePathProperty);
        }

        GUI.enabled = true;


        //==========================//
        // 		View Buttons		//
        //==========================//

        offsetX = 430;
        offsetY = 280;

        if (GUI.Button(new Rect(offsetX, offsetY, 100, 40), "Post Process"))
        {
            PostProcessGuiObject.SetActive(!PostProcessGuiObject.activeSelf);
        }

        offsetX += 110;

        if (GUI.Button(new Rect(offsetX, offsetY, 80, 40), "Show Full\r\nMaterial"))
        {
            CloseWindows();
            FixSize();
            MaterialGuiObject.SetActive(true);
            _materialGuiScript.Initialize();
        }

        offsetX += 90;

        if (GUI.Button(new Rect(offsetX, offsetY, 80, 40), "Next\r\nCube Map"))
        {
            _selectedCubemap += 1;
            if (_selectedCubemap >= CubeMaps.Length) _selectedCubemap = 0;

            //skyboxMaterial.SetTexture ("_Tex", CubeMaps[selectedCubemap] );
            Shader.SetGlobalTexture("_GlobalCubemap", CubeMaps[_selectedCubemap]);
            ReflectionProbe.RenderProbe();
        }

        offsetX += 90;

        GUI.enabled = HeightMap != null;

        if (GUI.Button(new Rect(offsetX, offsetY, 60, 40), "Tile\r\nMaps"))
        {
            CloseWindows();
            FixSize();
            TilingTextureMakerGuiObject.SetActive(true);
            _tilingTextureMakerGuiScript.Initialize();
        }

        GUI.enabled = true;

        offsetX += 70;

        if (HeightMap == null && DiffuseMapOriginal == null && MetallicMap == null && SmoothnessMap == null &&
            EdgeMap == null && AoMap == null)
            GUI.enabled = false;
        else
            GUI.enabled = true;

        if (GUI.Button(new Rect(offsetX, offsetY, 90, 40), "Adjust\r\nAlignment"))
        {
            CloseWindows();
            FixSize();
            //AlignmentGuiScript.gameObject.SetActive(true);
            AlignmentGuiScript.Initialize();
        }

        GUI.enabled = true;

        offsetX += 100;

        if (GUI.Button(new Rect(offsetX, offsetY, 120, 40), "Clear All\r\nTexture Maps")) _clearTextures = true;

        if (_clearTextures)
        {
            offsetY += 60;

            GUI.Box(new Rect(offsetX, offsetY, 120, 60), "Are You Sure?");

            if (GUI.Button(new Rect(offsetX + 10, offsetY + 30, 45, 20), "Yes"))
            {
                _clearTextures = false;
                ClearAllTextures();
                CloseWindows();
                SetMaterialValues();
                FixSizeSize(1024.0f, 1024.0f);
            }

            if (GUI.Button(new Rect(offsetX + 65, offsetY + 30, 45, 20), "No")) _clearTextures = false;
        }

        GUI.enabled = true;
    }

    private void SaveTextureFile(MapType mapType)
    {
        _textureToSave = HeightMap;
        var defaultName = "_" + mapType + ".png";
        var path = StandaloneFileBrowser.SaveFilePanel("Save Height Map", _lastDirectory, defaultName,
            _imageLoadFilter);
        if (path.IsNullOrEmpty()) return;

        _textureToSave = GetTextureToSave(mapType);
        var lastBar = path.LastIndexOf(_pathChar);
        _lastDirectory = path.Substring(0, lastBar + 1);
        SaveFile(path);
    }

    private Texture2D GetTextureToSave(MapType mapType)
    {
        switch (mapType)
        {
            case MapType.Height:
                return HeightMap;
            case MapType.Diffuse:
                return DiffuseMap != null ? DiffuseMap : DiffuseMapOriginal;
            case MapType.DiffuseOriginal:
                return DiffuseMapOriginal;
            case MapType.Metallic:
                return MetallicMap;
            case MapType.Smoothness:
                return SmoothnessMap;
            case MapType.Normal:
                return NormalMap;
            case MapType.Edge:
                return EdgeMap;
            case MapType.AO:
                return AoMap;
            case MapType.Property:
                return PropertyMap;
            default:
                throw new ArgumentOutOfRangeException(nameof(mapType), mapType, null);
        }
    }

    private void OpenTextureFile(MapType mapType)
    {
        _activeMapType = mapType;
        var title = "Open " + mapType + " Map";
        var path = StandaloneFileBrowser.OpenFilePanel(title, _lastDirectory, _imageLoadFilter, false);
        if (path[0].IsNullOrEmpty()) return;
        var lastBar = path[0].LastIndexOf(_pathChar);
        _lastDirectory = path[0].Substring(0, lastBar + 1);
        OpenFile(path[0]);
    }

    // ReSharper disable once InconsistentNaming
    private static string PCM2String(PropChannelMap pcm, string defaultName)
    {
        var returnString = defaultName;

        switch (pcm)
        {
            case PropChannelMap.Height:
                returnString = "Height";
                break;
            case PropChannelMap.Metallic:
                returnString = "Metallic";
                break;
            case PropChannelMap.Smoothness:
                returnString = "Smoothness";
                break;
            case PropChannelMap.Edge:
                returnString = "Edge";
                break;
            case PropChannelMap.Ao:
                returnString = "Ambient Occ";
                break;
            case PropChannelMap.AoEdge:
                returnString = "AO + Edge";
                break;
        }

        return returnString;
    }

    public void FlipNormalMapY()
    {
        if (NormalMap == null) return;
        for (var i = 0; i < NormalMap.width; i++)
        for (var j = 0; j < NormalMap.height; j++)
        {
            var pixelColor = NormalMap.GetPixel(i, j);
            pixelColor.g = 1.0f - pixelColor.g;
            NormalMap.SetPixel(i, j, pixelColor);
        }

        NormalMap.Apply();
    }

    private void ClearTexture(Texture2D textureToClear)
    {
        if (textureToClear)
        {
            Destroy(textureToClear);
            textureToClear = null;
        }

        Resources.UnloadUnusedAssets();
    }

    private void ClearTexture(MapType mapType)
    {
        switch (mapType)
        {
            case MapType.Height:
                if (HeightMap)
                {
                    Destroy(HeightMap);
                    HeightMap = null;
                }

                if (HdHeightMap)
                {
                    Destroy(HdHeightMap);
                    HdHeightMap = null;
                }

                break;
            case MapType.Diffuse:
                if (DiffuseMap)
                {
                    Destroy(DiffuseMap);
                    DiffuseMap = null;
                }

                if (DiffuseMapOriginal)
                {
                    Destroy(DiffuseMapOriginal);
                    DiffuseMapOriginal = null;
                }

                break;
            case MapType.Normal:
                if (NormalMap)
                {
                    Destroy(NormalMap);
                    NormalMap = null;
                }

                break;
            case MapType.Metallic:
                if (MetallicMap)
                {
                    Destroy(MetallicMap);
                    MetallicMap = null;
                }

                break;
            case MapType.Smoothness:
                if (SmoothnessMap)
                {
                    Destroy(SmoothnessMap);
                    SmoothnessMap = null;
                }

                break;
            case MapType.Edge:
                if (EdgeMap)
                {
                    Destroy(EdgeMap);
                    EdgeMap = null;
                }

                break;
            case MapType.AO:
                if (AoMap)
                {
                    Destroy(AoMap);
                    AoMap = null;
                }

                break;
            case MapType.DiffuseOriginal:
                if (DiffuseMapOriginal)
                {
                    Destroy(DiffuseMapOriginal);
                    DiffuseMapOriginal = null;
                }
                break;
            case MapType.Property:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mapType), mapType, null);
        }

        Resources.UnloadUnusedAssets();
    }

    public void ClearAllTextures()
    {
        ClearTexture(MapType.Height);
        ClearTexture(MapType.Diffuse);
        ClearTexture(MapType.Normal);
        ClearTexture(MapType.Metallic);
        ClearTexture(MapType.Smoothness);
        ClearTexture(MapType.Edge);
        ClearTexture(MapType.AO);
    }

    public void SetFormat(FileFormat newFormat)
    {
        _jpgSelected = false;
        _pngSelected = false;
        _tgaSelected = false;
        _exrSelected = false;

        switch (newFormat)
        {
            case FileFormat.jpg:
                _jpgSelected = true;
                break;
            case FileFormat.png:
                _pngSelected = true;
                break;
            case FileFormat.tga:
                _tgaSelected = true;
                break;
            case FileFormat.exr:
                _exrSelected = true;
                break;
        }

        SelectedFormat = newFormat;
    }

    public void SetFormat(string newFormat)
    {
        _jpgSelected = false;
        _pngSelected = false;
        _tgaSelected = false;
        _exrSelected = false;

        switch (newFormat)
        {
            case "jpg":
                _jpgSelected = true;
                SelectedFormat = FileFormat.jpg;
                break;
            case "png":
                _pngSelected = true;
                SelectedFormat = FileFormat.png;
                break;
            case "tga":
                _tgaSelected = true;
                SelectedFormat = FileFormat.tga;
                break;
            case "exr":
                _exrSelected = true;
                SelectedFormat = FileFormat.exr;
                break;
        }
    }

    public void SetLoadedTexture(MapType loadedTexture)
    {

        switch (loadedTexture)
        {
            case MapType.Height:
                SetPreviewMaterial(HeightMap);
                break;
            case MapType.Diffuse:
                SetPreviewMaterial(DiffuseMap);
                break;
            case MapType.DiffuseOriginal:
                SetPreviewMaterial(DiffuseMapOriginal);
                break;
            case MapType.Normal:
                SetPreviewMaterial(NormalMap);
                break;
            case MapType.Metallic:
                SetPreviewMaterial(MetallicMap);
                break;
            case MapType.Smoothness:
                SetPreviewMaterial(SmoothnessMap);
                break;
            case MapType.Edge:
                SetPreviewMaterial(EdgeMap);
                break;
            case MapType.AO:
                SetPreviewMaterial(AoMap);
                break;
        }

        FixSize();
    }

    //==================================================//
    //					Property Map					//
    //==================================================//

    private void SetPropertyTexture(string texPrefix, Texture2D texture, Texture2D overlayTexture)
    {
        _propertyCompMaterial.SetTexture(texPrefix + "Tex", texture != null ? texture : TextureBlack);

        _propertyCompMaterial.SetTexture(texPrefix + "OverlayTex", overlayTexture);
    }

    private void SetPropertyMapChannel(string texPrefix, PropChannelMap pcm)
    {
        switch (pcm)
        {
            case PropChannelMap.Height:
                SetPropertyTexture(texPrefix, HeightMap, TextureGrey);
                break;
            case PropChannelMap.Metallic:
                SetPropertyTexture(texPrefix, MetallicMap, TextureGrey);
                break;
            case PropChannelMap.Smoothness:
                SetPropertyTexture(texPrefix, SmoothnessMap, TextureGrey);
                break;
            case PropChannelMap.Edge:
                SetPropertyTexture(texPrefix, EdgeMap, TextureGrey);
                break;
            case PropChannelMap.Ao:
                SetPropertyTexture(texPrefix, AoMap, TextureGrey);
                break;
            case PropChannelMap.AoEdge:
                SetPropertyTexture(texPrefix, AoMap, EdgeMap);
                break;
            case PropChannelMap.None:
                SetPropertyTexture(texPrefix, TextureBlack, TextureGrey);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(pcm), pcm, null);
        }
    }

    public void ProcessPropertyMap()
    {
        SetPropertyMapChannel("_Red", PropRed);
        SetPropertyMapChannel("_Green", PropGreen);
        SetPropertyMapChannel("_Blue", PropBlue);

        var size = GetSize();
        var tempMap = RenderTexture.GetTemporary((int) size.x, (int) size.y, 0, RenderTextureFormat.ARGB32,
            RenderTextureReadWrite.Default);
        Graphics.Blit(MetallicMap, tempMap, _propertyCompMaterial, 0);
        RenderTexture.active = tempMap;

        if (PropertyMap != null)
        {
            Destroy(PropertyMap);
            PropertyMap = null;
        }

        PropertyMap = new Texture2D(tempMap.width, tempMap.height, TextureFormat.RGB24, false);
        PropertyMap.ReadPixels(new Rect(0, 0, tempMap.width, tempMap.height), 0, 0);
        PropertyMap.Apply();

        RenderTexture.ReleaseTemporary(tempMap);
        tempMap = null;
    }

    //==================================================//
    //					Project Saving					//
    //==================================================//

    private void LoadProject(string pathToFile)
    {
        _saveLoadProjectScript.LoadProject(pathToFile);
    }

    private void SaveFile(string pathToFile)
    {
        _saveLoadProjectScript.SaveFile(pathToFile, _textureToSave);
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
    private void CopyFile()
    {
#if UNITY_STANDALONE_WIN
        SaveLoadProjectScript.CopyFile(_textureToSave);
#endif
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
    private void PasteFile()
    {
#if UNITY_STANDALONE_WIN
        ClearTexture(_activeMapType);
        SaveLoadProjectScript.PasteFile(_activeMapType);
#endif
    }

    private void OpenFile(string pathToFile)
    {
        if (pathToFile == null) return;

        // clear the existing texture we are loading
        ClearTexture(_activeMapType);

        StartCoroutine(_saveLoadProjectScript.LoadTexture(_activeMapType, pathToFile));
    }

    //==================================================//
    //			Fix the size of the test model			//
    //==================================================//


    private Vector2 GetSize()
    {
        Texture2D mapToUse = null;

        var size = new Vector2(1024, 1024);

        if (HeightMap != null)
            mapToUse = HeightMap;
        else if (DiffuseMap != null)
            mapToUse = DiffuseMap;
        else if (DiffuseMapOriginal != null)
            mapToUse = DiffuseMapOriginal;
        else if (NormalMap != null)
            mapToUse = NormalMap;
        else if (MetallicMap != null)
            mapToUse = MetallicMap;
        else if (SmoothnessMap != null)
            mapToUse = SmoothnessMap;
        else if (EdgeMap != null)
            mapToUse = EdgeMap;
        else if (AoMap != null) mapToUse = AoMap;

        if (mapToUse == null) return size;
        size.x = mapToUse.width;
        size.y = mapToUse.height;

        return size;
    }

    public void FixSize()
    {
        var size = GetSize();
        FixSizeSize(size.x, size.y);
    }

    private void FixSizeMap(Texture mapToUse)
    {
        FixSizeSize(mapToUse.width, mapToUse.height);
    }

    private void FixSizeMap(RenderTexture mapToUse)
    {
        FixSizeSize(mapToUse.width, mapToUse.height);
    }

    private void FixSizeSize(float width, float height)
    {
        var testObjectScale = new Vector3(1, 1, 1);
        const float area = 1.0f;

        testObjectScale.x = width / height;

        var newArea = testObjectScale.x * testObjectScale.y;
        var areaScale = Mathf.Sqrt(area / newArea);

        testObjectScale.x *= areaScale;
        testObjectScale.y *= areaScale;

        TestObject.transform.localScale = testObjectScale;
    }
}