using UnityEngine;
using UnityEngine.UIElements;

class SettingsUIController : MonoBehaviour
{
    private VisualElement root;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        VisualElement backButton = root.Q<VisualElement>("BackButton");
        if (backButton != null)
        {
            backButton.RegisterCallback<ClickEvent>(ev => OnBackButtonClick());
        }
    }
    private void OnBackButtonClick()
    {
        GameManager.instance.SetPreviousState();
    }
}