// Designed by Kinemation, 2023

using System.Collections.Generic;
using Kinemation.FPSFramework.Runtime.FPSAnimator;
using UnityEngine;

namespace Demo.Scripts.Runtime
{
    public enum OverlayType
    {
        Default,
        Pistol,
        Rifle
    }
    
    public class Weapon : FPSAnimWeapon
    {
        public AnimSequence reloadClip;
        public AnimSequence grenadeClip;
        public OverlayType overlayType;

        [HideInInspector] public int stagedReloadSegment = 0;

        [SerializeField] private List<Transform> scopes;
        //[SerializeField] private GameObject magBone;
        private Animator _animator;
        private int _scopeIndex;

        private int _stagedSegments;

        protected void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            /*
            var animEvents = reloadClip.clip.events;

            foreach (var animEvent in animEvents)
            {
                if (animEvent.functionName.Equals("RefreshStagedState"))
                {
                    _stagedSegments++;
                }
            }
            
            _animator.Play("Empty");
            */
        }

        // Returns a normalized reload time ratio
        public float GetReloadTime()
        {
            if (_stagedSegments == 0) return 0f;

            return (float) stagedReloadSegment / _stagedSegments;
        }

        public override Transform GetAimPoint()
        {
            _scopeIndex++;
            _scopeIndex = _scopeIndex > scopes.Count - 1 ? 0 : _scopeIndex;
            return scopes[_scopeIndex];
        }
        
        public void OnFire()
        {
            if (_animator == null)
            {
                return;
            }

            _animator.Play("Fire", 0, 0f);
        }

        public void Reload()
        {
            if (_animator == null)
            {
                return;
            }
            
            _animator.Rebind();
            _animator.Play("Reload", 0);
        }

        public void UpdateMagVisibility(bool bVisible)
        {
            //if (magBone == null) return;

            //magBone.transform.localScale = bVisible ? Vector3.one : Vector3.zero;
        }
    }
}