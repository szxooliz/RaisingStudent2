using DS.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DS.Elements
{
    public class DSSingleChoiceNode : DSNode
    {
        public override void Init(Vector2 pos)
        {
            base.Init(pos);

            DialogueType = DSDialogueType.SingleChoice;
            Choices.Add("Next Dialogue");
        }

        public override void Draw()
        {
            base.Draw();

            // Output Container
            foreach (string choice in Choices)
            {
                Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                choicePort.portName = choice;

                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }
    }
}
