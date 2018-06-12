using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antigear.Graph {
    /// <summary>
    /// Manages the history of the objects. That is, a stack of repeatable
    /// commands for each action user does.
    /// </summary>
    public class HistoryController : MonoBehaviour {
        // Outlets.
        public IHistoryControllerDelegate controllerDelegate;

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

            if (cmd.type == Command.Type.CreateDrawable) {
                graph.content[cmd.layerIndex].RemoveAt(cmd.drawableIndex);
                Destroy(drawingView.paper.content.GetChild(cmd.layerIndex).GetChild(cmd.drawableIndex).gameObject);
            } else if (cmd.type == Command.Type.UpdateDrawable) {
                Drawable copy = cmd.previousDrawable.Copy();
                graph.content[cmd.layerIndex].RemoveAt(cmd.drawableIndex);
                graph.content[cmd.layerIndex].Insert(cmd.drawableIndex, copy);
                drawingView.paper.content.GetChild(cmd.layerIndex)
                    .GetChild(cmd.drawableIndex).GetComponent<DrawableView>()
                    .UpdateView(copy, graph.preferences, true);
            } else if (cmd.type == Command.Type.DeleteDrawable) {
                Drawable copy = cmd.previousDrawable.Copy();
                graph.content[cmd.layerIndex].Insert(cmd.drawableIndex, copy);
                DrawableView drawableView = drawingView.InstantiatePrefab(
                    cmd.previousDrawable, cmd.layerIndex);
                drawableView.transform.SetSiblingIndex(cmd.drawableIndex);
                drawableView.UpdateView(copy, graph.preferences, true);
            } else if (cmd.type == Command.Type.CreateLayer) {
                graph.content.RemoveAt(cmd.layerIndex);
                Destroy(drawingView.paper.content.GetChild(cmd.layerIndex).gameObject);
            } else if (cmd.type == Command.Type.UpdateLayer) {
                Layer copy = cmd.previousLayer.Copy();
                graph.content.RemoveAt(cmd.layerIndex);
                graph.content.Insert(cmd.layerIndex, copy);
                drawingView.LoadLayer(copy, cmd.layerIndex, graph.preferences);
            } else if (cmd.type == Command.Type.DeleteLayer) {
                Layer copy = cmd.previousLayer.Copy();
                graph.content.Insert(cmd.layerIndex, copy);
                drawingView.LoadLayer(copy, cmd.layerIndex, graph.preferences);
            }

            if (controllerDelegate != null)
                controllerDelegate.OnHistoryUndo(this);
        }

        public void Redo() {
            if (!CanRedo())
                return;

            Command cmd = future.Pop();
            history.Push(cmd);

            if (cmd.type == Command.Type.CreateDrawable) {
                Drawable copy = cmd.currentDrawable.Copy();
                graph.content[cmd.layerIndex].Insert(cmd.drawableIndex, copy);
                DrawableView drawableView = 
                    drawingView.InstantiatePrefab(copy, cmd.layerIndex);
                drawableView.transform.SetSiblingIndex(cmd.drawableIndex);
                drawableView.UpdateView(copy, graph.preferences, true);
            } else if (cmd.type == Command.Type.UpdateDrawable) {
                graph.content[cmd.layerIndex].RemoveAt(cmd.drawableIndex);
                Drawable copy = cmd.currentDrawable.Copy();
                graph.content[cmd.layerIndex].Insert(cmd.drawableIndex, copy);
                drawingView.paper.content.GetChild(cmd.layerIndex)
                    .GetChild(cmd.drawableIndex).GetComponent<DrawableView>()
                    .UpdateView(copy, graph.preferences, true);
            } else if (cmd.type == Command.Type.DeleteDrawable) {
                graph.content[cmd.layerIndex].RemoveAt(cmd.drawableIndex);
                Destroy(drawingView.paper.content.GetChild(cmd.layerIndex)
                    .GetChild(cmd.drawableIndex).gameObject);
            } else if (cmd.type == Command.Type.CreateLayer) {
                Layer copy = cmd.currentLayer.Copy();
                graph.content.Insert(cmd.layerIndex, copy);
                drawingView.LoadLayer(copy, cmd.layerIndex, graph.preferences);
            } else if (cmd.type == Command.Type.UpdateLayer) {
                Layer copy = cmd.currentLayer.Copy();
                graph.content.RemoveAt(cmd.layerIndex);
                graph.content.Insert(cmd.layerIndex, copy);
                drawingView.LoadLayer(copy, cmd.layerIndex, graph.preferences);
            } else if (cmd.type == Command.Type.DeleteLayer) {
                graph.content.RemoveAt(cmd.layerIndex);
                Destroy(drawingView.paper.content.GetChild(cmd.layerIndex)
                    .gameObject);
            }

            if (controllerDelegate != null)
                controllerDelegate.OnHistoryRedo(this);
        }

        /// <summary>
        /// Performs an action.
        /// </summary>
        /// <param name="cmd">Cmd.</param>
        public void Commit(Command cmd) {
            future.Clear();
            history.Push(cmd);

            if (controllerDelegate != null)
                controllerDelegate.OnHistoryCommit(this);
        }
    }

    public interface IHistoryControllerDelegate {
        /// <summary>
        /// Raised when the history has been undone.
        /// </summary>
        /// <param name="controller">Instance.</param>
        void OnHistoryUndo(HistoryController controller);

        /// <summary>
        /// Raised when the history has been redone.
        /// </summary>
        /// <param name="controller">Instance.</param>
        void OnHistoryRedo(HistoryController controller);

        /// <summary>
        /// Raised when the history has been made.
        /// </summary>
        /// <param name="controller">Instance.</param>
        void OnHistoryCommit(HistoryController controller);
    }
}
