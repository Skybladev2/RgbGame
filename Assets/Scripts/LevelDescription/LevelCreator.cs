using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class LevelCreator
{
    private IList<LevelDescription> levelDescriptions;
    private float nodeDistance;
    private Func<string, GameObject> instantiate;

    public LevelCreator(float nodeDistance, IList<LevelDescription> levelDescriptions, Func<string, GameObject> instantiateFunc)
    {
        this.nodeDistance = nodeDistance;
        this.levelDescriptions = levelDescriptions;
        this.instantiate = instantiateFunc;
    }

    public void CreateLevel(int levelIndex)
    {
        ClearSceneStructure();
        GameObject mainCamera = CreateCamera();
        if (levelIndex < 3)
        {
            CreateGridElements(levelIndex);
            CreateGlobalControllers(levelIndex);
            GameObject hero = CreateHero();
            SetCameraFollowHero(mainCamera, hero);
            CreateGui(mainCamera.GetComponent<Camera>());
        }
        else
        {
            Debug.Log("Demo End");
            CreateEndGui(mainCamera.GetComponent<Camera>());
            GameObject.Destroy(mainCamera.GetComponent<CameraFollow>());
        }
    }

    private void CreateEndGui(Camera camera)
    {
        instantiate("EventSystem");
        GameObject canvas = instantiate("CanvasEnd");
        canvas.GetComponent<Canvas>().worldCamera = camera;
    }

    private void CreateGui(Camera camera)
    {
        instantiate("EventSystem");
        GameObject canvas = instantiate("Canvas");
        canvas.GetComponent<Canvas>().worldCamera = camera;
    }

    private GameObject CreateCamera()
    {
        return instantiate("MainCamera");
    }

    private void SetCameraFollowHero(GameObject mainCamera, GameObject hero)
    {
        CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
        if (cameraFollow == null)
        {
            cameraFollow = mainCamera.AddComponent<CameraFollow>();
        }

        cameraFollow.target = hero.transform;
    }

    private GameObject CreateHero()
    {
        return instantiate("Hero");
    }

    private void CreateGridElements(int levelIndex)
    {
        GameObject grid = CreateGrid();
        Dictionary<int, GameObject> rows = CreateRows(grid, levelIndex);
        Dictionary<NodeDescription, GameObject> nodes = CreateNodes(rows, levelIndex);
        CreatePipeObjects(nodes);
    }

    private void CreateGlobalControllers(int levelIndex)
    {
        GameObject levelEndControllerObject = instantiate("LevelEndController");
        LevelEndController levelEndController = levelEndControllerObject.GetComponent<LevelEndController>();
        levelEndController.SetMovesLimit(levelDescriptions[levelIndex].MovesLimit);
        levelEndController.SetLevelIndex(levelIndex);

        instantiate("ColorFlowController");
    }

    public IEnumerable<PipeAnimator> CreatePipeAnimators(IEnumerable<Pipe> pipes, ICoroutineStarter coroutineStarter)
    {
        List<PipeAnimator> pipeAnimators = new List<PipeAnimator>(pipes.Count());
        foreach (var pipe in pipes)
        {
            PipeAnimator pipeAnimator = new PipeAnimator(pipe, null, coroutineStarter);
            pipeAnimators.Add(pipeAnimator);
        }

        return pipeAnimators;
    }

    public IEnumerable<Pipe> CreatePipes(Dictionary<NodeDescription, Node> nodes)
    {
        List<Pipe> pipes = new List<Pipe>();

        foreach (var nodeInfo in nodes)
        {
            if (nodeInfo.Key.PipeDescriptions == null
                ||
                !nodeInfo.Key.PipeDescriptions.Any())
            {
                continue;
            }

            foreach (var pipeInfo in nodeInfo.Key.PipeDescriptions)
            {
                Pipe pipe = new Pipe();
                pipe.SetStartNode(nodeInfo.Value);

                var endNodeInfo = nodes
                    .Keys
                    .Where(n => n.Column == pipeInfo.EndColumn && n.Row == pipeInfo.EndRow)
                    .FirstOrDefault();
                pipe.SetEndNode(nodes[endNodeInfo]);

                pipes.Add(pipe);
            }
        }

        return pipes;
    }

    private Dictionary<NodeDescription, GameObject> CreateNodes(Dictionary<int, GameObject> rows, int levelIndex)
    {
        Dictionary<NodeDescription, GameObject> nodes = new Dictionary<NodeDescription, GameObject>();

        foreach (KeyValuePair<int, GameObject> item in rows)
        {
            int rowIndex = item.Key;
            GameObject row = item.Value;

            NodeDescription[] rowNodes = levelDescriptions[levelIndex].Nodes.Where(n => n.Row == rowIndex).ToArray();

            Dictionary<NodeDescription, GameObject> rowNodesDictionary = CreateRowNodeGameObjects(rowNodes);
            SetParentRow(row, rowNodesDictionary.Values);

            foreach (KeyValuePair<NodeDescription, GameObject> nodeData in rowNodesDictionary)
            {
                nodes.Add(nodeData.Key, nodeData.Value);
            }
        }

        return nodes;
    }

    private void SetParentRow(GameObject row, IEnumerable<GameObject> nodes)
    {
        foreach (var node in nodes)
        {
            node.transform.SetParent(row.transform, true);
        }
    }

    public Dictionary<NodeDescription, GameObject> CreateRowNodeGameObjects(NodeDescription[] rowNodes)
    {
        Dictionary<NodeDescription, GameObject> nodeGameObjects = new Dictionary<NodeDescription, GameObject>();
        Dictionary<NodeDescription, Node> nodes = CreateRowNodes(rowNodes);

        foreach (var nodeInfo in nodes.Select(n => new { description = n.Key, node = n.Value }))
        {
            NodeDescription nodeDescription = nodeInfo.description;
            GameObject nodeGameObject = instantiate("Node");
            SetName(nodeGameObject, nodeDescription);

            SetCoords(nodeGameObject,
                nodeDescription.Column,
                nodeDescription.Row,
                nodeDistance);

            SetTargetColor(nodeGameObject, nodeDescription.TargetColor);
            nodeGameObjects.Add(nodeDescription, nodeGameObject);
        }

        return nodeGameObjects;
    }

    private void SetName(GameObject node, NodeDescription nodeDescription)
    {
        node.name = String.Format("Node ({0},{1})", nodeDescription.Column, nodeDescription.Row);
    }

    private void SetCoords(GameObject node, int column, int row, float nodeDistance)
    {
        NodeCoordsCalculator calculator = new NodeCoordsCalculator();
        node.transform.position = calculator.GetCoords(column, row, nodeDistance);
        node.GetComponent<NodeScript>().SetCoords(column, row);
    }

    public Dictionary<NodeDescription, Node> CreateRowNodes(NodeDescription[] rowNodesDescriptions)
    {
        Dictionary<NodeDescription, Node> nodes = new Dictionary<NodeDescription, Node>(rowNodesDescriptions.Length);

        for (int nodeIndex = 0; nodeIndex < rowNodesDescriptions.Length; nodeIndex++)
        {
            NodeDescription nodeDescription = rowNodesDescriptions[nodeIndex];
            Node node = new Node();
            node.SetCoords(nodeDescription.Column, nodeDescription.Row);
            nodes.Add(nodeDescription, node);
        }

        return nodes;
    }

    public void CreatePipeObjects(Dictionary<NodeDescription, GameObject> nodes)
    {
        foreach (KeyValuePair<NodeDescription, GameObject> nodeData in nodes)
        {
            if (nodeData.Key.PipeDescriptions == null)
            {
                continue;
            }

            NodeCoordsCalculator calculator = new NodeCoordsCalculator();
            NodeDescription nodeDescription = nodeData.Key;
            List<PipeDescription> pipes = nodeDescription.PipeDescriptions;

            for (int pipeIndex = 0; pipeIndex < pipes.Count; pipeIndex++)
            {
                PipeDescription pipeDescription = pipes[pipeIndex];
                GameObject pipe = instantiate("Pipe");

                Vector2 startCoords = calculator.GetCoords(
                    nodeDescription.Column,
                    nodeDescription.Row,
                    nodeDistance);

                Vector2 endCoords = calculator.GetCoords(
                    pipeDescription.EndColumn,
                    pipeDescription.EndRow,
                    nodeDistance);

                Vector2 middle = (startCoords + endCoords) / 2;
                Vector2 direction = endCoords - startCoords;

                pipe.transform.position = middle;
                pipe.transform.localRotation = Quaternion.FromToRotation(Vector2.right, direction);
                GameObject node = nodeData.Value;
                pipe.transform.SetParent(node.transform, true);

                NodeScript endNode = nodes
                    .Where(n => n.Key.Column == pipeDescription.EndColumn
                        && n.Key.Row == pipeDescription.EndRow)
                        .SingleOrDefault()
                        .Value
                        .GetComponent<NodeScript>();

                pipe.GetComponent<PipeScript>().SetEndNodeScript(endNode);
            }
        }
    }

    private void SetTargetColor(GameObject node, ColorType? colorType)
    {
        if (colorType == null)
            return;

        GameObject targetIndicator = instantiate("TargetIndicator");
        TargetColorScript targetScript = targetIndicator.GetComponentInChildren<TargetColorScript>();
        targetScript.TargetColorType = colorType.Value;
        targetIndicator.transform.SetParent(node.transform, false);
    }

    private GameObject CreateGrid()
    {
        return new GameObject("Grid");
    }

    private Dictionary<int, GameObject> CreateRows(GameObject grid, int levelIndex)
    {
        int[] rowIndices = levelDescriptions[levelIndex].Nodes.Select(n => n.Row).Distinct().OrderBy(r => r).ToArray();
        Dictionary<int, GameObject> rowsDictionary = new Dictionary<int, GameObject>(rowIndices.Length);

        for (int indexIndex = rowIndices.Length - 1; indexIndex >= 0; indexIndex--)
        {
            int rowIndex = rowIndices[indexIndex];
            GameObject row = new GameObject("Row " + rowIndex);
            row.transform.parent = grid.transform;
            rowsDictionary[rowIndex] = row;
        }

        return rowsDictionary;
    }

    private void ClearSceneStructure()
    {
        List<GameObject> sceneObjects = GameObject.FindObjectsOfType(typeof(GameObject)).Cast<GameObject>().ToList();
        foreach (var obj in sceneObjects)
        {
            GameObject.DestroyImmediate(obj);
        }
    }
}
