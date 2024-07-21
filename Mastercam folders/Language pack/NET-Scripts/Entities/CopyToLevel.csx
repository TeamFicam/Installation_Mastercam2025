// -----------------------------------------------------------------------------------------
// <copyright file="CopyGeometryToLevel.csx" company="CNC Software, LLC">
//   Copyright (c) 2024 CNC Software, LLC
// </copyright>
// <Author>
//  sdk@Mastercam.com
// </Author>
// <summary>
//   Copies geometry to the specified level.
// </summary>
// -----------------------------------------------------------------------------------------

// Assemblies Import for External Editor
#region Assemblies Import

// NOTE: Update paths to reference your Mastercam 2025 path
// #r "C:\Program Files\Mastercam 2025\NETHook3_0.dll"

#endregion

// Using for External Editor
#region Namespace Import

// using System.Linq;

// using Mastercam.Database.Types;
// using Mastercam.GeometryUtility;
// using Mastercam.IO;
// using Mastercam.Support;

#endregion

#region Consts

const int level = 12;

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

#region Public Methods

	///<summary>
	/// Main entry point to script
	/// </summary>
	public void Run()
	{
		CopyGeometryToLevel(level);
	}

#endregion

#region Private Methods

	/// <summary> 
	/// Copies the geometry to level described by level. 
	/// </summary>
	///
	/// <param name="level"> The level to move the geometry to (0 = use the active main level). </param>
	private void CopyGeometryToLevel(int level)
	{
		if (!SearchManager.IsAnyGeometry())
		{
			DialogManager.OK("No geometry in current file.", "Mastercam");
			return;
		}

		// get all geometry
		var geometryToCopy = SearchManager.GetGeometry().ToList();

		// select it
		geometryToCopy.ForEach(
		g =>
			{
				g.Selected = true;
				g.Commit();
			});

		// copy selected
		var itemCount = GeometryManipulationManager.CopySelectedGeometryToLevel(level, false);
		if (itemCount == -1)
		{
			DialogManager.OK($"Failed to copy geometry to level {level}", "Mastercam");
			return;
		}

		// get the result
		var selection = SearchManager.GetResultGeometry().ToList();

		// change the colour
		selection.ForEach(
		e =>
			{
				e.Color = 12;
				e.Commit();
			});

		// clear the geometry
		GraphicsManager.ClearColors(new GroupSelectionMask(true));

		LevelsManager.RefreshLevelsManager();
	}

#endregion

}

#endregion
