using FPlug;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Xwt;
using Xwt.Backends;
using Xwt.WPFBackend;
using SWC = System.Windows.Controls;

namespace FPlugWPF
{
    class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public enum ResizeDirection
        {
            None = 0,
            Left = 10,
            TopLeft = 14,
            Top = 12,
            TopRight = 13,
            Right = 11,
            BottomRight = 17,
            Bottom = 15,
            BottomLeft = 16,
        }

        public const int HTCAPTION = 2;

        public enum SysCommandSize : int
        {
            None = 0,
            SC_SIZE_HTLEFT = 1,
            SC_SIZE_HTRIGHT = 2,
            SC_SIZE_HTTOP = 3,
            SC_SIZE_HTTOPLEFT = 4,
            SC_SIZE_HTTOPRIGHT = 5,
            SC_SIZE_HTBOTTOM = 6,
            SC_SIZE_HTBOTTOMLEFT = 7,
            SC_SIZE_HTBOTTOMRIGHT = 8
        }

        static IntPtr ptr;

        static int borderWidth = 8;

        #region Window styles
        [Flags]
        public enum ExtendedWindowStyles
        {
            // ...
            WS_EX_TOOLWINDOW = 0x00000080,
            // ...
        }

        public enum GetWindowLongFields
        {
            // ...
            GWL_EXSTYLE = (-20),
            // ...
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            int error = 0;
            IntPtr result = IntPtr.Zero;
            // Win32 SetWindowLong doesn't clear error on success
            SetLastError(0);

            if (IntPtr.Size == 4)
            {
                // use SetWindowLong
                Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
                error = Marshal.GetLastWin32Error();
                result = new IntPtr(tempResult);
            }
            else
            {
                // use SetWindowLongPtr
                result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                error = Marshal.GetLastWin32Error();
            }

            if ((result == IntPtr.Zero) && (error != 0))
            {
                throw new System.ComponentModel.Win32Exception(error);
            }

            return result;
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(int dwErrorCode);
        #endregion

