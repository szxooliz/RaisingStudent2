using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class YellowTailBubble : Bubble
    {
        [SerializeField] Image IMG_Profile;
        [SerializeField] TextMeshProUGUI TMP_Name;

        public override void InitBubbleUI(UnitLog unitLog)
        {
            base.InitBubbleUI(unitLog);
            IMG_Profile.sprite = DataManager.Instance.GetOrLoadSprite(unitLog.charPath);
            TMP_Name.text = unitLog.name;
        }


    }
}
