using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lovatto.MobileInput
{
    public class WeaponInfoUI : MonoBehaviour
    {
        public static WeaponInfoUI Instance { get; private set; }

        void OnEnable()
        {
            Instance = this;
            SetAutoFire(false);
        }

        public void UpdateAmmoAmount(int amount, int magazine)
        {
            bl_SlotSwitcher.Instance.AmmoText.text = $"{amount}/{magazine}";
        }

        public void UpdateAmmoAmountBar(int amount, int maxAmount)
        {
            if (maxAmount.Equals(0))
                return;
            Debug.Log($"Amount : {amount} | Max Amount : {maxAmount}");
            bl_SlotSwitcher.Instance.amountBar.localScale = new Vector3(amount / (float)maxAmount, 1, 1);
        }

        public void SetAutoFire(bool value)
        {
            bl_MobileInput.EnableAutoFire = value;
        }
    }
}