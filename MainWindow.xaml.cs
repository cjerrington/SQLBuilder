using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.IO;


namespace SQLBuilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Icon by www.flaticon.com/authors/smashicons

        private void ChooseFolder_mdf(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txt_mdfloc.Text = folderDialog.SelectedPath;
                    // folderDialog.SelectedPath -- your result
                }
            }
        }

        private void ChooseFolder_ldf(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txt_ldfloc.Text = folderDialog.SelectedPath;
                    // folderDialog.SelectedPath -- your result
                }
            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            // Process needed inputs
            if (String.IsNullOrWhiteSpace(txt_mdfloc.Text))
            {
                System.Windows.Forms.MessageBox.Show("MDF file location cannot be blank.");
                return;
            }

            if (String.IsNullOrWhiteSpace(txt_ldfloc.Text))
            {
                System.Windows.Forms.MessageBox.Show("LDF file location cannot be blank.");
                return;
            }

            // Clear output box to allow for new content
            txt_querybox.Text = "";

            // create Tab character
            char tab = '\u0009';

            // Get all MDF files from the input field
            string[] mdf_files;
            mdf_files = Directory.GetFiles(txt_mdfloc.Text, $"*{txt_prefix.Text}*.mdf", SearchOption.TopDirectoryOnly);

            // Get all LDF files from the input field
            string[] ldf_files;
            ldf_files = Directory.GetFiles(txt_ldfloc.Text, $"*{txt_prefix.Text}*.ldf", SearchOption.TopDirectoryOnly);

            // Zip arrays together 
            var filenames = mdf_files.Zip(ldf_files, (m, l) => new { MDF = m, LDF = l });

            foreach (var filename in filenames)
            {
                // Parse base name of file for our database name
                string DBName = System.IO.Path.GetFileNameWithoutExtension(filename.MDF);

                // Update our text box by adding to the content
                // Accessing MDF file path with file.MDF
                // Accessing LDF file path with file.LDF
                txt_querybox.Text += $@"CREATE DATABASE {DBName}{Environment.NewLine}"
                    + $@"{tab}ON (FILENAME = '{filename.MDF}'),{Environment.NewLine}"
                    + $@"{tab}(FILENAME = '{filename.LDF}'){Environment.NewLine}"
                    + $@"{tab}FOR ATTACH;"
                    + Environment.NewLine + Environment.NewLine;
            }
        }

        private void Export_Query(object sender, RoutedEventArgs e)
        {
            string cwd = Directory.GetCurrentDirectory();
            string filename = null;

            if (String.IsNullOrEmpty(txt_prefix.Text))
            {
                filename = $@"{cwd}\MassAttach.sql";
            }
            else
            {
                filename = $@"{cwd}\{txt_prefix.Text}_MassAttach.sql";
            }
            
            try
            {
                File.WriteAllText(filename, txt_querybox.Text);
                System.Windows.Forms.MessageBox.Show($@"File saved to {filename}");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($@"Problem saving to {filename}" + Environment.NewLine + ex);
            }
            
        }
    }
}
