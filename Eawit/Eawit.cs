using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Eawit
{
    public partial class Eawit : Form
    {
        String fileName="untitled.txt";
        List<String> buffer = new List<String>();
        enum ControlMode{ None, ModeC, ModeQ, ModeX };
        ControlMode controlMode;
        int selectionStart;

        public Eawit()
        {
            InitializeComponent();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void aboutEawitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutEawit().Show();
        }

        private void readFile()
        {
            try
            {
                String text = File.ReadAllText(fileName);
                textBox.Text = text;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void openFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                fileName = dialog.FileName;
                readFile();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFile();
        }

        private void fileSave()
        {
            if(fileName=="untitled.txt")
            {
                fileSaveAs();
            }
            else
            {
                saveText();
            }
        }

        private void fileSaveAs()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                fileName = dialog.FileName;
                saveText();
            }
        }

        private void saveText()
        {
            try
            {
                String text = textBox.Text;
                File.WriteAllText(fileName, text);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileSave();
        }

        private void saveasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileSaveAs();
        }

        private void newFile()
        {
            if (textBox.Modified)
            {
                DialogResult res = MessageBox.Show("保存しますか？", "Warning", MessageBoxButtons.OKCancel);
                if (res == DialogResult.OK)
                {
                    fileSave();
                }
            }
            fileName = "untitled.txt";
            textBox.Clear();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newFile();
        }

        private void todoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newFile();
            fileName = "todo.txt";
            readFile();
        }

        private void moveCaret(int pos)
        {
            pos = Math.Min(pos, textBox.TextLength);
            pos = Math.Max(pos, 0);
            textBox.Select(pos, 0);
            //textBox.ScrollToCaret();
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            int row = textBox.GetLineFromCharIndex(textBox.SelectionStart)+1;
            int col = textBox.SelectionStart-textBox.GetFirstCharIndexOfCurrentLine()+1;
            int maxRow = textBox.Lines.Count();
            int orgPos = textBox.SelectionStart;

            if (Control.ModifierKeys==Keys.Control)
            {
                int pos = textBox.SelectionStart;
                int lineEnd = textBox.TextLength;
                if (row < maxRow)
                {
                    lineEnd = textBox.GetFirstCharIndexFromLine(row) - 1;
                }

                if (controlMode == ControlMode.ModeC)
                {

                    controlMode = ControlMode.None;
                }
                else if (controlMode == ControlMode.ModeQ)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.A:
                            pos = 0;
                            break;
                        case Keys.D:
                            int start = textBox.GetFirstCharIndexOfCurrentLine();
                            String lineStr = Environment.NewLine+textBox.Text.Substring(start, lineEnd - start);
                            textBox.Text = textBox.Text.Insert(lineEnd, lineStr);
                            pos = orgPos;
                            textBox.Modified = true;
                            break;
                        case Keys.E:
                            pos = textBox.TextLength;
                            break;
                        case Keys.T:
                            timestamp();
                            break;
                    }

                    switch (e.KeyCode)
                    {
                        case Keys.A:
                        case Keys.D:
                        case Keys.E:
                            moveCaret(pos);
                            break;
                    }

                    controlMode = ControlMode.None;
                }
                else if (controlMode == ControlMode.ModeX)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.C:
                            Dispose();
                            break;
                        case Keys.F:
                            openFile();
                            break;
                        case Keys.S:
                            fileSave();
                            break;
                    }
                    controlMode = ControlMode.None;
                }
                else
                {
                    switch (e.KeyCode)
                    {
                        case Keys.A:
                            pos = textBox.GetFirstCharIndexOfCurrentLine();
                            break;
                        case Keys.B:
                            pos--;
                            break;
                        case Keys.C:
                            controlMode = ControlMode.ModeC;
                            break;
                        case Keys.D:
                            if (orgPos < textBox.TextLength)
                            {
                                textBox.Text = textBox.Text.Remove(orgPos, 1);
                                pos = orgPos;
                            }
                            break;
                        case Keys.E:
                            pos = lineEnd;
                            break;
                        case Keys.F:
                            pos++;
                            break;
                        case Keys.K:
                            if(orgPos==lineEnd && 0<maxRow)
                            {
                                textBox.Text = textBox.Text.Remove(orgPos, 1);
                                pos = orgPos;
                            }
                            else
                            {
                                buffer.Add(textBox.Text.Substring(orgPos, lineEnd - orgPos));
                                textBox.Text = textBox.Text.Remove(orgPos, lineEnd - orgPos);
                                pos = orgPos;
                            }
                            break;
                        case Keys.H:
                            if (0 < orgPos)
                            {
                                textBox.Text = textBox.Text.Remove(orgPos - 1, 1);
                                pos = orgPos - 1;
                            }
                            break;
                        case Keys.L:
                            int cur = row + (int)((maxRow - row) * .5);
                            cur = Math.Min(cur, maxRow);
                            int tempCaretPos=textBox.GetFirstCharIndexFromLine(cur);
                            textBox.SelectionStart = tempCaretPos;
                            pos = orgPos;
                            break;
                        case Keys.M:
                            textBox.Text = textBox.Text.Insert(orgPos, Environment.NewLine);
                            pos = orgPos+1;
                            break;
                        case Keys.N:
                            if (row < maxRow)
                            {
                                pos = textBox.GetFirstCharIndexFromLine(row);
                                pos += col - 1;
                            }
                            break;
                        case Keys.P:
                            int currentRow = row - 1;
                            if (0 < currentRow)
                            {
                                pos = textBox.GetFirstCharIndexFromLine(currentRow - 1);
                                pos += col - 1;
                            }
                            break;
                        case Keys.Q:
                            controlMode = ControlMode.ModeQ;
                            break;
                        case Keys.V:
                            break;
                        case Keys.W:
                            if (0 <= selectionStart)
                            {
                                buffer.Add(textBox.Text.Substring(selectionStart, orgPos - selectionStart));
                                textBox.Text = textBox.Text.Remove(selectionStart, orgPos - selectionStart);
                                pos = selectionStart;
                            }
                            break;
                        case Keys.X:
                            controlMode = ControlMode.ModeX;
                            break;
                        case Keys.Y:
                            textBox.Text = textBox.Text.Insert(orgPos, buffer.Last());
                            pos = orgPos;
                            break;
                    }

                    // Keys for caret positioning
                    switch (e.KeyCode)
                    {
                        case Keys.A:
                        case Keys.B:
                        case Keys.D:
                        case Keys.E:
                        case Keys.F:
                        case Keys.K:
                        case Keys.H:
                        case Keys.L:
                        case Keys.M:
                        case Keys.N:
                        case Keys.P:
                        case Keys.W:
                        case Keys.Y:
                            moveCaret(pos);
                            break;
                    }
                    switch (e.KeyCode)
                    {
                        case Keys.D:
                        case Keys.K:
                        case Keys.H:
                        case Keys.M:
                        case Keys.Y:
                            textBox.Modified = true;
                            break;
                    }
                }
                // Disable the original 
                e.Handled = true;
            }
            statusText.Text = "Column "+col+" Line "+row+" / "+maxRow;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Redo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Paste();
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                if (e.KeyChar == ' ')
                {
                    selectionStart=textBox.SelectionStart;
                    e.Handled = true;
                }
            }
        }

        private delegate String ModifyString(String str);

        private String upperCase(String str)
        {
            return str.ToUpper();
        }

        private String lowerCase(String str)
        {
            return str.ToLower();
        }

        private String capitalize(String str)
        {
            int len = str.Length;
            if(0<len)
            {
                str = str.Substring(0, 1).ToUpper() + str.Substring(1);
            }
            return str;
        }

        private void applyChangesRegion(ModifyString func)
        {
            
            int pos = textBox.SelectionStart;
            String str = textBox.Text.Substring(pos, textBox.SelectionLength);
            str = func(str);
            textBox.Text = textBox.Text.Substring(0, pos) + str + textBox.Text.Substring(pos + textBox.SelectionLength);
            textBox.SelectionStart = pos;
        }


        private void uppercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModifyString func = upperCase;
            applyChangesRegion(func);
        }

        private void lowercaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModifyString func = lowerCase;
            applyChangesRegion(func);
        }

        private void capitalizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModifyString func = capitalize;
            applyChangesRegion(func);
        }

        private void timestampToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timestamp();
        }

        void timestamp()
        {
            String timestamp = DateTime.Today.ToString("ddd MMM dd");
            int pos = textBox.SelectionStart;
            textBox.Text = textBox.Text.Substring(0, pos) + timestamp + textBox.Text.Substring(pos);
            textBox.SelectionStart = pos + timestamp.Length;
        }
    }
}
