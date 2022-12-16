/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at https://www.live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */


using UnityEditor;


namespace Live2D.Cubism.PortfolioPlugin
{
    [CustomEditor(typeof(PortfolioPartBlendColorEditor)), CanEditMultipleObjects]
    internal sealed class PortfolioPartBlendColorEditorInspector : UnityEditor.Editor
    {
        private SerializedProperty childDrawableRenderers;

        #region Editor

        /// <summary>
        /// インスペクタの描画
        /// </summary>
        public override void OnInspectorGUI()
        {
            var blendColorEditor = target as PortfolioPartBlendColorEditor;

            // 空ならサイレントでリターン。
            if (blendColorEditor == null)
            {
                return;
            }

            // コンポーネントからプロパティを取得する。
            if (childDrawableRenderers == null)
            {
                childDrawableRenderers = serializedObject.FindProperty("_childDrawableRenderers");
            }

            if (childDrawableRenderers != null)
            {
                // インスペクタに表示する。
                EditorGUILayout.PropertyField(childDrawableRenderers);
            }

            EditorGUI.BeginChangeCheck();

            // OverwriteFlagForPartMultiplyColorsを表示
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                var overwriteFlagForPartMultiplyColors = EditorGUILayout.Toggle("OverwriteFlagForPartMultiplyColors", blendColorEditor.OverwriteFlagForPartMultiplyColors);

                if (scope.changed)
                {
                    foreach (PortfolioPartBlendColorEditor partBlendColorEditor in targets)
                    {
                        partBlendColorEditor.OverwriteFlagForPartMultiplyColors = overwriteFlagForPartMultiplyColors;

                        foreach (var element in partBlendColorEditor.ChildDrawableRenderers)
                        {
                            element.OverwriteFlagForDrawableMultiplyColors = partBlendColorEditor.OverwriteFlagForPartMultiplyColors;
                            element.LastMultiplyColor = partBlendColorEditor.OverwriteFlagForPartMultiplyColors ? partBlendColorEditor.MultiplyColor : element.LastMultiplyColor;
                            element.MultiplyColor = partBlendColorEditor.OverwriteFlagForPartMultiplyColors ? partBlendColorEditor.MultiplyColor : element.MultiplyColor;
                        }
                    }
                }
            }

            // OverwriteFlagForPartScreenColorsを表示
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                var overwriteFlagForPartScreenColors = EditorGUILayout.Toggle("OverwriteFlagForPartScreenColors", blendColorEditor.OverwriteFlagForPartScreenColors);

                if (scope.changed)
                {
                    foreach (PortfolioPartBlendColorEditor partBlendColorEditor in targets)
                    {
                        partBlendColorEditor.OverwriteFlagForPartScreenColors = overwriteFlagForPartScreenColors;

                        foreach (var element in partBlendColorEditor.ChildDrawableRenderers)
                        {
                            element.OverwriteFlagForDrawableScreenColors = partBlendColorEditor.OverwriteFlagForPartScreenColors;
                            element.LastScreenColor = partBlendColorEditor.OverwriteFlagForPartScreenColors ? partBlendColorEditor.ScreenColor : element.LastScreenColor;
                            element.ScreenColor = partBlendColorEditor.OverwriteFlagForPartScreenColors ? partBlendColorEditor.ScreenColor : element.ScreenColor;
                        }
                    }
                }
            }

            // 乗算色の表示
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                var multiplyColor = EditorGUILayout.ColorField("MultiplyColor", blendColorEditor.MultiplyColor);

                if (scope.changed)
                {
                    foreach (PortfolioPartBlendColorEditor partBlendColorEditor in targets)
                    {
                        partBlendColorEditor.MultiplyColor = multiplyColor;

                        foreach (var element in partBlendColorEditor.ChildDrawableRenderers)
                        {
                            element.MultiplyColor = partBlendColorEditor.MultiplyColor;
                        }
                    }
                }
            }

            // スクリーン色の表示
            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                var screenColor = EditorGUILayout.ColorField("ScreenColor", blendColorEditor.ScreenColor);

                if (scope.changed)
                {
                    foreach (PortfolioPartBlendColorEditor partBlendColorEditor in targets)
                    {
                        partBlendColorEditor.ScreenColor = screenColor;

                        foreach (var element in partBlendColorEditor.ChildDrawableRenderers)
                        {
                            element.ScreenColor = partBlendColorEditor.ScreenColor;
                        }
                    }
                }
            }


            // 変更を保存する。
            if (EditorGUI.EndChangeCheck())
            {
                foreach (PortfolioPartBlendColorEditor partBlendColorEditor in targets)
                {
                    EditorUtility.SetDirty(partBlendColorEditor);

                    foreach (var renderer in partBlendColorEditor.ChildDrawableRenderers)
                    {
                        EditorUtility.SetDirty(renderer);
                        // HACK メッシュレンダラーを直接取得する。
                        EditorUtility.SetDirty(renderer.MeshRenderer);
                    }
                }
            }
        }

        #endregion
    }
}
