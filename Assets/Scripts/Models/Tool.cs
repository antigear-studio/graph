using I2;

namespace Antigear.Graph {
    /// <summary>
    /// Enumerates the list of supported tools in the drawing space.
    /// </summary>
    public enum Tool {
        Unknown,
        StraightLine,
        BezierCurve,
        Arc,
        BSpline,
        FreeformLine,
        Pencil,
        Eraser,
        Text,
        Image,
        RectangleSelection,
        LassoSelection,
        Zoom,
        Pan
    }

    static class ToolMethods {
        public static string LocalizedName(this Tool tool) {
            switch (tool) {
                case Tool.StraightLine:
                    return I2.Loc.ScriptLocalization.Tool.StraightLine;
                case Tool.BezierCurve:
                    return I2.Loc.ScriptLocalization.Tool.Curve;
                case Tool.Arc:
                    return I2.Loc.ScriptLocalization.Tool.Arc;
                case Tool.BSpline:
                    return I2.Loc.ScriptLocalization.Tool.BSpline;
                case Tool.FreeformLine:
                    return I2.Loc.ScriptLocalization.Tool.FreeformLine;
                case Tool.Pencil:
                    return I2.Loc.ScriptLocalization.Tool.Pencil;
                case Tool.Eraser:
                    return I2.Loc.ScriptLocalization.Tool.Eraser;
                case Tool.Text:
                    return I2.Loc.ScriptLocalization.Tool.Text;
                case Tool.Image:
                    return I2.Loc.ScriptLocalization.Tool.Image;
                case Tool.RectangleSelection:
                    return I2.Loc.ScriptLocalization.Tool.RectangleSelection;
                case Tool.LassoSelection:
                    return I2.Loc.ScriptLocalization.Tool.LassoSelection;
                case Tool.Zoom:
                    return I2.Loc.ScriptLocalization.Tool.Zoom;
                case Tool.Pan:
                    return I2.Loc.ScriptLocalization.Tool.Pan;
                default:
                    return I2.Loc.ScriptLocalization.Tool.Unknown;
            }
        }
    }
}

