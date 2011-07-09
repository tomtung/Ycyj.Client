using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Office.Interop.Word;
using Ycyj.Client.Message;
using Ycyj.Client.Model;
using Application = System.Windows.Application;
using ContentControl = Microsoft.Office.Interop.Word.ContentControl;
using WordApplication = Microsoft.Office.Interop.Word.Application;

namespace Ycyj.Client.UserControl
{
    /// <summary>
    /// Interaction logic for NodeEditControl.xaml
    /// </summary>
    public partial class NodeEditorControl
    {
        #region Dependency Property

        public const string NodePropertyName = "Node";

        public static readonly DependencyProperty NodeProperty = DependencyProperty.Register(
            NodePropertyName,
            typeof (Node),
            typeof (NodeEditorControl),
            new UIPropertyMetadata(null, NodeChanged));

        public Node Node
        {
            get { return (Node) GetValue(NodeProperty); }
            set { SetValue(NodeProperty, value); }
        }

        #endregion

        /// <summary>
        /// Callbacks used to later gather values of edited properties.
        /// </summary>
        private readonly Dictionary<string, Func<object>> _propertyRetrieveCallbacks =
            new Dictionary<string, Func<object>>();


        public NodeEditorControl()
        {
            InitializeComponent();
            ReloadNode();
            Messenger.Default.Register(this, (NotificationMessageAction m) =>
                                                 {
                                                     if (m.Notification != Notifications.UpdateNode)
                                                         return;
                                                     foreach (var v in _propertyRetrieveCallbacks)
                                                         Node[v.Key] = v.Value();
                                                     m.Execute();
                                                 });
            Messenger.Default.Register(this, (NotificationMessage m) =>
                                                 {
                                                     if (m.Notification == Notifications.ReloadNode)
                                                         ReloadNode();
                                                 });
        }


