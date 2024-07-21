// -----------------------------------------------------------------------------------------
// <copyright file="MastercamDialogs.csx" company="CNC Software, LLC">
//   Copyright (c) 2024 CNC Software, LLC
// </copyright>
// <Author>
//  sdk@Mastercam.com
// </Author>
// <summary>
//   Demonstrates the DialogManager class methods.
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

// using Mastercam.App.Exceptions;
// using Mastercam.IO;
// using Mastercam.IO.Types;

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
		ScriptRun();
	}

#region Private Methods

	/// <summary>
	/// Runs the script and displays various messages
	/// </summary>
	private void ScriptRun()
	{
		var dialogs = new MastercamDialogs();

		dialogs.ShowMessageBox("Hello World!");

		dialogs.ShowErrorMessageBox("An error has occured!");

		dialogs.ShowExceptionMessageBox();

		dialogs.ShowYesNoMessageBox();

		dialogs.ShowYesNoCancelMessageBox();

		dialogs.ShowAskForColor();

		dialogs.ShowAskForAngle();

		dialogs.ShowAskForDoubleBetween1And100();
	}

#endregion

}

/// <summary>
/// Defines our MastercamDialogs class
/// </summary>
public class MastercamDialogs
{
#region Private Fields

	/// <summary>
	/// Backing field for the Title string
	/// </summary>
	private const string Title = "Mastercam Dialogs";

#endregion

#region Public Methods

	/// <summary> Shows the message box. </summary>
	public void ShowMessageBox(string message) => DialogManager.OK(message, Title);

	/// <summary> Shows the error message box. </summary>
	public void ShowErrorMessageBox(string message) => DialogManager.Error(message, Title);

	/// <summary> Shows the exception message box. </summary>
	///
	/// <exception cref="MastercamException"> Thrown when a Mastercam error condition occurs. </exception>
	/// <exception cref="Exception">          Thrown when an exception error condition occurs. </exception>
	///
	/// <param name="throwMastercam"> (Optional) True to throw mastercam. </param>
	public void ShowExceptionMessageBox(bool throwMastercam = true)
	{
		try
		{
			if (throwMastercam)
			{
				throw new MastercamException("Mastercam Exception raised");
			}

			throw new Exception("System Exception raised");
		}
		catch (MastercamException e)
		{
			DialogManager.Exception(e);
		}
		catch (Exception e)
		{
			DialogManager.Error(e.Message, Title);
		}
	}

	/// <summary> Shows the yes no message box. </summary>
	public void ShowYesNoMessageBox()
	{
		var result = DialogManager.YesNo("Yes/No message", Title);
		DialogManager.OK(result == DialogReturnType.Yes ?
				"Yes clicked" :
			"No clicked", Title);
	}

	/// <summary> Shows the yes no cancel message box. </summary>
	public void ShowYesNoCancelMessageBox()
	{
		var result = DialogManager.YesNoCancel("Yes/No/Cancel message", Title);
		switch (result)
		{
			case DialogReturnType.Yes:
				DialogManager.OK("Yes clicked", Title);
				break;

			case DialogReturnType.No:
				DialogManager.OK("No clicked", Title);
				break;

			case DialogReturnType.Cancel:
				DialogManager.OK("Cancel clicked", Title);
				break;
		}
	}

	/// <summary> Shows the ask for color dialog. </summary>
	public void ShowAskForColor()
	{
		var color = -1;
		var result = DialogManager.AskForColor(ref color);
		DialogManager.OK(result == DialogReturnType.Okay ?
				$"Color at index {color}" :
			"No color selected", Title);
	}

	/// <summary> Prompts user for an angle. </summary>
	public void ShowAskForAngle()
	{
		var angle = 0.0;
		var result = DialogManager.AskForAngle("Enter an angle", ref angle);
		DialogManager.OK(result == DialogReturnType.Okay ?
				$"Angle is {angle}" :
			"No angle input", Title);
	}

	/// <summary>
	/// Prompts user for a number between 1 and 100
	/// </summary>
	public void ShowAskForDoubleBetween1And100()
	{
		var number = 1.0;
		var result = DialogManager.AskForNumber(Title, 1, 100, ref number);
		DialogManager.OK(result == DialogReturnType.Okay ?
				$"Number is {number}" :
			"No valid number input", Title);
	}

#endregion
}

#endregion
