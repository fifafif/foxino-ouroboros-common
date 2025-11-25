using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;

public class TextureChannelCombinerEditor : EditorWindow
{
    private Texture2D baseTexture;
    private Texture2D emissionTexture;
    private Texture2D aoTexture;
    private Texture2D outputTexture;
    private float aoStrength = 1f;

    private ObjectField baseTextureField;
    private ObjectField emissionTextureField;
    private ObjectField aoTextureField;
    private Slider aoStrengthSlider;
    private FloatField aoStrengthField;
    private VisualElement aoStrengthContainer;
    private Button combineButton;
    private Button saveButton;
    private VisualElement previewContainer;
    private Image previewImage;
    private HelpBox statusHelpBox;
    private HelpBox outputHelpBox;

    [MenuItem("Ouroboros/Utils/Texture Channel Combiner")]
    public static void ShowWindow()
    {
        var window = GetWindow<TextureChannelCombinerEditor>("Texture Channel Combiner");
        window.minSize = new Vector2(400, 500);
    }

    private void CreateGUI()
    {
        // Root container
        var root = rootVisualElement;
        root.style.paddingTop = 10;
        root.style.paddingBottom = 10;
        root.style.paddingLeft = 10;
        root.style.paddingRight = 10;

        // Title
        var title = new Label("Texture Channel Combiner");
        title.style.fontSize = 16;
        title.style.unityFontStyleAndWeight = FontStyle.Bold;
        title.style.marginBottom = 10;
        root.Add(title);

        // Info help box
        var infoBox = new HelpBox("Combines a base texture with emission (additive) and/or ambient occlusion (multiplicative). Either AO or Emission must be provided.", HelpBoxMessageType.Info);
        infoBox.style.marginBottom = 10;
        root.Add(infoBox);

        // Input section
        var inputLabel = new Label("Input Textures");
        inputLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        inputLabel.style.marginBottom = 5;
        root.Add(inputLabel);

        // Base texture field
        baseTextureField = new ObjectField("Base Texture");
        baseTextureField.objectType = typeof(Texture2D);
        baseTextureField.allowSceneObjects = false;
        baseTextureField.style.marginBottom = 5;
        baseTextureField.RegisterValueChangedCallback(evt =>
        {
            baseTexture = evt.newValue as Texture2D;
            UpdateCombineButtonState();
        });
        root.Add(baseTextureField);

        // Emission texture field
        emissionTextureField = new ObjectField("Emission Texture");
        emissionTextureField.objectType = typeof(Texture2D);
        emissionTextureField.allowSceneObjects = false;
        emissionTextureField.style.marginBottom = 5;
        emissionTextureField.RegisterValueChangedCallback(evt =>
        {
            emissionTexture = evt.newValue as Texture2D;
            UpdateCombineButtonState();
        });
        root.Add(emissionTextureField);

        // AO texture field
        aoTextureField = new ObjectField("Ambient Occlusion Texture");
        aoTextureField.objectType = typeof(Texture2D);
        aoTextureField.allowSceneObjects = false;
        aoTextureField.style.marginBottom = 5;
        aoTextureField.RegisterValueChangedCallback(evt =>
        {
            aoTexture = evt.newValue as Texture2D;
            UpdateCombineButtonState();
            UpdateAOStrengthVisibility();
        });
        root.Add(aoTextureField);

        // AO strength container (hidden by default)
        aoStrengthContainer = new VisualElement();
        aoStrengthContainer.style.marginBottom = 10;
        aoStrengthContainer.style.display = DisplayStyle.None;
        root.Add(aoStrengthContainer);

        // Label for AO strength
        var aoStrengthLabel = new Label("AO Strength");
        aoStrengthLabel.style.marginBottom = 2;
        aoStrengthContainer.Add(aoStrengthLabel);

        // Horizontal container for slider and field
        var aoStrengthRow = new VisualElement();
        aoStrengthRow.style.flexDirection = FlexDirection.Row;
        aoStrengthRow.style.alignItems = Align.Center;
        aoStrengthRow.style.minHeight = 20; // Maintain minimum height
        aoStrengthContainer.Add(aoStrengthRow);

        // Slider (takes most of the space)
        aoStrengthSlider = new Slider(0f, 1f);
        aoStrengthSlider.value = aoStrength;
        aoStrengthSlider.style.flexGrow = 1;
        aoStrengthSlider.style.minHeight = 20; // Prevent slider from shrinking vertically
        aoStrengthSlider.style.marginRight = 5;
        aoStrengthSlider.RegisterValueChangedCallback(evt =>
        {
            aoStrength = evt.newValue;
            aoStrengthField.SetValueWithoutNotify(aoStrength);
        });
        aoStrengthRow.Add(aoStrengthSlider);

        // Float field (fixed width)
        aoStrengthField = new FloatField();
        aoStrengthField.value = aoStrength;
        aoStrengthField.style.width = 60;
        aoStrengthField.style.minHeight = 20; // Prevent field from shrinking vertically
        aoStrengthField.RegisterValueChangedCallback(evt =>
        {
            aoStrength = Mathf.Clamp(evt.newValue, 0f, 1f);
            aoStrengthSlider.SetValueWithoutNotify(aoStrength);
            aoStrengthField.SetValueWithoutNotify(aoStrength); // Clamp the displayed value
        });
        aoStrengthRow.Add(aoStrengthField);

        // Combine button
        combineButton = new Button(CombineTextures);
        combineButton.text = "Combine Textures";
        combineButton.style.height = 30;
        combineButton.style.marginBottom = 10;
        combineButton.SetEnabled(false);
        root.Add(combineButton);

        // Output section
        var outputLabel = new Label("Output");
        outputLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        outputLabel.style.marginBottom = 5;
        root.Add(outputLabel);

        // Preview container
        previewContainer = new VisualElement();
        previewContainer.style.marginBottom = 10;
        previewContainer.style.display = DisplayStyle.None;
        root.Add(previewContainer);

        var previewLabel = new Label("Preview:");
        previewLabel.style.marginBottom = 5;
        previewContainer.Add(previewLabel);

        // Preview image
        previewImage = new Image();
        previewImage.style.width = 256;
        previewImage.style.height = 256;
        previewImage.style.marginBottom = 10;
        previewImage.scaleMode = ScaleMode.ScaleToFit;
        previewContainer.Add(previewImage);

        // Save button
        saveButton = new Button(SaveTexture);
        saveButton.text = "Save As...";
        saveButton.style.height = 30;
        saveButton.style.marginBottom = 10;
        previewContainer.Add(saveButton);

        // Output help box (shown when no output)
        outputHelpBox = new HelpBox("No output texture generated yet. Select base texture and at least one of Emission or AO texture, then click 'Combine Textures'.", HelpBoxMessageType.Warning);
        outputHelpBox.style.marginBottom = 10;
        root.Add(outputHelpBox);

        // Status help box
        statusHelpBox = new HelpBox("", HelpBoxMessageType.Info);
        statusHelpBox.style.marginBottom = 10;
        statusHelpBox.style.display = DisplayStyle.None;
        root.Add(statusHelpBox);
    }

