using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

public class CardDisplay : MonoBehaviour
{
    public Image frontImage;
    public GameObject front;
    public GameObject back;
   

    private CardData data;
    private bool isFaceUp;
    private bool isLocked;
    //data filling
    public void SetData(CardData cardData)
    {
        data = cardData;
        frontImage.sprite = data.image;
        isFaceUp = false;
        isLocked = false;
        front.SetActive(false);
        back.SetActive(true);
        transform.localRotation = Quaternion.identity;
    }

    public void OnCardClicked()
    {
        if (isLocked || isFaceUp)
            return;

        GameController.Instance.OnCardSelected(this);
    }
    //prime tween for easier and efficent code
    public void FlipToFront()
    {
        if (isLocked || isFaceUp)
            return;
        AudioController.Instance.PlayFlip();
        isFaceUp = true;
        Sequence.Create()
            .Group(Tween.LocalRotation(transform, new Vector3(0, 90, 0), 0.15f))
            .ChainCallback(() => { front.SetActive(true); back.SetActive(false); })
            .Group(Tween.LocalRotation(transform, Vector3.zero, 0.15f));
    }

    public void FlipToBack()
    {
        if (!isFaceUp)
            return;
        AudioController.Instance.PlayFlip();
        isFaceUp = false;
        Sequence.Create()
            .Group(Tween.LocalRotation(transform, new Vector3(0, 90, 0), 0.15f))
            .ChainCallback(() => { front.SetActive(false); back.SetActive(true); })
            .Group(Tween.LocalRotation(transform, Vector3.zero, 0.15f));
    }

    public CardData GetData()
    {
        return data;
    }

    public void Lock()
    {
        isLocked = true;

       

        Sequence.Create()
        .Group(Tween.ShakeScale(transform, new Vector3(0.1f, 0.1f, 0.1f), 0.2f))
        .Chain(Tween.Scale(transform, Vector3.zero, 0.3f, Ease.InBack))
        .ChainCallback(() => {
            front.SetActive(false);
            back.SetActive(false);
        });
    }

    public bool IsLocked()
    {
        return isLocked;
    }
    //for the load so it doesnt have
    // the annoying squares..
    public void ForceMatch()
    {
        isLocked = true;
        isFaceUp = true;
        front.SetActive(false);
        back.SetActive(false);
    }
}
