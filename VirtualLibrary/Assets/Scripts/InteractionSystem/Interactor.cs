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

    // Replace with a dynamic user ID if needed
    private long userId = 1;

    private void Update()
    {
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionRadius, _colliders, _interactionMask);

        if (_numFound > 0)
        {
            Debug.Log("Press E to interact with the chest or F to rent the book");

            _interactable = _colliders[0].GetComponent<IInteractable>();
            if (_interactable != null)
            {
                if (!_interactionPromptUI.isDisplayed)
                {
                    Debug.Log("Displaying interaction prompt");
                    _interactionPromptUI.SetUp(_interactable.InteractionPrompt);
                }

                // Handle "E" key for interaction
                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
                    Debug.Log("E key pressed");
                    _interactable.Interact(this);
                }

                // Handle "F" key for renting a book
                if (Keyboard.current.fKey.wasPressedThisFrame)
                {
                    Debug.Log("F key pressed - Attempting to rent the book");
                    Chest chest = _colliders[0].GetComponent<Chest>();
                    if (chest != null)
                    {
                        chest.RentBook(userId);
                    }
                }

                // Handle "T" key for adding the book to favorites
                if (Keyboard.current.tKey.wasPressedThisFrame)
                {
                    Debug.Log("T key pressed - Adding book to favorites");
                    Chest chest = _colliders[0].GetComponent<Chest>();
                    if (chest != null)
                    {
                        chest.AddToFavorites(userId);
                    }
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
