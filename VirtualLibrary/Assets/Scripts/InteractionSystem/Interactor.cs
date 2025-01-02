

using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionRadius;
    [SerializeField] private LayerMask _interactionMask;

    [SerializeField] private InteractionPromptUI _interactionPromptUI;

    private readonly Collider[] _colliders = new Collider[3];
    private IInteractable _interactable;
    [SerializeField] private int _numFound;
    private void Update()
    {
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionRadius, _colliders, _interactionMask);

        if (_numFound > 0)
        {
            Debug.Log("Press E to interact with the chest");

            _interactable = _colliders[0].GetComponent<IInteractable>();
            if (_interactable != null)
            {
                if (!_interactionPromptUI.isDisplayed)
                {
                    Debug.Log("Displaying interaction prompt");
                    _interactionPromptUI.SetUp(_interactable.InteractionPrompt);
                }

                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    Debug.Log("E key pressed");
                    _interactable.Interact(this);
                }
            }
        }
        else
        {
            if (_interactable != null) _interactable = null;
            if (_interactionPromptUI.isDisplayed)
            {
                Debug.Log("Closing interaction prompt");
                _interactionPromptUI.Close();
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionRadius);
    }
}