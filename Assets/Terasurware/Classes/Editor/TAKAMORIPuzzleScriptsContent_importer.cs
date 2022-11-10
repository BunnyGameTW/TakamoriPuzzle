using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class TAKAMORIPuzzleScriptsContent_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/ExcelData/TAKAMORIPuzzleScriptsContent.xlsx";
	private static readonly string exportPath = "Assets/ExcelData/TAKAMORIPuzzleScriptsContent.asset";
	private static readonly string[] sheetNames = { "content", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			Entity_content data = (Entity_content)AssetDatabase.LoadAssetAtPath (exportPath, typeof(Entity_content));
			if (data == null) {
				data = ScriptableObject.CreateInstance<Entity_content> ();
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

					Entity_content.Sheet s = new Entity_content.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						Entity_content.Param p = new Entity_content.Param ();
						
					cell = row.GetCell(0); p.id = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.zh = (cell == null ? "" : cell.StringCellValue);
					p.chioce = new string[3];
					cell = row.GetCell(2); p.chioce[0] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.chioce[1] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(4); p.chioce[2] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(5); p.en = (cell == null ? "" : cell.StringCellValue);
					p.chioceEn = new string[3];
					cell = row.GetCell(6); p.chioceEn[0] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(7); p.chioceEn[1] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(8); p.chioceEn[2] = (cell == null ? "" : cell.StringCellValue);
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
