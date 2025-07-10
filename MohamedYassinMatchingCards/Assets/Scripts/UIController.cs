using UnityEngine;
using TMPro;
using PrimeTween;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI turnsText;
    public TextMeshProUGUI comboText;

    private Tween comboTween;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score\n{score}";
    }

    public void UpdateTurns(int turns)
    {
        turnsText.text = $"Turns:\n{turns}";
    }

    public void ShowCombo(int combo)
    {
        if (combo <= 1)
            return;

        comboText.text = $"Combo x{combo}!";
        comboText.color = new Color(comboText.color.r, comboText.color.g, comboText.color.b, 0f);
        comboText.transform.localScale = Vector3.one;

        Sequence.Create()
            .Group(Tween.Color(comboText, new Color(comboText.color.r, comboText.color.g, comboText.color.b, 1f), 0.2f))
            .Group(Tween.Scale(comboText.transform, Vector3.one * 1.3f, 0.2f, Ease.OutBack))
            .ChainDelay(1.0f)
            .Chain(Tween.Color(comboText, new Color(comboText.color.r, comboText.color.g, comboText.color.b, 0f), 0.3f));
    }

    public void HideCombo()
    {
        comboText.text = "";
        comboText.color = new Color(comboText.color.r, comboText.color.g, comboText.color.b, 0f);
    }
}
