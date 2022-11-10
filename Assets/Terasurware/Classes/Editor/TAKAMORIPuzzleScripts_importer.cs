using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class TAKAMORIPuzzleScripts_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/ExcelData/TAKAMORIPuzzleScripts.xlsx";
	private static readonly string exportPath = "Assets/ExcelData/TAKAMORIPuzzleScripts.asset";
	private static readonly string[] sheetNames = { "all", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			Entity_all data = (Entity_all)AssetDatabase.LoadAssetAtPath (exportPath, typeof(Entity_all));
			if (data == null) {
				data = ScriptableObject.CreateInstance<Entity_all> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				data.hideFlags = HideFlags.NotEditable;
			}
			
			data.sheets.Clear ();
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				IWorkbook book = null;
				if (Path.GetExtension (filePath) == ".xls") {
					book = new HSSFWorkbook(stream);
				} else {
					book = new XSSFWorkbook(stream);
				}
				
				foreach(string sheetName in sheetNames) {
					ISheet sheet = book.GetSheet(sheetName);
					if( sheet == null ) {
						Debug.LogError("[QuestData] sheet not found:" + sheetName);
						continue;
					}

					Entity_all.Sheet s = new Entity_all.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						Entity_all.Param p = new Entity_all.Param ();
						
					cell = row.GetCell(0); p.episodeId = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.levelId = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.contentId = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.unlockLevelId = (int)(cell == null ? 0 : cell.NumericCellValue);
					p.chioceId = new int[3];
					cell = row.GetCell(4); p.chioceId[0] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.chioceId[1] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.chioceId[2] = (int)(cell == null ? 0 : cell.NumericCellValue);
						s.list.Add (p);
					}
					data.sheets.Add(s);
				}
			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
