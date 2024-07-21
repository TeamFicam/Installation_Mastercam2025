// -----------------------------------------------------------------------------------------
// <copyright file="BatchImportDxf.csx" company="CNC Software, LLC">
//   Copyright (c) 2024 CNC Software, LLC
// </copyright>
// <Author>
//  sdk@Mastercam.com
// </Author>
// <summary>
//   Batch Import dxf files and save as native mastercam drawings.
//   Select folder containing dxf files and select a folder to save drawings.
// </summary>
// -----------------------------------------------------------------------------------------

//Assemblies Import for External Editor
#region Assemblies Import

// NOTE: Update paths to reference your Mastercam 2025 path
// #r "C:\Program Files\Mastercam 2025\NETHook3_0.dll"

#endregion

// Using for External Editor
#region Namespace Import
/*
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using Mastercam.IO;
*/
#endregion

// instantiate
var app = new App();

// Execute the script
app.Run();

#region Classes

/// <summary>
/// Defines our top level App class
/// </summary>
public class App
{
	///<summary>
	/// Main entry point to script
	/// </summary>
	public void Run()
	{
		// call main method.
		BatchImportFiles();
	}

	/// <summary>
	/// Batch import a folder of DXF files and save as Mastercam files.
	/// </summary>
	private void BatchImportFiles()
	{
		// dialog strings
		var title = "Batch Import Files";
		var description = "Choose Files Folder";
		var noFilesFound = "No files found";
		var fileSaveFailed = "Failed to save file.";
		var importComplete = "Import complete";
		var importError = "Error importing files:";
		var prompt = "Importing file {0} of {1}...\n{2}";

		// edit to suit the file type to import
		var searchPattern = "*.dxf";
		var searchExtension = ".dxf";

		// file type to save
		var mastercamExtension = ".mcam";

		try
		{
			FileManager.New();

			// browse for folder
			using (var browser = new FolderBrowserDialog())
			{
				browser.ShowNewFolderButton = false;
				browser.Description = description;
				if (browser.ShowDialog() == DialogResult.OK)
				{
					var folder = browser.SelectedPath;

					var files = Directory.GetFiles(folder, searchPattern, SearchOption.AllDirectories);
					if (!files.Any())
					{
						DialogManager.OK(noFilesFound, title);
						return;
					}

					var count = files.Length;
					int index = 0;

					foreach (var file in files)
					{
						index++;
						PromptManager.WriteString(string.Format(prompt, index, count, file));

						var success = FileManager.Open(file);
						if (success)
						{
							var mastercamFile = file.Replace(searchExtension, mastercamExtension);
							if (!FileManager.SaveAs(mastercamFile))
							{
								DialogManager.OK(fileSaveFailed, title);
								return;
							}
						}
					}

					FileManager.New();

					DialogManager.OK(importComplete, title);
				}
			}
		}
		catch (Exception e)
		{
			DialogManager.OK($"{importError} {e.Message}", title);
			return;
		}
		finally
		{
			//clean up
			PromptManager.Clear();
		}
	}
}

#endregion
