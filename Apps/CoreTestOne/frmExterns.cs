using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DataTools.Essentials.Observable;

namespace CoreTestOne
{
    public partial class frmExterns : Form
    {
        private int mode = 0;
        private int vis = 0;

        private List<ExternInfo> results;
        private SamViewModel vm = new SamViewModel();

        public frmExterns()
        {
            SyncObservableBase.Initialize();
            InitializeComponent();
            //AllocConsole();
            Load += FrmExterns_Load;
            vm.PropertyChanged += Vm_PropertyChanged;
            vm.Title = Text;
            vm.GetButtonRequest += Vm_GetButtonRequest;
        }

        private void Vm_GetButtonRequest(object sender, GetButtonEventArgs e)
        {
            e.ButtonText = button1.Text;
            textBox2.Text = button1.Text;
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SamViewModel.Title))
            {
                this.Text = vm.Title;
                if (textBox2.Text != vm.Title)
                {
                    textBox2.Text = vm.Title;
                }
            }
        }

        private void FrmExterns_Load(object sender, EventArgs e)
        {
        }

        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        private void button1_Click(object sender, EventArgs e)
        {
            var dlg = new FolderBrowserDialog();
            dlg.InitialDirectory = "E:\\Projects\\Personal Projects\\DataTools";
            var res = dlg.ShowDialog();

            if (res != DialogResult.OK) return;

            _ = Task.Run(() =>
            {
                this.results = CoreTestOne.Program.ScanForExterns(dlg.SelectedPath);
                FilterItems();
            });
        }

        private void FilterItems()
        {
            if (this.results == null || this.results.Count == 0) return;
            var results = this.results.ToList();

            if (mode != 2)
            {
                if (mode == 1)
                {
                    results = this.results.Where(x => x.ReferenceFiles == null || x.ReferenceFiles.Count == 0).ToList();
                }
                else if (mode == 3)
                {
                    results = results.Where(x => x.ReferenceFiles != null && x.ReferenceFiles.Count != 0).ToList();
                }

                if (vis == 1)
                {
                    results = results.Where(x => x.Visibility == "public").ToList();
                }

                treeView1.Invoke(new Action(() =>
                {
                    treeView1.Nodes.Clear();

                    var projects = results.GroupBy(x => x.Project).ToList();
                    var tvn = new List<TreeNode>();

                    foreach (var project in projects)
                    {
                        var n = new TreeNode()
                        {
                            Text = project.Key,
                            Tag = project
                        };

                        treeView1.Nodes.Add(n);
                        var files = project.GroupBy(x => x.FileName).ToList();

                        foreach (var file in files)
                        {
                            var n2 = new TreeNode()
                            {
                                Text = file.Key,
                                Tag = file
                            };

                            n.Nodes.Add(n2);

                            foreach (var item in file)
                            {
                                var n3 = new TreeNode()
                                {
                                    Text = item.ToString(),
                                    Tag = item
                                };

                                n2.Nodes.Add(n3);

                                if (item.ReferenceFiles != null && item.ReferenceFiles.Count > 0)
                                {
                                    foreach (var reff in item.ReferenceFiles)
                                    {
                                        n3.Nodes.Add(reff);
                                    }
                                }
                            }

                            if (n2.Nodes.Count == 0)
                            {
                                n.Nodes.Remove(n2);
                            }
                        }

                        if (n.Nodes.Count == 0)
                        {
                            treeView1.Nodes.Remove(n);
                        }
                    }
                }));
            }
            else
            {
                treeView1.Invoke(new Action(() =>
                {
                    treeView1.Nodes.Clear();

                    if (vis == 1)
                    {
                        results = results.Where(x => x.Visibility == "public").ToList();
                    }

                    var grps = results.GroupBy(x => x.DLLName + "." + x.EntryPoint).Where(x => x.Count() > 1).ToList();

                    foreach (var grp in grps)
                    {
                        var n = new TreeNode()
                        {
                            Text = grp.Key + $" [Declared {grp.Count()} Times]",
                            Tag = grp
                        };

                        treeView1.Nodes.Add(n);
                        var files = grp.GroupBy(x => x.FileName).ToList();

                        foreach (var file in files)
                        {
                            var n2 = new TreeNode()
                            {
                                Text = file.Key,
                                Tag = file
                            };

                            n.Nodes.Add(n2);

                            foreach (var item in file)
                            {
                                var n3 = new TreeNode()
                                {
                                    Text = item.ToString(),
                                    Tag = item
                                };

                                n2.Nodes.Add(n3);

                                if (item.ReferenceFiles != null && item.ReferenceFiles.Count > 0)
                                {
                                    foreach (var reff in item.ReferenceFiles)
                                    {
                                        n3.Nodes.Add(reff);
                                    }
                                }
                            }

                            if (n2.Nodes.Count == 0)
                            {
                                n.Nodes.Remove(n2);
                            }
                        }

                        if (n.Nodes.Count == 0)
                        {
                            treeView1.Nodes.Remove(n);
                        }
                    }
                }));
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            mode = 0;
            FilterItems();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            mode = 1;
            FilterItems();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            mode = 2;
            FilterItems();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            mode = 3;
            FilterItems();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            vis = checkBox1.Checked ? 1 : 0;
            FilterItems();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            vm.Title = textBox2.Text;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var str = await vm.GetButtonText();
        }
    }
}