using UnityEngine;
using UnityEngine.UIElements;

class LevelWonUIController : MonoBehaviour
{
    private VisualElement root;

    private Label currentGoldLabel;
    private Label levelCompleteGoldLabel;
    private Label newTotalGoldLabel;
    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        VisualElement nextLevelButton = root.Q<VisualElement>("NextLevelButton");
        if (nextLevelButton != null)
        {
            nextLevelButton.RegisterCallback<ClickEvent>(ev => OnNextLevelButtonClick());
        }
        currentGoldLabel = root.Q<Label>("CurrentGoldLabel");
        levelCompleteGoldLabel = root.Q<Label>("LevelCompleteGoldLabel");
        newTotalGoldLabel = root.Q<Label>("NewTotalGoldLabel");
    }
    private void OnNextLevelButtonClick()
    {
        Debug.Log("next");
        GameManager.instance.NextLevel();
    }

    public void ShowGoldStatus(int currentGold, int levelCompleteGold)
    {
        currentGoldLabel.text = currentGold.ToString();
        levelCompleteGoldLabel.text = levelCompleteGold.ToString();
        newTotalGoldLabel.text = (currentGold + levelCompleteGold).ToString();
    }
}