    private void UpdateCombineButtonState()
    {
        // Base texture is required, and at least one of emission or AO must be present
        bool hasBaseTexture = baseTexture != null;
        bool hasEmissionOrAO = emissionTexture != null || aoTexture != null;
        combineButton.SetEnabled(hasBaseTexture && hasEmissionOrAO);
    }

    private void UpdateAOStrengthVisibility()
    {
        // Show AO strength container only when AO texture is assigned
        aoStrengthContainer.style.display = aoTexture != null ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void UpdateOutputDisplay(bool hasOutput)
    {
        if (hasOutput)
        {
            previewContainer.style.display = DisplayStyle.Flex;
            outputHelpBox.style.display = DisplayStyle.None;
        }
        else
        {
            previewContainer.style.display = DisplayStyle.None;
            outputHelpBox.style.display = DisplayStyle.Flex;
        }
    }

    private void SetStatus(string message, bool isError = false)
    {
        if (string.IsNullOrEmpty(message))
        {
            statusHelpBox.style.display = DisplayStyle.None;
        }
        else
        {
            statusHelpBox.text = message;
            statusHelpBox.messageType = isError ? HelpBoxMessageType.Error : HelpBoxMessageType.Info;
            statusHelpBox.style.display = DisplayStyle.Flex;
        }
    }

    private void CombineTextures()
    {
        if (baseTexture == null)
        {
            SetStatus("Error: Base texture is required.", true);
            return;
        }

        if (emissionTexture == null && aoTexture == null)
        {
            SetStatus("Error: At least one of Emission or Ambient Occlusion texture is required.", true);
            return;
        }

        // Use the base texture dimensions
        int width = baseTexture.width;
        int height = baseTexture.height;

        // Make textures readable (works for both readable and non-readable textures)
        Texture2D readableBase = MakeTextureReadable(baseTexture, width, height);
        Texture2D readableEmission = emissionTexture != null ? MakeTextureReadable(emissionTexture, width, height) : null;
        Texture2D readableAO = aoTexture != null ? MakeTextureReadable(aoTexture, width, height) : null;

        // Get pixels from base texture
        Color[] basePixels = readableBase.GetPixels();
        Color[] emissionPixels = readableEmission?.GetPixels();
        Color[] aoPixels = readableAO?.GetPixels();

        // Create output texture
        outputTexture = new Texture2D(width, height, TextureFormat.RGBA32, true);

        // Combine pixels
        Color[] combinedPixels = new Color[basePixels.Length];
        for (int i = 0; i < basePixels.Length; i++)
        {
            Color resultColor = basePixels[i];

            // Apply AO multiplication if present (using grayscale value)
            if (aoPixels != null)
            {
                float aoValue = aoPixels[i].grayscale;
                // Lerp between full darkening and no effect based on aoStrength
                float aoMultiplier = Mathf.Lerp(1f, aoValue, aoStrength);
                resultColor.r *= aoMultiplier;
                resultColor.g *= aoMultiplier;
                resultColor.b *= aoMultiplier;
            }

            // Add emission if present
            if (emissionPixels != null)
            {
                resultColor.r = Mathf.Clamp01(resultColor.r + emissionPixels[i].r);
                resultColor.g = Mathf.Clamp01(resultColor.g + emissionPixels[i].g);
                resultColor.b = Mathf.Clamp01(resultColor.b + emissionPixels[i].b);
            }

            // Keep base alpha
            resultColor.a = basePixels[i].a;
            combinedPixels[i] = resultColor;
        }

        // Apply combined pixels to output texture
        outputTexture.SetPixels(combinedPixels);
        outputTexture.Apply();

        // Clean up temporary textures
        if (readableBase != baseTexture) DestroyImmediate(readableBase);
        if (readableEmission != null && readableEmission != emissionTexture) DestroyImmediate(readableEmission);
        if (readableAO != null && readableAO != aoTexture) DestroyImmediate(readableAO);

        // Update UI
        previewImage.image = outputTexture;
        UpdateOutputDisplay(true);

        // Build status message
        string operationsApplied = "";
        if (aoTexture != null) operationsApplied += $"AO (strength: {aoStrength:F2})";
        if (aoTexture != null && emissionTexture != null) operationsApplied += " + ";
        if (emissionTexture != null) operationsApplied += "Emission";

        SetStatus($"Successfully combined textures! Output size: {width}x{height}. Applied: {operationsApplied}");
    }

    private void SaveTexture()
    {
        if (outputTexture == null)
        {
            SetStatus("Error: No output texture to save.", true);
            return;
        }

        // Get the base texture's directory
        string defaultDirectory = "Assets";
        string defaultName = "CombinedTexture";

        if (baseTexture != null)
        {
            string baseTexturePath = AssetDatabase.GetAssetPath(baseTexture);
            if (!string.IsNullOrEmpty(baseTexturePath))
            {
                defaultDirectory = System.IO.Path.GetDirectoryName(baseTexturePath);

                // Generate a name based on the base texture
                string baseTextureName = System.IO.Path.GetFileNameWithoutExtension(baseTexturePath);
                defaultName = $"{baseTextureName}_combined";
            }
        }

        // Open save dialog
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Combined Texture",
            defaultName,
            "png",
            "Choose where to save the combined texture",
            defaultDirectory
        );

        if (string.IsNullOrEmpty(path))
        {
            return; // User cancelled
        }

        // Encode to PNG
        byte[] bytes = outputTexture.EncodeToPNG();

        // Write to file
        File.WriteAllBytes(path, bytes);

        // Refresh asset database
        AssetDatabase.Refresh();

        // Import the texture with proper settings
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Default;
            importer.isReadable = true;
            importer.SaveAndReimport();
        }

        SetStatus($"Texture saved successfully to: {path}");

        // Ping the asset in the project window
        Object savedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        EditorGUIUtility.PingObject(savedTexture);
    }

    private Texture2D MakeTextureReadable(Texture2D texture, int width = -1, int height = -1)
    {
        if (texture == null) return null;

        // Use texture dimensions if not specified
        if (width <= 0) width = texture.width;
        if (height <= 0) height = texture.height;

        // Create a temporary RenderTexture
        RenderTexture rt = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);

        // Blit the texture to the RenderTexture
        Graphics.Blit(texture, rt);

        // Save the current active RenderTexture
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        // Create a new readable Texture2D and read pixels from the RenderTexture
        Texture2D readableTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        readableTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        readableTexture.Apply();

        // Restore the previous active RenderTexture
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return readableTexture;
    }
}
