// -----------------------------------------------------------------------------------------
// <copyright file="filter.csx" company="CNC Software, LLC">
//   Copyright (c) 2024 CNC Software, LLC
// </copyright>
// <Author>
//  sdk@Mastercam.com
// </Author>
// <summary>
//   Shows how to filter lines and arcs and move to a level
// </summary>
// -----------------------------------------------------------------------------------------

// Assemblies Import for External Editor
#region Assemblies Import

// NOTE: Update paths to reference your Mastercam 2025 path
// #r "C:\Program Files\Mastercam 2025\NETHook3_0.dll"

#endregion

// Using for External Editor
#region Namespace Import

// using System;
// using System.Collections.Generic;
// using System.Linq;

// using Mastercam.Curves;
// using Mastercam.Database;
// using Mastercam.IO;
// using Mastercam.Support;

#endregion

#region Consts

private const int OuterBoundaryLevel = 10;

private const int ArcMatchTolerance = 1;

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
#region Public Method

	///<summary>
	/// Main entry point to script
	/// </summary>
	public void Run()
	{
		RunScript();
	}

#endregion

#region private methods

	///<summary>
	/// Main entry point to the script
	/// </summary>
	private void RunScript()
	{
		// NOTE: file needs to be open and contain geometry
		if (!SearchManager.IsAnyGeometry())
		{
			DialogManager.OK("No geometry in current file.", "Mastercam");
			return;
		}

		// get the geometry
		var geometry = SearchManager.GetGeometry();

		// move the geometry
		var levels = FilterAndMoveGeometry(geometry);

		// force level manager to redraw to show updates
		LevelsManager.RefreshLevelsManager();

		// The number of levels that were populated with data.
		DialogManager.OK($"{levels} levels updated.", "Mastercam");
	}

	/// <summary> Filter all of the supplied geometry. </summary>
	///
	/// <remarks> Places all the lines on their own (outer boundary) level.
	///           Places all NON full circles (arcs) on the same levels as the lines.
	///           Places the Arcs on levels, sorted by their size. </remarks>
	///
	/// <param name="geometry"> The geometry to be filtered. </param>
	///
	/// <returns> The number of levels that were populated with data. </returns>
	private int FilterAndMoveGeometry(Geometry[] geometry)
	{
		var newLevels = 0;

		// lines and partial arcs
		var lines = FilterLines(geometry);

		// full arcs
		var arcs = FilterArcs(geometry);

		if (lines.Count > 0 || arcs.Count > 0)
		{
			if (lines.Count > 0)
			{
				MoveToLevel(lines, OuterBoundaryLevel, "OUTER level");
			}

			if (arcs.Count > 0)
			{
				MoveToLevel(arcs, OuterBoundaryLevel, "OUTER level");
			}

			newLevels++;
		}

		var circles = FilterCircles(geometry, ArcMatchTolerance);
		if (circles.Any())
		{
			var level = 0;

			// key value pair
			foreach (var kvp in circles)
			{
				MoveToLevel(kvp.Value.ToArray(), level++, kvp.Key);
				newLevels++;
			}
		}

		return newLevels;
	}

	/// <summary> Filter just the lines in the supplied list of geometry. </summary>
	///
	/// <param name="entities"> The geometry to be processed. </param>
	///
	/// <returns> An array containing just the lines found in the supplied geometry. </returns>
	private List<Geometry> FilterLines(Geometry[] entities)
	{
		return entities.Where(l => l is LineGeometry).ToList();
	}

	/// <summary> Filter the arcs to find all NON full circle entities. </summary>
	///
	/// <param name="entities"> The arc entities to the processed. </param>
	///
	/// <returns> A list of the NON full circle arcs.  </returns>
	private List<Geometry> FilterArcs(Geometry[] entities)
	{
		var arcs = entities.Where(a => a is ArcGeometry arc &&
	Math.Abs(arc.Data.EndAngleDegrees - arc.Data.StartAngleDegrees) < SettingsManager.SystemTolerance).ToList();

		return arcs;
	}

	/// <summary> Filter the full circles by size. </summary>
	///
	/// <remarks> Arcs that are NOT full circles are NOT processed! </remarks>
	///
	/// <param name="entities">  The arc entities to the processed. </param>
	/// <param name="tolerance"> The arc radius match tolerance. </param>
	///
	/// <returns> A data dictionary, where... Key = arcs size. Value = the arcs entities of this size. </returns>
	private Dictionary<string, List<Geometry>> FilterCircles(Geometry[] entities, int tolerance)
	{
		var filteredCircles = new Dictionary<string, List<Geometry>>();
		var fmt = new string('0', tolerance);
		fmt = "{0:0." + fmt + "}";
		foreach (var entity in entities)
		{
			if (entity is ArcGeometry arc)
			{
				if (Math.Abs((arc.Data.EndAngleDegrees - arc.Data.StartAngleDegrees) - 360.0)
				< SettingsManager.SystemTolerance)
				{
					var radius = string.Format(fmt, arc.Data.Radius);
					if (!filteredCircles.ContainsKey(radius))
					{
						filteredCircles[radius] = new List<Geometry>();
					}

					filteredCircles[radius].Add(arc);
				}
			}
		}

		return filteredCircles;
	}

	/// <summary> Move the supplied geometry to the specified level. </summary>
	///
	/// <param name="geometry"> The geometry to be processed. </param>
	/// <param name="level">    The level to move the geometry to (0 = use the active main level). </param>
	/// <param name="name">     The name to set on the target level (empty string means do not set the name). </param>
	private void MoveToLevel(IEnumerable<Geometry> geometry, int level, string name)
	{
		if (level < 1)
		{
			level = LevelsManager.GetMainLevel();
		}

		foreach (var entity in geometry)
		{
			entity.Level = level;
			entity.Commit();
		}

		if (!string.IsNullOrWhiteSpace(name))
		{
			LevelsManager.SetLevelName(level, name);
		}
	}

#endregion

}

#endregion
