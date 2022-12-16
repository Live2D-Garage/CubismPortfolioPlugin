/**
 * Copyright(c) Live2D Inc. All rights reserved.
 *
 * Use of this source code is governed by the MIT License.
 */

using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using Live2D.Cubism.Framework.Expression;
using Live2D.Cubism.Framework.LookAt;
using Live2D.Cubism.Framework.Motion;
using Live2D.Cubism.Framework.Raycasting;


namespace Live2D.Cubism.PortfolioPlugin
{
    [RequireComponent(typeof(CubismMotionController))]
    [RequireComponent(typeof(CubismRaycaster))]
    [RequireComponent(typeof(CubismExpressionController))]
    [RequireComponent(typeof(CubismLookController))]
    [RequireComponent(typeof(PortfolioLookTarget))]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(1)]
    public class PortfolioController : MonoBehaviour
    {
        #region Varuables

        // モーションリスト
        [SerializeField] public PortfolioMotionList MotionList;

        // 表示するモデルのインデックス
        [SerializeField] public int ModelGroupNumber;

        // 当たり判定用コンポーネント
        private CubismRaycaster _raycaster;

        // モーション再生用コンポーネント
        private CubismMotionController _motionController;

        // 表情用コンポーネント
        private CubismExpressionController _expressionController;

        // 当たり判定用アートメッシュのメタ情報
        private CubismHitDrawable[] _hitDrawables;

        // 当たり判定用アートメッシュ
        private CubismDrawable[] _drawables;

        // 当たり判定のあったアートメッシュの情報
        private CubismRaycastHit[] _raycastHits;

        // Idleモーションのインデックス
        private int _idleIndex;

        #endregion


        #region Unity Events Handling


        /// <summary>
        /// Called by Unity. Revives instance.
        /// </summary>
        private void OnEnable()
        {
            Refresh();
        }


        /// <summary>
        /// Called by Unity. Revives instance.
        /// </summary>
        private void OnDisable()
        {
            CleanUp();
        }


        /// <summary>
        /// Updates the junction and all related data.
        /// </summary>
        private void Update()
        {
            if (!Input.GetMouseButtonDown(0) || _raycaster == null || MotionList == null)
            {
                return;
            }


            // 当たり判定チェック
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            _raycastHits = new CubismRaycastHit[4];
            var hitCount = _raycaster.Raycast(ray, _raycastHits);

            for (var i = 0; i < hitCount; i++)
            {
                var hitDrawable = _raycastHits[i].Drawable;

                if (hitDrawable.gameObject.GetComponent<CubismHitDrawable>().Name == "Head")
                {
                    // 表情切り替え
                    ChangeExpression();

                    break;
                }


                // モーション再生
                for (var j = 0; j < _hitDrawables.Length; j++)
                {

                    if (hitDrawable != _drawables[j])
                    {
                        continue;
                    }


                    for (var k = 0; k < MotionList.Motions.Length; k++)
                    {
                        var motionGroup = MotionList.Motions[k];
                        if (motionGroup.MotionGroupName != _hitDrawables[j].Name)
                        {
                            continue;
                        }

                        var index = UnityEngine.Random.Range(0, MotionList.Motions[k].Motions.Length - 1);
                        PlayMotion(k, index);
                        break;
                    }

                    break;
                }
            }
        }

        #endregion


        #region Function

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Refresh()
        {
            _expressionController = GetComponent<CubismExpressionController>();
            _raycaster = GetComponent<CubismRaycaster>();
            _motionController = GetComponent<CubismMotionController>();

            _motionController.AnimationEndHandler += AnimationEnded;

            GetComponent<CubismLookController>().Target = GetComponent<PortfolioLookTarget>();

            // 当たり判定用アートメッシュの情報取得
            _hitDrawables = gameObject.GetComponentsInChildren<CubismHitDrawable>();
            _drawables = new CubismDrawable[_hitDrawables.Length];
            for (var i = 0; i < _drawables.Length; i++)
            {
                _drawables[i] = _hitDrawables[i].gameObject.GetComponent<CubismDrawable>();
            }

            // Idleモーションのインデックス
            _idleIndex = -1;
            for (var i = 0; i < MotionList.Motions.Length; i++)
            {
                if (MotionList.Motions[i].MotionGroupName != "Idle")
                {
                    continue;
                }

                _idleIndex = i;
                break;
            }

            PlayIdleMotion(CubismMotionPriority.PriorityForce);
        }

        /// <summary>
        ///  終了処理
        /// </summary>
        private void CleanUp()
        {
            _motionController.AnimationEndHandler -= AnimationEnded;
        }


        /// <summary>
        /// モーション終了時に呼び出される
        /// </summary>
        /// <param name="instanceId"></param>
        private void AnimationEnded(float instanceId)
        {
            PlayIdleMotion();
        }

        /// <summary>
        /// モーション再生
        /// </summary>
        /// <param name="motionGroupIndex">再生するモーショングループのインデックス</param>
        /// <param name="motionIndex">再生するモーションのインデックス</param>
        /// <param name="priority">再生するモーションの優先度</param>
        /// <param name="isLoop">再生するモーションをループ再生するか</param>
        public void PlayMotion(int motionGroupIndex, int motionIndex, int priority = CubismMotionPriority.PriorityForce,
            bool isLoop = false)
        {
            if (_motionController == null || motionGroupIndex < 0 || motionIndex < 0)
            {
                return;
            }


            _motionController.PlayAnimation(MotionList.Motions[motionGroupIndex].Motions[motionIndex],
                priority: priority, isLoop: isLoop);
        }

        /// <summary>
        /// アイドルモーションを再生
        /// </summary>
        /// <param name="priority">再生するモーションの優先度</param>
        public void PlayIdleMotion(int priority = CubismMotionPriority.PriorityIdle)
        {
            if (_motionController == null || MotionList == null || _idleIndex < 0)
            {
                return;
            }

            var idleMotions = MotionList.Motions[_idleIndex];
            var index = UnityEngine.Random.Range(0, idleMotions.Motions.Length - 1);

            // アイドルモーション再生
            PlayMotion(_idleIndex, index, priority);
        }


        /// <summary>
        /// 表情をランダムに切り替え
        /// </summary>
        public void ChangeExpression()
        {
            SetExpression(new System.Random().Next(
                0,
                _expressionController.ExpressionsList.CubismExpressionObjects.Length - 1));
        }


        /// <summary>
        /// 表情を切り替え
        /// </summary>
        /// <param name="expressionIndex">切り替える表情のインデックス</param>
        public void SetExpression(int expressionIndex)
        {
            if (_expressionController == null || expressionIndex < 0)
            {
                return;
            }

            _expressionController.CurrentExpressionIndex = expressionIndex;
        }

        #endregion
    }
}
