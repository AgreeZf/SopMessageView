using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Ast.Data;
using Ast.Exchange.Contracts;
using Ast.Exchange.Core;
using Ast.Exchange.Models;
using Ast.Exchange.Sop;
using Microsoft.Win32;
using SopMessageView.Libs;


namespace SopMessageView
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _rcdFullPath;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnImportRcd_Click(object sender, RoutedEventArgs e)
        {
            FileDialog dlg=new OpenFileDialog();
            if (dlg.ShowDialog()==true)
            {
                _rcdFullPath = dlg.FileName;
            }
        }


        private void buttonParse_Click(object sender, RoutedEventArgs e)
        {
            var text = textBoxMsg.Text.Trim();
            textBoxResult.Text = string.Empty;
            var msg = MainFrameTradeLogHelper.GetHexMessageFromLog(text.Trim());
            if (radioButtonRequest.IsChecked==true)
            {
                var fields = RcdHelper.GetRequestMessageNodes(_rcdFullPath, msg);
                foreach (var messageNode in fields)
                {
                    textBoxResult.Text += messageNode.Text;
                    textBoxResult.Text += Environment.NewLine;
                }
            }
            else
            {
                var basePath = AppDomain.CurrentDomain.BaseDirectory;
                var factoryProvider = new DefaultFactoryProvider(basePath + "\\Libs\\");

                var serviceProvider = new DefaultServiceProvider();
                serviceProvider.AddService(typeof(IMessageNodeFactoryProvider), factoryProvider);
                if (File.Exists(_rcdFullPath))
                {
                    var xDoc = XDocument.Load(_rcdFullPath);
                    var element = xDoc.Descendants(XName.Get("Message", xDoc.Root.Name.NamespaceName)).FirstOrDefault();
                    var message = factoryProvider.GetFactory("Message").CreateNode(serviceProvider, element, null);

                    textBoxResult.Text = "";
                    ExtractPackage((Message)message, msg);
                }
            }
            
        }

        private void ExtractPackage(Message message, object value)
        {
            var sopPkgExtractor = new SopPackageExtractor();
            var dataAccessor = new ContextAccessor();
            int currIndex;
            var result = sopPkgExtractor.Extract(message, (byte[])value, dataAccessor, out currIndex);
            textBoxResult.Text += string.Format("目前解析到{0}，{1}的位置", currIndex, currIndex.ToString("x2"));
            foreach (var sopObjectResult in result.Objects)
            {
                textBoxResult.Text += Environment.NewLine;
                textBoxResult.Text += (sopObjectResult.ObjectName);

                foreach (var field in sopObjectResult.Fields)
                {
                    if (field.Value.GetType().Name != "AstTable")
                    {
                        textBoxResult.Text += Environment.NewLine;
                        textBoxResult.Text += string.Format("{0}:{1}", field.Key, field.Value);
                    }
                    else
                    {
                        Ast.Data.AstTable table = field.Value as AstTable;
                        textBoxResult.Text += Environment.NewLine;
                        textBoxResult.Text += string.Format("表格{0}数据", field.Key);
                        for (int i = 0; i < table.RowCount; i++)
                        {
                            textBoxResult.Text += Environment.NewLine;
                            textBoxResult.Text += string.Format(string.Join(",", table[i].ItemArray));
                        }
                    }

                }
            }


        }
    }
}
