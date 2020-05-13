using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPanel : MonoBehaviour
{
    public enum ConfirmationPanelState
    {
        Sell,
        Buy,
        Delete,
        None
    }
    public ConfirmationPanelState CurrentState;

    public static ConfirmationPanel instance;

    private CanvasGroup canvas;
    private Text Message;
    private Slot CurrentItemSlot;
    private Button buttonYes;
    private Button buttonNo;
    private Button buttonOk;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else Debug.LogError("More than One Confirmation Panels!");
        canvas = GetComponent<CanvasGroup>();
        Message = transform.GetChild(0).GetComponent<Text>();
        buttonYes = transform.GetChild(1).GetComponent<Button>();
        buttonNo = transform.GetChild(2).GetComponent<Button>();
        buttonOk = transform.GetChild(3).GetComponent<Button>();
        buttonOk.gameObject.SetActive(false);
    }

    void Start()
    {
        canvas.alpha = 0;
        canvas.blocksRaycasts = false;
    }

    public void DisplayConfirmationPanel(Slot slot)
    {
        PlayerMovement.instance.enabled = false;
        PlayerData.instance.animator.SetFloat("SpeedMovement", 0);

        PlayerMovement.instance.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        GameObject.FindObjectOfType<PlayerCombat>().enabled = false;
        Message.text = "Would you like to ";
        canvas.blocksRaycasts = true;
        CurrentItemSlot = slot;
        switch (CurrentState)
        {
            case ConfirmationPanelState.Buy:
                Message.text += "buy " + slot.item.name + " for " + slot.item.BuyingPrice + " G?";
                break;

            case ConfirmationPanelState.Sell:
                Message.text += "sell " + slot.item.name + " for " + slot.item.SellingPrice + " G?";
                break;

            case ConfirmationPanelState.Delete:
                Message.text = "Are you sure you want to delete " + slot.item.name + " ?";
                break;

            case ConfirmationPanelState.None:
                break;
            default:
                break;
        }


        canvas.alpha = 1;

    }

    public void CompleteAction()
    {
        switch (CurrentState)
        {
            case ConfirmationPanelState.Buy:
                if (PlayerData.instance.IsItemAffordable(CurrentItemSlot.item.BuyingPrice))
                {
                    if (PlayerInventory.instance.CanAddItem(CurrentItemSlot.item))
                    {
                        PlayerData.instance.UpdateGold(-CurrentItemSlot.item.BuyingPrice);
                        PlayerInventory.instance.AddItem(CurrentItemSlot.item);
                        Cancel();
                    }
                    else
                    {
                        ActivateOK_Button(true);
                        Message.text = "You don't have enough space!";
                    }
                }
                else
                {
                    ActivateOK_Button(true);
                    Message.text = "You don't have enough gold!";
                }

                break;

            case ConfirmationPanelState.Sell:
                PlayerData.instance.UpdateGold(CurrentItemSlot.item.SellingPrice);
                PlayerInventory.instance.RemoveItem(CurrentItemSlot);
                Cancel();
                break;

            case ConfirmationPanelState.Delete:
                PlayerInventory.instance.RemoveItem(CurrentItemSlot);
                Cancel();
                break;

            case ConfirmationPanelState.None:
                break;
            default:
                break;
        }
    }

    public void Cancel()
    {
        PlayerMovement.instance.enabled = true;
        GameObject.FindObjectOfType<PlayerCombat>().enabled = true;

        Message.text = " ";
        canvas.alpha = 0;
        canvas.blocksRaycasts = false;
        CurrentItemSlot = null;
        CurrentState = ConfirmationPanelState.None;
        ActivateOK_Button(false);
    }

    private void ActivateOK_Button(bool enable)
    {
        if (enable)
        {
            buttonNo.gameObject.SetActive(false);
            buttonYes.gameObject.SetActive(false);
            buttonOk.gameObject.SetActive(true);
        }
        else
        {
            buttonNo.gameObject.SetActive(true);
            buttonYes.gameObject.SetActive(true);
            buttonOk.gameObject.SetActive(false);

        }
    }
}
