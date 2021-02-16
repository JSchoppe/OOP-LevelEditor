using UnityEngine;

namespace InputBroadcasters
{
    #region Broadcaster Listener Delegates
    /// <summary>
    /// A reciever for when a cursor input raycast hit something.
    /// </summary>
    /// <param name="hitInfo">The raycast hit info.</param>
    public delegate void CursorCollisionListener(RaycastHit hitInfo);
    #endregion
    /// <summary>
    /// Provides an input mechanism for when the user hovers or clicks a collider in the viewport.
    /// </summary>
    public sealed class ColliderCursorBroadcaster : MonoBehaviour
    {
        #region Broadcaster Events
        /// <summary>
        /// Called whenever a collider is clicked in the scene.
        /// </summary>
        public event CursorCollisionListener ColliderClicked;
        /// <summary>
        /// Called each update when a collider is being hovered over.
        /// </summary>
        public event CursorCollisionListener ColliderHovered;
        #endregion
        #region Temporary Use Fields
        private RaycastHit hit;
        #endregion
        #region Inspector Fields
        [Header("Raycast Parameters")]
        [Tooltip("The length of the raycast from the camera.")]
        [SerializeField] private float maxCheckDistance = 1f;
        [Tooltip("What to cast against.")]
        [SerializeField] private LayerMask layerMask = default;
        [Tooltip("Which layers will block casts.")]
        [SerializeField] private LayerMask blockingMask = default;
        private void OnValidate()
        {
            if (maxCheckDistance < 0f)
                maxCheckDistance = 0f;
        }
        #endregion
        #region Properties
        /// <summary>
        /// Defines the layers that this raycast can hit.
        /// </summary>
        public LayerMask LayerMask
        {
            get => layerMask;
            set => layerMask = value;
        }
        #endregion
        #region Recieve Input
        private void Update()
        {
            // If this is a button down event and a raycast
            // finds a suitable hit target...
            if (Physics.Raycast(
                    Camera.main.ScreenPointToRay(Input.mousePosition),
                    out hit,
                    maxCheckDistance,
                    layerMask))
            {
                // Check if a blocking mask was hit and thus the events
                // should be blocked.
                if (blockingMask != (blockingMask | (1 << hit.transform.gameObject.layer)))
                {
                    // If not Then notify any listeners that this action has been invoked.
                    ColliderHovered?.Invoke(hit);
                    if (Input.GetMouseButtonDown(0))
                        ColliderClicked?.Invoke(hit);
                }
            }
        }
        #endregion
    }
}
