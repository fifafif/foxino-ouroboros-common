using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ouroboros.Common.Utils.Transforms
{
    public class TransformCopyWindow : EditorWindow
    {
        private static TransformCopyWindow window;
        private static bool isCopyingMode;
        private static GameObject sourceObject;
        private static Transform sourceTransform;

        private Button startButton;
        private Button stopButton;
        private VisualElement modeActiveContainer;
        private VisualElement modeInactiveContainer;
        private Label sourceNameLabel;
        private Label positionLabel;
        private Label rotationLabel;
        private Label scaleLabel;
        private Label waitingLabel;

        [MenuItem("Ouroboros/Utils/Transform Copy Tool")]
        public static void ShowWindow()
        {
            window = GetWindow<TransformCopyWindow>("Transform Copy");
            window.minSize = new Vector2(300, 200);
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            SceneView.duringSceneGui -= OnSceneGUI;
            StopCopying();
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            root.style.paddingTop = 10;
            root.style.paddingBottom = 10;
            root.style.paddingLeft = 10;
            root.style.paddingRight = 10;

            // Title
            var title = new Label("Transform Copy Tool");
            title.style.fontSize = 16;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.marginBottom = 10;
            root.Add(title);

            // Help box
            var helpBox = new HelpBox(
                "Click 'Start Copying' and select objects in hierarchy or scene.\n" +
                "First selection becomes the source, next selections will receive the transform.\n" +
                "Press ESC or 'Stop Copying' to exit.",
                HelpBoxMessageType.Info);
            helpBox.style.marginBottom = 10;
            root.Add(helpBox);

            // Inactive mode container
            modeInactiveContainer = new VisualElement();
            startButton = new Button(StartCopying) { text = "Start Copying" };
            startButton.style.height = 30;
            modeInactiveContainer.Add(startButton);
            root.Add(modeInactiveContainer);

            // Active mode container
            modeActiveContainer = new VisualElement();
            modeActiveContainer.style.display = DisplayStyle.None;

            var activeLabel = new Label("COPYING MODE ACTIVE");
            activeLabel.style.fontSize = 14;
            activeLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            activeLabel.style.color = Color.green;
            activeLabel.style.marginBottom = 10;
            modeActiveContainer.Add(activeLabel);

            // Source info container
            var sourceContainer = new VisualElement();
            sourceContainer.style.marginBottom = 10;

            var sourceLabel = new Label("Source:");
            sourceLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            sourceContainer.Add(sourceLabel);

            sourceNameLabel = new Label();
            sourceNameLabel.style.marginLeft = 10;
            sourceNameLabel.style.marginBottom = 5;
            sourceContainer.Add(sourceNameLabel);

            positionLabel = new Label();
            positionLabel.style.marginLeft = 10;
            sourceContainer.Add(positionLabel);

            rotationLabel = new Label();
            rotationLabel.style.marginLeft = 10;
            sourceContainer.Add(rotationLabel);

            scaleLabel = new Label();
            scaleLabel.style.marginLeft = 10;
            sourceContainer.Add(scaleLabel);

            modeActiveContainer.Add(sourceContainer);

            // Waiting label
            waitingLabel = new Label("Waiting for source selection...");
            waitingLabel.style.marginBottom = 10;
            waitingLabel.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);
            waitingLabel.style.paddingTop = 5;
            waitingLabel.style.paddingBottom = 5;
            waitingLabel.style.paddingLeft = 5;
            waitingLabel.style.paddingRight = 5;
            modeActiveContainer.Add(waitingLabel);

            // Stop button
            stopButton = new Button(StopCopying) { text = "Stop Copying" };
            stopButton.style.height = 30;
            stopButton.style.backgroundColor = new Color(0.8f, 0.2f, 0.2f);
            stopButton.style.marginTop = 10;
            modeActiveContainer.Add(stopButton);

            root.Add(modeActiveContainer);

            // Register keyboard shortcuts
            root.RegisterCallback<KeyDownEvent>(OnKeyDown);

            UpdateUI();
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (!isCopyingMode) return;

            if (evt.keyCode == KeyCode.Escape)
            {
                StopCopying();
                evt.StopPropagation();
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (isCopyingMode)
            {
                Event e = Event.current;
                if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
                {
                    StopCopying();
                    e.Use();
                }

                if (sourceObject != null)
                {
                    Handles.color = Color.cyan;
                    Handles.SphereHandleCap(0, sourceTransform.position, Quaternion.identity, 0.5f, EventType.Repaint);
                    Handles.Label(sourceTransform.position + Vector3.up * 2f, "SOURCE", EditorStyles.whiteLargeLabel);
                }
            }
        }

        private void StartCopying()
        {
            isCopyingMode = true;
            sourceObject = null;
            sourceTransform = null;
            Debug.Log("Transform Copy Mode: STARTED. Select first object as source.");
            UpdateUI();
        }

        private void StopCopying()
        {
            isCopyingMode = false;
            sourceObject = null;
            sourceTransform = null;
            Debug.Log("Transform Copy Mode: STOPPED.");
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (modeActiveContainer == null || modeInactiveContainer == null) return;

            if (isCopyingMode)
            {
                modeInactiveContainer.style.display = DisplayStyle.None;
                modeActiveContainer.style.display = DisplayStyle.Flex;

                if (sourceObject != null)
                {
                    sourceNameLabel.parent.style.display = DisplayStyle.Flex;
                    waitingLabel.style.display = DisplayStyle.None;

                    sourceNameLabel.text = sourceObject.name;
                    positionLabel.text = "Position: " + sourceTransform.position.ToString("F3");
                    rotationLabel.text = "Rotation: " + sourceTransform.eulerAngles.ToString("F3");
                    scaleLabel.text = "Scale: " + sourceTransform.localScale.ToString("F3");
                }
                else
                {
                    sourceNameLabel.parent.style.display = DisplayStyle.None;
                    waitingLabel.style.display = DisplayStyle.Flex;
                }
            }
            else
            {
                modeInactiveContainer.style.display = DisplayStyle.Flex;
                modeActiveContainer.style.display = DisplayStyle.None;
            }
        }

        private void OnSelectionChanged()
        {
            if (!isCopyingMode) return;

            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject == null) return;

            if (sourceObject == null)
            {
                // First selection - set as source
                sourceObject = selectedObject;
                sourceTransform = selectedObject.transform;
                Debug.Log($"Transform Copy: Source set to '{sourceObject.name}'");
            }
            else
            {
                // Subsequent selections - copy transform to target
                if (selectedObject == sourceObject)
                {
                    Debug.LogWarning("Transform Copy: Cannot copy to the same object.");
                    return;
                }

                CopyTransform(sourceTransform, selectedObject.transform);
                Debug.Log($"Transform Copy: Copied from '{sourceObject.name}' to '{selectedObject.name}'");

                sourceObject = null;
            }

            UpdateUI();
        }

        private void CopyTransform(Transform source, Transform target)
        {
            Undo.RecordObject(target, "Copy Transform");

            target.SetPositionAndRotation(source.position, source.rotation);
            target.localScale = source.localScale;

            EditorUtility.SetDirty(target);
        }
    }
}
