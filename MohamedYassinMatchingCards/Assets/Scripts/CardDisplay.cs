using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Image frontImage;
    public GameObject front;
    public GameObject back;

    private CardData data;

    public void SetData(CardData cardData)
    {
        data = cardData;
        frontImage.sprite = data.image;
        front.SetActive(false);
        back.SetActive(true);
    }

    public void Flip()
    {
        bool isShowingFront = front.activeSelf;
        front.SetActive(!isShowingFront);
        back.SetActive(isShowingFront);
    }

    public CardData GetData()
    {
        return data;
    }
}
