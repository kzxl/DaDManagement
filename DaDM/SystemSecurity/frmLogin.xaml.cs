using Controllers;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using Models;
using System.Data;

namespace DaDM.SystemSercurity
{
    /// <summary>
    /// Interaction logic for frmLogin.xaml
    /// </summary>
    public partial class frmLogin : Window
    {
        public frmLogin()
        {
            InitializeComponent();
            lbMessage.Visibility = Visibility.Hidden;
        }
        public static bool LoginSuccessful = false;
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Username = txtUserName.Text.Trim();
                string Password = txtPass.Text.Trim();
                string MK;
                if (Convert.ToBoolean(chkRemember.Tag) == true)
                {
                    MK = txtPass.Tag.ToString();
                }
                else
                {
                    MK = clsFunctions.MD5(Password);
                }
                LoginSuccessful = clsFunctions.Login(Username, MK, chkRemember.IsChecked ?? false);
                if (!LoginSuccessful)
                {
                    lbMessage.Content = "User/Pass incorrect";
                    lbMessage.Visibility = Visibility.Visible;
                }
                else
                    this.Close();
            }
            catch (Exception xxx)
            {
                MessageBox.Show(xxx.Message);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string xmlFile = "Settings.xml";
                DataSet dsXML = new DataSet();
                dsXML.ReadXml(xmlFile, XmlReadMode.InferSchema);
                string Username = dsXML.Tables["Login"].Rows[0]["Username"].ToString();
                string PassWord = dsXML.Tables["Login"].Rows[0]["Password"].ToString();
                if (Username != "" && PassWord != "")
                {
                    chkRemember.IsChecked = true;
                    txtUserName.Text = Username;
                    txtPass.Text = "Admin";

                    //Nhớ mật khẩu từ file
                    chkRemember.Tag = true;
                    txtPass.Tag = PassWord;
                }
                else
                {
                    //Nhớ mật khẩu từ file
                    chkRemember.Tag = false;
                }

            }
            catch (Exception)
            {
                chkRemember.IsChecked = false;
                txtUserName.Text = "";
                txtPass.Text = "";
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!LoginSuccessful) { Application.Current.Shutdown(); }

        }
    }
}
