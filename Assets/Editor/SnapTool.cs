// using UnityEditor;
// using UnityEngine;
//
// namespace Editor
// {
//     public static class SnapTool 
//     {
//         [MenuItem("CustomTools/SnapTool/Snap To Grid %&X", isValidateFunction:true)]
//         public static bool SnapToGridValidate()
//         {
//             return Selection.gameObjects.Length > 0; 
//         }
//     
//         [MenuItem("CustomTools/SnapTool/Snap To Grid %&X")]
//         public static void SnapToGrid()
//         {
//             foreach (var g in Selection.gameObjects)
//             {
//                 g.transform.position = g.transform.position.Round();
//             }
//         }
//
//         private static Vector3 Round(this Vector3 v)
//         {
//             v.x = Mathf.Round(v.x);
//             v.y = Mathf.Round(v.y);
//             v.z = Mathf.Round(v.z);
//             return v;
//         }
//     }
// }