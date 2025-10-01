using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MaethrillianInstaller.GUI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            toolTip1 = new ToolTip(components);
            connectivityTimer = new System.Windows.Forms.Timer(components);
            mainLayout = new TableLayoutPanel();
            imageBox = new PictureBox();
            modGroup = new GroupBox();
            modListPanel = new FlowLayoutPanel();
            modInfoGroup = new GroupBox();
            modInfoTextBox = new TextBox();
            bottomLayout = new TableLayoutPanel();
            linkDiscord = new LinkLabel();
            buttonPTR = new CheckBox();
            installButton = new Button();
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            installProgressBar = new ToolStripProgressBar();
            mainLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)imageBox).BeginInit();
            modGroup.SuspendLayout();
            bottomLayout.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            //
            // connectivityTimer
            //
            connectivityTimer.Interval = 5000;
            //
            // mainLayout
            mainLayout.ColumnCount = 1;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.Controls.Add(imageBox, 0, 0);
            mainLayout.Controls.Add(modGroup, 0, 1);
            mainLayout.Controls.Add(modInfoGroup, 0, 2);
            mainLayout.Controls.Add(bottomLayout, 0, 3);
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Location = new Point(12, 12);
            mainLayout.Margin = new Padding(4, 3, 4, 3);
            mainLayout.Name = "mainLayout";
            mainLayout.Padding = new Padding(0, 0, 0, 12);
            mainLayout.RowCount = 4;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.Size = new Size(696, 596);
            mainLayout.TabIndex = 0;
            //
            // imageBox
            //
            imageBox.Dock = DockStyle.Fill;
            imageBox.Margin = new Padding(0, 0, 0, 12);
            imageBox.Name = "imageBox";
            imageBox.Size = new Size(696, 174);
            imageBox.SizeMode = PictureBoxSizeMode.Zoom;
            imageBox.TabIndex = 0;
            imageBox.TabStop = false;
            //
            // modGroup
            //
            modGroup.Controls.Add(modListPanel);
            modGroup.Dock = DockStyle.Fill;
            modGroup.Location = new Point(0, 186);
            modGroup.Margin = new Padding(0, 0, 0, 12);
            modGroup.Name = "modGroup";
            modGroup.Padding = new Padding(8);
            modGroup.Size = new Size(696, 252);
            modGroup.TabIndex = 1;
            modGroup.TabStop = false;
            modGroup.Text = "Select Mod";
            //
            // modListPanel
            //
            modListPanel.AutoScroll = true;
            modListPanel.Dock = DockStyle.Fill;
            modListPanel.FlowDirection = FlowDirection.TopDown;
            modListPanel.Location = new Point(8, 24);
            modListPanel.Margin = new Padding(0);
            modListPanel.Name = "modListPanel";
            modListPanel.Padding = new Padding(0);
            modListPanel.Size = new Size(680, 220);
            modListPanel.TabIndex = 0;
            modListPanel.WrapContents = false;
            //
            // modInfoGroup
            //
            modInfoGroup.Controls.Add(modInfoTextBox);
            modInfoGroup.Dock = DockStyle.Fill;
            modInfoGroup.Location = new Point(0, 264);
            modInfoGroup.Margin = new Padding(0, 0, 0, 12);
            modInfoGroup.Name = "modInfoGroup";
            modInfoGroup.Padding = new Padding(8);
            modInfoGroup.Size = new Size(696, 300);
            modInfoGroup.TabIndex = 2;
            modInfoGroup.TabStop = false;
            modInfoGroup.Text = "Mod Details";
            //
            // modInfoTextBox
            //
            modInfoTextBox.BackColor = SystemColors.Window;
            modInfoTextBox.BorderStyle = BorderStyle.FixedSingle;
            modInfoTextBox.Dock = DockStyle.Fill;
            modInfoTextBox.Location = new Point(8, 24);
            modInfoTextBox.Margin = new Padding(0);
            modInfoTextBox.Multiline = true;
            modInfoTextBox.Name = "modInfoTextBox";
            modInfoTextBox.ReadOnly = true;
            modInfoTextBox.ScrollBars = ScrollBars.Vertical;
            modInfoTextBox.Size = new Size(680, 268);
            modInfoTextBox.TabIndex = 0;
            //
            // bottomLayout
            //
            bottomLayout.ColumnCount = 3;
            bottomLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            bottomLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            bottomLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            bottomLayout.Controls.Add(linkDiscord, 0, 0);
            bottomLayout.Controls.Add(buttonPTR, 1, 0);
            bottomLayout.Controls.Add(installButton, 2, 0);
            bottomLayout.Dock = DockStyle.Fill;
            bottomLayout.Location = new Point(0, 576);
            bottomLayout.Margin = new Padding(0);
            bottomLayout.Name = "bottomLayout";
            bottomLayout.RowCount = 1;
            bottomLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            bottomLayout.Size = new Size(696, 32);
            bottomLayout.TabIndex = 3;
            //
            // linkDiscord
            //
            linkDiscord.AutoSize = true;
            linkDiscord.Dock = DockStyle.Fill;
            linkDiscord.LinkBehavior = LinkBehavior.HoverUnderline;
            linkDiscord.Location = new Point(0, 0);
            linkDiscord.Margin = new Padding(0, 0, 12, 0);
            linkDiscord.Name = "linkDiscord";
            linkDiscord.Size = new Size(205, 32);
            linkDiscord.TabIndex = 0;
            linkDiscord.TabStop = true;
            linkDiscord.Text = "discord.gg/CWbugEvu9N";
            linkDiscord.TextAlign = ContentAlignment.MiddleLeft;
            //
            // buttonPTR
            //
            buttonPTR.Anchor = AnchorStyles.Right;
            buttonPTR.AutoSize = true;
            buttonPTR.Location = new Point(217, 6);
            buttonPTR.Margin = new Padding(0, 6, 12, 6);
            buttonPTR.Name = "buttonPTR";
            buttonPTR.Size = new Size(46, 19);
            buttonPTR.TabIndex = 1;
            buttonPTR.Text = "PTR";
            buttonPTR.UseVisualStyleBackColor = true;
            //
            // installButton
            //
            installButton.Anchor = AnchorStyles.Right;
            installButton.AutoSize = true;
            installButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            installButton.Location = new Point(275, 0);
            installButton.Margin = new Padding(0);
            installButton.MinimumSize = new Size(150, 32);
            installButton.Name = "installButton";
            installButton.Padding = new Padding(12, 4, 12, 4);
            installButton.Size = new Size(150, 32);
            installButton.TabIndex = 2;
            installButton.Text = "Install";
            installButton.UseVisualStyleBackColor = true;
            //
            // statusStrip
            //
            statusStrip.ImageScalingSize = new Size(20, 20);
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel, installProgressBar });
            statusStrip.Location = new Point(0, 618);
            statusStrip.Name = "statusStrip";
            statusStrip.Padding = new Padding(1, 0, 16, 0);
            statusStrip.Size = new Size(720, 22);
            statusStrip.TabIndex = 1;
            statusStrip.Text = "statusStrip";
            //
            // statusLabel
            //
            statusLabel.Margin = new Padding(0, 3, 0, 3);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(43, 16);
            statusLabel.Text = "Ready.";
            //
            // installProgressBar
            //
            installProgressBar.Alignment = ToolStripItemAlignment.Right;
            installProgressBar.AutoSize = false;
            installProgressBar.Margin = new Padding(8, 3, 1, 3);
            installProgressBar.Name = "installProgressBar";
            installProgressBar.Size = new Size(180, 16);
            installProgressBar.Visible = false;
            //
            // MainForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(720, 640);
            Controls.Add(mainLayout);
            Controls.Add(statusStrip);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new Padding(4, 3, 4, 3);
            MinimumSize = new Size(420, 640);
            Name = "MainForm";
            Padding = new Padding(12, 12, 12, 0);
            Text = "Maethrillian";
            mainLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)imageBox).EndInit();
            modGroup.ResumeLayout(false);
            modInfoGroup.ResumeLayout(false);
            modInfoGroup.PerformLayout();
            bottomLayout.ResumeLayout(false);
            bottomLayout.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolTip toolTip1;
        private TableLayoutPanel mainLayout;
        private PictureBox imageBox;
        private GroupBox modGroup;
        private FlowLayoutPanel modListPanel;
        private GroupBox modInfoGroup;
        private TextBox modInfoTextBox;
        private TableLayoutPanel bottomLayout;
        private LinkLabel linkDiscord;
        private CheckBox buttonPTR;
        private Button installButton;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private ToolStripProgressBar installProgressBar;
        private System.Windows.Forms.Timer connectivityTimer;
    }
}
