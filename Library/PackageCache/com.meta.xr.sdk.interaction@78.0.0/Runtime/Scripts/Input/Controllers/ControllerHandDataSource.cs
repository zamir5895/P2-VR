/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction.Input
{
    public class ControllerHandDataSource : DataSource<HandDataAsset>
    {
        [SerializeField]
        private DataSource<ControllerDataAsset> _controllerSource;

        [SerializeField]
        private Transform _root;

        [SerializeField]
        private Transform _openXRRoot;

        public Transform Root
        {
#if ISDK_OPENXR_HAND
            get => _openXRRoot;
            set => _openXRRoot = value;
#else
            get => _root;
            set => _root = value;
#endif
        }

        [SerializeField]
        private bool _rootIsLocal = true;
        public bool RootIsLocal
        {
            get => _rootIsLocal;
            set => _rootIsLocal = value;
        }

        [SerializeField]
        [FormerlySerializedAs("_bones")]
        [FormerlySerializedAs("_joints")]
        private Transform[] _jointTransforms;

        [SerializeField]
        private Transform[] _openXRJointTransforms;

        public Transform[] Joints
        {
#if ISDK_OPENXR_HAND
            get => _openXRJointTransforms;
#else
            get => _jointTransforms;
#endif
        }

        private HandDataSourceConfig _config;
        private readonly HandDataAsset _handDataAsset = new HandDataAsset();
        protected override HandDataAsset DataAsset => _handDataAsset;

        private HandDataSourceConfig Config
        {
            get
            {
                if (_config == null)
                {
                    _config = new HandDataSourceConfig();
                }

                return _config;
            }
        }

        protected virtual void Awake()
        {
#if ISDK_OPENXR_HAND
            if (_root != null)
            {
                _root.gameObject.SetActive(false);
            }
            if (_openXRRoot != null)
            {
                _openXRRoot.gameObject.SetActive(true);
            }
#else
            if (_root != null)
            {
                _root.gameObject.SetActive(true);
            }
            if (_openXRRoot != null)
            {
                _openXRRoot.gameObject.SetActive(false);
            }
#endif
        }

        protected override void Start()
        {
            this.BeginStart(ref _started, () => base.Start());
            this.AssertField(_controllerSource, nameof(_controllerSource));
#if ISDK_OPENXR_HAND
            this.AssertCollectionField(_openXRJointTransforms, nameof(_openXRJointTransforms));
            this.AssertField(_openXRRoot, nameof(_openXRRoot));
#else
            this.AssertCollectionField(_jointTransforms, nameof(_jointTransforms));
            this.AssertField(_root, nameof(_root));
#endif
            UpdateConfig();
            this.EndStart(ref _started);
        }

        private void UpdateConfig()
        {
            ControllerDataSourceConfig controllerConfig = _controllerSource.GetData().Config;

            Config.Handedness = controllerConfig.Handedness;
            Config.TrackingToWorldTransformer = controllerConfig.TrackingToWorldTransformer;
            Config.HandSkeleton = HandSkeleton.FromJoints(Joints);
        }

        protected override void UpdateData()
        {
            ControllerDataAsset controllerData = _controllerSource.GetData();
            _handDataAsset.Config = Config;
            _handDataAsset.IsDataValid = controllerData.IsDataValid;
            _handDataAsset.IsConnected = controllerData.IsConnected;

            if (!_handDataAsset.IsConnected || !this.isActiveAndEnabled)
            {
                _handDataAsset.IsTracked = default;
                _handDataAsset.RootPoseOrigin = default;
                _handDataAsset.PointerPoseOrigin = default;
                _handDataAsset.IsHighConfidence = default;
                for (var fingerIdx = 0; fingerIdx < Constants.NUM_FINGERS; fingerIdx++)
                {
                    _handDataAsset.IsFingerPinching[fingerIdx] = default;
                    _handDataAsset.IsFingerHighConfidence[fingerIdx] = default;
                }
                return;
            }

            _handDataAsset.IsTracked = controllerData.IsTracked;
            _handDataAsset.IsHighConfidence = true;
            _handDataAsset.IsDominantHand = controllerData.IsDominantHand;

            float pinchStrength = controllerData.Input.Trigger;
            float gripStrength = controllerData.Input.Grip;

            bool isPinching = controllerData.Input.TriggerButton;
            bool isGripping = controllerData.Input.GripButton;

            _handDataAsset.IsFingerHighConfidence[(int)HandFinger.Thumb] = true;
            _handDataAsset.IsFingerPinching[(int)HandFinger.Thumb] = isPinching || isGripping;
            _handDataAsset.FingerPinchStrength[(int)HandFinger.Thumb] = Mathf.Max(pinchStrength, gripStrength);

            _handDataAsset.IsFingerHighConfidence[(int)HandFinger.Index] = true;
            _handDataAsset.IsFingerPinching[(int)HandFinger.Index] = isPinching;
            _handDataAsset.FingerPinchStrength[(int)HandFinger.Index] = pinchStrength;

            _handDataAsset.IsFingerHighConfidence[(int)HandFinger.Middle] = true;
            _handDataAsset.IsFingerPinching[(int)HandFinger.Middle] = isGripping;
            _handDataAsset.FingerPinchStrength[(int)HandFinger.Middle] = gripStrength;

            _handDataAsset.IsFingerHighConfidence[(int)HandFinger.Ring] = true;
            _handDataAsset.IsFingerPinching[(int)HandFinger.Ring] = false;
            _handDataAsset.FingerPinchStrength[(int)HandFinger.Ring] = 0f;

            _handDataAsset.IsFingerHighConfidence[(int)HandFinger.Pinky] = true;
            _handDataAsset.IsFingerPinching[(int)HandFinger.Pinky] = false;
            _handDataAsset.FingerPinchStrength[(int)HandFinger.Pinky] = 0f;

            _handDataAsset.PointerPoseOrigin = PoseOrigin.FilteredTrackedPose;
            _handDataAsset.PointerPose = controllerData.PointerPose;

            for (int i = 0; i < Joints.Length; i++)
            {
#pragma warning disable 0618
                _handDataAsset.Joints[i] = Joints[i].localRotation;
#pragma warning restore 0618
#if ISDK_OPENXR_HAND
                _handDataAsset.JointPoses[i] = PoseUtils.Delta(Root, Joints[i]);
#endif
            }

            if (_rootIsLocal)
            {
                Pose offset = Root.GetPose(Space.Self);
                Pose controllerPose = controllerData.RootPose;
                PoseUtils.Multiply(controllerPose, offset, ref _handDataAsset.Root);
                _handDataAsset.HandScale = Root.localScale.x;
            }
            else
            {
                _handDataAsset.Root = Root.GetPose(Space.World);
                _handDataAsset.HandScale = Root.lossyScale.x;
            }

            _handDataAsset.RootPoseOrigin = PoseOrigin.FilteredTrackedPose;
        }

        #region Inject

        public void InjectAllControllerHandDataSource(UpdateModeFlags updateMode, IDataSource updateAfter,
            DataSource<ControllerDataAsset> controllerSource, Transform[] jointTransforms)
        {
            base.InjectAllDataSource(updateMode, updateAfter);
            InjectControllerSource(controllerSource);
            InjectJointTransforms(jointTransforms);
        }

        public void InjectControllerSource(DataSource<ControllerDataAsset> controllerSource)
        {
            _controllerSource = controllerSource;
        }

        [Obsolete("Use " + nameof(InjectJointTransforms) + " instead")]
        public void InjectBones(Transform[] joints)
        {
            InjectJointTransforms(joints);
        }

        public void InjectJointTransforms(Transform[] jointTransforms)
        {
#if ISDK_OPENXR_HAND
            _openXRJointTransforms = jointTransforms;
#else
            _jointTransforms = jointTransforms;
#endif
        }
        #endregion
    }
}
