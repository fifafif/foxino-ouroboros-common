using Ouroboros.Common.Platform;
using Ouroboros.Common.Utils;
using Ouroboros.Common.Utils.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ouroboros.Common.Logging
{
    public class LogsWindow : EditorWindow
    {
        private ActiveLogContexts contexts;

        private class LogContext : VisualElement
        {
            private bool isEnabled;
            private string typeFullname;
            private string classname;

            private ActiveLogContexts contexts;
            private Toggle toggle;

            public void ShowFullName(bool isFullName)
            {
                toggle.text = isFullName ? typeFullname : classname;
            }

            public LogContext(ActiveLogContexts contexts, ActiveLogContexts.Context type)
            {
                typeFullname = type.Type;
                classname = type.GetClassName();
                isEnabled = type.IsActive;

                toggle = this.AddToggleLabelOnRight(type.Type, type.IsActive, (isActive) =>
                {
                    type.IsActive = isActive;

                    var classType = LogsManager.GetType(type.Type);
                    if (classType == null)
                    {
                        Debug.LogError($"[Logging] No type found! type={type.Type}");
                        return;
                    }

                    if (isActive)
                    {
                        Logs.ActivateType(classType);
                    }
                    else
                    {
                        Logs.DeactivateType(classType);
                    }

                    EditorUtility.SetDirty(contexts);
                });
            }

            internal void Enable(bool isEnabled)
            {
                toggle.value = isEnabled;
            }
        }

        private List<LogContext> logContexts = new List<LogContext>();
        private bool isShowingFullName;
        private Button showFullNameButton;
        private List<string> classes;
        private VisualElement unspecifiedLogsContainer;
        private readonly HashSet<Type> unspecifiedTypes = new HashSet<Type>();

        [MenuItem("Ouroboros/Logs/Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<LogsWindow>();
            window.titleContent = new GUIContent("Logs Settings");

            window.minSize = new Vector2(250, 50);
        }

        private void LoadContexts()
        {
            contexts = AssetFinder.FindAssetsByType<ActiveLogContexts>().FirstOrDefault();
        }

        private void OnEnable()
        {
            CreateGUI();
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            root.Clear();

            LoadContexts();
            //LoadClasses();

            Logs.OnUnspecifiedContextLogged -= OnUnspecifiedContextLogged;
            Logs.OnUnspecifiedContextLogged += OnUnspecifiedContextLogged;

            //var dropdown = new DropdownField("Classes", classes, 0);
            //root.Add(dropdown);

            if (contexts != null)
            {
                var scrollView = new ScrollView();
                root.Add(scrollView);

                var sortedContexts = new List<ActiveLogContexts.Context>(contexts.Contexts);
                sortedContexts.Sort((a, b) => a.GetClassName().CompareTo(b.GetClassName()));

                foreach (var type in sortedContexts)
                {
                    var context = new LogContext(contexts, type);
                    logContexts.Add(context);
                    scrollView.Add(context);
                }

                root.AddToggleLabelOnRight("Is Unspecified Active", contexts.IsUnspecifiedContextActive, (isActive) =>
                {
                    contexts.IsUnspecifiedContextActive = isActive;
                    Logs.IsUnspecifiedContextActive = isActive;
                    EditorUtility.SetDirty(contexts);
                });

                CreateContextButtons(root);
                CreateMainButtons(root);
                CreateUnspecifiedLogs(root);
            }
        }

        private void OnUnspecifiedContextLogged(Type type)
        {
            if (unspecifiedTypes.Contains(type)) return;

            unspecifiedTypes.Add(type);

            AddUnspecifiedContext(type);
        }

        private void AddUnspecifiedContext(Type type)
        {
            var row = new VisualElement();
            unspecifiedLogsContainer.Add(row);
            row.style.flexDirection = FlexDirection.Row;

            row.AddButton("Add", () =>
            {
                if (contexts.Contexts.FirstOrDefault(c => c.Type == type.FullName) == null)
                {
                    Array.Resize(ref contexts.Contexts, contexts.Contexts.Length + 1);

                    contexts.Contexts[contexts.Contexts.Length - 1] = new ActiveLogContexts.Context
                    {
                        Assembly = type.Assembly.GetName().Name,
                        IsActive = true,
                        Type = type.FullName
                    };

                    EditorUtility.SetDirty(contexts);
                }

                unspecifiedTypes.Remove(type);
                Logs.ActivateType(type.FullName);
                CreateGUI();
            });

            row.Add(new Label(type.FullName));
        }

        private void CreateUnspecifiedLogs(VisualElement root)
        {
            unspecifiedLogsContainer = new VisualElement();
            root.Add(unspecifiedLogsContainer);

            foreach (var type in unspecifiedTypes)
            {
                AddUnspecifiedContext(type);
            }
        }

        private void CreateMainButtons(VisualElement root)
        {
            var container = new VisualElement();
            root.Add(container);
            container.style.flexDirection = FlexDirection.Row;

            var enable = container.AddToggle("Enable Logs", PlatformUtilsEditor.ScriptDefinesContains(Logs.LogDefine), (v) =>
            {
                if (v)
                {
                    PlatformUtilsEditor.AddToScriptDefines(Logs.LogDefine);
                }
                else
                {
                    PlatformUtilsEditor.RemoveFromScriptDefines(Logs.LogDefine);
                }
            });
        }

        private void CreateContextButtons(VisualElement root)
        {
            var buttons = new VisualElement();
            root.Add(buttons);
            buttons.style.flexDirection = FlexDirection.Row;

            buttons.AddButton("Enable All", () =>
            {
                foreach (var context in logContexts)
                {
                    context.Enable(true);
                }
            });

            buttons.AddButton("Disable All", () =>
            {
                foreach (var context in logContexts)
                {
                    context.Enable(false);
                }
            });

            showFullNameButton = buttons.AddButton("Show Full Name", () =>
            {
                ShowFullName(!isShowingFullName);
            });

            ShowFullName(false);
        }

        private void LoadClasses()
        {
            classes = Assembly.GetAssembly(typeof(Logs)).GetTypes().Where(t => t.IsClass).Select(t => t.FullName).ToList();
        }

        private void ShowFullName(bool isFullName)
        {
            isShowingFullName = isFullName;

            foreach (var context in logContexts)
            {
                context.ShowFullName(isFullName);
            }

            showFullNameButton.text = $"{(isFullName ? "Show" : "Hide")} Full Name";
        }
    }
}