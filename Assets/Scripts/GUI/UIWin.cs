using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWin : UIElement
{
    public override bool ManualHide => true;

    public override bool DestroyOnHide => false;

    public override bool UseBehindPanel => true;

    [SerializeField] TextMeshProUGUI coinText;

    [SerializeField] Button nextLevelButton;

    public void NextLevelButton()
    {
        Hide();
        GameManager.Instance.ChangeState(GameStates.NextLevel);
    }

    public void SetCoinText(float coin)
    {
        coinText.text = coin.ToString();
    }

    private void Start()
    {
        nextLevelButton.onClick.AddListener(NextLevelButton);
    }

}
