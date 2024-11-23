using UnityEngine;
using UnityEngine.UIElements;

public class DragUI : MonoBehaviour
{
    private bool isDragging = false;  // Flag to track if the element is being dragged
    private Vector2 originalMousePosition;  // Store the original mouse position when dragging starts
    private Vector2 originalElementPosition;  // Store the original position of the element
    private VisualElement headerBar;  // Reference to the header bar element
    private VisualElement draggablePanel;  // Reference to the draggable panel element

    public UIDocument uiDocument;  // Reference to the UIDocument

    void OnEnable()
    {
        // Get the root VisualElement from the UIDocument
        var root = uiDocument.rootVisualElement;

        // Find the header bar and the panel that you want to drag
        headerBar = root.Q<VisualElement>("HeaderBar");
        draggablePanel = root.Q<VisualElement>("ShopPanel");  // Replace with the actual ID of your panel

        // Register the mouse down event on the header bar
        headerBar.RegisterCallback<PointerDownEvent>(OnPointerDown);
        // Register the mouse move event for dragging the panel
        headerBar.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        // Register the mouse up event to stop dragging
        headerBar.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    // Called when the mouse button is pressed down
    private void OnPointerDown(PointerDownEvent evt)
    {
        // Only start dragging if the left mouse button is pressed
        if (evt.button == (int)MouseButton.LeftMouse)
        {
            isDragging = true;
            originalMousePosition = evt.localPosition;  // Store the mouse position at the start of the drag

            // Store both left and top values of the element's position
            originalElementPosition = new Vector2(
                draggablePanel.resolvedStyle.left,   // Horizontal (left) position
                draggablePanel.resolvedStyle.top     // Vertical (top) position
            );
        }
    }

    // Called when the mouse is moved while dragging
    private void OnPointerMove(PointerMoveEvent evt)
    {
        if (isDragging)
        {
            // Calculate how much the mouse has moved
            // Convert evt.localPosition to Vector2 if it's a Vector3
            Vector2 currentMousePosition = new Vector2(evt.localPosition.x, evt.localPosition.y);

            Vector2 delta = currentMousePosition - originalMousePosition;

            // Move the draggable panel by the same amount (X and Y positions)
            draggablePanel.style.left = originalElementPosition.x + delta.x;
            draggablePanel.style.top = originalElementPosition.y + delta.y;
        }
    }

    // Called when the mouse button is released
    private void OnPointerUp(PointerUpEvent evt)
    {
        if (isDragging)
        {
            isDragging = false;
        }
    }
}
