using UnityEngine;
using UnityEngine.UIElements;

class LevelWonUIController : MonoBehaviour
{
    private VisualElement root;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        VisualElement nextLevelButton = root.Q<VisualElement>("NextLevelButton");
        if (nextLevelButton != null)
        {
            nextLevelButton.RegisterCallback<ClickEvent>(ev => OnNextLevelButtonClick());
        }
    }
    private void OnNextLevelButtonClick()
    {
        Debug.Log("next");
        GameManager.instance.NextLevel();
    }
}