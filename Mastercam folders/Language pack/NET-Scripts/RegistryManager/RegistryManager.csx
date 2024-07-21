//-----------------------------------------------------------------------------------------
// <copyright file="RegistryManager.csx" company="CNC Software, LLC">
//   Copyright (c) 2024 CNC Software, LLC
// </copyright>
// <Author>
//  sdk@mastercam.com
// </Author>
// <summary>
//   Gets some Mastercam information using the RegistryManager
// </summary>
// -----------------------------------------------------------------------------------------

// Assemblies Import for External Editor
#region Assemblies Import

// NOTE: Update paths to reference your Mastercam 2025 path
// #r "C:\Program Files\Mastercam 2025\NETHook3_0.dll"

#endregion

// Using for External Editor
#region Namespace Import

// StringBuilder
using System.Text;
/*
using System.Windows.Forms;
using Mastercam.Support;
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
		var lastInstalledVersion = RegistryManager.LastInstalledVersion();
		var mastercamKey = RegistryManager.MastercamKey();

		var serialNumber = Configuration.GetSerialNumber();
		var userType = Configuration.GetUserType();

		// format our message
		var builder = new StringBuilder();
		builder.Append($"last Installed Version:  {lastInstalledVersion}");
		builder.AppendLine();
		builder.Append($"Mastercam Key:  {mastercamKey}");
		builder.AppendLine();
		builder.Append($"Serial Number:  {serialNumber}");
		builder.AppendLine();
		builder.Append($"User Type:  {userType}");

		// display the message
		MessageBox.Show(builder.ToString(), "Mastercam", MessageBoxButtons.OK, MessageBoxIcon.Information);
	}
}

#endregion
