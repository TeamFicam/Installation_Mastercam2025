// -----------------------------------------------------------------------------------------
// <copyright file="pick_geometry.csx" company="CNC Software, LLC">
//   Copyright (c) 2024 CNC Software, LLC
// </copyright>
// <Author>
//  sdk@Mastercam.com
// </Author>
// <summary>
//   Create two points, draw a line between them and prompt the user to select the mid-point 
//   of the line...
// </summary>
// -----------------------------------------------------------------------------------------

// Assemblies Import for External Editor
#region Assemblies Import

// NOTE: Update paths to reference your Mastercam 2025 path
// #r "C:\Program Files\Mastercam 2025\NETHook3_0.dll"

#endregion

// Using for External Editor
#region Namespace Import

// using Mastercam.IO;
// using Mastercam.BasicGeometry;
// using Mastercam.Curves;

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
		FileManager.New();

		// Create 2 points
		PointGeometry pt1 = new PointGeometry();
		PointGeometry pt2 = new PointGeometry(1, 1, 0);
		pt2.Color = 12;
		pt1.Commit();
		pt2.Commit();

		// Create a line between the points
		LineGeometry line1 = new LineGeometry(pt1.Data, pt2.Data);
		line1.Commit();

		// Ask user to select mid point of our line
		AskForGeometry();

		SelectionManager.UnselectAllGeometry();

		GraphicsManager.ClearColors(new Mastercam.Database.Types.GroupSelectionMask(true));
		GraphicsManager.Repaint(true);
	}

#endregion

#region Private Methods

	/// <summary>
	/// Prompt user to select a line
	/// </summary>
	private void AskForGeometry()
	{
		var geometry = SelectionManager.AskForGeometry("Select Line", new Mastercam.Database.Types.GeometryMask(false, true, false, false, false, false, false, false));
		if (geometry != null)
		{
			DialogManager.OK($"Line on level: {geometry.Level}", "Mastercam");
		}
		else
		{
			DialogManager.OK("No geometry selected", "Mastercam");
		}
	}

#endregion

}

#endregion
