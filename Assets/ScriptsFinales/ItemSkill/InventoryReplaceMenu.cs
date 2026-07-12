using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryReplaceMenu : MonoBehaviour
{
    public static InventoryReplaceMenu current;

    [Header("Item Buttons")]
    public ItemSkillButton[] itemButtons;

    [Header("Cancel")]
    public Button cancelButton;

    private UnitCreature owner;
    private WorldItem worldItem;

    void Awake()
    {
        Debug.Log("InventoryReplaceMenu Awake");

        current = this;

        gameObject.SetActive(false);

        cancelButton.onClick.AddListener(Cancel);
    }

    public void Show(UnitCreature owner, WorldItem worldItem)
    {
        this.owner = owner;
        this.worldItem = worldItem;

        gameObject.SetActive(true);

        for (int i = 0; i < itemButtons.Length; i++)
        {
            if (i < owner.inventory.Count)
            {
                int index = i;
                ItemInstance item = owner.inventory[i];

                itemButtons[i].ConfigureReplace(
                    item.itemSkill.itemSkillName,
                    () => ReplaceItem(index)
                );

                itemButtons[i].gameObject.SetActive(true);
            }
            else
            {
                itemButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void ReplaceItem(int index)
    {
        owner.ReplaceItem(index, worldItem.itemSkill);

        GameManager.current.itemSpawner.RemoveItem(worldItem);

        Destroy(worldItem.gameObject);

        Hide();
    }

    private void Cancel()
    {
        Hide();
    }

    public void Hide()
    {
        owner = null;
        worldItem = null;

        gameObject.SetActive(false);
    }

}
