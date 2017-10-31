using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Manages the history of the objects. That is, a stack of repeatable
    /// commands for each action user does.
    /// </summary>
    public class HistoryController : MonoBehaviour {
        // Used to keep track of which actions can be done.
        Stack<Command> history;
        Stack<Command> future;
        Graph graph;
        DrawingView drawingView;

        /// <summary>
        /// Loads the history of the graph to be managed by this controller.
        /// </summary>
        /// <param name="graph">Graph.</param>
        /// <param name = "drawingView"></param>
        public void SetupController(Graph graph, DrawingView drawingView) {
            this.graph = graph;
            this.drawingView = drawingView;
            history = graph.history;
            future = graph.future;
        }

        public bool CanUndo() {
            return history.Count > 0;
        }

        public bool CanRedo() {
            return future.Count > 0;
        }

        public void Undo() {
            if (!CanUndo())
                return;

            Command cmd = history.Pop();
            future.Push(cmd);

            switch (cmd.type) {
                case Command.Type.CreateDrawable:
                    graph.content[cmd.layerIndex]
                        .RemoveAt(cmd.drawableIndex);
                    Destroy(drawingView.paper.content.GetChild(cmd.layerIndex)
                        .GetChild(cmd.drawableIndex).gameObject);
                    break;
                case Command.Type.UpdateDrawable:
                    graph.content[cmd.layerIndex]
                        .RemoveAt(cmd.drawableIndex);
                    graph.content[cmd.layerIndex]
                        .Insert(cmd.drawableIndex, cmd.previousDrawable);
                    drawingView.paper.content.GetChild(cmd.layerIndex)
                        .GetChild(cmd.drawableIndex)
                        .GetComponent<DrawableView>()
                        .UpdateView(cmd.previousDrawable, graph.preferences, 
                            true);
                    break;
                case Command.Type.DeleteDrawable:
                    graph.content[cmd.layerIndex]
                        .Insert(cmd.drawableIndex, cmd.previousDrawable);
                    drawingView
                        .InstantiatePrefab(cmd.previousDrawable, cmd.layerIndex)
                        .transform.SetSiblingIndex(cmd.drawableIndex);
                    break;
                case Command.Type.CreateLayer:
                    graph.content.RemoveAt(cmd.layerIndex);
                    Destroy(drawingView.paper.content.GetChild(cmd.layerIndex)
                        .gameObject);
                    break;
                case Command.Type.UpdateLayer:
                    graph.content.RemoveAt(cmd.layerIndex);
                    graph.content.Insert(cmd.layerIndex, cmd.previousLayer);
                    drawingView.LoadLayer(cmd.previousLayer, cmd.layerIndex, 
                        graph.preferences);
                    break;
                case Command.Type.DeleteLayer:
                    graph.content.Insert(cmd.layerIndex, 
                        cmd.previousLayer);
                    drawingView.LoadLayer(cmd.previousLayer, cmd.layerIndex, 
                        graph.preferences);
                    break;
            }
        }

        public void Redo() {
            if (!CanRedo())
                return;

            Command cmd = future.Pop();
            history.Push(cmd);

            switch (cmd.type) {
                case Command.Type.CreateDrawable:
                    graph.content[cmd.layerIndex]
                        .Insert(cmd.drawableIndex, cmd.currentDrawable);
                    drawingView
                        .InstantiatePrefab(cmd.currentDrawable, cmd.layerIndex)
                        .transform.SetSiblingIndex(cmd.drawableIndex);
                    break;
                case Command.Type.UpdateDrawable:
                    graph.content[cmd.layerIndex]
                        .RemoveAt(cmd.drawableIndex);
                    graph.content[cmd.layerIndex]
                        .Insert(cmd.drawableIndex, cmd.currentDrawable);
                    drawingView.paper.content.GetChild(cmd.layerIndex)
                        .GetChild(cmd.drawableIndex)
                        .GetComponent<DrawableView>()
                        .UpdateView(cmd.currentDrawable, graph.preferences, 
                            true);
                    break;
                case Command.Type.DeleteDrawable:
                    graph.content[cmd.layerIndex]
                        .RemoveAt(cmd.drawableIndex);
                    Destroy(drawingView.paper.content.GetChild(cmd.layerIndex)
                        .GetChild(cmd.drawableIndex).gameObject);
                    break;
                case Command.Type.CreateLayer:
                    graph.content.Insert(cmd.layerIndex, 
                        cmd.currentLayer);
                    drawingView.LoadLayer(cmd.currentLayer, cmd.layerIndex, 
                        graph.preferences);
                    break;
                case Command.Type.UpdateLayer:
                    graph.content.RemoveAt(cmd.layerIndex);
                    graph.content.Insert(cmd.layerIndex, cmd.currentLayer);
                    drawingView.LoadLayer(cmd.currentLayer, cmd.layerIndex, 
                        graph.preferences);
                    break;
                case Command.Type.DeleteLayer:
                    graph.content.RemoveAt(cmd.layerIndex);
                    Destroy(drawingView.paper.content.GetChild(cmd.layerIndex)
                        .gameObject);
                    break;
            }
        }

        /// <summary>
        /// Performs an action.
        /// </summary>
        /// <param name="cmd">Cmd.</param>
        public void Commit(Command cmd) {
            future.Clear();
            history.Push(cmd);
        }
    }
}
