using Unity.Mathematics;
using Unity.XR.CoreUtils.Bindings;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets
{
	/// <summary>
	/// Follow animation affordance for <see cref="IPokeStateDataProvider"/>, such as <see cref="XRPokeFilter"/>.
	/// Used to animate a pressed transform, such as a button to follow the poke position.
	/// </summary>
	[AddComponentMenu("XR/XR Poke Follow Affordance", 22)]
	public class XRPokeFollowAffordance : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Transform that will move in the poke direction when this or a parent GameObject is poked." +
		         "\nNote: Should be a direct child GameObject.")]
		private Transform m_PokeFollowTransform;

		[SerializeField] [Range(0f, 20f)] [Tooltip("Multiplies transform position interpolation as a factor of Time.deltaTime. If 0, no smoothing will be applied.")]
		private float m_SmoothingSpeed = 16f;

		[SerializeField] [Tooltip("When this component is no longer the target of the poke, the Poke Follow Transform returns to the original position.")]
		private bool m_ReturnToInitialPosition = true;

		[SerializeField]
		[Tooltip("Whether to apply the follow animation if the target of the poke is a child of this transform. " +
		         "This is useful for UI objects that may have child graphics.")]
		private bool m_ApplyIfChildIsTarget = true;

		[SerializeField] [Tooltip("Whether to keep the Poke Follow Transform from moving past a maximum distance from the poke target.")]
		private bool m_ClampToMaxDistance;

		[SerializeField] [Tooltip("The maximum distance from this transform that the Poke Follow Transform can move.")]
		private float m_MaxDistance;

		private readonly BindingsGroup m_BindingsGroup = new();

		private readonly Vector3TweenableVariable m_TransformTweenableVariable = new();
		private bool m_IsFirstFrame;
		private IMultiPokeStateDataProvider m_MultiPokeStateDataProvider;

		private IPokeStateDataProvider m_PokeDataProvider;

		/// <summary>
		/// Transform that will animate along the axis of interaction when this interactable is poked.
		/// Note: Must be a direct child GameObject as it moves in local space relative to the poke target's transform.
		/// </summary>
		public Transform pokeFollowTransform
		{
			get => m_PokeFollowTransform;
			set => m_PokeFollowTransform = value;
		}

		/// <summary>
		/// Multiplies transform position interpolation as a factor of <see cref="Time.deltaTime"/>. If <c>0</c>, no smoothing will
		/// be applied.
		/// </summary>
		public float smoothingSpeed
		{
			get => m_SmoothingSpeed;
			set => m_SmoothingSpeed = value;
		}

		/// <summary>
		/// When this component is no longer the target of the poke, the <see cref="pokeFollowTransform"/> returns to the original
		/// position.
		/// </summary>
		public bool returnToInitialPosition
		{
			get => m_ReturnToInitialPosition;
			set => m_ReturnToInitialPosition = value;
		}

		/// <summary>
		/// Whether to apply the follow animation if the target of the poke is a child of this transform.
		/// This is useful for UI objects that may have child graphics.
		/// </summary>
		public bool applyIfChildIsTarget
		{
			get => m_ApplyIfChildIsTarget;
			set => m_ApplyIfChildIsTarget = value;
		}

		/// <summary>
		/// Whether to keep the <see cref="pokeFollowTransform"/> from moving past <see cref="maxDistance"/> from the poke target.
		/// </summary>
		public bool clampToMaxDistance
		{
			get => m_ClampToMaxDistance;
			set => m_ClampToMaxDistance = value;
		}

		/// <summary>
		/// The maximum distance from this transform that the <see cref="pokeFollowTransform"/> can move when
		/// <see cref="clampToMaxDistance"/> is <see langword="true"/>.
		/// </summary>
		public float maxDistance
		{
			get => m_MaxDistance;
			set => m_MaxDistance = value;
		}

		/// <summary>
		/// The original position of this interactable before any pushes have been applied.
		/// </summary>
		public Vector3 initialPosition { get; set; }

		/// <summary>
		/// See <see cref="MonoBehaviour"/>.
		/// </summary>
		protected void Awake()
		{
			m_MultiPokeStateDataProvider = GetComponentInParent<IMultiPokeStateDataProvider>();
			if (m_MultiPokeStateDataProvider == null)
				m_PokeDataProvider = GetComponentInParent<IPokeStateDataProvider>();
		}

		/// <summary>
		/// See <see cref="MonoBehaviour"/>.
		/// </summary>
		protected void Start()
		{
			if (m_PokeFollowTransform != null)
			{
				initialPosition = m_PokeFollowTransform.localPosition;
				m_BindingsGroup.AddBinding(m_TransformTweenableVariable.Subscribe(OnTransformTweenableVariableUpdated));

				if (m_MultiPokeStateDataProvider != null)
					m_BindingsGroup.AddBinding(m_MultiPokeStateDataProvider.GetPokeStateDataForTarget(transform).Subscribe(OnPokeStateDataUpdated));
				else if (m_PokeDataProvider != null)
					m_BindingsGroup.AddBinding(m_PokeDataProvider.pokeStateData.SubscribeAndUpdate(OnPokeStateDataUpdated));
			}
			else
			{
				enabled = false;
				Debug.LogWarning($"Missing Poke Follow Transform assignment on {this}. Disabling component.", this);
			}
		}

		/// <summary>
		/// See <see cref="MonoBehaviour"/>.
		/// </summary>
		protected void LateUpdate()
		{
			if (m_IsFirstFrame)
			{
				m_TransformTweenableVariable.HandleTween(1f);
				m_IsFirstFrame = false;
				return;
			}

			m_TransformTweenableVariable.HandleTween(m_SmoothingSpeed > 0f ? Time.deltaTime * m_SmoothingSpeed : 1f);
		}

		/// <summary>
		/// See <see cref="MonoBehaviour"/>.
		/// </summary>
		protected void OnDestroy()
		{
			m_BindingsGroup.Clear();
			m_TransformTweenableVariable?.Dispose();
		}

		private void OnTransformTweenableVariableUpdated(float3 position)
		{
			m_PokeFollowTransform.localPosition = position;
		}

		private void OnPokeStateDataUpdated(PokeStateData data)
		{
			var pokeTarget = data.target;
			var applyFollow = m_ApplyIfChildIsTarget
				? pokeTarget != null && pokeTarget.IsChildOf(transform)
				: pokeTarget == transform;

			if (applyFollow)
			{
				var targetPosition = pokeTarget.InverseTransformPoint(data.axisAlignedPokeInteractionPoint);
				if (m_ClampToMaxDistance && targetPosition.sqrMagnitude > m_MaxDistance * m_MaxDistance)
					targetPosition = Vector3.ClampMagnitude(targetPosition, m_MaxDistance);

				m_TransformTweenableVariable.target = targetPosition;
			}
			else if (m_ReturnToInitialPosition)
			{
				m_TransformTweenableVariable.target = initialPosition;
			}
		}

		public void ResetFollowTransform()
		{
			if (!m_ClampToMaxDistance || m_PokeFollowTransform == null)
				return;

			m_PokeFollowTransform.localPosition = initialPosition;
		}
	}
}