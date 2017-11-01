﻿using System.Collections;
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
                    DrawableView drawableView = drawingView
                        .InstantiatePrefab(cmd.previousDrawable, 
                                                    cmd.layerIndex);
                    drawableView.transform.SetSiblingIndex(cmd.drawableIndex);
                    drawableView.UpdateView(cmd.previousDrawable, 
                        graph.preferences, true);
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

            if (controllerDelegate != null)
                controllerDelegate.OnHistoryUndo(this);
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
                    DrawableView drawableView = drawingView
                        .InstantiatePrefab(cmd.currentDrawable, cmd.layerIndex);
                    drawableView.transform.SetSiblingIndex(cmd.drawableIndex);
                    drawableView.UpdateView(cmd.currentDrawable,
                        graph.preferences, true);
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
