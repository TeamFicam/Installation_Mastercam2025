// -----------------------------------------------------------------------------------------
// <copyright file="MoveToLevel.csx" company="CNC Software, LLC">
//   Copyright (c) 2024 CNC Software, LLC
// </copyright>
// <Author>
//  sdk@mastercam.com
// </Author>
// <summary>
//   Moves all geometry to a level
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

// using Mastercam.IO;
// using Mastercam.Support;

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
		RunScript(100);
	}


#region private methods

	/// moves all geometry to the specified level
	private void RunScript(int level)
	{
		// NOTE: file needs to be open and contain geometry
		if (!SearchManager.IsAnyGeometry())
		{
			DialogManager.OK("No geometry in current file.", "Mastercam");
			return;
		}

		// get all geometry
		var geometry = SearchManager.GetGeometry().ToList();

		// using System.Linq
		geometry.ForEach(
		g =>
			{
				if (g.Level != level)
				{
					g.Level = level;
					g.Commit();
				}
			});

		// force an update to reflect the changes
		LevelsManager.RefreshLevelsManager();
	}

#endregion

}

#endregion
