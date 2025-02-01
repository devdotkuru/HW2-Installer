namespace MaethrillianInstallerWin
{
    partial class MaethrillianInstallerWin
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
            imageBox = new PictureBox();
            group = new GroupBox();
            installButton = new Button();
            statusStrip1 = new StatusStrip();
            status = new ToolStripStatusLabel();
            buttonPTR = new CheckBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)imageBox).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // imageBox
            // 
            imageBox.BackgroundImageLayout = ImageLayout.Stretch;
            imageBox.Location = new Point(12, 12);
            imageBox.Margin = new Padding(4, 3, 4, 3);
            imageBox.Name = "imageBox";
            imageBox.Size = new Size(309, 174);
            imageBox.SizeMode = PictureBoxSizeMode.StretchImage;
            imageBox.TabIndex = 1;
            imageBox.TabStop = false;
            // 
            // group
            // 
            group.Location = new Point(12, 192);
            group.Margin = new Padding(4, 3, 4, 3);
            group.Name = "group";
            group.Padding = new Padding(4, 3, 4, 3);
            group.Size = new Size(309, 284);
            group.TabIndex = 2;
            group.TabStop = false;
            group.Text = "Select Mod";
            // 
            // installButton
            // 
            installButton.Location = new Point(11, 500);
            installButton.Margin = new Padding(4, 3, 4, 3);
            installButton.Name = "installButton";
            installButton.Size = new Size(309, 30);
            installButton.TabIndex = 3;
            installButton.Text = "Install";
            installButton.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { status });
            statusStrip1.Location = new Point(0, 533);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(333, 22);
            statusStrip1.TabIndex = 4;
            statusStrip1.Text = "statusStrip1";
            // 
            // status
            // 
            status.Margin = new Padding(0, 3, 0, 3);
            status.Name = "status";
            status.Size = new Size(87, 16);
            status.Text = "PLACEHOLDER";
            // 
            // buttonPTR
            // 
            buttonPTR.AutoSize = true;
            buttonPTR.Location = new Point(274, 475);
            buttonPTR.Name = "buttonPTR";
            buttonPTR.Size = new Size(46, 19);
            buttonPTR.TabIndex = 5;
            buttonPTR.Text = "PTR";
            buttonPTR.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 479);
            label1.Name = "label1";
            label1.Size = new Size(142, 15);
            label1.TabIndex = 6;
            label1.Text = "discord.gg/CWbugEvu9N";
            // 
            // MaethrillianInstallerWin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(333, 555);
            Controls.Add(label1);
            Controls.Add(buttonPTR);
            Controls.Add(statusStrip1);
            Controls.Add(installButton);
            Controls.Add(group);
            Controls.Add(imageBox);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            Name = "MaethrillianInstallerWin";
            Text = "Maethrillian";
            ((System.ComponentModel.ISupportInitialize)imageBox).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolTip toolTip1;
        private PictureBox imageBox;
        private GroupBox group;
        private Button installButton;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel status;
        private CheckBox buttonPTR;
        private LinkLabel lblDiscord;
        private Label label1;
    }
}
