using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Windows
{
    using Elements;
    public class DSGraphView : GraphView
    {
        public DSGraphView()
        {
            AddManipulators();
            AddGrid();
            AddStyles();
        }

        #region Overrided Method
        /// <summary>
        /// 포트 연결
        /// </summary>
        /// <param name="startPort"></param>
        /// <param name="nodeAdapter"></param>
        /// <returns></returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if(startPort == port)
                {
                    return;
                }

                if (startPort.node == port.node)
                {
                    return;
                }

                if (startPort.direction == port.direction)
                {
                    return;
                }

                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }
        #endregion

        #region Elements Addition
        /// <summary>
        /// 배경에 그리드 추가
        /// </summary>
        private void AddGrid()
        {
            var grid = new GridBackground();
            grid.StretchToParentSize();
            Insert(0, grid);
        }

        /// <summary>
        /// 색상, 그리드 라인 컬러 변경 내용 적용
        /// </summary>
        private void AddStyles()
        {
            StyleSheet graphViewStyleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/DialogueViewStyles.uss");
            StyleSheet nodeStyleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/DSNodeStyles.uss");

            styleSheets.Add(graphViewStyleSheet);
            styleSheets.Add(nodeStyleSheet);
        }
        #endregion

        #region Manipulators
        /// <summary>
        /// 조작 기능 추가
        /// </summary>
        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(CreateNodeContextualMenu("Add Node - Single Choice", DSDialogueType.SingleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Add Node - Multiple Choice", DSDialogueType.MultipleChoice));
            this.AddManipulator(CreateGroupContextualMenu());
        }

        /// <summary>
        /// 노드 생성 메뉴
        /// </summary>
        /// <param name="actionTitle"></param>
        /// <param name="dialogueType"></param>
        /// <returns></returns>
        private IManipulator CreateNodeContextualMenu(string actionTitle, DSDialogueType dialogueType)
        {
            // 마우스 위치에 노드 생성
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(dialogueType, actionEvent.eventInfo.localMousePosition)))
            );

            return contextualMenuManipulator;
        }

        /// <summary>
        /// 그룹 생성 메뉴
        /// </summary>
        /// <returns></returns>
        private IManipulator CreateGroupContextualMenu()
        {
            // 마우스 위치에 그룹 생성
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => AddElement(CreateGroup("DialogueGroup", actionEvent.eventInfo.localMousePosition)))
            );
            return contextualMenuManipulator;
        }
        #endregion

        #region Elements Creation
        /// <summary>
        /// 그룹 생성
        /// </summary>
        /// <param name="title"></param>
        /// <param name="localMousePosition"></param>
        /// <returns></returns>
        private Group CreateGroup(string title, Vector2 localMousePosition)
        {
            Group group = new Group()
            {
                title = title
            };

            group.SetPosition(new Rect(localMousePosition, Vector2.zero));

            return group;
        }

        /// <summary>
        /// 노드 생성
        /// </summary>
        private DSNode CreateNode(DSDialogueType dialogueType, Vector2 pos)
        {
            Type nodeType = Type.GetType($"DS.Elements.DS{dialogueType}Node");
            DSNode node = (DSNode)Activator.CreateInstance(nodeType);

            node.Init(pos);
            node.Draw();
            AddElement(node);

            return node;
        }

        #endregion
    }
}