        private static void NodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NodeEditorControl) d).ReloadNode();
        }

        private void ReloadNode()
        {
            _stackPanel.Children.Clear();
            _propertyRetrieveCallbacks.Clear();
            PropertyEditHelperForMsDoc.ResetWordApp();
            if (Node == null) return;

            _stackPanel.Children.Add(new TextBlock {Text = Node.Metadata.Name, FontWeight = FontWeights.Bold});

            foreach (NodeProperty property in Node.Properties)
            {
                Tuple<UIElement, Func<object>> tuple = property.GetPropertyEditControl();
                _stackPanel.Children.Add(tuple.Item1);
                _propertyRetrieveCallbacks.Add(property.PropertyName, tuple.Item2);
            }
        }
    }

    internal static class PropertyEditHelper
    {
        public static Tuple<UIElement, Func<object>> GetPropertyEditControl(this NodeProperty property)
        {
            var panel = new StackPanel();

            Func<object> callback = () => property.Value;

            // Maybe move all these "if"s to elsewhere later
            if (property.PropertyType == typeof (string))
                return property.GetStringPropertyEditControls();
            if (property.PropertyType == typeof (int))
                return property.GetIntPropertyEditControls();
            if (property.PropertyType == typeof (MsDoc))
                return property.GetMsDocPropertyEditControls();

            return Tuple.Create(panel as UIElement, callback);
        }
    }

    internal static class PropertyEditHelperForIntAndString
    {
        public static Tuple<UIElement, Func<object>> GetIntPropertyEditControls(this NodeProperty property)
        {
            var panel = new StackPanel();
            TextBox textBox = panel.AddTextBoxForPropertyEditing(property);
            // Maybe add some validation here later ...

            Func<object> callback = () =>
                                        {
                                            int value;
                                            int.TryParse(textBox.Text, out value);
                                            return value;
                                        };
            return Tuple.Create(panel as UIElement, callback);
        }

        public static Tuple<UIElement, Func<object>> GetStringPropertyEditControls(this NodeProperty property)
        {
            var panel = new StackPanel();
            TextBox textBox = panel.AddTextBoxForPropertyEditing(property);

            Func<object> callback = () => textBox.Text;
            return Tuple.Create(panel as UIElement, callback);
        }

        public static TextBox AddTextBoxForPropertyEditing(this Panel panel, NodeProperty property)
        {
            panel.Children.Add(new TextBlock {Text = property.PropertyName});
            var textBox = new TextBox {Text = property.Value.ToString()};
            panel.Children.Add(textBox);
            return textBox;
        }
    }

    public static class PropertyEditHelperForMsDoc
    {
        private static WordApplication _wordApp;

        static PropertyEditHelperForMsDoc()
        {
            // Register message of node changes
            //Messenger.Default.Register(Application.Current, (NotificationMessage m) =>
            //                                                    {
            //                                                        if (m.Notification == Notifications.ReloadNode)
            //                                                            ResetWordApp();
            //                                                    });

            // Try to close the m$ word when the application starts to shut down.
            Application.Current.Exit += (o, e) =>
                                            {
                                                try
                                                {
                                                    IsWordAppProtected = false;
                                                    _wordApp.Quit(false);
                                                }
                                                catch (Exception exception)
                                                {
                                                    ; // Eat it.
                                                }
                                            };

            ResetWordApp();
        }

        private static bool IsWordAppProtected { get; set; }

        public static Tuple<UIElement, Func<object>> GetMsDocPropertyEditControls(this NodeProperty property)
        {
            try
            {
                _wordApp.Visible = true;
            }
            catch (Exception exception)
            {
                ; // Eat it.
            }

            var panel = new StackPanel();
            panel.Children.Add(new TextBlock {Text = property.PropertyName});
            panel.Children.Add(new TextBox {Text = "请在Word中编辑", IsEnabled = false});

            Func<object> callback;
            try
            {
                // Create control
                Document doc = _wordApp.Documents[1];
                ++doc.Content.End;
                doc.Range(doc.Content.End - 1, doc.Content.End).Select();
                ContentControl control = doc.ContentControls.Add();
                control.SetPlaceholderText(null, null, "请在此输入" + property.PropertyName + "...");

                // Insert existing content
                try
                {
                    control.Range.InsertXML(((MsDoc) property.Value).Content);
                    control.Range.Characters.Last.Delete();
                }
                catch (Exception exception)
                {
                    NextLine(control, doc);
                }

                // Set callback
                callback = () =>
                               {
                                   try
                                   {
                                       return new MsDoc {Content = control.Range.XML};
                                   }
                                   catch
                                   {
                                       return new MsDoc();
                                   }
                               };
            }
            catch (Exception)
            {
                callback = () => property.Value;
            }

            return Tuple.Create(panel as UIElement, callback);
        }

        private static void NextLine(ContentControl control, Document doc)
        {
            Selection currSelection = doc.Application.Selection;
            int endOfRange = control.Range.End + 1;
            currSelection.SetRange(endOfRange, endOfRange);
            currSelection.TypeParagraph();
        }

        public static void ResetWordApp()
        {
            IsWordAppProtected = false;
            try
            {
                if (_wordApp.Documents.Count != 0)
                    _wordApp.Documents.Close(false);
                _wordApp.Documents.Add();
                _wordApp.Visible = false;
            }
            catch
            {
                _wordApp = new WordApplication();
                _wordApp.Documents.Add();
                _wordApp.Visible = false;
                _wordApp.DocumentBeforeClose += (Document doc, ref bool cancel) => cancel = IsWordAppProtected;
                _wordApp.DocumentBeforeSave +=
                    (Document doc, ref bool b, ref bool cancel) => cancel = IsWordAppProtected;

                _wordApp.DocumentOpen += DocumentOpenHandler;
                (_wordApp as ApplicationEvents4_Event).NewDocument += NewDocumentHandler;
            }
            IsWordAppProtected = true;
        }

        private static void NewDocumentHandler(Document doc)
        {
            if (!IsWordAppProtected) return;
            IsWordAppProtected = false;
            doc.Close();
            IsWordAppProtected = true;
            new WordApplication {Visible = true}.
                Documents.Add();
        }

        private static void DocumentOpenHandler(Document doc)
        {
            if (!IsWordAppProtected) return;
            IsWordAppProtected = false;
            string fileName = doc.FullName;
            doc.Close();
            IsWordAppProtected = true;
            try
            {
                new WordApplication {Visible = true}.Documents.Open(fileName);
            }
            catch (Exception exception)
            {
                ; // Eat it.
            }
        }
    }
}