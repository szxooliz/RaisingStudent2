using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Client
{
    public class Bubble : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI TMP_Line;
        
        public virtual void InitBubbleUI(UnitLog unitLog)
        {
            TMP_Line.text = unitLog.line;
        }
    }

}
