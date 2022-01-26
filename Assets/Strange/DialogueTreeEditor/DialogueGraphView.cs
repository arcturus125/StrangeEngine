using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    const float StartNodePosX = 100;
    const float StartNodePosY = 200;
    const float StartNodeWidth = 100;
    const float StartNodeHeight= 150;
    public  Vector2 defaultNodeSize = new Vector2(150, 200);
    readonly Vector2 defaultNodePos = new Vector2(100, 100);

    public bool autosave = true;
    private bool activeFile = false;
    private string activePath = "";

    public DialogueGraphView()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));

        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale); // allows zooming of graph

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        GridBackground grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();
        activeFile = false;
        graphViewChanged = OnGraphChange;

        AddElement( GenerateEntryPointNode());
    }

    private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    public DialogueNode GenerateEntryPointNode()
    {
        // Generate a blank node
        DialogueNode node = new DialogueNode 
        {
            title = "START", // name of the node
            GUID = Guid.NewGuid().ToString(), // ID of the node
            DialogueText = "ENTRYPOINT", 
            entrypoint = true
        };

        // generate an output port
        Port port = GeneratePort(node, Direction.Output); 
        port.portName = "next"; // set the title of this port
        node.outputContainer.Add(port); // add the port to the node

        node.capabilities &= ~Capabilities.Deletable;
        node.capabilities &= ~Capabilities.Movable;

        // update the node (also removed the input section if not being used)
        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(StartNodePosX, StartNodePosY, StartNodeWidth, StartNodeHeight));
        return node;
    }

    public void GenerateEntryPointNode(string customID)
    {
        // Generate a blank node
        DialogueNode node = new DialogueNode
        {
            title = "START", // name of the node
            GUID = customID, // ID of the node
            DialogueText = "ENTRYPOINT",
            entrypoint = true
        };

        // generate an output port
        Port port = GeneratePort(node, Direction.Output);
        port.portName = "next"; // set the title of this port
        node.outputContainer.Add(port); // add the port to the node

        

        // update the node (also removed the input section if not being used)
        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(StartNodePosX, StartNodePosY, StartNodeWidth, StartNodeHeight));
        AddElement(node);
    }

    public void CreateNode(string nodename)
    {
        AddElement(CreateDialogueNode(nodename));
        OnUpdate();
    }

    public Quest nodeQuest;
    private  DialogueNode CreateDialogueNode(string nodeName)
    {
        DialogueNode newNode = new DialogueNode
        {
            title = nodeName,
            DialogueText = nodeName,
            GUID = Guid.NewGuid().ToString()
        };


        //          Input Port          //
        //////////////////////////////////
        Port inputPort = GeneratePort(newNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "input";
        newNode.inputContainer.Add(inputPort);


        //          text field          //
        //////////////////////////////////
        TextField textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt => //when the value of textfield is changed, run the below block
        {
            // update the title of the node
            newNode.title = evt.newValue;
            // change the text stored within the node
            newNode.DialogueText = evt.newValue;
            OnUpdate();
        });
        textField.SetValueWithoutNotify(newNode.title);
        newNode.extensionContainer.Add(textField);


        //          objectField          //
        ///////////////////////////////////
        ObjectField objectfield = new ObjectField("Quest");
        objectfield.objectType = typeof(Quest);

        objectfield.RegisterValueChangedCallback((quest) =>
        {
            newNode.quest = quest.newValue as Quest;
            OnUpdate();
        });

        newNode.extensionContainer.Add(objectfield);


        //          Button              //
        //////////////////////////////////
        Button newChoiceButton = new Button(() =>
        {
            AddChoicePort(newNode);
            OnUpdate();
        });
        newChoiceButton.text = "Add Output";
        newNode.titleContainer.Add(newChoiceButton);

        newNode.RegisterCallback((ContextualMenuPopulateEvent evt) =>
        {

            evt.menu.AppendAction("Copy GUID to Clipboard", (x) => 
            {
                Debug.Log("Copied GUID to clipboard: " + newNode.GUID);
                newNode.GetGUID();
            });
            OnUpdate();
        });

        // refresh whenever changes are made
        newNode.RefreshExpandedState();
        newNode.RefreshPorts();

        // set position of new node
        newNode.SetPosition(new Rect( new Vector2(-contentViewContainer.transform.position.x, -contentViewContainer.transform.position.y) + defaultNodePos, defaultNodeSize));

        return newNode;
    }

    private void AddChoicePort(DialogueNode dialogueNode, string portName = "")
    {
        Port outputPort = GeneratePort(dialogueNode, Direction.Output);

        var oldlabel = outputPort.contentContainer.Q<Label>("type");
        outputPort.contentContainer.Remove(oldlabel);

        int outputCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
        if (portName == "")
            portName = "Output " + outputCount;
        outputPort.portName = portName;
        dialogueNode.outputContainer.Add(outputPort);

        TextField textField = new TextField
        {
            name = string.Empty,
            value = portName
        };
        textField.RegisterValueChangedCallback(evt =>
        {
            outputPort.portName = evt.newValue;
            OnUpdate();
        });
        outputPort.contentContainer.Add(new Label("  "));
        outputPort.contentContainer.Add(textField);

        Button deleteButton = new Button(() => RemovePort(dialogueNode, outputPort))
        {
            text = "X"
        };
        outputPort.contentContainer.Add(deleteButton);

        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
    }

    private void RemovePort(DialogueNode dialogueNode, Port outputPort)
    {

        // before we can remove a port, we must first remove all the connections it has
        var targetEdge = edges.ToList().Where(x => x.output.portName == outputPort.portName && x.output.node == outputPort.node);
        if (targetEdge.Any())
        {
            Debug.LogError("if");
            Edge edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(edge);
        }

        // once the edjes are all removed, the port can be removed
        dialogueNode.outputContainer.Remove(outputPort);

        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        //return base.GetCompatiblePorts(startPort, nodeAdapter);
        List<Port> compatibleports = new List<Port>();

        ports.ForEach((port) => // fancy foreach statement
        {
            // startport = port at start of connection
            //      port = port at end of connection 

            if (startPort != port) // make sure a port doesn't go into itself (is this necessary?)
            {
                if (startPort.node != port.node) // make sure a node can not call itself
                {
                    compatibleports.Add(port);
                }
            }

        });

        return compatibleports;
    }

    /////////////////////////////////////////////////////
    //                SAVE AND LOAD                    //
    /////////////////////////////////////////////////////

    public void Save(string path)
    {
        List<NodeSaveState> graphNodes = new List<NodeSaveState>();
        //List<EdgeSaveState> graphEdges = new List<EdgeSaveState>();

        nodes.ForEach((node) => // the fancy way of saying 'foreach(var node in nodes)' but for UQueryStates
        {
            // get node data and store it
            NodeSaveState tempNode = new NodeSaveState();
            List<DialoguePort> ports = new List<DialoguePort>();

            tempNode.nodePos = node.GetPosition();
            tempNode.isEntryPoint = (node as DialogueNode).entrypoint;
            tempNode.dialogueText = (node as DialogueNode).DialogueText;
            tempNode.guid = (node as DialogueNode).GUID;
            tempNode.triggeredQuest = (node as DialogueNode).quest;

            // search through all the edges in the graph and gather the ones that connect to this node's output container
            List<Edge> nodeOutputs = edges.ToList().Where(x => (x.output.node as DialogueNode).GUID == (node as DialogueNode).GUID).ToList();
            nodeOutputs.Reverse(); // if you dont reverse the order, they are saved backwards from the order they are created, this created tangles in the edges
            nodeOutputs.ForEach((outputEdge) =>
            {
                DialoguePort tempPort = new DialoguePort();
                tempPort.inputID = (outputEdge.input.node as DialogueNode).GUID;
                tempPort.portName = outputEdge.output.portName;
                ports.Add(tempPort);
            });

            tempNode.ports = ports.ToArray();

            // add the node to the list that is saved
            graphNodes.Add(tempNode);
        });



        // create an instance of the DialogueGraphSave scriptableObject
        DialogueTree save = ScriptableObject.CreateInstance<DialogueTree>();
        // set the data to the nodes and edges we have just gathered
        save.nodes = graphNodes.ToArray();
        //save.connections = graphEdges.ToArray();

        string newpath = "Assets/" + path.Trim(Application.dataPath.ToCharArray()) + "asset";
        Debug.Log($" Dialogue Tree saved to {newpath}");
        // save the scriptableObject to a file
        AssetDatabase.CreateAsset(save,newpath);
        AssetDatabase.SaveAssets();


        activeFile = true;
        activePath = path;
    }



    public void Load(string filename)
    {
        ClearAll();

        Debug.Log($"Loading file: {filename}");
        // load the file from the resources folder
        DialogueTree save = Resources.Load<DialogueTree>(filename);


        // GENERATE NODES
        foreach(NodeSaveState node in save.nodes)
        {
            if (node.isEntryPoint)
            {
                GenerateEntryPointNode(node.guid);
            }
            else
            {
                // create a node and set the GUID so match the one we are loading from the file
                DialogueNode tempNode = CreateDialogueNode(node.dialogueText);
                tempNode.GUID = node.guid;
                tempNode.SetQuest(node.triggeredQuest);
                AddElement(tempNode); // actually add the node to the graph view

                foreach ( DialoguePort port in node.ports)
                {
                    AddChoicePort(tempNode, port.portName);
                }

                tempNode.SetPosition(node.nodePos);
            }
        }

        // CONNECT NODES

        List<Node> listOfNodes = nodes.ToList();
        // for each node on the graph
        for (int i = 0; i < listOfNodes.Count; i++)
        {
            DialogueNode node = listOfNodes[i] as DialogueNode;
            //if (node.entrypoint) continue;

            NodeSaveState nodeSave = save.nodes.First(x => x.guid == node.GUID); // get the save state of the node we are connecting up to the graph
            // loop through the ports saved in the node 
            foreach( DialoguePort port in nodeSave.ports)
            {
                // find the input node
                string targetNodeID = port.inputID;
                Node targetNode = listOfNodes.First(x => (x as DialogueNode).GUID == targetNodeID);

                //loop through all this node's port and find the one with the correct name
                for (int j = 0; j < node.outputContainer.childCount; j++)
                {
                    // if the portname on the graph matches the one in the save, draw the edge
                    if(node.outputContainer[j].Q<Port>().portName == port.portName)
                    {
                        LinkNodesTogether(node.outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);
                    }
                }
                
            }
        }


        // P:/Github/StrangeUnity/Assets/Resources/DialogueTrees/test.asset
        // DialogueTrees/test
        activeFile = true;
        activePath = $"{Application.dataPath}/Resources/{filename}.asset";
    }

    /// <summary>
    /// clears all nodes and edges from the graph EXCEPT the entrypoint node
    /// </summary>
    public void ClearGraph()
    {
        activeFile = false;
        // for all the nodes on the graph
        nodes.ForEach((node) =>
        {
            if (!(node as DialogueNode).entrypoint)// remove all nodes except the entyrypoint
            {
                // remove all edges on the node first
                edges.ToList().Where(x => x.input.node == node).ToList()
                    .ForEach(edge => RemoveElement(edge));
                // then remove the node
                RemoveElement(node);
            }

        });
    }
    /// <summary>
    /// clears all nodes and edges from the graph INCLUDING the entrypoint node
    /// </summary>
    public void ClearAll()
    {
        // for all the nodes on the graph
        nodes.ForEach((node) =>
        {
            // remove all edges on the node first
            edges.ToList().Where(x => x.input.node == node).ToList()
                .ForEach(edge => RemoveElement(edge));
            // then remove the node
            RemoveElement(node);
        });
    }

    /// <summary>
    /// used when loading in dialogue trees
    /// procedurally conencts two nodes with an edge
    /// </summary>
    /// <param name="outputSocket"> the port coming out of a node</param>
    /// <param name="inputSocket"> the port going in to a node</param>
    private void LinkNodesTogether(Port outputSocket, Port inputSocket)
    {
        var tempEdge = new Edge()
        {
            output = outputSocket,
            input = inputSocket
        };
        tempEdge?.input.Connect(tempEdge);
        tempEdge?.output.Connect(tempEdge);
        Add(tempEdge);
    }

    private GraphViewChange OnGraphChange(GraphViewChange change)
    {
        OnUpdate();
        return change;
    }
    private void OnUpdate()
    {
        if (autosave)
        {
            if (activeFile)
            {
                Save(activePath);
            }
        }
        else Debug.Log("Warning: Autosave is off, these changes will not be saved");
    }

}
