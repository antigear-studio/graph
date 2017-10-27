using UnityEngine;

namespace I2.Loc
{
	public static class ScriptLocalization
	{
		public static string Get(string Term, bool FixForRTL=true, int maxLineLengthForRTL=0, bool ignoreRTLnumbers=true, bool applyParameters=false, GameObject localParametersRoot=null, string overrideLanguage=null) { return LocalizationManager.GetTranslation(Term, FixForRTL, maxLineLengthForRTL, ignoreRTLnumbers, applyParameters, localParametersRoot, overrideLanguage); }


		public static class Tool
		{
			public static string Arc 		{ get{ return Get ("Tool/Arc"); } }
			public static string Curve 		{ get{ return Get ("Tool/Curve"); } }
			public static string Eraser 		{ get{ return Get ("Tool/Eraser"); } }
			public static string FreeformLine 		{ get{ return Get ("Tool/FreeformLine"); } }
			public static string Image 		{ get{ return Get ("Tool/Image"); } }
			public static string LassoSelection 		{ get{ return Get ("Tool/LassoSelection"); } }
			public static string Pan 		{ get{ return Get ("Tool/Pan"); } }
			public static string Pencil 		{ get{ return Get ("Tool/Pencil"); } }
			public static string RectangleSelection 		{ get{ return Get ("Tool/RectangleSelection"); } }
			public static string StraightLine 		{ get{ return Get ("Tool/StraightLine"); } }
			public static string Text 		{ get{ return Get ("Tool/Text"); } }
			public static string Unknown 		{ get{ return Get ("Tool/Unknown"); } }
			public static string Zoom 		{ get{ return Get ("Tool/Zoom"); } }
			public static string BSpline 		{ get{ return Get ("Tool/BSpline"); } }
		}

	}
}
