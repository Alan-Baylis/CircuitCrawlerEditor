﻿#region License
/**
 * Copyright (c) 2010-2012 Lightning Development Studios <lightningdevelopmentstudios@gmail.com>
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
 * of the Software, and to permit persons to whom the Software is furnished to do
 * so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using CircuitCrawlerEditor.Entities;
using CircuitCrawlerEditor.Triggers;

namespace CircuitCrawlerEditor
{
	public class UICauseListEditor : UITypeEditor
	{
		public static List<Cause> causeList;

		private IWindowsFormsEditorService service;

		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (provider != null)
			{
				service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

				if (service != null)
				{
					ListBox list = new ListBox();
					list.Click += new EventHandler(list_Click);
					foreach (Cause c in causeList)
					{
						list.Items.Add(c);
					}

					service.DropDownControl(list);

					if (list.SelectedItem != null && list.SelectedIndices.Count == 1)
					{
						value = list.SelectedItem;
					}
				}
			}
			return value;
		}

		void list_Click(object sender, EventArgs e)
		{
			if (service != null)
				service.CloseDropDown();
		}

		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
	}

	public class UIEffectListEditor : UITypeEditor
	{
		public static List<Effect> effectList;

		private IWindowsFormsEditorService service;

		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (provider != null)
			{
				service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

				if (service != null)
				{
					ListBox list = new ListBox();
					list.Click += new EventHandler(list_Click);
					foreach (Effect e in effectList)
					{
						list.Items.Add(e);
					}

					service.DropDownControl(list);

					if (list.SelectedItem != null && list.SelectedIndices.Count == 1)
					{
						value = list.SelectedItem;
					}
				}
			}
			return value;
		}

		void list_Click(object sender, EventArgs e)
		{
			if (service != null)
				service.CloseDropDown();
		}

		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
	}

	public class UIEntListEditor : UITypeEditor
	{
		public static List<Entity> entList;

		private IWindowsFormsEditorService service;

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (provider != null)
			{
				service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

				if (service != null)
				{
					ListBox list = new ListBox();
					list.Click += new EventHandler(list_Click);
					foreach (Entity ent in entList)
					{
						if (value == null)
							list.Items.Add(ent);
						else if (value != null && ent.GetType().Equals(value.GetType()))
							list.Items.Add(ent);
					}

					service.DropDownControl(list);

					if (list.SelectedItem != null && list.SelectedIndices.Count == 1)
					{
						value = list.SelectedItem;
					}
				}
			}
			return value;
		}

		void list_Click(object sender, EventArgs e)
		{
			if (service != null)
				service.CloseDropDown();
		}

		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
	}

	

	public class UIFormEffectListEditor : UITypeEditor
	{
		public static List<Effect> effectList;

		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			IWindowsFormsEditorService svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if (svc != null)
			{
				FormEffectListEditor effectEdit = new FormEffectListEditor((List<Effect>)value, effectList);
				svc.ShowDialog(effectEdit);
				value = effectEdit.curList;
			}

			return value;
		}
	}
}
