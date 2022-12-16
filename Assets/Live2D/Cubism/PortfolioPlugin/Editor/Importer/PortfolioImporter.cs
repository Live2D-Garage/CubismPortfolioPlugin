/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the MIT License.
 */

using System.IO;
using UnityEngine;
using UnityEditor;
using Live2D.Cubism.Core;
using Live2D.Cubism.Editor.Importers;
using Live2D.Cubism.Framework;
using Live2D.Cubism.Framework.HarmonicMotion;
using Live2D.Cubism.Framework.LookAt;
using Live2D.Cubism.Framework.Raycasting;

namespace Live2D.Cubism.PortfolioPlugin.Editor
{
    public class PortfolioImporter
    {
        #region Unity Event Handling

        /// <summary>
        /// Register fadeMotion importer.
        /// </summary>
        [InitializeOnLoadMethod]
        private static void RegisterMotionImporter()
        {
            CubismImporter.OnDidImportModel += OnModelImport;
        }

        #endregion

        #region Cubism Import Event Handling

        /// <summary>
        /// Portfolio settings.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="model">Imported model.</param>
        private static void OnModelImport(CubismModel3JsonImporter sender, CubismModel model)
        {
            var gameObject = model.gameObject;

            // ポートフォリオ用設定
            var portfolioController = gameObject.GetOrAddComponent<PortfolioController>();

            // 視線追従
            var lookController = gameObject.GetOrAddComponent<CubismLookController>();
            if (lookController.Target == null)
            {
                lookController.Target = gameObject.GetOrAddComponent<PortfolioLookTarget>();
            }

            lookController.BlendMode = CubismParameterBlendMode.Additive;

            // まばたき
            var eyeBlinkController = gameObject.GetOrAddComponent<CubismEyeBlinkController>();
            eyeBlinkController.BlendMode = CubismParameterBlendMode.Multiply;

            // 自動まばたき用コンポーネント
            gameObject.GetOrAddComponent<CubismAutoEyeBlinkInput>();


            var parameters = model.Parameters;
            for (var i = 0; i < parameters.Length; i++)
            {
                switch (parameters[i].Id)
                {
                    // 呼吸用パラメータにマーカーのコンポーネントを追加
                    case "ParamBreath":
                    case "PARAM_BREATH":
                        var harmonicMotion = gameObject.GetOrAddComponent<CubismHarmonicMotionController>();
                        harmonicMotion.BlendMode = CubismParameterBlendMode.Additive;
                        parameters[i].gameObject.GetOrAddComponent<CubismHarmonicMotionParameter>();
                        continue;


                    // 視線追従用パラメータにマーカーのコンポーネントを追加
                    case "ParamAngleX":
                    case "ParamAngleY":
                    case "ParamEyeBallX":
                    case "ParamEyeBallY":
                    case "ParamBodyAngleX":
                    case "PARAM_ANGLE_X":
                    case "PARAM_ANGLE_Y":
                    case "PARAM_EYE_BALL_X":
                    case "PARAM_EYE_BALL_Y":
                    case "PARAM_BODY_ANGLE_X":
                        parameters[i].gameObject.GetOrAddComponent<CubismLookParameter>();
                        continue;


                    // まばたき用パラメータにマーカーのコンポーネントを追加
                    case "ParamEyeLOpen":
                    case "ParamEyeROpen":
                    case "PARAM_EYE_L_OPEN":
                    case "PARAM_EYE_R_OPEN":
                        parameters[i].gameObject.GetOrAddComponent<CubismEyeBlinkParameter>();
                        continue;
                }
            }

            // 当たり判定用アートメッシュの設定
            var drawables = model.Drawables;
            var hitAreas = sender.Model3Json.HitAreas;
            if (hitAreas != null)
            {
                for (var i = 0; i < hitAreas.Length; i++)
                {
                    for (var j = 0; j < drawables.Length; j++)
                    {
                        if (drawables[j].Id != hitAreas[i].Id)
                        {
                            continue;
                        }

                        // .model3.jsonに記述された当たり判定用アートメッシュに設定を追加

                        var raycastable = drawables[j].gameObject.GetOrAddComponent<CubismRaycastable>();
                        raycastable.Precision = CubismRaycastablePrecision.BoundingBox;

                        var hitDrawable = drawables[j].gameObject.GetOrAddComponent<CubismHitDrawable>();
                        hitDrawable.Name = hitAreas[i].Name;

                        break;
                    }
                }
            }

            // モーションリストアセットの生成／読み込み
            var motionListPath = sender.AssetPath.Replace(".model3.json", ".motionList.asset");

            var assetList = CubismCreatedAssetList.GetInstance();
            var assetListIndex = assetList.AssetPaths.Contains(motionListPath)
                ? assetList.AssetPaths.IndexOf(motionListPath)
                : -1;

            PortfolioMotionList motionList;

            if (assetListIndex < 0)
            {
                motionList = AssetDatabase.LoadAssetAtPath<PortfolioMotionList>(motionListPath);
                if (motionList == null)
                {
                    motionList = ScriptableObject.CreateInstance<PortfolioMotionList>();
                    AssetDatabase.CreateAsset(motionList, motionListPath);
                }
                assetList.Assets.Add(motionList);
                assetList.AssetPaths.Add(motionListPath);
                assetList.IsImporterDirties.Add(true);
            }
            else
            {
                motionList = (PortfolioMotionList)assetList.Assets[assetListIndex];
            }


            // モーションの登録

            var assetsDirectoryPath = Application.dataPath.Replace("Assets", "");
            var directoryPath =
                Path.GetDirectoryName(sender.AssetPath.Replace(assetsDirectoryPath, "")).Replace("\\", "/") + "/";

            var motions = sender.Model3Json.FileReferences.Motions;
            var groupedMotions = new PortfolioGroupedMotion[motions.GroupNames.Length];
            for (var i = 0; i < groupedMotions.Length; i++)
            {
                groupedMotions[i] = new PortfolioGroupedMotion();
                groupedMotions[i].MotionGroupName = motions.GroupNames[i];
                groupedMotions[i].Motions = new AnimationClip[motions.Motions[i].Length];

                for (var j = 0; j < motions.Motions[i].Length; j++)
                {
                    var animationClipPath = directoryPath + motions.Motions[i][j].File.Replace(".motion3.json", ".anim");
                    
                    assetListIndex = assetList.AssetPaths.Contains(animationClipPath)
                        ? assetList.AssetPaths.IndexOf(animationClipPath)
                        : -1;

                    AnimationClip animationClip;

                    if (assetListIndex < 0)
                    {
                        animationClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animationClipPath);

                        if (animationClip == null)
                        {
                            animationClip = new AnimationClip
                            {
                                name = Path.GetFileName(motions.Motions[i][j].File).Replace(".motion3.json", "")
                            };

                            AssetDatabase.CreateAsset(animationClip, animationClipPath);
                        }

                        assetList.Assets.Add(animationClip);
                        assetList.AssetPaths.Add(animationClipPath);
                        assetList.IsImporterDirties.Add(true);
                    }
                    else
                    {
                        animationClip = (AnimationClip)assetList.Assets[assetListIndex];
                    }


                    EditorUtility.SetDirty(animationClip);
                    groupedMotions[i].Motions[j] = animationClip;
                }
            }

            // モーションの設定をモーションリストに保存
            motionList.Motions = groupedMotions;
            portfolioController.MotionList = motionList;
            EditorUtility.SetDirty(motionList);
            AssetDatabase.SaveAssets();
        }

        #endregion
    }
}
