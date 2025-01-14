using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements
{
    public class DSNode : Node
    {
        public string DialogueName { get; set; }
        public List<string> Choices { get; set; }
        public string Text {  get; set; }
        public DSDialogueType DialogueType { get; set; }

        /// <summary>
        /// 노드 초기화
        /// </summary>
        /// <param name="pos">노드 생성 위치</param>
        public virtual void Init(Vector2 pos)
        {
            DialogueName = "이벤트 이름";
            Choices = new List<string>();
            Text = "이벤트 대사 텍스트";

            SetPosition(new Rect(pos, Vector2.zero));

            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
        }

        /// <summary>
        /// 노드 내부 구성요소 UI 추가
        /// </summary>
        public virtual void Draw()
        {
            // 1. Title Container
            TextField dialogueNameTextField = new TextField()
            {
                value = DialogueName
            };

            dialogueNameTextField.AddToClassList("ds-node__textfield");
            dialogueNameTextField.AddToClassList("ds-node__filename-textfield");
            dialogueNameTextField.AddToClassList("ds-node__textfield__hidden");

            titleContainer.Insert(0, dialogueNameTextField);

            // 2. Input Container
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            inputPort.portName = "대사 연결";
            inputContainer.Add(inputPort);

            // 3. Extension Container
            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("ds-node__custom-data-container");

            Foldout textFoldOut = new Foldout()
            {
                text = "대화 텍스트"
            };

            TextField textField = new TextField()
            {
                value = Text
            };

            textField.AddToClassList("ds-node__textfield");
            textField.AddToClassList("ds-node__quote-textfield");

            textFoldOut.Add(textField);
            customDataContainer.Add(textFoldOut);
            extensionContainer.Add(customDataContainer);
        }
    }
}