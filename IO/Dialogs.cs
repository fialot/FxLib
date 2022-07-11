using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Fx.IO
{
    /// <summary>
    /// Dialogs
    /// Version:    1.0
    /// Date:       2015-06-26    
    /// </summary>
    public static class Dialogs
    {

        /// <summary>
        /// InputBox
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="promptText">Text</param>
        /// <param name="value">Value</param>
        /// <returns>Dialog result</returns>
        public static DialogResult InputBox(string title, string promptText, ref string value, bool pass = false)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            if (pass) textBox.UseSystemPasswordChar = true;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterParent;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;
            
            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        /// <summary>
        /// InputBox
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="promptText">Text</param>
        /// <param name="value">Value</param>
        /// <param name="promptText2">Text 2</param>
        /// <param name="value2">Value 2</param>
        /// <returns>Dialog result</returns>
        public static DialogResult InputBox2(string title, string promptText, ref string value, string promptText2, ref string value2)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Label label2 = new Label();
            TextBox textBox2 = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;
            label2.Text = promptText2;
            textBox2.Text = value2;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            label2.SetBounds(9, 60, 372, 13);
            textBox2.SetBounds(12, 76, 372, 20);
            buttonOk.SetBounds(228, 112, 75, 23); //72
            buttonCancel.SetBounds(309, 112, 75, 23);

            label.AutoSize = true;
            label2.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            textBox2.Anchor = textBox2.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 147); // 107
            form.Controls.AddRange(new Control[] { label, textBox, label2, textBox2, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterParent;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            value2 = textBox2.Text;
            return dialogResult;

        }

        public static DialogResult InputComboBox(string title, string promptText, string[] options, ref int value)
        {
            Form form = new Form();
            Label label = new Label();
            ComboBox comboBox = new ComboBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;

            comboBox.Items.AddRange(options);
            if (value >= 0 && value < comboBox.Items.Count)
                comboBox.SelectedIndex = value;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            comboBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            comboBox.Anchor = comboBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, comboBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterParent;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = comboBox.SelectedIndex;
            return dialogResult;
        }

        /// <summary>
        /// Show Error Dialog
        /// </summary>
        /// <param name="msg">Message</param>
        /// <returns>DialogResult</returns>
        public static DialogResult ShowError(string msg)
        {
            return MessageBox.Show(msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Show Error Dialog
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="caption">Caption</param>
        /// <returns>DialogResult</returns>
        public static DialogResult ShowError(string msg, string caption)
        {
            return MessageBox.Show(msg, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Show Warning Dialog
        /// </summary>
        /// <param name="msg">Message</param>
        /// <returns>DialogResult</returns>
        public static DialogResult ShowWarning(string msg)
        {
            return MessageBox.Show(msg, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Show Warning Dialog
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="caption">Caption</param>
        /// <returns>DialogResult</returns>
        public static DialogResult ShowWarning(string msg, string caption)
        {
            return MessageBox.Show(msg, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Show Question Dialog
        /// </summary>
        /// <param name="msg">Message</param>
        /// <returns>DialogResult</returns>
        public static DialogResult ShowQuestion(string msg)
        {
            return MessageBox.Show(msg, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>
        /// Show Question Dialog
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="caption">Caption</param>
        /// <returns>DialogResult</returns>
        public static DialogResult ShowQuestion(string msg, string caption)
        {
            return MessageBox.Show(msg, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>
        /// Show Info Dialog
        /// </summary>
        /// <param name="msg">Message</param>
        /// <returns>DialogResult</returns>
        public static DialogResult ShowInfo(string msg)
        {
            return MessageBox.Show(msg, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Show Info Dialog
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="caption">Caption</param>
        /// <returns>DialogResult</returns>
        public static DialogResult ShowInfo(string msg, string caption)
        {
            return MessageBox.Show(msg, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
