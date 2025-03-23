using UnityEngine;
using ASimpleRoguelike.Entity;

namespace ASimpleRoguelike {
    public class ChainRotater : MonoBehaviour {
        [ContextMenu("Make New Child")]
        void MakeNewChild() {
            NewChild();
        }
        
        public GameObject child;
        public GameObject type;

        [Tooltip("The object to rotate around")]
        public Transform rotateAroundTransform;

        [Tooltip("Degrees per second")]
        public float rotateSpeed = 10f;

        [Tooltip("The offset from the rotateAroundTransform, only really used for instant snapping")]
        public float offset = 0f;
        [Tooltip("The offset of rotation in degrees")]
        public float rotationOffset = 0f;
        [Tooltip("Instantly snap to the rotateAroundTransform")]
        public bool instantSnap = false;

        [Tooltip("Can be paused")]
        public bool isPausable = true;
        [Tooltip("Other rotation")]
        public float otherRotation = 0f;
        [Tooltip("Look at")]
        public bool lookAt = false;

        private float timerer;

        public void Init(Transform rotateAroundTransform, float rotateSpeed, float offset, float rotationOffset, bool instantSnap, bool isPausable, float otherRotation, bool lookAt) {
            this.rotateAroundTransform = rotateAroundTransform;
            this.rotateSpeed = rotateSpeed;
            this.offset = offset;
            this.rotationOffset = rotationOffset;
            this.instantSnap = instantSnap;
            this.isPausable = isPausable;
            this.otherRotation = otherRotation;
            this.lookAt = lookAt;
        }

        void Update() {
            if (GlobalGameData.isPaused && isPausable) return;

            timerer += Time.deltaTime;

            if (instantSnap) {
                Vector3 pos = rotateAroundTransform.position + new Vector3(Mathf.Sin((timerer * rotateSpeed + rotationOffset) * Mathf.Deg2Rad), Mathf.Cos((timerer * rotateSpeed + rotationOffset) * Mathf.Deg2Rad), 0) * offset;
                transform.position = pos;
            } else if (rotateAroundTransform != null) {
                transform.RotateAround(rotateAroundTransform.position, Vector3.forward, rotateSpeed * Time.deltaTime);
            }

            if (lookAt) {
                transform.rotation = Quaternion.Euler(0, 0, Util.ActualAngle(transform.position, rotateAroundTransform.position) + otherRotation);
            }
        }

        public void NewChild() {
            if (child != null) {
                child.GetComponent<ChainRotater>().NewChild();
                return;
            }

            child = Instantiate(type);

            child.transform.SetParent(transform);
            ChainRotater chainRotater = child.GetComponent<ChainRotater>();
            chainRotater.Init(transform, rotateSpeed * 1.5f, offset, rotationOffset, instantSnap, isPausable, otherRotation, lookAt);
        }
    }
}