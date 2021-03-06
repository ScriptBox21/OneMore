﻿//************************************************************************************************
// Copyright © 2020 Steven M Cohn.  All rights reserved.
//************************************************************************************************

#pragma warning disable CS3003 // Type is not CLS-compliant

namespace River.OneMoreAddIn.Commands
{
	using River.OneMoreAddIn.Dialogs;
	using System;
	using Resx = River.OneMoreAddIn.Properties.Resources;


	internal partial class InsertCellsDialog : LocalizableForm
	{
		public InsertCellsDialog()
		{
			InitializeComponent();

			if (NeedsLocalizing())
			{
				//Text = Resx.InsertCellsDialog_Text;

				Localize(new string[]
				{
					"shiftDownRadio",
					"shiftRightRadio",
					"numLabel",
					"okButton",
					"cancelButton"
				});
			}
		}

		protected override void OnShown(EventArgs e)
		{
			Location = new System.Drawing.Point(Location.X, Location.Y - (Height / 2));
			UIHelper.SetForegroundWindow(this);
		}


		public bool ShiftDown => shiftDownRadio.Checked;


		public int NumCells => (int)numCellsBox.Value;
	}
}
