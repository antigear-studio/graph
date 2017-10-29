using I2.Loc;

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
        Pan,
        Zoom,
    }

    static class ToolMethods {
        public static string LocalizedName(this Tool tool) {
            switch (tool) {
                case Tool.StraightLine:
                    return LocalizationManager.GetTranslation(
                        "Tool/StraightLine");
                case Tool.BezierCurve:
                    return LocalizationManager.GetTranslation("Tool/Curve");
                case Tool.Arc:
                    return LocalizationManager.GetTranslation("Tool/Arc");
                case Tool.BSpline:
                    return LocalizationManager.GetTranslation("Tool/BSpline");
                case Tool.FreeformLine:
                    return LocalizationManager.GetTranslation(
                        "Tool/FreeformLine");
                case Tool.Pencil:
                    return LocalizationManager.GetTranslation("Tool/Pencil");
                case Tool.Eraser:
                    return LocalizationManager.GetTranslation("Tool/Eraser");
                case Tool.Text:
                    return LocalizationManager.GetTranslation("Tool/Text");
                case Tool.Image:
                    return LocalizationManager.GetTranslation("Tool/Image");
                case Tool.RectangleSelection:
                    return LocalizationManager.GetTranslation(
                        "Tool/RectangleSelection");
                case Tool.LassoSelection:
                    return LocalizationManager.GetTranslation(
                        "Tool/LassoSelection");
                case Tool.Zoom:
                    return LocalizationManager.GetTranslation("Tool/Zoom");
                case Tool.Pan:
                    return LocalizationManager.GetTranslation("Tool/Pan");
                default:
                    return LocalizationManager.GetTranslation("Tool/Unknown");
            }
        }
    }
}

