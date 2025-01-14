using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements
{
    public class DSMultipleChoiceNode : DSNode
    {
        public override void Init(Vector2 pos)
        {
            base.Init(pos);

            DialogueType = DSDialogueType.SingleChoice;
            Choices.Add("New Choice");
        }

        public override void Draw()
        {
            base.Draw();

            // Main Container
            Button addChoiceButton = new Button()
            {
                text = "Add Choice"
            };

            addChoiceButton.AddToClassList("ds-node__button");
            mainContainer.Insert(1, addChoiceButton);

            // Output Container
            foreach (string choice in Choices)
            {
                Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                choicePort.portName = "";

                Button deleteChoiceButton = new Button()
                {
                    text = "X"
                };
                deleteChoiceButton.AddToClassList("ds-node__button");


                TextField choiceTextField = new TextField()
                {
                    value = choice
                };

                choiceTextField.AddToClassList("ds-node__textfield");
                choiceTextField.AddToClassList("ds-node__choice-textfield");
                choiceTextField.AddToClassList("ds-node__textfield__hidden");

                choicePort.Add(choiceTextField);
                choicePort.Add(deleteChoiceButton);
                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }
    }
}