        [STAThread]
        public static void Main(string[] args)
        {
            #region aaaa
            //App.InitDropShadow = (backend) =>
            //    {
            //        ((Xwt.WPFBackend.WidgetBackend)backend).Widget.Effect = new DropShadowEffect()
            //        {
            //            Color = Colors.Black,
            //            Direction = -90,
            //            ShadowDepth = 1,
            //            //Softness = 20,
            //            BlurRadius = 7,
            //            Opacity = .3,
            //            RenderingBias = RenderingBias.Performance
            //        };
            //    };

            //App.InitDropShadow = (backend) =>
            //    {
            //        //if (new Random().Next() > int.MaxValue/2)
            //        ((Xwt.WPFBackend.WidgetBackend)backend).Widget.Effect = new BlurEffect()
            //        {
            //            KernelType = System.Windows.Media.Effects.KernelType.Gaussian,
            //            Radius = 10,
            //            RenderingBias = RenderingBias.Performance
            //        };
            //    };
            #endregion

            //App.BeforeWindowShown = (window) =>
            //    {
            //        var backend = (Xwt.WPFBackend.WindowBackend)window;
            //
            //        if (App.WpfFancyStyle)
            //        {
            //            var win = backend.Window;
            //            win.Loaded += (s, e) =>
            //                {
            //                    //win.WindowStyle = WindowStyle.None;
            //                    //win.ResizeMode = ResizeMode.NoResize;
            //                    
            //                    ptr = new WindowInteropHelper(win).Handle;
            //                    
            //                    int exStyle = (int)GetWindowLong(ptr, (int)GetWindowLongFields.GWL_EXSTYLE);
            //                    exStyle = int.MinValue;
            //                    exStyle |= 0x80;
            //                    exStyle |= 0x00020000; // CS_DROPSHADOW
            //                    exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            //                    exStyle &= ~0x0002;
            //                    exStyle &= ~0x00800000;
            //                    exStyle |= 0x00800000;
            //                    exStyle |= 0x00400000;
            //                    exStyle |= 0x08000000;
            //                    exStyle |= 0x00040000;
            //                    SetWindowLong(ptr, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
            //                };
            //        }
            //    };

            // set gray color
            App.InitMainWindow = (window) =>
                {
                    var backend = (Xwt.WPFBackend.WindowBackend)window;
                    //backend.mainMenu.Background = new LinearGradientBrush(Color.FromRgb(192, 192, 192), Color.FromRgb(240, 240, 240), 90);
                    //backend.mainMenu.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                    backend.mainMenu.Background = new SolidColorBrush(Colors.White);
                    //backend.mainMenu.Background = new SolidColorBrush(Color.FromRgb(64, 64, 64));
                    

                    if (App.WpfFancyStyle)
                    {
                        var win = backend.Window;

                        ptr = new WindowInteropHelper(win).Handle;

                        // style
                        //int exStyle = (int)GetWindowLong(ptr, (int)GetWindowLongFields.GWL_EXSTYLE);
                        //
                        //exStyle |= 0x00020000;
                        //SetWindowLong(ptr, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
                        // end style

                        //win.Opacity = 0;
                        backend.mainMenu.PreviewMouseDown += (s, e) =>
                        {
                            //ReleaseCapture();
                            //SendMessage(ptr, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
                        };

                        //win.MouseDown += (s, e) =>
                        //{
                        //    ReleaseCapture();
                        //    SendMessage(ptr, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
                        //};
                        //
                        //win.MouseDown += (s, e) =>
                        //{
                        //    ReleaseCapture();
                        //    SendMessage(ptr, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
                        //};

                        var grid = ((Grid)win.Content);

                        //grid.Background = Brushes.Transparent;
                        ////backend.mainMenu.Margin = new Thickness(0, 12, 0, 0);
                        //
                        ////grid.Margin = new Thickness(8);
                        //
                        //List<UIElement> elements = new List<UIElement>();
                        //foreach (UIElement e in grid.Children)
                        //    elements.Add(e);
                        //
                        //var newGrid = new System.Windows.Controls.Grid();
                        //
                        ////newGrid.Background = Brushes.Transparent;
                        //
                        //foreach (UIElement e in elements)
                        //{
                        //    grid.Children.Remove(e);
                        //    newGrid.Children.Add(e);
                        //}
                        ////grid.Background = Brushes.Transparent;
                        ////newGrid.Background = Brushes.Transparent;

                        var newGrid = new Grid();

                        win.Content = null;

                        newGrid.Background = Brushes.Transparent;
                        newGrid.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                        newGrid.Children.Add(grid);
                        win.Content = newGrid;

                        grid.Background = Brushes.Red;
                        grid.Margin = new Thickness(6);
                        //grid.Children.Add(newGrid);

                        newGrid.PreviewMouseLeftButtonDown += (s, e) =>
                        {
                            var p = e.GetPosition(newGrid);

                            var dir = SysCommandSize.None;

                            if (p.Y < borderWidth) // top
                            {
                                if (p.X < borderWidth)
                                    dir = SysCommandSize.SC_SIZE_HTTOPLEFT;
                                else if (p.X < grid.ActualWidth - borderWidth)
                                    dir = SysCommandSize.SC_SIZE_HTTOP;
                                else
                                    dir = SysCommandSize.SC_SIZE_HTTOPRIGHT;
                            }
                            else if (p.Y < grid.ActualHeight - borderWidth) // center
                            {
                                if (p.X < borderWidth)
                                    dir = SysCommandSize.SC_SIZE_HTLEFT;
                                else if (p.X < grid.ActualWidth - borderWidth)
                                    dir = SysCommandSize.None;
                                else
                                    dir = SysCommandSize.SC_SIZE_HTRIGHT;
                            }
                            else // bottom
                            {
                                if (p.X < borderWidth)
                                    dir = SysCommandSize.SC_SIZE_HTBOTTOMLEFT;
                                else if (p.X < grid.ActualWidth - borderWidth)
                                    dir = SysCommandSize.SC_SIZE_HTBOTTOM;
                                else
                                    dir = SysCommandSize.SC_SIZE_HTBOTTOMRIGHT;
                            }

                            if (dir != SysCommandSize.None)
                            {
                                ReleaseCapture();
                                SendMessage(ptr, 0x0112, new IntPtr(0xF000 + (int)dir), IntPtr.Zero);
                            }
                        };

                        win.PreviewMouseLeftButtonDown += (s, e) =>
                        {
                            //ReleaseCapture();
                            //SendMessage(ptr, WM_NCLBUTTONDOWN, (IntPtr)ResizeDirection.Right, IntPtr.Zero);
                        };

                        win.Effect = new DropShadowEffect() { ShadowDepth = -5 };

                        //win.BeginAnimation(System.Windows.Window.OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromSeconds(.25)));

                        //win.MouseDown += (s, e) => { if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) win.DragMove(); };
                    }
                };

            App.SetCustomImageViewImageDataDownloadedEvent = (webClient) =>
                {
                    webClient.DownloadDataCompleted += (s, e) =>
			        {
                        ImageView imageView = (ImageView)e.UserState;

                        Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                BitmapImage img = new BitmapImage();
                                img.BeginInit();
                                img.CacheOption = BitmapCacheOption.OnLoad;
                                img.StreamSource = new MemoryStream(e.Result);
                                img.EndInit();

                                img.Freeze();

                                var backend = (Xwt.WPFBackend.ImageViewBackend)Xwt.Toolkit.GetBackend(imageView);

                                var imageWidget = (Xwt.WPFBackend.ImageBox)backend.Widget;

                                ((Xwt.WPFBackend.ImageBox)imageWidget).Dispatcher.Invoke(new Action(() =>
                                {
                                    Type wpfimg = Assembly.GetAssembly(typeof(WPFEngine)).GetType("Xwt.WPFBackend.WpfImage");
                                    object i = Activator.CreateInstance(wpfimg, img);

                                    backend.SetImage(new ImageDescription
                                    {
                                        Alpha = 1.0,
                                        Backend = i,
                                        Size = new Xwt.Size(FPlug.Widgets.HudsTFWidget.ImageSize.Width, FPlug.Widgets.HudsTFWidget.ImageSize.Height)
                                    });
                                }));
                            }
                            catch (Exception exc)
                            {
                                Console.WriteLine(exc.Message);
                            }
                        });
			        };
                };

            App.CustomSelectFolder = path =>
            {
                FolderBrowser2 browser = new FolderBrowser2();
                if (path != null)
                    browser.DirectoryPath = path;
                return browser.ShowDialog(null);
            };


            //App.AddImage = (element, url, webclient) =>
            //    {
            //        var imageview = element as ImageView;
            //        var backend = (Xwt.WPFBackend.ImageViewBackend)Xwt.Toolkit.GetBackend(element);
            //
            //        var imageWidget = backend.Widget as Xwt.WPFBackend.ImageBox;
            //
            //        new Task(() =>
            //        {
            //            try
            //            {
            //                webclient.DownloadDataAsync(new Uri(url, UriKind.Absolute), imageWidget);
            //
            //                webclient.DownloadDataCompleted += (s, e) =>
            //                {
            //                    if (e.UserState == imageWidget)
            //                    {
            //                        BitmapImage img = new BitmapImage();
            //                        img.BeginInit();
            //                        img.CacheOption = BitmapCacheOption.OnLoad;
            //                        img.StreamSource = new MemoryStream(e.Result);
            //                        img.EndInit();
            //
            //                        img.Freeze();
            //
            //                        ((Xwt.WPFBackend.ImageBox)e.UserState).Dispatcher.Invoke(new Action(() =>
            //                        {
            //                            //imageview.Image = (Xwt.Drawing.Image)e.Result;
            //
            //                            //var bitmap = (BitmapImage)e.Result;
            //
            //                            Type wpfimg = Assembly.GetAssembly(typeof(WPFEngine)).GetType("Xwt.WPFBackend.WpfImage");
            //                            object i = Activator.CreateInstance(wpfimg, img);
            //
            //                            backend.SetImage(new ImageDescription
            //                            {
            //                                Alpha = 1.0,
            //                                Backend = i,
            //                                Size = new Xwt.Size(190, 150)
            //                            });
            //                        }));
            //                    }
            //                };
            //            }
            //            catch (Exception exc)
            //            {
            //                Console.WriteLine(exc.Message);
            //            }
            //
            //        }).Start();
            //    };

            // iniate the move animation
            App.InitMoveWidget = (backend) =>
                {
                    var element = ((Xwt.WPFBackend.WidgetBackend)backend).Widget;

                    element.UseLayoutRounding = true;

                    TranslateTransform trans = new TranslateTransform();
                    element.RenderTransform = trans;
                };

            // move animation
            App.MoveWidget = (backend, oldx, oldy, x, y, animate) =>
                {
                    var element = ((Xwt.WPFBackend.WidgetBackend)backend).Widget;

                    TranslateTransform trans = (TranslateTransform)element.RenderTransform;

                    if (animate)
                    {
                        CubicEase easing = new CubicEase();
                        easing.EasingMode = EasingMode.EaseInOut;

                        DoubleAnimation anim1 = new DoubleAnimation(Math.Round(oldx), Math.Round(x), TimeSpan.FromSeconds(0.25));
                        DoubleAnimation anim2 = new DoubleAnimation(Math.Round(oldy), Math.Round(y), TimeSpan.FromSeconds(0.25));
                        anim1.EasingFunction = anim2.EasingFunction = easing;
                        trans.BeginAnimation(TranslateTransform.XProperty, anim1);
                        trans.BeginAnimation(TranslateTransform.YProperty, anim2);
                    }
                    else
                    {
                        trans.BeginAnimation(TranslateTransform.XProperty, null);
                        trans.BeginAnimation(TranslateTransform.YProperty, null);
                        trans.X = Math.Round(x);
                        trans.Y = Math.Round(y);
                    }
                };

            // opacity animation
            App.AnimateOpacityIn = (backend) =>
                {
                    // opacity
                    var element = ((Xwt.WPFBackend.WidgetBackend)backend).Widget;
                    var a = new DoubleAnimation
                    {
                        From = 0.0,
                        To = 1.0,
                        BeginTime = TimeSpan.FromSeconds(.2),
                        Duration = new Duration(TimeSpan.FromSeconds(0.2))
                    };

                    element.BeginAnimation(UIElement.OpacityProperty, a);
                };

            // recycle bin
            /*App.CustomDelete = (path) =>
                {
                    path = path + "\\";
                    path.Log();
                    SHFILEOPSTRUCT fileop = new SHFILEOPSTRUCT()
                    {
                        wFunc = 3, // FO_DELETE
                        pFrom = path + "\0\0",
                        fFlags = 0x40 | 0x10 // FOF_ALLOWUNDO | FOF_NOCONFIRMATION
                    };
                    
                    return SHFileOperation(ref fileop).Log() == 0;
                };*/

            App.Run(ToolkitType.Wpf);
        }

        #region Native Methods
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        public struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.U4)]
            public int wFunc;
            public string pFrom;
            public string pTo;
            public short fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }
        #endregion
    }
}
