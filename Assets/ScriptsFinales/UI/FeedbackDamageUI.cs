using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FeedbackDamageUI : MonoBehaviour, IMessageListener
{
    public TextMeshProUGUI  damageLabel;

    public Color regularColor = new Color(1, 1, 1);
    public Color criticalColor = new Color(1, 1, 1);

    public Vector3 offset = new Vector3(.5f, .5f, 0);

    void Start()
    {
        this.Hide();

        MessageManager.current.AddListener(MessageTag.ITEMSKILL_DAMAGE, this);
    }

    public void Receive(Message msg)
    {
        ItemSkillDamageMessage sdm = msg as ItemSkillDamageMessage;
        this.Configure(sdm.receiver.transform.position, sdm.damage, sdm.critical);
    }

    public void Configure(Vector3 position, int damageAmount, bool isCritical)
    {
        this.transform.position = position + this.offset;

        this.damageLabel.text = damageAmount.ToString();

        if (isCritical)
            this.damageLabel.color = this.criticalColor;
        else
            this.damageLabel.color = this.regularColor;

        this.Show();
        Invoke("Hide", 2f);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}